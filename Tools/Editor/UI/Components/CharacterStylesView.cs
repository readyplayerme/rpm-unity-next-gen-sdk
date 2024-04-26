using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.V1.Assets.Models;
using ReadyPlayerMe.Runtime.Api.V1.Image;
using ReadyPlayerMe.Runtime.Data.ScriptableObjects;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts;
using ReadyPlayerMe.Tools.Editor.Cache;
using ReadyPlayerMe.Tools.Editor.UI.Components.Common;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace ReadyPlayerMe.Tools.Editor.UI.Components
{
    public class CharacterStylesView
    {
        private IList<CharacterStyle> _characterStyles;
        private bool _loading;

        private readonly DeveloperAccountApi _developerAccountApi;

        public CharacterStylesView(DeveloperAccountApi developerAccountApi)
        {
            _developerAccountApi = developerAccountApi;
        }

        public async Task Init()
        {
            _loading = true;

            var settings = Resources.Load<Settings>("Settings");

            if (string.IsNullOrEmpty(settings.ApplicationId))
            {
                _characterStyles = new List<CharacterStyle>();
                _loading = false;
                return;
            }

            var response = await _developerAccountApi.ListCharacterStylesAsync(new AssetListRequest
            {
                Params = new AssetListQueryParams
                {
                    ApplicationId = settings.ApplicationId,
                    Type = "baseModel"
                }
            });

            var imageApi = new ImageApi();

            _characterStyles = (
                await Task.WhenAll(response.Data.Select(async asset =>
                    {
                        var data = CharacterStyleTemplateCache.Data;

                        var stylesCache =
                            data?.FirstOrDefault(template => template.CharacterStyleId == asset.Id);

                        var obj = new ObjectInput();

                        if (stylesCache == null)
                            return new CharacterStyle()
                            {
                                Id = asset.Id,
                                Image = await imageApi.DownloadImageAsync(asset.IconUrl),
                                ObjectInput = obj,
                                GlbUrl = asset.GlbUrl
                            };

                        obj.Init(stylesCache.Id);

                        return new CharacterStyle()
                        {
                            Id = asset.Id,
                            Image = await imageApi.DownloadImageAsync(asset.IconUrl),
                            ObjectInput = obj,
                            GlbUrl = asset.GlbUrl
                        };
                    }
                ))).ToList();

            _loading = false;
        }

        public async Task Render()
        {
            if (_loading)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Character styles loading...", new GUIStyle()
                    {
                        alignment = TextAnchor.MiddleCenter,
                        normal = new GUIStyleState()
                        {
                            textColor = Color.white
                        }
                    });
                    GUILayout.FlexibleSpace();
                }

                return;
            }

            GUILayout.Label("Character Styles", new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                },
                margin = new RectOffset(10, 10, 0, 0)
            });

            if (!_loading && _characterStyles?.Count <= 0)
            {
                GUILayout.Label("You have no character styles setup for this application.",
                    new GUIStyle(GUI.skin.label)
                    {
                        margin = new RectOffset(9, 10, 0, 0)
                    });

                if (GUILayout.Button("Create first style in Studio", new GUIStyle(GUI.skin.button)
                    {
                        normal =
                        {
                            background = Texture2D.grayTexture,
                            textColor = Color.white
                        },
                        margin = new RectOffset(12, 12, 5, 0),
                        alignment = TextAnchor.MiddleCenter,
                    }))
                {
                    Application.OpenURL("https://studio.readyplayer.me/");
                }

                return;
            }

            GUILayout.Label("Here you can configure the in-game templates for each of your character styles",
                new GUIStyle(GUI.skin.label)
                {
                    margin = new RectOffset(9, 10, 0, 0)
                });

            using (new GUILayout.VerticalScope(new GUIStyle
                   {
                       margin = new RectOffset(9, 9, 5, 5)
                   }))
            {
                var windowWidth = EditorGUIUtility.currentViewWidth - 18;

                for (var x = 0; x < (_characterStyles.Count / 3) + 1; x++)
                {
                    using (new GUILayout.HorizontalScope(new GUIStyle()
                           {
                               margin = new RectOffset(0, 0, 0, 10)
                           }))
                    {
                        for (var y = 0; y < 3; y++)
                        {
                            var index = x * 3 + y;

                            if (_characterStyles.Count <= index || _characterStyles[index] == null)
                            {
                                using (new GUILayout.VerticalScope())
                                {
                                    EditorGUILayout.Space(windowWidth / 3);
                                }

                                continue;
                            }

                            await new CharacterStyleView().Render(_characterStyles[index]);
                        }
                    }
                }
            }
        }
    }
}