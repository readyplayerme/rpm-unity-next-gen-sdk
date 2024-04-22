using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Loader;

namespace ReadyPlayerMe.Tests
{
    public class AvatarLoaderTests
    {
        [Test]
        [RequiresPlayMode]
        public async Task Load_Avatar_With_Valid_Glb_Url()
        {
            AvatarLoader avatarLoader = new AvatarLoader();
            GameObject avatar = await avatarLoader.LoadAvatar("https://models.readyplayer.me/660d3617f2b8572436137dfa.glb");
            
            Assert.IsNotNull(avatar);
        }
    }
}
