using System.Collections.Generic;
using Newtonsoft.Json;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.Api.V1
{
    public class CharacterCreateRequest
    {
        public CharacterCreateRequestBody Payload { get; set; } = new CharacterCreateRequestBody();
    }

    public class CharacterCreateRequestBody
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; } = Resources.Load<Settings>("ReadyPlayerMeSettings")?.ApplicationId;

        [JsonProperty("assets")]
        public IDictionary<string, string> Assets { get; set; }
    }
}
