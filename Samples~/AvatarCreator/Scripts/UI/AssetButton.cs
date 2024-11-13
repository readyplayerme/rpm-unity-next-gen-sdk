using System;
using ReadyPlayerMe.Api.V1;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class AssetButton : MonoBehaviour, IPointerClickHandler, ISelectable
    {
        public Action<AssetButton> OnAssetClicked;
        
        [SerializeField] private Image buttonImage;
        [SerializeField] private Image iconImage;
        
        public bool IsSelected { get; protected set; }
        public Action<bool> OnSelectionChanged { get; set; }

        public Asset Asset { get; private set; }
        private FileApi fileApi;
        
        public void Initialize(Asset asset)
        {
            fileApi = new FileApi();
            Asset = asset;
            LoadIcon();
        }

        private async void LoadIcon()
        {
            //TODO add method to get as sprite directly
            gameObject.SetActive(false);
            var iconTexture = await fileApi.DownloadAssetIconAsync( Asset );
            var sprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), Vector2.zero);
            gameObject.SetActive(true);
            iconImage.sprite = sprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SetSelected(!IsSelected);
            OnAssetClicked?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            OnSelectionChanged?.Invoke(IsSelected);
        }
    }
}