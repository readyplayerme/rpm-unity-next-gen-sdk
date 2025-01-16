using System;
using System.Collections.Generic;

namespace PlayerZero
{
    [Serializable]
    public class CharacterLoaderConfig 
    {
        public int meshLod = 0;

        public string textureAtlas = "none";
        
        public string textureQuality = "high";

        public int textureSizeLimit = 1024;

        public List<string> morphTargets = new List<string>
        {
            "none",
        };

        public List<string> morphTargetsGroup = new List<string>();
    }
}