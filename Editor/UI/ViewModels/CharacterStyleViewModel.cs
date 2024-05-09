using System;
using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Cache;
using ReadyPlayerMe.Data.V1;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterStyleViewModel
    {
        public Asset CharacterStyle { get; set; }

        public string CacheId { get; private set; }

        public Texture2D Image { get; private set; }

        private CharacterStyleCache _characterStyleCache;
        private CharacterStyleTemplateCache _characterStyleTemplateCache;
        private FileApi _fileApi;

        public async Task Init(Asset characterStyle)
        {
            _characterStyleCache = new CharacterStyleCache();
            _characterStyleTemplateCache = new CharacterStyleTemplateCache();
            
            CharacterStyle = characterStyle;
            CacheId = _characterStyleTemplateCache.GetCacheId(CharacterStyle.Id);

            _fileApi = new FileApi();
            Image = await _fileApi.DownloadImageAsync(CharacterStyle.IconUrl);
        }

        public async Task LoadStyleAsync()
        {
            try
            {
                var bytes = await _fileApi.DownloadFileIntoMemoryAsync(CharacterStyle.GlbUrl);

                await _characterStyleCache.Save(bytes, CharacterStyle.Id);

                var character = _characterStyleCache.Load(CharacterStyle.Id);
                PrefabUtility.InstantiatePrefab(character); ;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void SaveTemplate(Object templateObject)
        {
            _characterStyleTemplateCache.Save(templateObject, CharacterStyle.Id);
        }
    }
}