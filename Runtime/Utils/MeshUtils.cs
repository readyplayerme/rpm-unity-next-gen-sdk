using System.Linq;
using UnityEngine;
using ReadyPlayerMe.Runtime.Loader;

namespace ReadyPlayerMe.Runtime.Utils
{
    public class MeshUtils 
    {
        public static void TransferMesh(GameObject source, GameObject target)
        {
            Renderer[] targetRenderers = target.transform.Find("Armature").GetComponentsInChildren<Renderer>().Where(renderer => renderer.GetComponent<AvatarAttachment>() == null).ToArray();
            Transform[] bones = (targetRenderers[0] as SkinnedMeshRenderer).bones.Where(bone => bone.GetComponent<AvatarAttachment>() == null).ToArray();
        
            foreach (Renderer renderer in targetRenderers)
            {
                Object.Destroy(renderer.gameObject);
            }
        
            Renderer[] sourceRenderers = source.transform.Find("Armature").GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in sourceRenderers)
            {
                renderer.gameObject.transform.SetParent(target.transform.Find("Armature"));

                if (renderer is SkinnedMeshRenderer)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
                    skinnedMeshRenderer.rootBone = target.transform.Find("Armature/Hips");
                    skinnedMeshRenderer.bones = bones;
              
                    skinnedMeshRenderer.sharedMesh.RecalculateBounds();
                }
            }
            
            Object.Destroy(source);
        }
    }
}
