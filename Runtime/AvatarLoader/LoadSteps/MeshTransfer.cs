using System;
using UnityEngine;
using ReadyPlayerMe.Data;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe.AvatarLoader
{
    public class MeshTransfer
    {
        private const string ARMATURE_NAME = "Armature";
        private const string HIPS_BONE_NAME = "Hips";
        
        /// <summary>
        ///     Transfer meshes from source to target GameObject
        /// </summary>
        /// <param name="source">New avatar model</param>
        /// <param name="target">Avatar model existing in the scene</param>
        public void Transfer(GameObject source, GameObject target, AvatarSkeletonDefinition definition = null)
        {
            string hipsBoneName = definition?.BoneGroups[0].BonesValues[0] ?? HIPS_BONE_NAME;
            string armatureName = definition?.Root ?? ARMATURE_NAME;
            
            Transform sourceArmature = source.transform.Find(ARMATURE_NAME);
            Transform targetArmature = target.transform.Find(armatureName);
            
            RemoveMeshes(targetArmature, hipsBoneName);
            TransferMeshes(targetArmature, sourceArmature, hipsBoneName);

            Object.Destroy(source);
        }

        /// Remove all meshes from the target armature
        private void RemoveMeshes(Transform targetArmature, string hipBoneName)
        {
            int childCount = targetArmature.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform mesh = targetArmature.GetChild(i);
                if (targetArmature.GetChild(i).name != hipBoneName)
                {
                    Object.Destroy(mesh.gameObject);
                }
            }
        }

        /// Set meshes from source armature to target armature
        private void TransferMeshes(Transform targetArmature, Transform sourceArmature, string hipBoneName)
        {
            Transform rootBone = targetArmature.Find(hipBoneName);
            Transform[] bones = rootBone != null ? GetBones(targetArmature) : Array.Empty<Transform>();
            Renderer[] sourceRenderers = sourceArmature.parent.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in sourceRenderers)
            {
                renderer.gameObject.transform.SetParent(targetArmature);
                renderer.gameObject.transform.localPosition = Vector3.zero;
                renderer.gameObject.transform.localEulerAngles = Vector3.zero;

                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    skinnedMeshRenderer.rootBone = rootBone;
                    skinnedMeshRenderer.bones = bones;

                    skinnedMeshRenderer.sharedMesh.RecalculateBounds();
                }
            }

            if (rootBone != null)
                rootBone.SetAsLastSibling();
        }

        /// Get bones from the target armature
        private Transform[] GetBones(Transform targetArmature)
        {
            SkinnedMeshRenderer sampleMesh = targetArmature.parent.GetComponentsInChildren<SkinnedMeshRenderer>()[0];
            Transform[] bones = sampleMesh.bones;
            return bones;
        }
    }
}
