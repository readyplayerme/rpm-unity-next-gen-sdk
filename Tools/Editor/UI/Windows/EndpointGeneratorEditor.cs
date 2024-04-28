using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Tools.Editor.UI.Windows
{
    public class EndpointGeneratorEditor : EditorWindow
    {
        private string inputText = "";
        private static EndpointGenerator endpointGenerator;
    
        [MenuItem("Tools/Generate Endpoint")]
        public static void Generate()
        {
            var window = GetWindow<EndpointGeneratorEditor>("Generate Endpoint");
            window.maxSize = window.minSize = new Vector2(380, 120);
        
            endpointGenerator = new EndpointGenerator();
        }
    
        private void OnGUI()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.HelpBox("Generate an endpoint class from a model class", MessageType.Info);
        
            GUILayout.Space(10);
        
            inputText = EditorGUILayout.TextField("Endpoint Name:", inputText);

            if (GUILayout.Button("Create Endpoint"))
            {
                string path = endpointGenerator.Generate(inputText);
                AssetDatabase.Refresh();
                EditorUtility.RevealInFinder(path);
            }
        
            GUILayout.EndVertical();
        }
    }
}
