using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadePanel : MonoBehaviour
{
    private static readonly int FadeInTrigger = Animator.StringToHash("FadeIn");
    private static readonly int FadeOutTrigger = Animator.StringToHash("FadeOut");
    
    [SerializeField]
    private Animator animator;
    
    [SerializeField]
    private bool fadeInOnStart = true;
    [SerializeField]
    private string sceneNameToLoad;
    
    private void Start()
    {
        if(animator == null)
        {
            Debug.LogError("Animator is not set in SceneFadePanel");
            return;
        }
        if(fadeInOnStart)
        {
            Invoke(nameof(FadeIn), 0.1f);
        }
    }
    
    public void FadeIn()    
    {
        if (animator == null) return;
        animator.SetTrigger(FadeInTrigger);
    }

    public void FadeOutAndLoadScene()
    {
        if (animator == null) return;
        animator.SetTrigger(FadeOutTrigger);
    }

    public void OnFadeOutComplete()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
