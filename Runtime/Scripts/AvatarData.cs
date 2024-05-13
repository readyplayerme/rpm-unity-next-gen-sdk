using UnityEngine;

namespace ReadyPlayerMe
{
    /// <summary>
    ///  This component is attached to the avatar game object to store avatar metadata.
    /// </summary>
    public class AvatarData: MonoBehaviour
    {
        public string Id { get; private set; }

        /// <summary>
        ///     Initialize avatar data with the given id.
        /// </summary>
        /// <param name="id">Ready Player Me Avatar ID.</param>
        public GameObject Initialize(string id)
        {
            Id = id;

            return gameObject;
        }
    }
}
