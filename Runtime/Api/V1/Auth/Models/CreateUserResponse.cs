using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class CreateUserResponse : ApiResponse
    {
        [JsonProperty("data")]
        public CreateUserResponseBody Data { get; set; } = new CreateUserResponseBody();
    }
    
    public class CreateUserResponseBody
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("email")]
        public string Email;
        [JsonProperty("token")]
        public string Token;
    }
}
