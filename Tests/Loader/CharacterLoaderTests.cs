using NUnit.Framework;
using System.Threading.Tasks;

namespace ReadyPlayerMe.Tests.Loader
{
    public class CharacterLoaderTests
    {
        private string glbUrl = "https://files.readyplayer.me/character/glbUrl/6628d1c497cb7a2453b807b1/6628d1c497cb7a2453b807b1-1713961736590.glb";
        private string updatedGlbUrl = "https://files.readyplayer.me/character/glbUrl/6628d1c497cb7a2453b807b1/6628d1c497cb7a2453b807b1-1714039561609.glb";
        private string id = "6628d1c497cb7a2453b807b1";
        
        [Test]
        public async Task Load_Character_With_Valid_Glb_Url()
        {
            var characterManager = new CharacterManager();
            var character = await characterManager.LoadCharacter(glbUrl, id);
            
            Assert.IsNotNull(character);
        }
        
        [Test]
        public async Task Loaded_Character_Has_CharacterData_Component()
        {
            var characterManager = new CharacterManager();
            var character = await characterManager.LoadCharacter(glbUrl, id);

            Assert.IsNotNull(character);
            Assert.AreEqual(id, character.Id);
        }
        
        [Test]
        public async Task Update_Character()
        {
            var characterManager = new CharacterManager();
            var character = await characterManager.LoadCharacter(glbUrl, id);
            var characterHashCode = character.GetHashCode();

            var updatedCharacter = await characterManager.LoadCharacter(updatedGlbUrl, id);
            var updatedCharacterHashCode = updatedCharacter.GetHashCode();
            
            Assert.AreEqual(characterHashCode, updatedCharacterHashCode);
        }
    }
}