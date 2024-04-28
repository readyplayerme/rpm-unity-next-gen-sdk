using System.Linq;
using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Runtime.Api.V1.Images;
using ReadyPlayerMe.Runtime.Data.V1;
using ReadyPlayerMe.Tools.Editor.Cache;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Tools.Editor.UI.ViewModels
{
    public class CharacterStyleViewModel
    {
        private Asset CharacterStyle { get; set; }
        
        public string CacheId { get; private set; }
        
        public Texture2D Image { get; private set; }

        public async Task Init(Asset characterStyle)
        {
            CharacterStyle = characterStyle;
            
            var data = CharacterStyleTemplateCache.Data;

            var stylesCache =
                data?.FirstOrDefault(template => template.CharacterStyleId == CharacterStyle.Id);
            
            CacheId = stylesCache?.Id;

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
            var data = CharacterStyleTemplateCache.Data;
            var matchInCache = data.FirstOrDefault(p => p.CharacterStyleId == CharacterStyle.Id);
            var assetPath = AssetDatabase.GetAssetPath(templateObject);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);

            if (matchInCache == null)
            {
                var template = new CharacterStyleTemplate
                {
                    Id = guid,
                    CharacterStyleId = CharacterStyle.Id
                };

                data.Add(template);
            }
            else
            {
                matchInCache.Id = guid;
            }

            CharacterStyleTemplateCache.Data = data;
        }
    }
}