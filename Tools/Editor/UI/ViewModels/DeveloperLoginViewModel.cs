using System;
using System.Threading.Tasks;
using ReadyPlayerMe.Tools.Editor.Api.V1.Auth;
using ReadyPlayerMe.Tools.Editor.Api.V1.Auth.Models;
using ReadyPlayerMe.Tools.Editor.Cache;

namespace ReadyPlayerMe.Tools.Editor.UI.ViewModels
{
    public class DeveloperLoginViewModel
    {
        private readonly DeveloperAuthApi _developerAuthApi;
        public string Username { get; set; }

        public string Password { get; set; }

        public bool Loading { get; private set; }

        public string Error { get; private set; }

        public DeveloperLoginViewModel(DeveloperAuthApi developerAuthApi)
        {
            _developerAuthApi = developerAuthApi;
        }

        public async Task SignIn(Action onSuccess)
        {
            Loading = true;

            var response = await _developerAuthApi.LoginAsync(new DeveloperLoginRequest()
            {
                Payload = new DeveloperLoginRequestBody
                {
                    Email = Username,
                    Password = Password
                }
            });

            if (!response.IsSuccess)
            {
                Error = "Studio login failed. Double check your username and password.";
                Loading = false;
                return;
            }
            
            DeveloperAuthCache.Data = new DeveloperAuth()
            {
                Name = response.Data.Name,
                Token = response.Data.Token,
                RefreshToken = response.Data.RefreshToken
            };

            Loading = false;
            onSuccess();
        }
    }
}
