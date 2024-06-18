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
            var request = new CharacterCreateRequest();
            
            // Act
            var response = await _characterApi.CreateAsync(request);
            Debug.Log(response.Data.GlbUrl);
            
            // Assert
            Assert.IsTrue(Uri.TryCreate(response.Data.GlbUrl, UriKind.Absolute, out _));
        }
        
        [Test, Order(1)]
        public void Preview_Character()
        {
            // Arrange
            var request = new CharacterPreviewRequest()
            {
                Id = TestConstants.CharacterId,
                Params = new CharacterPreviewQueryParams()
                {
                    Assets = new Dictionary<string, string>
                    {
                        {"top", TestConstants.TopAssetId}
                    }
                }
            };
            
            // Act
            var response = _characterApi.GeneratePreviewUrl(request);
            
            // Assert
            Assert.NotNull(response);
        }
        
        [Test, Order(2)]
        public async Task Update_Character()
        {
            // Arrange
            var request = new CharacterUpdateRequest()
            {
                Id = TestConstants.CharacterId,
                Payload = new CharacterUpdateRequestBody()
                {
                    Assets = new Dictionary<string, string>
                    {
                        { "top", TestConstants.TopAssetId }
                    }
                }
            };
            
            // Act
            var response = await _characterApi.UpdateAsync(request);
            Debug.Log(response.Data.Id);
            
            // Assert
            Assert.IsTrue(Uri.TryCreate(response.Data.GlbUrl, UriKind.Absolute, out _));
        }
    }
}
