using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Api.V1.Analytics;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterBlueprintsViewModel
    {
        private const string BASE_MODEL_LABEL = "baseModel";
        
        public bool Loading { get; private set; }

        public IList<CharacterBlueprint> CharacterBlueprints { get; private set; } = new List<CharacterBlueprint>();
        private BlueprintApi _blueprintApi;
        private readonly Settings _settings;
        public readonly AnalyticsApi AnalyticsApi;

        public CharacterBlueprintsViewModel(BlueprintApi blueprintApi, Settings settings, AnalyticsApi analyticsApi)
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