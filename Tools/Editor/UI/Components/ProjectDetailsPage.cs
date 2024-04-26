using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Data.ScriptableObjects;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts;
using ReadyPlayerMe.Tools.Editor.Services;
using ReadyPlayerMe.Tools.Editor.UI.Components.Common;
using UnityEngine;
using Application = ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models.Application;

namespace ReadyPlayerMe.Tools.Editor.UI.Components
{
    public class CharacterStyle
    {
        public string Id { get; set; }

        public Texture2D Image { get; set; }
        
        public ObjectInput ObjectInput { get; set; }
        
        public string GlbUrl { get; set; }
    }

    public class ProjectDetailsPage
    {
        private readonly DeveloperAccountApi _developerAccountApi;
        
        private string _error;
        private bool _loading;

        private IList<Application> _applications;
        private readonly SelectInput _selectInput;

        private CharacterStylesView _characterStylesView;

        public ProjectDetailsPage(DeveloperAccountApi developerAccountApi)
        {
            _developerAccountApi = developerAccountApi;

            _selectInput = new SelectInput();
            _characterStylesView = new CharacterStylesView(_developerAccountApi);
        }

        public async Task Init()
        {
            _error = null;
            _loading = true;

            var applicationLoader = new ApplicationsLoaderService(_developerAccountApi);
            var result = await applicationLoader.LoadAsync();

            _error = result.Error;
            _applications = result.Data;

            if (!result.IsSuccess)
            {
                _loading = false;
                return;
            }

            var settings = Resources.Load<Settings>("Settings");

            if (result.Data.FirstOrDefault(p => p.Id == settings.ApplicationId) == null)
                settings.ApplicationId = string.Empty;
            
            _selectInput.Init(
                _applications
                    .ToList()
                    .Select(app => new Option()
                    {
                        Label = $"{app.Name} ({app.Id})",
                        Value = app.Id,
                    })
                    .ToArray(),
                settings.ApplicationId
            );

            await _characterStylesView.Init();
            _loading = false;
        }

        public async Task Render()
        {
            GUILayout.Space(15);

            if (_loading)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Loading...", new GUIStyle()
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

            GUILayout.Label("Project Settings", new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                },
                margin = new RectOffset(10, 10, 0, 0),
                fontSize = 14
            });

            GUILayout.Label("Select the Ready Player Me application to link to project", new GUIStyle(GUI.skin.label)
            {
                margin = new RectOffset(9, 10, 0, 0)
            });

            using (new GUILayout.HorizontalScope(new GUIStyle()
                   {
                       margin = new RectOffset(7, 7, 5, 0)
                   }))
            {
                _selectInput.Render(async (applicationId) =>
                {
                    var settings = Resources.Load<Settings>("Settings");
                    settings.ApplicationId = applicationId;

                    await _characterStylesView.Init();
                });
            }


            GUILayout.Label(_error, new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    textColor = Color.red
                },
                margin = new RectOffset()
                {
                    left = 5
                }
            });

            GUILayout.Space(10);

            await _characterStylesView.Render();
        }
    }
}