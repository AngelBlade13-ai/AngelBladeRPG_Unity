using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class BattleEncounterInteractable2DTests
    {
        [TestCase("BattleScene", "TownReturn", "Goblin", 35, true)]
        [TestCase("", "TownReturn", "Goblin", 35, false)]
        [TestCase("BattleScene", "", "Goblin", 35, false)]
        [TestCase("BattleScene", "TownReturn", "", 35, false)]
        [TestCase("BattleScene", "TownReturn", "Goblin", 0, false)]
        public void HasEncounterConfigurationValidatesRequiredValues(
            string sceneName,
            string spawnId,
            string monsterName,
            int monsterHp,
            bool expected)
        {
            bool valid = BattleEncounterInteractable2D.HasEncounterConfiguration(
                sceneName,
                spawnId,
                monsterName,
                monsterHp);

            Assert.That(valid, Is.EqualTo(expected));
        }
    }
}
