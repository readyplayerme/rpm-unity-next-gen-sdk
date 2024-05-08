using UnityEngine;
using NUnit.Framework;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.V1;

namespace ReadyPlayerMe.Tests.Api.V1
{
    public class AssetApiTests
    {
        [Test]
        public async Task List_Assets()
        {
            // Arrange
            var assetApi = new AssetApi();
            var request = new AssetListRequest()
            {
                Params =
                {
                    ApplicationId = "6628c280ecb07cb9d9cd7238",
                    Type = "baseModel"
                }
            };

            // Act
            var response = await assetApi.ListAssetsAsync(request);
            Debug.Log(response.Data[0].Id);
            Debug.Log(response.Data[0].Type);

            // Assert
            Assert.IsNotNull(response);
            Assert.Greater(response.Pagination.TotalDocs, 0);
        }
    }
}