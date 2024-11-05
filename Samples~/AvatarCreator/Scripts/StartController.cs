using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ReadyPlayerMe.Demo
{
    public class StartController : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        private void Start()
        {
            startButton.onClick.AddListener(() =>
            {
                SceneManager.LoadSceneAsync(1);
            });
        }
    }
}
