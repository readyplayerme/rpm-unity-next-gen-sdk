
using UnityEngine;

public class SceneFadePanel : MonoBehaviour
{
    private static readonly int FadeInTrigger = Animator.StringToHash("FadeIn");
    private static readonly int FadeOutTrigger = Animator.StringToHash("FadeOut");
    private static readonly int OpaqueTrigger = Animator.StringToHash("Opaque");
    private static readonly int TransparentTrigger = Animator.StringToHash("Transparent");
    
    [SerializeField]
    private Animator animator;
    
    [SerializeField]
    private bool fadeInOnStart = true;
    [SerializeField]
    private int sceneIndexToLoad = 0;
    
    void Start()
    {
        if(animator == null)
        {
            Debug.LogError("Animator is not set in SceneFadePanel");
            return;
        }
        if(fadeInOnStart)
        {
            animator.SetTrigger(OpaqueTrigger);
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
        if(sceneIndexToLoad >= 0 && sceneIndexToLoad < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndexToLoad);
        }
    }
}
