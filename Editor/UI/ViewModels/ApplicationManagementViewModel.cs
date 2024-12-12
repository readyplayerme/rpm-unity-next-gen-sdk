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
using UnityEngine;
using Application = ReadyPlayerMe.Editor.Api.V1.DeveloperAccounts.Models.Application;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class ApplicationManagementViewModel
    {
        public string Error { get; private set; }

        public bool Loading { get; private set; }

        public IList<Application> Applications { get; private set; } = new List<Application>();

        public readonly AnalyticsApi AnalyticsApi;
        public readonly BlueprintApi BlueprintApi;
        public readonly Settings Settings;

        private readonly DeveloperAccountApi _developerAccountApi;

        public ApplicationManagementViewModel(
            AnalyticsApi analyticsApi,
            BlueprintApi blueprintApi,
            DeveloperAccountApi developerAccountApi,
            Settings settings
        )
        {
            AnalyticsApi = analyticsApi;
            BlueprintApi = blueprintApi;
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

            // Check if the ID exists in the application list
            var matchingApplication = Applications.FirstOrDefault(p => p.Id == Settings.ApplicationId);

            if (matchingApplication == null)
            {
                Settings.ApplicationId = Applications.Count > 0 ? Applications[0].Id : string.Empty; 
            }
            else
            {
                Settings.ApplicationId = matchingApplication.Id; // Use the matched application
            }
            
            EditorUtility.SetDirty(Settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Loading = false;
        }
    }
}