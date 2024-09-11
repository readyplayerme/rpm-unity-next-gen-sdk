using System;
using ReadyPlayerMe.Api.V1;

namespace ReadyPlayerMe.Samples.UseCache
{
    /// <summary>
    ///     Event aggregator for communication between UI components.
    /// </summary>
    public class EventAggregator
    {
        private static EventAggregator instance;
        public static EventAggregator Instance => instance ??= new EventAggregator();

        public event Action<string> OnCategorySelected;
        public event Action OnCategoriesLoaded;
        public event Action<Asset> OnAssetSelected;
        public event Action OnAssetsLoaded;
        public event Action<int> OnPageChanged;

        public void RaiseCategorySelected(string category)
        {
            OnCategorySelected?.Invoke(category);
        }

        public void RaiseCategoriesLoaded()
        {
            OnCategoriesLoaded?.Invoke();
        }

        public void RaiseAssetSelected(Asset asset)
        {
            OnAssetSelected?.Invoke(asset);
        }

        public void RaiseAssetsLoaded()
        {
            OnAssetsLoaded?.Invoke();
        }

        public void RaisePageChanged(int page)
        {
            OnPageChanged?.Invoke(page);
        }
    }
}