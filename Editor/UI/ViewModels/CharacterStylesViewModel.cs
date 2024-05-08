using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.V1;
using ReadyPlayerMe.Runtime.Data;
using ReadyPlayerMe.Runtime.Data.V1;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterStylesViewModel
    {
        public bool Loading { get; private set; }

        public IList<Asset> CharacterStyles { get; private set; } = new List<Asset>();

        private readonly AssetApi _assetApi;
        private readonly Settings _settings;

        public CharacterStylesViewModel(AssetApi assetApi, Settings settings)
        {
            _assetApi = assetApi;
            _settings = settings;
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