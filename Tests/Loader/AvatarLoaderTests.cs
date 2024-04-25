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
            AvatarLoader avatarLoader = new AvatarLoader();
            GameObject avatar = await avatarLoader.LoadAvatar("https://files.readyplayer.me/character/glbUrl/6628d1c497cb7a2453b807b1/6628d1c497cb7a2453b807b1-1713961736590.glb");
            
            Assert.IsNotNull(avatar);
        }
    }
}
