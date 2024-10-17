using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class CreateUserRequest
    {
        [JsonProperty("applicationId")]
        public string ApplicationId;
        [JsonProperty("requestToken")]
        public bool RequestToken = true;
    }
}
