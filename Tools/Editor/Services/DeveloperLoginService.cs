using System.Threading.Tasks;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models;
using ReadyPlayerMe.Tools.Editor.Cache;
using ReadyPlayerMe.Tools.Editor.Data;

namespace ReadyPlayerMe.Tools.Editor.Services
{
    public class DeveloperLoginService
    {
        private readonly DeveloperAccountApi _developerAccountApi;

        public DeveloperLoginService(DeveloperAccountApi developerAccountApi)
        {
            _developerAccountApi = developerAccountApi;
        }

        public async Task<ServiceResponse<DeveloperLoginResponseBody>> LoginAsync(
            DeveloperLoginRequest developerLoginRequest)
        {
            var response = await _developerAccountApi.LoginAsync(new DeveloperLoginRequest()
            {
                Payload = new DeveloperLoginRequestBody
                {
                    Email = developerLoginRequest.Payload.Email,
                    Password = developerLoginRequest.Payload.Password
                }
            });

            if (!response.IsSuccess)
                return new ServiceResponse<DeveloperLoginResponseBody>()
                {
                    IsSuccess = false,
                    Error = "Studio login failed. Double check your username and password."
                };

            DeveloperDetailsCache.Data = new Developer()
            {
                Name = response.Data.Name,
                Token = response.Data.Token,
                RefreshToken = response.Data.RefreshToken
            };

            return new ServiceResponse<DeveloperLoginResponseBody>()
            {
                IsSuccess = true,
                Data = response.Data
            };
        }
    }
}