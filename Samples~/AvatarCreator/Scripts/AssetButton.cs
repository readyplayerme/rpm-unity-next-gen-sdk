using System;
using ReadyPlayerMe.Api.V1;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class AssetButton : MonoBehaviour, IPointerClickHandler
    {
        public Action<AssetButton> OnAssetSelected;
        
        [SerializeField] private Image buttonImage;
        [SerializeField] private Image iconImage;
        
        private bool isSelected;
        
        private Asset asset;
        private FileApi fileApi;
        
        public void Initialize(Asset asset)
        {
            fileApi = new FileApi();
            this.asset = asset;
            LoadIcon();
        }

        private async void LoadIcon()
        {
            //TODO add method to get as sprite directly
            var iconTexture = await fileApi.DownloadAssetIconAsync( asset );
            var sprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), Vector2.zero);
            iconImage.sprite = sprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SetSelected(!isSelected);
            OnAssetSelected?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
        }
    }
}