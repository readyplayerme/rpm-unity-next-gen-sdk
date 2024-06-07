using System;
using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;

    private Asset asset;

    private void Awake()
    {
    }

    public void Init(Asset asset, Action<Asset> onButtonClicked = null)
    {
        this.asset = asset;
        
        LoadImage(asset.IconUrl);
        
        if (onButtonClicked != null)
        {
            button.onClick.AddListener(() => onButtonClicked(asset));
        }
    }
    
    private async void LoadImage(string url)
    {
        var texture = await FileApi.DownloadImageAsync(url);

        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
