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
            SetTimeScale(isPaused ? 0f : 1f);
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

        public void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
        }
    }
}