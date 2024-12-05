using UnityEngine;
using UnityEngine.Events;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class EscapeListener : MonoBehaviour
    {
        public UnityEvent OnEscapeEvent;
        
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            OnEscapeEvent.Invoke();
        }
    }
}