using System.Collections.Generic;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Demo;
using ReadyPlayerMe.Samples.AvatarCreator;
using UnityEngine;
using UnityEngine.Events;

public class CreatorMenuController : MonoBehaviour
{
    [SerializeField] private CategoryPanel categoryPanel;
    [SerializeField] private AssetPanel assetPanelPrefab;
    [SerializeField] private Transform assetPanelContainer;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Animation menuAnimation;
    [SerializeField] private DragRotate dragRotate;
    public UnityEvent<Asset> OnAssetSelected;

    private AssetPanel activeAssetPanel;
    private Dictionary<string, AssetPanel> assetPanelMap = new Dictionary<string, AssetPanel>();
    
    private void Start()
    {
        categoryPanel.LoadCategories();
    }

    public void CreatePanelsFromCategories(string[] categories)
    {
        var indexCount = 0;
        foreach (var category in categories)
        {
            var assetPanel = Instantiate(assetPanelPrefab, assetPanelContainer);
            assetPanel.LoadAssetsOfCategory(category);
            assetPanel.OnAssetSelected += asset => OnAssetSelected.Invoke(asset);  
            if(indexCount == 0)
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
        }
    }
}