using Newtonsoft.Json;

namespace ReadyPlayerMe.Phoenix
{
    public struct UserSession
    {
        [JsonProperty("_id")]
        public string Id;
        public string Name;
        public string Email;
        public string Token;
        public string RefreshToken;
        public string LastModifiedAvatarId;
    }
}