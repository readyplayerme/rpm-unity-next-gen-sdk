using UnityEditor;
using UnityEngine;
using ReadyPlayerMe.Editor.UI.ViewModels;

namespace ReadyPlayerMe.Editor.UI.Views
{
    public class CacheCreatorView
    {
        private int cacheItemCount = 10;
        private string remoteCacheUrl = "";
        private bool isGenerating;
        private bool isDownloadingRemoteCache;
        
        private readonly CacheCreatorViewModel viewModel;

        public CacheCreatorView(CacheCreatorViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.viewModel.OnCacheGenerated += () =>
            {
                isGenerating = false;
            };
            this.viewModel.OnRemoteCacheDownloaded += () =>
            {
                isDownloadingRemoteCache = false;
            };
        }
        
        public void Render()
        {
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Label("Cache Creator", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(isGenerating);
            cacheItemCount = EditorGUILayout.IntSlider("Items Each Category", cacheItemCount, 1, 100);
            
            if (GUILayout.Button(isGenerating ? "[Generating cache, please wait...]" : "Generate Cache to Streaming Assets Folder"))
            {
                viewModel.GenerateCache(cacheItemCount);
                isGenerating = true;
            }
            
            GUILayout.Space(10);
            if (GUILayout.Button("Extract Cache to Local Folder"))
            {
                viewModel.ExtractCache();
            }
            if (GUILayout.Button("Open Local Cache Folder"))
            {
                viewModel.OpenCacheFolder();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Label("Get Cache from Remote URL", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(isDownloadingRemoteCache);
            remoteCacheUrl = EditorGUILayout.TextField("Cache URL", remoteCacheUrl);
        
            if (GUILayout.Button(isDownloadingRemoteCache ? "[Downloading remote cache, please wait...]" : "Download Remote Cache"))
            {
                if (string.IsNullOrEmpty(remoteCacheUrl))
                {
                    Debug.LogWarning("Cache URL cannot be empty.");
                }
                else
                {
                    isDownloadingRemoteCache = true;
                    viewModel.DownloadAndExtractRemoveCache(remoteCacheUrl);
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }
}
