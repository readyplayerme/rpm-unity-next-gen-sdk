using Newtonsoft.Json;

namespace PlayerZero.Api.V1
{
    public class SendLoginCodeRequest
    {
        [JsonProperty("email")]
        public string Email;
    }
}
