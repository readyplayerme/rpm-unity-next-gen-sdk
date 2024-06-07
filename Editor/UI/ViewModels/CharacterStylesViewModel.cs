using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Api.V1.Analytics;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterStylesViewModel
    {
        public bool Loading { get; private set; }

        public IList<Asset> CharacterStyles { get; private set; } = new List<Asset>();
        
        private readonly AssetApi _assetApi;
        private readonly Settings _settings;
        public readonly AnalyticsApi AnalyticsApi;

        public CharacterStylesViewModel(AssetApi assetApi, Settings settings, AnalyticsApi analyticsApi)
        {
            _assetApi = assetApi;
            _settings = settings;
            AnalyticsApi = analyticsApi;
        }

        public async Task Init()
        {
            Loading = true;

            if (string.IsNullOrEmpty(_settings.ApplicationId))
            {
                CharacterStyles = new List<Asset>();
                Loading = false;
                return;
            }

            var response = await _assetApi.ListAssetsAsync(new AssetListRequest
            {
                Params = new AssetListQueryParams
                {
                    ApplicationId = _settings.ApplicationId,
                    Type = "baseModel"
                }
            });
            
            CharacterStyles = response.Data.ToList();

            Loading = false;
        }
    }
}