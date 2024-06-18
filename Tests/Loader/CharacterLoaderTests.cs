using NUnit.Framework;
using System.Threading.Tasks;
using UnityEngine.TestTools;

namespace ReadyPlayerMe.Tests.Loader
{
    public class CharacterLoaderTests
    {
        [Test, RequiresPlayMode]
        public async Task Load_Character_With_Valid_Glb_Url()
        {
            var characterManager = new CharacterManager();
            var character = await characterManager.LoadCharacter(TestConstants.CharacterId, TestConstants.TemplateId);
            
            Assert.IsNotNull(character);
        }
        
        [Test, RequiresPlayMode]
        public async Task Loaded_Character_Has_CharacterData_Component()
        {
            var characterManager = new CharacterManager();
            var character = await characterManager.LoadCharacter(TestConstants.CharacterId, TestConstants.TemplateId);

            Assert.IsNotNull(character);
            Assert.AreEqual(TestConstants.CharacterId, character.Id);
        }
    }
}
