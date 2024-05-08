using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Data;
using UnityEngine.Networking;
using UnityEngine;

namespace ReadyPlayerMe.Runtime.Api
{
    public abstract class WebApi
    {
        private Settings _settings;
        protected Settings Settings => _settings ??= Resources.Load<Settings>("ReadyPlayerMeSettings");

        protected virtual async Task<TResponse> Dispatch<TResponse, TRequestBody>(ApiRequest<TRequestBody> data) where TResponse : ApiResponse, new()
        {
            var payload = JsonConvert.SerializeObject(new ApiRequestBody<TRequestBody>()
                {
                    Data = data.Payload
                }, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );

            return await Dispatch<TResponse>(new ApiRequest<string>
            {
                Headers = data.Headers,
                Method = data.Method,
                Url = data.Url,
                Payload = payload
            });
        }

        protected virtual async Task<TResponse> Dispatch<TResponse>(ApiRequest<string> data) where TResponse : ApiResponse, new()
        {
            using var request = new UnityWebRequest();
            request.url = data.Url;
            request.method = data.Method;
            request.downloadHandler = new DownloadHandlerBuffer();

            if (data.Headers != null)
            {
                foreach (var header in data.Headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            if (!string.IsNullOrEmpty(data.Payload))
            {
                var bodyRaw = Encoding.UTF8.GetBytes(data.Payload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            var asyncOperation = request.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
                return JsonConvert.DeserializeObject<TResponse>(request.downloadHandler.text);

            Debug.LogWarning($"{request.error} - {request.url}\n{request.downloadHandler.text}");

            return new TResponse()
            {
                IsSuccess = false,
                Status = request.responseCode,
                Error = request.error
            };
        }

        protected virtual string BuildQueryString(object queryParams)
        {
            var properties = queryParams.GetType().GetProperties()
                .Where(prop => prop.GetValue(queryParams, null) != null)
                .ToDictionary(
                    GetPropertyName,
                    prop => prop.GetValue(queryParams, null));

            if (properties.Count == 0)
                return string.Empty;

            var queryString = new StringBuilder();
            queryString.Append('?');

            foreach (var (key, value) in properties)
            {
                if (value is IDictionary dictionary)
                {
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        queryString.Append($"{Uri.EscapeDataString(entry.Key.ToString())}={Uri.EscapeDataString(entry.Value.ToString())}&");
                    }
                }
                else
                {
                    queryString.Append($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value.ToString())}&");
                }
            }

            return queryString.ToString();
        }

        private static string GetPropertyName(MemberInfo prop)
        {
            return prop.GetCustomAttribute<JsonPropertyAttribute>() != null
                ? prop.GetCustomAttribute<JsonPropertyAttribute>().PropertyName
                : prop.Name;
        }
        
        private class ApiRequestBody<T> {
            [JsonProperty("data")]
            public T Data { get; set; }
        }
    }
}