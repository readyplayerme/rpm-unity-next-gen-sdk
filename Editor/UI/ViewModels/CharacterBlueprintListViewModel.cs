using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayerZero.Api.V1;
using PlayerZero.Data;
using PlayerZero.Editor.Api.V1.Analytics;

namespace PlayerZero.Editor.UI.ViewModels
{
    public class CharacterBlueprintListViewModel
    {
        public bool Loading { get; private set; }

        public IList<CharacterBlueprint> CharacterBlueprints { get; private set; } = new List<CharacterBlueprint>();
        private BlueprintApi _blueprintApi;
        private readonly Settings _settings;
        public readonly AnalyticsApi AnalyticsApi;

        public CharacterBlueprintListViewModel(BlueprintApi blueprintApi, Settings settings, AnalyticsApi analyticsApi)
        {
            _blueprintApi = blueprintApi;
            _settings = settings;
            AnalyticsApi = analyticsApi;
        }

        public async Task Init()
        {
            Loading = true;

            if (string.IsNullOrEmpty(_settings.ApplicationId))
            {
                CharacterBlueprints = new List<CharacterBlueprint>();
                Loading = false;
                return;
            }
            var request = new BlueprintListRequest()
            {
                ApplicationId = _settings?.ApplicationId,
            };
            
            var response = await _blueprintApi.ListAsync(request);
            CharacterBlueprints = response.Data.ToList();

            Loading = false;
        }
    }
}