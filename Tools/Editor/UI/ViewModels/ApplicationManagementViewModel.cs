using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.V1.Assets;
using ReadyPlayerMe.Runtime.Data.ScriptableObjects;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models;
using Application = ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models.Application;

namespace ReadyPlayerMe.Tools.Editor.UI.ViewModels
{
    public class ApplicationManagementViewModel
    {
        public string Error { get; private set; }

        public bool Loading { get; private set; }

        public IList<Application> Applications { get; private set; } = new List<Application>();
        
        public readonly AssetApi AssetApi;
        private readonly DeveloperAccountApi _developerAccountApi;
        public readonly Settings Settings;

        public ApplicationManagementViewModel(
            AssetApi assetApi,
            DeveloperAccountApi developerAccountApi,
            Settings settings
        )
        {
            AssetApi = assetApi;
            _developerAccountApi = developerAccountApi;
            Settings = settings;
        }

        public async Task Init()
        {
            Error = null;
            Loading = true;

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

            if (!applicationListResponse.IsSuccess)
            {
                Loading = false;
                Applications = new List<Application>();
                Error = organizationListResponse.Error;
                return;
            }

            Applications = applicationListResponse.Data?.ToList() ?? new List<Application>();

            if (Applications.FirstOrDefault(p => p.Id == Settings.ApplicationId) == null)
                Settings.ApplicationId = string.Empty;

            Loading = false;
        }
    }
}