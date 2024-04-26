using UnityEngine;

namespace ReadyPlayerMe.Runtime.Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Ready Player Me/Settings", order = 1)]
    public class Settings : ScriptableObject
    {
        public string ApiBaseUrl = "https://api.readyplayer.me/v1/";

        public string ApplicationId = "";
        
        [TextArea(5, 10)] public string Token = "";
    }
}
