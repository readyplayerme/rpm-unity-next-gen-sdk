using ReadyPlayerMe.Samples.QuickStart;
using UnityEngine;
using UnityEngine.Events;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class PausePanel : MonoBehaviour
    {
        public UnityEvent OnPause;
        public UnityEvent OnResume;
        private PlayerInput playerInput;
        
        public void TogglePause()
        {
            var isPaused = !gameObject.activeSelf;
            gameObject.SetActive(isPaused);
            if (playerInput != null)
            {
                playerInput.enabled = !isPaused;
            }
            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                
                OnPause.Invoke();
                return;
            }
            OnResume.Invoke();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        public void OnCharacterLoaded(GameObject character)
        {
            playerInput = character.GetComponent<PlayerInput>();
        }
    }
}