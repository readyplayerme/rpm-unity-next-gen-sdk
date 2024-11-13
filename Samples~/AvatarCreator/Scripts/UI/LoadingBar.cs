using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ReadyPlayerMe.Demo
{
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private RectTransform glow;

        private void Start()
        {
            StartCoroutine(Progress());
        }

        private IEnumerator Progress()
        {
            float progress = 0;
            while (progress < 0.5f)
            {
                progress += Time.deltaTime * 0.4f;
                fillImage.fillAmount = progress * 0.8f + 0.2f;
                glow.anchoredPosition = new Vector2(progress * 160, 0);

                yield return null;
            }
            
            while (progress < 0.9f)
            {
                progress += Time.deltaTime * 0.2f;
                fillImage.fillAmount = progress * 0.8f + 0.2f;
                glow.anchoredPosition = new Vector2(progress * 160, 0);

                yield return null;
            }
        }
    }
}
