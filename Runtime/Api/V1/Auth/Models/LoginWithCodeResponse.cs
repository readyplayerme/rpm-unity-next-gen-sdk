using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class LoginWithCodeResponse : ApiResponse
    {
        [JsonProperty("_id")]
        public string Id;
        
        [JsonProperty("email")]
        public string Email;
        
        [JsonProperty("name")]
        public string Name;
        
        [JsonProperty("token")]
        public string Token;
        
        [JsonProperty("refreshToken")]
        public string RefreshToken;
    }
}
