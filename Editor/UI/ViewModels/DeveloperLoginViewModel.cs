using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Api.V1.Analytics;
using ReadyPlayerMe.Editor.Api.V1.Analytics.Models;
using ReadyPlayerMe.Editor.Api.V1.Auth;
using ReadyPlayerMe.Editor.Api.V1.Auth.Models;
using ReadyPlayerMe.Editor.Cache.EditorPrefs;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class DeveloperLoginViewModel
    {
        private const string DemoProxyURL = "https://api.readyplayer.me/demo";
        private const string DemoApplicationId = "665e05a50c62c921e5a6ab84";

        private readonly DeveloperAuthApi _developerAuthApi;
        private readonly AnalyticsApi _analyticsApi;
        
        public string Username { get; set; }

        public string Password { get; set; }

        public bool Loading { get; private set; }

        public string Error { get; private set; }

        public DeveloperLoginViewModel(
            DeveloperAuthApi developerAuthApi,
            AnalyticsApi analyticsApi
        )
        {
            _developerAuthApi = developerAuthApi;
            _analyticsApi = analyticsApi;
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

            var settings = Resources.Load<Settings>("ReadyPlayerMeSettings");

            if (settings.ApiProxyUrl == DemoProxyURL)
                settings.ApiProxyUrl = string.Empty;

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

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

            _analyticsApi.SendEvent(new AnalyticsEventRequest()
            {
                Payload = new AnalyticsEventRequestBody()
                {
                    Event = "next gen unity sdk action",
                    Properties =
                    {
                        { "type", "Sign In" },
                        { "username", Username }
                    }
                }
            });

            Loading = false;
            onSuccess();
        }

        public void SignInToDemoAccount(Action onSuccess)
        {
            Loading = true;

            DeveloperAuthCache.Data = new DeveloperAuth()
            {
                Name = "guest user",
                IsDemo = true,
            };

            var settings = Resources.Load<Settings>("ReadyPlayerMeSettings");
            settings.ApiProxyUrl = DemoProxyURL;
            settings.ApplicationId = DemoApplicationId;

            var skeletonDefinitionConfig = Resources.Load<SkeletonDefinitionConfig>("SkeletonDefinitionConfig");

            var links = skeletonDefinitionConfig.definitionLinks?.ToList() ?? new List<SkeletonDefinitionLink>();
            var existingLink = links.FirstOrDefault(p => p.characterBlueprintId == "665e05e758e847063761c985");
            if (existingLink == null)
            {
                var matchingAssets = AssetDatabase.FindAssets("RPM_Character_Skeleton_Definition");
                var assetPath = AssetDatabase.GUIDToAssetPath(matchingAssets[0]);
                var asset = AssetDatabase.LoadAssetAtPath<SkeletonDefinition>(assetPath);

                links.Add(new SkeletonDefinitionLink()
                {
                    characterBlueprintId = "665e05e758e847063761c985",
                    definitionCacheId = matchingAssets[0],
                    definition = asset
                });
            }

            skeletonDefinitionConfig.definitionLinks = links.ToArray();

            EditorUtility.SetDirty(settings);
            EditorUtility.SetDirty(skeletonDefinitionConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _analyticsApi.SendEvent(new AnalyticsEventRequest()
            {
                Payload = new AnalyticsEventRequestBody()
                {
                    Event = "next gen unity sdk action",
                    Properties =
                    {
                        { "type", "Sign In to Demo Account" },
                    }
                }
            });

            Loading = false;
            
            onSuccess();
        }
    }
}