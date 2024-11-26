using System.Collections.Generic;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Demo;
using ReadyPlayerMe.Samples.AvatarCreator;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class CreatorMenuController : MonoBehaviour
    {
        [FormerlySerializedAs("categoryPanel"),SerializeField] private CategoryTextButtonPanel categoryTextButtonPanel;
        [SerializeField] private AssetPanel assetPanelPrefab;
        [SerializeField] private Transform assetPanelContainer;
        [SerializeField] private GameObject loadingCanvas;
        [SerializeField] private Animation menuAnimation;
        [FormerlySerializedAs("dragRotate"),SerializeField] private DragRotatePanel dragRotatePanel;

        private AssetPanel activeAssetPanel;
        private Dictionary<string, AssetPanel> assetPanelMap = new Dictionary<string, AssetPanel>();

        [Header("Events")]
        public UnityEvent<Asset> OnAssetSelected;
        public UnityEvent<Asset> OnAssetRemoved;

        private async void Start()
        {
            categoryTextButtonPanel.LoadCategories();
        }

        public async void CreatePanelsFromCategories(string[] categories)
        {
            var indexCount = 0;
            foreach (var category in categories)
            {
                var assetPanel = Instantiate(assetPanelPrefab, assetPanelContainer);
                assetPanel.LoadAssetsOfCategory(category);
                assetPanel.OnAssetSelected += asset => OnAssetSelected.Invoke(asset);
                assetPanel.OnAssetRemoved += asset => OnAssetRemoved.Invoke(asset);
                if (indexCount == 0)
                {
                    activeAssetPanel = assetPanel;
                }
                else
                {
                    assetPanel.gameObject.SetActive(false);
                }
                assetPanelMap.Add(category, assetPanel);

                indexCount++;
            }
        }

        public void ShowAssetPanel(string category)
        {
            if (activeAssetPanel != null)
            {
                activeAssetPanel.gameObject.SetActive(false);
            }
            if (assetPanelMap.ContainsKey(category))
            {
                activeAssetPanel = assetPanelMap[category];
                activeAssetPanel.gameObject.SetActive(true);
                activeAssetPanel.CheckForAssetUpdates();
            }
        }
    }
}
