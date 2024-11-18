using System;
using System.Threading;
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
        [SerializeField] private bool disableDeselectOnClick;
        public bool IsSelected { get; protected set; }
        public Action<bool> OnSelectionChanged { get; set; }

        public Asset Asset { get; private set; }
        private FileApi fileApi;
        
        private CancellationTokenSource cancellationTokenSource;
        
        public void Initialize(Asset asset)
        {
            if (asset.IsStyleAsset())
            {
                disableDeselectOnClick = true;
            }
            gameObject.name = asset.Name;
            fileApi = new FileApi();
            Asset = asset;
            LoadIcon();
        }

        private async void LoadIcon()
        {
            try
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                
                gameObject.SetActive(false);
                
                var iconTexture = await fileApi.DownloadImageAsync(Asset.IconUrl, cancellationTokenSource.Token);

                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                if(cancellationTokenSource.Token.IsCancellationRequested || iconImage == null) return;
                
                var sprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), Vector2.zero);
                iconImage.sprite = sprite;
                gameObject.SetActive(true);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Icon loading was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load icon: {ex.Message}");
            }
            finally
            {
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (disableDeselectOnClick && IsSelected) return;
            SetSelected(!IsSelected);
            OnAssetClicked?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            OnSelectionChanged?.Invoke(IsSelected);
        }

        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
        }
    }
}