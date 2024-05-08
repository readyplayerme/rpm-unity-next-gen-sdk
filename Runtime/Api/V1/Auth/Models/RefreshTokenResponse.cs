using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1
{
    public class RefreshTokenResponse : ApiResponse
    {
        [JsonProperty("data")]
        public RefreshTokenResponseBody Data { get; set; } = new RefreshTokenResponseBody();
    }

    public class RefreshTokenResponseBody
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
    }
}