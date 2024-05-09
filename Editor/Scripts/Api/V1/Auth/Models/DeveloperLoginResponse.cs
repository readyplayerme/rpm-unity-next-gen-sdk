using Newtonsoft.Json;
using ReadyPlayerMe.Api;

namespace ReadyPlayerMe.Editor.Api.V1.Auth.Models
{
    public class DeveloperLoginResponse : ApiResponse
    {
        [JsonProperty("data")]
        public DeveloperLoginResponseBody Data { get; set; } = new DeveloperLoginResponseBody();
    }

    public class DeveloperLoginResponseBody
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}