using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.Common
{
    public class ApiRequestPayload<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}