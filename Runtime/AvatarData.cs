using UnityEngine;

namespace ReadyPlayerMe.Runtime
{
    /// <summary>
    ///  This component is attached to the avatar game object to store avatar metadata.
    /// </summary>
    public class AvatarData: MonoBehaviour
    {
        public string Id { get; private set; }
        
        private bool isInitialized;
        
        /// <summary>
        ///     Initialize avatar data with the given url.
        /// </summary>
        /// <param name="url">Ready Player Me Avatar URL.</param>
        public void Initialize(string id)
        {
            if (isInitialized)
            {
                Debug.LogWarning("AvatarData is already initialized.");    
                return;
            }

            Id = id;
            
            isInitialized = true;
        }
    }
}
