using Newtonsoft.Json;

namespace ReadyPlayerMe.Editor.Api.V1.Auth.Models
{
    public class DeveloperLoginRequest
    {
        public DeveloperLoginRequestBody Payload { get; set; } = new DeveloperLoginRequestBody();
    }

    public class DeveloperLoginRequestBody
    {
        [JsonProperty("loginId")]
        public string Email { get; set; }
        
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}