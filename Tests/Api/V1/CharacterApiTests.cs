using System;
using UnityEngine;
using NUnit.Framework;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;

namespace ReadyPlayerMe.Tests.Api.V1
{
    public class CharacterApiTests
    {
        private readonly CharacterApi _characterApi = new CharacterApi();
        
        [Test, Order(0)]
        public async Task Find_Character_By_Id()
        {
            // Arrange
            var request = new CharacterFindByIdRequest();
            request.Id = TestConstants.CharacterId;
            
            // Act
            var response = await _characterApi.FindByIdAsync(request);
            Debug.Log(response.Data.ModelUrl);
            
            // Assert
            Assert.IsTrue(Uri.TryCreate(response.Data.ModelUrl, UriKind.Absolute, out _));
        }
    }
}
