using UnityEngine;

namespace PlayerZero.Data
{
    /// <summary>
    ///  This component is attached to the character game object to store character metadata.
    /// </summary>
    public class CharacterData: MonoBehaviour
    {
        public string Id { get; private set; }
        
        public string BlueprintId { get; private set; }
        
        /// <summary>
        /// Initialize character data with the given id.
        /// </summary>
        /// <param name="id">Ready Player Me Character ID.</param>
        /// <param name="blueprintId">Ready Player Me Character blueprint ID.</param>
        public CharacterData Initialize(string id, string blueprintId)
        {
            Id = id;
            BlueprintId = blueprintId;

            return this;
        }
    }
}
