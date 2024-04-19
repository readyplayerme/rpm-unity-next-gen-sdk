using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Data.V1
{
    public class Character
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("organizationId")]
        public string OrganizationId { get; set; }

        [JsonProperty("glbUrl")]
        public string GlbUrl { get; set; }
        
        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }
        
        [JsonProperty("assets")]
        public IDictionary<string, string> Assets { get; set; }
        
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}