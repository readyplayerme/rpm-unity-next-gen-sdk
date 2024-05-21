using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ReadyPlayerMe.Api.V1;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;

    private Asset asset;

    public void Init(Asset asset, Action<Asset> onButtonClicked = null)
    {
        this.asset = asset;
        
        StartCoroutine(LoadImage(asset.IconUrl));
        
        if (onButtonClicked != null)
        {
            button.onClick.AddListener(() => onButtonClicked(asset));
        }
    }
    
    private IEnumerator LoadImage(string url)
    {
        var request = new WWW(url);
        yield return request;
        
        image.sprite = Sprite.Create(request.texture, new Rect(0, 0, request.texture.width, request.texture.height), Vector2.zero);
    }
}
