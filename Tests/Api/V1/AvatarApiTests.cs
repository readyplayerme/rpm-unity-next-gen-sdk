using System;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using System.Collections.Generic;
using ReadyPlayerMe.Runtime.Api.V1.Avatars;
using ReadyPlayerMe.Runtime.Api.V1.Avatars.Models;

namespace ReadyPlayerMe.Tests.Api.V1
{
    public class AvatarApiTests
    {
        private readonly AvatarApi avatarApi = new AvatarApi();
        
        [Test, Order(0)]
        public async Task Create_Avatar()
        {
            // Arrange
            var request = new AvatarCreateRequest()
            {
                Payload = new AvatarCreateRequestBody
                {
                    ApplicationId = "6628c280ecb07cb9d9cd7238"
                }
            };
            
            // Act
            var response = await avatarApi.CreateAvatarAsync(request);
            Debug.Log(response.Data.GlbUrl);
            
            // Assert
            Assert.IsTrue(Uri.TryCreate(response.Data.GlbUrl, UriKind.Absolute, out _));
        }
        
        [Test, RequiresPlayMode]
        public async Task Update_Avatar()
        {
            // Arrange
            var createRequest = new AvatarCreateRequest()
            {
                Payload = new AvatarCreateRequestBody
                {
                    ApplicationId = "6628c280ecb07cb9d9cd7238"
                }
            };
            var createResponse = await avatarApi.CreateAvatarAsync(createRequest);
            
            var updateRequest = new AvatarUpdateRequest()
            {
                AvatarId = createResponse.Data.Id,
                Payload = new AvatarUpdateRequestBody()
                {
                    Assets = new Dictionary<string, string>
                    {
                        { "top", "662a22be282104d791d4a123" }
                    }
                }
            };
            
            // Act
            var updateResponse = await avatarApi.UpdateAvatarAsync(updateRequest);
            Debug.Log(updateResponse.Data.GlbUrl);
            
            // Assert
            Assert.IsTrue(Uri.TryCreate(updateResponse.Data.GlbUrl, UriKind.Absolute, out _));
        }
        
        [Test, RequiresPlayMode]
        public async Task Preview_Avatar()
        {
            // Arrange
            var request = new AvatarPreviewRequest()
            {
                AvatarId = "6628d1c497cb7a2453b807b1",
                Params = new AvatarPreviewQueryParams()
                {
                    Assets = new Dictionary<string, string>
                    {
                        {"top", "662a22be282104d791d4a123"}
                    }
                }
            };
            
            // Act
            var response = await avatarApi.PreviewAvatarAsync(request);
            
            // Assert
            Assert.NotNull(response);
        }
    }
}
