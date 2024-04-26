using System;
using System.Threading.Tasks;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models;
using ReadyPlayerMe.Tools.Editor.Services;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Tools.Editor.UI.Components
{
    public class DeveloperLoginPage
    {
        private readonly DeveloperAccountApi _developerAccountApi;

        private string _inputText = "";
        private string _passwordText = "";

        private bool _loading;
        private string _error;

        public DeveloperLoginPage(DeveloperAccountApi developerAccountApi)
        {
            _developerAccountApi = developerAccountApi;
        }

        public async Task Render(Action onLogin)
        {
            using (new GUILayout.VerticalScope(new GUIStyle()
                   {
                       margin = new RectOffset(9, 9, 9, 0),
                   }))
            {
                EditorGUILayout.LabelField("Sign in with your Ready Player Me Studio account to start.");

                EditorGUILayout.Space(10);

                _inputText = EditorGUILayout.TextField("Username:", _inputText);

                EditorGUILayout.Space(5);

                _passwordText = EditorGUILayout.PasswordField("Password:", _passwordText);

                EditorGUILayout.Space(5);

                if (!string.IsNullOrEmpty(_error))
                {
                    GUILayout.Label(_error, new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            textColor = Color.red
                        },
                        margin = new RectOffset()
                        {
                            left = 5,
                            bottom = 5,
                        }
                    });
                }
            }

            if (GUILayout.Button(_loading ? "Loading..." : "Sign In", new GUIStyle(GUI.skin.button)
                {
                    margin = new RectOffset(12, 12, 0, 0)
                }))
            {
                _loading = true;

                var developerLoginService = new DeveloperLoginService(_developerAccountApi);
                var result = await developerLoginService.LoginAsync(new DeveloperLoginRequest
                {
                    Payload = new DeveloperLoginRequestBody
                    {
                        Email = _inputText,
                        Password = _passwordText
                    }
                });

                _error = result.Error;

                if (result.IsSuccess)
                    onLogin();

                _loading = false;
            }
        }
    }
}