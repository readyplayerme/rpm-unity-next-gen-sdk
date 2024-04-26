using UnityEngine;

namespace ReadyPlayerMe.Runtime.Utils
{
    public static class MeshUtils 
    {
        private const string ARMATURE_NAME = "Armature";
        private const string HIPS_BONE_NAME = "Hips";
        
        public static void TransferMesh(GameObject source, GameObject target)
        {
            Transform targetArmature = target.transform.Find(ARMATURE_NAME);
            Transform sourceArmature = source.transform.Find(ARMATURE_NAME);
            
            RemoveCurrentMeshes(targetArmature);
            SetMeshes(targetArmature, sourceArmature);
            
            Object.Destroy(source);
        }

        private static void RemoveCurrentMeshes(Transform targetArmature)
        {
            int childCount = targetArmature.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform mesh = targetArmature.GetChild(i);
                if (targetArmature.GetChild(i).name != HIPS_BONE_NAME)
                {
                    Object.Destroy(mesh.gameObject);
                }
            }
        }
        
        private static void SetMeshes(Transform targetArmature, Transform sourceArmature)
        {
            Transform rootBone = targetArmature.Find(HIPS_BONE_NAME);
            Transform[] bones = GetBones(targetArmature);
            Renderer[] sourceRenderers = sourceArmature.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in sourceRenderers)
            {
                renderer.gameObject.transform.SetParent(targetArmature);

                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    skinnedMeshRenderer.rootBone = rootBone;
                    skinnedMeshRenderer.bones = bones;
              
                    skinnedMeshRenderer.sharedMesh.RecalculateBounds();
                }
            }
        }
        
        private static Transform[] GetBones(Transform targetArmature)
        {
            SkinnedMeshRenderer sampleMesh = targetArmature.GetComponentsInChildren<SkinnedMeshRenderer>()[0];
            Transform[] bones = sampleMesh.bones;
            return bones;
        }
    }
}
