using UnityEngine;

namespace ReadyPlayerMe
{
    /// <summary>
    /// Attribute to mark a property as read-only so that it is visible in the inspector but not settable.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}