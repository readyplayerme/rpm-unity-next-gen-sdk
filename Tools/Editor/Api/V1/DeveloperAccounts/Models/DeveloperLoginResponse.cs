using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;

namespace ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models
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