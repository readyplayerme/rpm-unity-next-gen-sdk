using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Data;
using ReadyPlayerMe.Runtime.Loader;

namespace ReadyPlayerMe.Tests.Loader
{
    public class AvatarLoaderTests
    {
        [Test, RequiresPlayMode]
        public async Task Load_Avatar_With_Valid_Glb_Url()
        {
            AvatarLoader avatarLoader = new AvatarLoader();
            GameObject avatar = await avatarLoader.LoadAvatar("https://files.readyplayer.me/character/glbUrl/6628d1c497cb7a2453b807b1/6628d1c497cb7a2453b807b1-1713961736590.glb");
            
            Assert.IsNotNull(avatar);
        }
        
        [Test, RequiresPlayMode]
        public async Task Loaded_avatar_Has_AvatarData_Component()
        {
            AvatarLoader avatarLoader = new AvatarLoader();
            GameObject avatar = await avatarLoader.LoadAvatar("https://files.readyplayer.me/character/glbUrl/6628d1c497cb7a2453b807b1/6628d1c497cb7a2453b807b1-1713961736590.glb");
            AvatarData avatarData = avatar.GetComponent<AvatarData>();
            
            Assert.IsNotNull(avatarData);
            Assert.AreEqual("6628d1c497cb7a2453b807b1", avatarData.Id);
        }
        
        [Test, RequiresPlayMode]
        public async Task Update_Avatar()
        {
            AvatarLoader avatarLoader = new AvatarLoader();
            GameObject avatar = await avatarLoader.LoadAvatar("https://files.readyplayer.me/character/glbUrl/6628d1c497cb7a2453b807b1/6628d1c497cb7a2453b807b1-1713961736590.glb");
            int avatarHashCode = avatar.GetHashCode();

            GameObject updatedAvatar = await avatarLoader.LoadAvatar("https://files.readyplayer.me/character/glbUrl/6628d1c497cb7a2453b807b1/6628d1c497cb7a2453b807b1-1714039561609.glb");
            int updatedAvatarHashCode = updatedAvatar.GetHashCode();
            
            Assert.AreEqual(avatarHashCode, updatedAvatarHashCode);
        }
    }
}
