using System;
using Newtonsoft.Json;
using ReadyPlayerMe.Api.V1;
using System.Collections.Generic;

namespace ReadyPlayerMe
{
    public class CachedAsset
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("glbUrl")]
        public Dictionary<string, string> GlbUrls { get; set; }
        
        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
        
        public static implicit operator CachedAsset(Asset cachedAsset)
        {
            return new CachedAsset()
            {
                Id = cachedAsset.Id,
                Name = cachedAsset.Name,
                IconUrl = cachedAsset.IconUrl,
                Type = cachedAsset.Type,
                CreatedAt = cachedAsset.CreatedAt,
                UpdatedAt = cachedAsset.UpdatedAt
            };
        }
    }
}