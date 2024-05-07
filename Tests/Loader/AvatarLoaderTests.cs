using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime;
using ReadyPlayerMe.Runtime.Data;
using ReadyPlayerMe.Runtime.Loader;

namespace ReadyPlayerMe.Tests.Loader
{
    public class AvatarLoaderTests
    {
        private string glbUrl = "https://files.readyplayer.me/character/glbUrl/6628d1c497cb7a2453b807b1/6628d1c497cb7a2453b807b1-1713961736590.glb";
        private string updatedGlbUrl = "https://files.readyplayer.me/character/glbUrl/6628d1c497cb7a2453b807b1/6628d1c497cb7a2453b807b1-1714039561609.glb";
        private string id = "6628d1c497cb7a2453b807b1";
        
        [Test, RequiresPlayMode]
        public async Task Load_Avatar_With_Valid_Glb_Url()
        {
            AvatarLoader avatarLoader = new AvatarLoader();
            GameObject avatar = await avatarLoader.LoadAvatar(glbUrl, id);
            
            Assert.IsNotNull(avatar);
        }
        
        [Test, RequiresPlayMode]
        public async Task Loaded_avatar_Has_AvatarData_Component()
        {
            AvatarLoader avatarLoader = new AvatarLoader();
            GameObject avatar = await avatarLoader.LoadAvatar(glbUrl, id);
            AvatarData avatarData = avatar.GetComponent<AvatarData>();
            
            Assert.IsNotNull(avatarData);
            Assert.AreEqual(id, avatarData.Id);
        }
        
        [Test, RequiresPlayMode]
        public async Task Update_Avatar()
        {
            AvatarLoader avatarLoader = new AvatarLoader();
            GameObject avatar = await avatarLoader.LoadAvatar(glbUrl, id);
            int avatarHashCode = avatar.GetHashCode();

            GameObject updatedAvatar = await avatarLoader.LoadAvatar(updatedGlbUrl, id);
            int updatedAvatarHashCode = updatedAvatar.GetHashCode();
            
            Assert.AreEqual(avatarHashCode, updatedAvatarHashCode);
        }
    }
}