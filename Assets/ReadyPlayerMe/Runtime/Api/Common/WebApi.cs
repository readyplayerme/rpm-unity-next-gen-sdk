using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Runtime.Api.Common
{
    public abstract class WebApi
    {
        protected virtual async Task<TResponse> Dispatch<TResponse, TRequestBody>(RequestData<TRequestBody> data)
        {
            var payload = JsonConvert.SerializeObject(new ApiRequestPayload<TRequestBody>()
                {
                    Data = data.Payload
                }, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );

            return await Dispatch<TResponse>(new RequestData<string>
            {
                Headers = data.Headers,
                Method = data.Method,
                Url = data.Url,
                Payload = payload
            });
        }

        protected virtual async Task<TResponse> Dispatch<TResponse>(RequestData<string> data)
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

            throw new WebException($"{request.error}\n{request.downloadHandler.text}");
        }

        protected virtual string BuildQueryString(object queryParams)
        {
            var properties = queryParams.GetType().GetProperties()
                .Where(prop => prop.GetValue(queryParams, null) != null)
                .ToDictionary(
                    GetPropertyName,
                    prop => prop.GetValue(queryParams, null).ToString());

            if (properties.Count == 0)
                return string.Empty;

            var queryString = new StringBuilder();
            queryString.Append('?');

            foreach (var (key, value) in properties)
            {
                queryString.Append($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}&");
            }

            return queryString.ToString();
        }

        private string GetPropertyName(MemberInfo prop)
        {
            return prop.GetCustomAttribute<JsonPropertyAttribute>() != null
                ? prop.GetCustomAttribute<JsonPropertyAttribute>().PropertyName
                : prop.Name;
        }
    }
}