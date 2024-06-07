using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Api.V1.Analytics;
using ReadyPlayerMe.Editor.Api.V1.Auth;
using ReadyPlayerMe.Editor.Api.V1.DeveloperAccounts;
using ReadyPlayerMe.Editor.Api.V1.DeveloperAccounts.Models;
using ReadyPlayerMe.Editor.Cache.EditorPrefs;
using UnityEditor;
using Application = ReadyPlayerMe.Editor.Api.V1.DeveloperAccounts.Models.Application;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class ApplicationManagementViewModel
    {
        public string Error { get; private set; }

        public bool Loading { get; private set; }

        public IList<Application> Applications { get; private set; } = new List<Application>();

        public readonly AnalyticsApi AnalyticsApi;
        public readonly AssetApi AssetApi;
        public readonly Settings Settings;

        private readonly DeveloperAccountApi _developerAccountApi;

        public ApplicationManagementViewModel(
            AnalyticsApi analyticsApi,
            AssetApi assetApi,
            DeveloperAccountApi developerAccountApi,
            Settings settings
        )
        {
            AnalyticsApi = analyticsApi;
            AssetApi = assetApi;
            Settings = settings;
            _developerAccountApi = developerAccountApi;
        }

        public async Task Init()
        {
            Error = null;
            Loading = true;

            if (DeveloperAuthCache.Data.IsDemo)
                _developerAccountApi.SetAuthenticationStrategy(new ApiKeyAuthStrategy());
            else
                _developerAccountApi.SetAuthenticationStrategy(new DeveloperTokenAuthStrategy());
            
            var organizationListResponse =
                await _developerAccountApi.ListOrganizationsAsync(new OrganizationListRequest());

            if (!organizationListResponse.IsSuccess)
            {
                Loading = false;
                Applications = new List<Application>();
                Error = organizationListResponse.Error;
                return;
            }

            var organizationId = organizationListResponse.Data[0].Id;

            var applicationListResponse = await _developerAccountApi.ListApplicationsAsync(new ApplicationListRequest
            {
                Params = new ApplicationListQueryParams
                {
                    OrganizationId = organizationId
                }
            });

            if (!applicationListResponse.IsSuccess || !string.IsNullOrEmpty(applicationListResponse.Error))
            {
                Loading = false;
                Applications = new List<Application>();
                Error = applicationListResponse.Error;
                return;
            }

            Applications = applicationListResponse.Data?.ToList() ?? new List<Application>();

            if (Applications?.FirstOrDefault(p => p.Id == Settings.ApplicationId) == null)
            {
                Settings.ApplicationId = string.Empty;
                EditorUtility.SetDirty(Settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Loading = false;
        }
    }
}