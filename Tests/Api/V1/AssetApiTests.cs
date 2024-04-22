using NUnit.Framework;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.V1.Assets;
using ReadyPlayerMe.Runtime.Api.V1.Assets.Models;

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
                    OrganizationId = "63eba72b442b6965bf9f1ed2",
                    Type = "baseModel"
                }
            };

            // Act
            var response = await assetApi.ListAssetsAsync(request);

            // Assert
            Assert.IsNotNull(response);
            Assert.Greater(response.Pagination.TotalDocs, 0);
        }
    }
}