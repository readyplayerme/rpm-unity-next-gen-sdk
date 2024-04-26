using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Data.V1;

namespace ReadyPlayerMe.Runtime.Api.V1.Characters.Models
{
    public class CharacterCreateResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Character Data { get; set; }
    }
}