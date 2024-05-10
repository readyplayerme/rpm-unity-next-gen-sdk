using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class SimpleAvatarLoader
    {
        private readonly AvatarApi _avatarApi;
        private readonly MeshTransfer _meshTransfer;
        private readonly SkeletonBuilder _skeletonBuilder;

        public SimpleAvatarLoader()
        {
            _avatarApi = new AvatarApi();
            _meshTransfer = new MeshTransfer();
            _skeletonBuilder = new SkeletonBuilder();
        }

        public async Task<GameObject> LoadAsync(string id, SimpleAvatarLoadConfig config = null)
        {
            if (config == null)
            {
                var avatarResponse = await _avatarApi.FindAvatarByIdAsync(new AvatarFindByIdRequest()
                {
                    AvatarId = id,
                });

                config = new SimpleAvatarLoadConfig
                {
                    LoadFrom = avatarResponse.Data.GlbUrl,
                    CharacterStyleId = avatarResponse.Data.Assets["baseModel"]
                };
            }

            var gltf = new GltfImport();

            if (!await gltf.Load(config.LoadFrom))
                return null;

            var avatar = new GameObject(id);

            await gltf.InstantiateSceneAsync(avatar.transform);

            var template = Resources
                .Load<CharacterStyleTemplateReference>($"Character Templates Links/{config.CharacterStyleId}")?
                .characterStyleTemplate;

            if (!template)
            {
                avatar
                    .AddComponent<AvatarData>()
                    .Initialize(id);

                return avatar;
            }

            var instance = Object.Instantiate(template);

            // Update skeleton and transfer mesh
            _skeletonBuilder.Build(instance);
            _meshTransfer.Transfer(avatar, instance);

            // Set avatar data
            instance
                .AddComponent<AvatarData>()
                .Initialize(id);

            return instance;
        }
    }
}