using System;
using UnityEngine;
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
        public void Transfer(GameObject source, GameObject target)
        {
            Transform targetArmature = target.transform.Find(ARMATURE_NAME) ?? target.transform;
            Transform sourceArmature = source.transform.Find(ARMATURE_NAME) ?? source.transform;

            RemoveMeshes(targetArmature);
            TransferMeshes(targetArmature, sourceArmature);

            Object.Destroy(source);
        }

        /// Remove all meshes from the target armature
        private void RemoveMeshes(Transform targetArmature)
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

        /// Set meshes from source armature to target armature
        private void TransferMeshes(Transform targetArmature, Transform sourceArmature)
        {
            Transform rootBone = targetArmature.Find(HIPS_BONE_NAME);
            Transform[] bones = rootBone != null ? GetBones(targetArmature) : Array.Empty<Transform>();
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

            rootBone.SetAsLastSibling();
        }

        /// Get bones from the target armature
        private Transform[] GetBones(Transform targetArmature)
        {
            SkinnedMeshRenderer sampleMesh = targetArmature.GetComponentsInChildren<SkinnedMeshRenderer>()[0];
            Transform[] bones = sampleMesh.bones;
            return bones;
        }
    }
}