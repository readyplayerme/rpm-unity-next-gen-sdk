using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class RefreshTokenRequest
    {
        public RefreshTokenRequestBody Payload { get; set; } = new RefreshTokenRequestBody();
    }

    public class RefreshTokenRequestBody
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
    }

}