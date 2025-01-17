using UnityEngine;

namespace PlayerZero
{
    public class CachePaths
    {
        public readonly static string CACHE_ROOT = Application.persistentDataPath + "/Ready Player Me/Local Cache/";
        public readonly static string CACHE_ASSET_ROOT = CACHE_ROOT + "Assets/";
        public readonly static string CACHE_ASSET_ICON_PATH = CACHE_ASSET_ROOT + "Icons/";
    }
}
