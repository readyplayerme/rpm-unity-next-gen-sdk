using System;
using ReadyPlayerMe.Editor.UI.ViewModels;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.Views
{
    public class DeveloperLoginView
    {
        private readonly DeveloperLoginViewModel _viewModel;

        public DeveloperLoginView(DeveloperLoginViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Render(Action onLogin)
        {
            using (new GUILayout.VerticalScope(new GUIStyle()
                   {
                       margin = new RectOffset(9, 9, 9, 0),
                   }))
            {
                EditorGUILayout.LabelField("Sign in with your Ready Player Me Studio account to start.");

                EditorGUILayout.Space(10);

                _viewModel.Username = EditorGUILayout.TextField("Username:", _viewModel.Username);

                EditorGUILayout.Space(5);

                _viewModel.Password = EditorGUILayout.PasswordField("Password:", _viewModel.Password);

                EditorGUILayout.Space(5);
            }

            if (GUILayout.Button(_viewModel.Loading ? "Loading..." : "Sign In", new GUIStyle(GUI.skin.button)
                {
                    margin = new RectOffset(12, 12, 0, 0)
                }))
            {
                _viewModel.SignIn(onSuccess: onLogin);
            }
            
            if (!string.IsNullOrEmpty(_viewModel.Error))
            {
                GUILayout.Label(_viewModel.Error, new GUIStyle()
                {
                    normal = new GUIStyleState()
                    {
                        textColor = Color.red
                    },
                    margin = new RectOffset()
                    {
                        left = 12, 
                    }
                });
            }
        }
    }
}