using UnityEngine;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class QuitGame : MonoBehaviour
    {
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            Quit();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            // if in editor, stop playing
            UnityEditor.EditorApplication.isPlaying = false;
            return;
#endif
            Application.Quit();
        }
    }
}