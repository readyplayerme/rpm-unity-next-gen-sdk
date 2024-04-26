using UnityEngine;
using ReadyPlayerMe.Runtime.Utils;

namespace ReadyPlayerMe.Runtime.Data
{
    /// <summary>
    ///     This component is attached to the avatar game object to store avatar metadata.
    /// </summary>
    public class AvatarData: MonoBehaviour
    {
        public string Id { get; private set; }
        
        private bool isInitialized;
        
        /// <summary>
        ///     Initialize avatar data with the given url.
        /// </summary>
        /// <param name="url">Ready Player Me Avatar URL.</param>
        public void Initialize(string url)
        {
            if (isInitialized)
            {
                Debug.LogWarning("AvatarData is already initialized.");    
                return;
            }
            
            Id = UrlUtils.GetIdFromUrl(url);
            
            isInitialized = true;
        }
    }
}
