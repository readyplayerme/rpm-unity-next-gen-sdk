using System;
using Newtonsoft.Json;

namespace PlayerZero.Api.V1
{
    [Serializable]
    public class CharacterBlueprint
    {
        [JsonProperty("name")]
        public string Name;
                
        [JsonProperty("id")]
        public string Id;
                
        [JsonProperty("characterModel")]
        public BlueprintCharacterModel CharacterModel { get; set; } = new BlueprintCharacterModel();
                
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
                
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
    
    [Serializable]
    public class BlueprintCharacterModel
    {
        [JsonProperty("name")]
        public string Name;
        
        [JsonProperty("id")]
        public string Id;
        
        [JsonProperty("modelUrl")]
        public string ModelUrl;
        
        [JsonProperty("iconUrl")]
        public string IconUrl;
        
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
