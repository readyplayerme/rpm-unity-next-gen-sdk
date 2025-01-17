using Newtonsoft.Json;

namespace PlayerZero.Api.V1
{
    public class LoginWithCodeRequest
    {
        [JsonProperty("code")]
        public string Code;

        [JsonProperty("appId")]
        public string AppId;
    }
}
