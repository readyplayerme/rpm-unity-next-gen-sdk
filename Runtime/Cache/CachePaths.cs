using UnityEngine;

namespace ReadyPlayerMe
{
    public class CachePaths
    {
        // User Local Cache Paths
        public readonly static string CACHE_ROOT = Application.persistentDataPath + "/Ready Player Me/Local Cache/";
        public readonly static string CACHE_ASSET_ROOT = CACHE_ROOT + "Assets/";
        public readonly static string CACHE_ASSET_JSON_PATH = CACHE_ASSET_ROOT + "assets.json";
        public readonly static string CACHE_TYPES_JSON_PATH = CACHE_ASSET_ROOT + "types.json";
        public readonly static string CACHE_ASSET_ZIP_PATH = CACHE_ROOT + "Assets.zip";
        public readonly static string CACHE_ASSET_ICON_PATH = CACHE_ASSET_ROOT + "Icons/";
        
        // Project Cache Paths
        public readonly static string PROJECT_CACHE_ROOT = Application.streamingAssetsPath + "/Ready Player Me/Local Cache/";
        public readonly static string PROJECT_CACHE_ASSET_ROOT = PROJECT_CACHE_ROOT + "Assets/";
        public readonly static string PROJECT_CACHE_ASSET_JSON_PATH = PROJECT_CACHE_ASSET_ROOT + "assets.json";
        public readonly static string PROJECT_CACHE_TYPES_JSON_PATH = PROJECT_CACHE_ASSET_ROOT + "types.json";
        public readonly static string PROJECT_CACHE_ASSET_ZIP_PATH = PROJECT_CACHE_ROOT + "Assets.zip";
        public readonly static string PROJECT_CACHE_ASSET_ICON_PATH = PROJECT_CACHE_ASSET_ROOT + "Icons/";
    }
}
