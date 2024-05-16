using System.Linq;
using ReadyPlayerMe.Editor.UI.Components;
using ReadyPlayerMe.Editor.UI.ViewModels;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.Views
{
    public class CreateCharacterTemplateView
    {
        private readonly TextInput _tagInput;
        private readonly ObjectInput<GameObject> _templateInput;
        private readonly CreateCharacterTemplateViewModel _viewModel;

        public CreateCharacterTemplateView(CreateCharacterTemplateViewModel viewModel)
        {
            _tagInput = new TextInput();
            _templateInput = new ObjectInput<GameObject>();
            _viewModel = viewModel;
        }

        public void Render()
        {
            GUILayout.Label("Add new template");
            
            _templateInput.Render(template => { _viewModel.Template.template = template; }, "Template");
            _tagInput.Render("Tag", (tag) => { _viewModel.Tag = tag; });
            
            GUILayout.Space(2);
            
            if (GUILayout.Button("Add"))
            {
                _viewModel.Create();
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
                        left = 4, 
                    }
                });
            }
        }
    }
}