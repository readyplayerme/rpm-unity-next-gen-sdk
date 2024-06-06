using System;
using UnityEngine;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using ReadyPlayerMe.Api.V1;

namespace ReadyPlayerMe.Tests.Api.V1
{
    public class CharacterApiTests
    {
        private readonly CharacterApi _characterApi = new CharacterApi();
        
        [Test, Order(0)]
        public async Task Create_Character()
        {
            // Arrange
            var request = new CharacterCreateRequest()
            {
                Payload = new CharacterCreateRequestBody
                {
                    ApplicationId = "6628c280ecb07cb9d9cd7238"
                }
            };
            
            // Act
            var response = await _characterApi.CreateAsync(request);
            Debug.Log(response.Data.GlbUrl);
            
            // Assert
            Assert.IsTrue(Uri.TryCreate(response.Data.GlbUrl, UriKind.Absolute, out _));
        }
        
        [Test]
        public async Task Update_Character()
        {
            // Arrange
            var request = new CharacterUpdateRequest()
            {
                Id = "6628d1c497cb7a2453b807b1",
                Payload = new CharacterUpdateRequestBody()
                {
                    Assets = new Dictionary<string, string>
                    {
                        { "top", "662a22be282104d791d4a123" }
                    }
                }
            };
            
            // Act
            var response = await _characterApi.UpdateAsync(request);
            Debug.Log(response.Data.GlbUrl);
            
            // Assert
            Assert.IsTrue(Uri.TryCreate(response.Data.GlbUrl, UriKind.Absolute, out _));
        }
        
        [Test]
        public void Preview_Character()
        {
            // Arrange
            var request = new CharacterPreviewRequest()
            {
                Id = "6628d1c497cb7a2453b807b1",
                Params = new CharacterPreviewQueryParams()
                {
                    Assets = new Dictionary<string, string>
                    {
                        {"top", "662a22be282104d791d4a123"}
                    }
                }
            };
            
            // Act
            var response = _characterApi.GeneratePreviewUrl(request);
            
            // Assert
            Assert.NotNull(response);
        }
    }
}
