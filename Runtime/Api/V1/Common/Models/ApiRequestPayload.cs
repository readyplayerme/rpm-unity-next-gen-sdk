using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1.Common.Models
{
    public class ApiRequestPayload<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}