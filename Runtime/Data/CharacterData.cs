using UnityEngine;

namespace ReadyPlayerMe.Data
{
    /// <summary>
    ///  This component is attached to the character game object to store character metadata.
    /// </summary>
    public class CharacterData: MonoBehaviour
    {
        public string Id { get; private set; }
        
        public string StyleId { get; private set; }

        /// <summary>
        /// Initialize character data with the given id.
        /// </summary>
        /// <param name="id">Ready Player Me Character ID.</param>
        /// <param name="styleId">Ready Player Me Character style ID.</param>
        public CharacterData Initialize(string id, string styleId)
        {
            Id = id;
            StyleId = styleId;

            return this;
        }
    }
}
