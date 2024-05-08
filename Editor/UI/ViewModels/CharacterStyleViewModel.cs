using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Runtime.Api.V1;
using ReadyPlayerMe.Runtime.Cache;
using ReadyPlayerMe.Runtime.Data.V1;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterStyleViewModel
    {
        public Asset CharacterStyle { get; set; }
        
        public string CacheId { get; private set; }
        
        public Texture2D Image { get; private set; }

        public async Task Init(Asset characterStyle)
        {
            CharacterStyle = characterStyle;
            CacheId = CharacterStyleTemplateCache.GetCacheId(CharacterStyle.Id);

            var imageApi = new ImageApi();

            Image = await imageApi.DownloadImageAsync(CharacterStyle.IconUrl);
        }
        
        public async Task LoadStyleAsync()
        {
            var gltf = new GltfImport(deferAgent: new UninterruptedDeferAgent());
            await gltf.Load(CharacterStyle.GlbUrl);

            var gameObject = new GameObject();
            gameObject.name = CharacterStyle.Id;
            await gltf.InstantiateSceneAsync(gameObject.transform);
        }

        public void SaveTemplate(Object templateObject)
        {
            CharacterStyleTemplateCache.Save(templateObject, CharacterStyle.Id);
        }
    }
}