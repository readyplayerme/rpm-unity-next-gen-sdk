namespace ReadyPlayerMe 
{
    public static class CharacterLoaderConfigExtensions
    {
        public static string GetTextureAtlasString(this TextureAtlas textureAtlas)
        {
            return textureAtlas switch
            {
                TextureAtlas.None => "none",
                TextureAtlas.High => "1024",
                TextureAtlas.Medium => "512",
                TextureAtlas.Low => "256",
                _ => "none"
            };
        }
        
        public static string GetMeshLodString(this MeshLod meshLod)
        {
            return meshLod switch
            {
                MeshLod.High => "0",
                MeshLod.Medium => "1",
                MeshLod.Low => "2",
                _ => "0"
            };
        }
        
        public static string GetMorphTargetsString(this CharacterLoaderConfig loaderConfig)
        {
            return loaderConfig.MorphTargets.Count > 0 ? string.Join(",", loaderConfig.MorphTargets) : "none";
        }
        
        public static string GetMorphTargetsGroupString(this CharacterLoaderConfig loaderConfig)
        {
            return loaderConfig.MorphTargetsGroup.Count > 0 ? string.Join(",", loaderConfig.MorphTargetsGroup) : "none";
        }
        
        public static string BuildQueryParams(this CharacterLoaderConfig config)
        {
            var queryBuilder = new QueryBuilder();
            queryBuilder.AddKeyValue("meshLOD", config.MeshLod.GetMeshLodString());
            queryBuilder.AddKeyValue("textureAtlas", config.TextureAtlas.GetTextureAtlasString());
            queryBuilder.AddKeyValue("textureQuality", config.TextureQuality.ToString().ToLower());
            queryBuilder.AddKeyValue("textureSizeLimit", config.TextureSizeLimit.ToString());
            
            if (config.MorphTargets.Count == 0 && config.MorphTargetsGroup.Count == 0)
            {
                config.MorphTargets.Add("none");
            }
            if (config.MorphTargets.Count > 0)
            {
                queryBuilder.AddKeyValue("morphTargets", config.GetMorphTargetsString());
            }
            if (config.MorphTargetsGroup.Count > 0)
            {
                queryBuilder.AddKeyValue("morphTargetsGroup", config.GetMorphTargetsGroupString());
            }

            return queryBuilder.Query;
        }
    }
}