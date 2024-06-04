using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Api.V1.Auth;
using ReadyPlayerMe.Editor.Api.V1.Auth.Models;
using ReadyPlayerMe.Editor.EditorPrefs;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
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
                RefreshToken = response.Data.RefreshToken,
            };

            Loading = false;
            onSuccess();
        }

        public async Task SignInToDemoAccount(Action onSuccess)
        {
            Loading = true;

            DeveloperAuthCache.Data = new DeveloperAuth()
            {
                Name = "guest user",
                IsDemo = true,
            };

            var settings = Resources.Load<Settings>("ReadyPlayerMeSettings");

            // NOTE: This API Key is a special key made for the demo account, it has minimal read only permissions for the demo organizations.
            settings.ApiKey = "sk_live_303Y8tHtKmYTzK9eWo4og7I1eptXrE2eCc9n";
            settings.ApplicationId = "665e05a50c62c921e5a6ab84";

            var avatarDefinition = Resources.Load<AvatarSkeletonDefinitionConfig>("AvatarSkeletonDefinitionConfig");

            var links = avatarDefinition.definitionLinks?.ToList() ?? new List<AvatarSkeletonDefinitionLink>();
            var existingLink = links.FirstOrDefault(p => p.characterStyleId == "665e05e758e847063761c985");
            if (existingLink == null)
            {
                var matchingAssets = AssetDatabase.FindAssets("RPM_Character_Skeleton_Definition");
                var assetPath = AssetDatabase.GUIDToAssetPath(matchingAssets[0]);
                var asset = AssetDatabase.LoadAssetAtPath<AvatarSkeletonDefinition>(assetPath);

                links.Add(new AvatarSkeletonDefinitionLink()
                {
                    characterStyleId = "665e05e758e847063761c985",
                    definitionCacheId = matchingAssets[0],
                    definition = asset
                });
            }

            avatarDefinition.definitionLinks = links.ToArray();

            EditorUtility.SetDirty(settings);
            EditorUtility.SetDirty(avatarDefinition);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Loading = false;
            onSuccess();
        }
    }
}