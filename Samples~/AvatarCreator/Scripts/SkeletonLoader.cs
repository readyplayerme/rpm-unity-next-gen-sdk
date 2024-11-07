using System.Linq;
using ReadyPlayerMe;
using ReadyPlayerMe.Data;
using UnityEngine;

public class SkeletonLoader : MonoBehaviour
{
    [SerializeField]
    private string templateTagOrId = "";
    void Start()
    {
        CreateAndSetSkeleton();
    }

    private void CreateAndSetSkeleton()
    {
        var skeletonBuilder = new SkeletonBuilder();
        gameObject.SetActive(false);
        var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>("SkeletonDefinitionConfig")
            .definitionLinks
            .FirstOrDefault(p => p.characterStyleId == templateTagOrId)?
            .definition;

        gameObject.TryGetComponent<Animator>(out var animator);
        animator.enabled = false;
            
        var animationAvatar = animator.avatar;
        if (animationAvatar == null)
        {
            skeletonBuilder.Build(gameObject, skeletonDefinition != null
                ? skeletonDefinition.GetHumanBones()
                : null
            );
        }
                
        animator.enabled = true;
        gameObject.SetActive(true);
    }
}
