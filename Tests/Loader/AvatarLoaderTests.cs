using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Loader;

namespace ReadyPlayerMe.Tests.Loader
{
    public class AvatarLoaderTests
    {
        [Test]
        [RequiresPlayMode]
        public async Task Load_Avatar_With_Valid_Glb_Url()
        {
            // Arrange
            AvatarLoader avatarLoader = new AvatarLoader();
            
            // Act
            GameObject avatar = await avatarLoader.LoadAvatar("https://models.readyplayer.me/660d3617f2b8572436137dfa.glb");
            
            // Assert
            Assert.IsNotNull(avatar);
        }
    }
}
