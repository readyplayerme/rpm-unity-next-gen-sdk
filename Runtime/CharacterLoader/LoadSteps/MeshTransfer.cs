using System.Linq;
using UnityEngine;
using ReadyPlayerMe.Data;
using System.Collections.Generic;

namespace ReadyPlayerMe
{
    public class MeshTransfer
    {
        /// <summary>
        ///     Transfer meshes from source to target GameObject
        /// </summary>
        /// <param name="source">New character model</param>
        /// <param name="target">Character model existing in the scene</param>
        /// <param name="definition">Skeleton definition</param>
        public void Transfer(GameObject source, GameObject target, SkeletonDefinition definition = null)
        {
            Transform rootBone =
                target.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == definition?.Root) ??
                target.transform;

            var bones = GetBones(target.transform);
            RemoveMeshes(target.transform);
            TransferMeshes(target.transform, source.transform, rootBone, bones);

            Object.Destroy(source);
        }
        
        public void Transfer(GameObject source, CharacterData data, SkeletonDefinition definition = null)
        {
            Transform target = data.gameObject.transform;
            Transform rootBone =
                target.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == definition?.Root) ??
                target.transform;

            var bones = GetBones(target.transform);
            RemoveMeshes(target.transform);
            TransferMeshes(target.transform, source.transform, rootBone, bones);

            Object.Destroy(source);
        }

        /// Remove all meshes from the target armature
        private void RemoveMeshes(Transform targetArmature)
        {
            Renderer[] renderers = GetRenderers(targetArmature);
            foreach (Renderer renderer in renderers)
            {
                if (!renderer.gameObject.TryGetComponent<TemplateAttachment>(out _))
                    Object.Destroy(renderer.gameObject);
            }
        }

        /// Set meshes from source armature to target armature
        private void TransferMeshes(Transform targetArmature, Transform sourceArmature, Transform rootBone, Transform[] bones)
        {
            Renderer[] sourceRenderers = sourceArmature.GetComponentsInChildren<Renderer>();
            
            foreach (Renderer renderer in sourceRenderers)
            {
                Transform[] bonesCopy = new Transform[bones.Length];
                Transform[] sourceBones = GetBones(sourceArmature);
            
                for (int i = 0; i < bones.Length; i++)
                {
                    for(int j = 0; j < bones.Length; j++)
                    {
                        if(bones.Length <= j)
                            continue;

                        if (sourceBones.Length <= i)
                            continue;
                    
                        if (bones[j].name == sourceBones[i].name)
                        {
                            bonesCopy[i] = bones[j];
                            break;
                        }
                    }
                }
                
                renderer.gameObject.transform.SetParent(targetArmature);
                renderer.gameObject.transform.localPosition = Vector3.zero;
                renderer.gameObject.transform.localEulerAngles = Vector3.zero;

                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    skinnedMeshRenderer.rootBone = rootBone;
                    skinnedMeshRenderer.bones = bonesCopy;

                    skinnedMeshRenderer.sharedMesh.RecalculateBounds();
                }
            }

            if (rootBone != null)
                rootBone.SetAsLastSibling();
        }

        /// Get bones from the target armature
        private Transform[] GetBones(Transform targetArmature)
        {
            SkinnedMeshRenderer sampleMesh = targetArmature.GetComponentsInChildren<SkinnedMeshRenderer>()[0];
            return sampleMesh.bones;
        }

        private Renderer[] GetRenderers(Transform targetArmature)
        {
            List<Renderer> renderers = new List<Renderer>();
            GetRenderersRecursive(targetArmature, renderers);
            return renderers.ToArray();
        }

        private void GetRenderersRecursive(Transform parent, List<Renderer> renderers)
        {
            // Ignore from TemplateAttachment
            if (parent.GetComponent<TemplateAttachment>() != null)
            {
                return;
            }

            foreach (Transform child in parent)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderers.Add(renderer);
                }

                GetRenderersRecursive(child, renderers);
            }
        }
    }
}