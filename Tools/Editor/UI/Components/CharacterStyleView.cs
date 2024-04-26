using System.Linq;
using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Tools.Editor.Cache;
using ReadyPlayerMe.Tools.Editor.Data;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Tools.Editor.UI.Components
{
    public class CharacterStyleView
    {
        public async Task Render(CharacterStyle characterStyle)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = Texture2D.grayTexture,
                },
                margin = new RectOffset(5, 5, 5, 5)
            });
            GUILayout.FlexibleSpace();
            GUILayout.Label(characterStyle.Image,
                new GUIStyle()
                {
                    stretchWidth = true,
                    stretchHeight = true,
                    fixedWidth = 120,
                    fixedHeight = 120,
                    alignment = TextAnchor.MiddleCenter,
                });
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Load Style"))
            {
                var gltf = new GltfImport(deferAgent: new UninterruptedDeferAgent());
                await gltf.Load(characterStyle.GlbUrl);

                var gameObject = new GameObject();
                gameObject.name = characterStyle.Id;
                await gltf.InstantiateSceneAsync(gameObject.transform);
            }

            GUILayout.Label("Template");
            characterStyle.ObjectInput.Render(onChange: o =>
            {
                var data = CharacterStyleTemplateCache.Data;
                var matchInCache = data.FirstOrDefault(p => p.CharacterStyleId == characterStyle.Id);

                var assetPath = AssetDatabase.GetAssetPath(o);
                var guid = AssetDatabase.AssetPathToGUID(assetPath);

                if (matchInCache == null)
                {
                    var template = new CharacterStyleTemplate
                    {
                        Id = guid,
                        CharacterStyleId = characterStyle.Id
                    };

                    data.Add(template);
                }
                else
                {
                    matchInCache.Id = guid;
                }

                CharacterStyleTemplateCache.Data = data;
            });

            EditorGUILayout.EndVertical();
        }
    }
}