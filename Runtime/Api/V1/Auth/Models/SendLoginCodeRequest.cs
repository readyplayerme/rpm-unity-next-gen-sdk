using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class SendLoginCodeRequest
    {
        [JsonProperty("email")]
        public string Email;
    }
}
