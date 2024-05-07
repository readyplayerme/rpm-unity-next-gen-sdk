using UnityEngine;

namespace ReadyPlayerMe.Runtime.Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Ready Player Me/Settings", order = 1)]
    public class Settings : ScriptableObject
    {
        [SerializeField]
        private string _apiBaseUrl = "https://api.readyplayer.me";

        public string ApiBaseAuthUrl = "https://readyplayer.me/api/auth";

        public string ApplicationId = "";

        /// <warning>
        /// Setting this property locally means that your Ready Player Me API Key will be present in your game build.
        /// It is our advice that this property should not be set, and instead you should set the ApiBaseUrl and ApiBaseAuthUrl to point
        /// to your own backend server which then makes requests to the Ready Player Me API.
        /// 
        /// However, it is up to you whether having your API key appear in your build is an acceptable risk as a trade off for the
        /// convenience of not having to run your own proxy backend server.
        /// </warning>
        [Header("Warning: Setting the API Key property can pose a security risk. See Setting.cs for more details.")]
        public string ApiKey = "";

        [SerializeField]
        private string _apiProxyUrl = "";

        public string ApiBaseUrl => string.IsNullOrEmpty(_apiProxyUrl) ? _apiBaseUrl : _apiProxyUrl;
    }
}
