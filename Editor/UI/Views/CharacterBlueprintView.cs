using System.Linq;
using PlayerZero.Data;
using System.Threading.Tasks;
using PlayerZero.Api.V1;
using PlayerZero.Editor.UI.Components;
using PlayerZero.Editor.UI.ViewModels;
using UnityEditor;
using UnityEngine;

namespace PlayerZero.Editor.UI.Views
{
    public class CharacterBlueprintView
    {
        private readonly CharacterBlueprintViewModel _viewModel;
        private readonly ObjectInput<SkeletonDefinition> _boneDefinitionInput;
        private ObjectInput<GameObject> _defaultTemplatePrefab;
        private CharacterTemplateConfig _characterTemplateConfig;
        private string characterBlueprintId;
        public CharacterBlueprintView(CharacterBlueprintViewModel viewModel)
        {
            _viewModel = viewModel;
            _boneDefinitionInput = new ObjectInput<SkeletonDefinition>();
            _defaultTemplatePrefab = new ObjectInput<GameObject>();
        }

        public async Task Init(CharacterBlueprint characterBlueprint, CharacterTemplateConfig characterTemplateConfig)
        {
            await _viewModel.Init(characterBlueprint);
            characterBlueprintId = characterBlueprint.Id;
            _characterTemplateConfig = characterTemplateConfig;
            _characterTemplateConfig.GetTemplatePrefabGUID(characterBlueprintId);
            _defaultTemplatePrefab.Init(_characterTemplateConfig.GetTemplatePrefabGUID(characterBlueprintId));
            _boneDefinitionInput.Init(_viewModel.BoneDefinitionCacheId);
        }

        public void Render()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(new GUIStyle()
                       {
                           fixedWidth = 120
                       }))
                {
                    using (new EditorGUILayout.HorizontalScope(new GUIStyle()
                           {
                               normal = new GUIStyleState()
                               {
                                   background = Texture2D.grayTexture,
                               },
                               margin = new RectOffset(5, 5, 5, 5),
                           }))
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(_viewModel.Image,
                            new GUIStyle()
                            {
                                stretchWidth = true,
                                stretchHeight = true,
                                fixedWidth = 90,
                                fixedHeight = 90,
                                alignment = TextAnchor.MiddleCenter,
                            });
                        GUILayout.FlexibleSpace();
                    }

                    if (GUILayout.Button("Load Blueprint"))
                    {
#pragma warning disable CS4014
                        _viewModel.LoadBlueprintAsync();
#pragma warning restore CS4014
                    }
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("ID: " + _viewModel.CharacterBlueprint.Id, EditorStyles.label);
                    GUILayout.Space(3); 
                    EditorGUILayout.LabelField("Default Template Prefab", EditorStyles.boldLabel);
                    _defaultTemplatePrefab.Render(OnTemplatePrefabChange);
                }
            }
        }
        
        private void OnTemplatePrefabChange(GameObject newDefaultTemplatePrefab)
        {
            if (newDefaultTemplatePrefab == null)
            {
                Debug.LogWarning("No template prefab provided.");
                return;
            }
            
            Debug.Log($"Template prefab changed to {newDefaultTemplatePrefab.name}");
            
            // Get the template from the _characterTemplateConfig
            var template = _characterTemplateConfig.GetTemplate(characterBlueprintId);
            if (template == null)
            {
                Debug.LogWarning($"Template with ID {characterBlueprintId} not found.");
                return;
            }

            // Convert the Prefabs array to a list to make it easier to modify
            var prefabList = template.Prefabs.ToList();
            
            // Find if the prefab already exists in the list
            var existingPrefabIndex = prefabList.FindIndex(p => p.Prefab == newDefaultTemplatePrefab);

            if (existingPrefabIndex == 0)
            {
                // The prefab is already the first element, do nothing
            }
            else
            {
                if (existingPrefabIndex > 0)
                {
                    // Case 2: The prefab exists but is not in the first position
                    Debug.Log($"Prefab exists at index {existingPrefabIndex}, moving to the front.");
                    // Remove it from its current position
                    var prefabToMove = prefabList[existingPrefabIndex];
                    prefabList.RemoveAt(existingPrefabIndex);
                    // Insert it at the first position
                    prefabList.Insert(0, prefabToMove);
                }
                else
                {
                    // Case 3: The prefab does not exist in the list, so add it at the front
                    Debug.Log("Prefab does not exist in the list, adding as the first element.");
                    prefabList.Insert(0, new BlueprintPrefab()
                    {
                        Prefab = newDefaultTemplatePrefab,
                        Tags = new[] { "" }
                    });
                }
            }

            template.Prefabs = prefabList.ToArray();
            EditorUtility.SetDirty(_characterTemplateConfig);
            AssetDatabase.Refresh();
        }
    }
}