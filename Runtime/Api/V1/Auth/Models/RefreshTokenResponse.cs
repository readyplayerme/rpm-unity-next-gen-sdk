using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;

namespace ReadyPlayerMe.Runtime.Api.V1.Auth.Models
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