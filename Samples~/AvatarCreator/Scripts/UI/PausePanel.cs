using UnityEngine;
using UnityEngine.Events;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class PausePanel : MonoBehaviour
    {
        public UnityEvent OnPause;
        public UnityEvent OnResume;
        
        public void TogglePause()
        {
            var isPaused = !gameObject.activeSelf;
            gameObject.SetActive(isPaused);
            Time.timeScale = isPaused ? 0 : 1;
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
    }
}