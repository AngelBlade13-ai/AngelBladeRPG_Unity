using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class BattleEncounterInteractable2DTests
    {
        [TestCase("BattleScene", "TownReturn", "monster_goblin", true)]
        [TestCase("", "TownReturn", "monster_goblin", false)]
        [TestCase("BattleScene", "", "monster_goblin", false)]
        [TestCase("BattleScene", "TownReturn", "", false)]
        [TestCase("BattleScene", "TownReturn", "monster_unknown", false)]
        public void HasEncounterConfigurationValidatesRequiredValues(
            string sceneName,
            string spawnId,
            string monsterId,
            bool expected)
        {
            bool valid = BattleEncounterInteractable2D.HasEncounterConfiguration(
                sceneName,
                spawnId,
                monsterId);

            Assert.That(valid, Is.EqualTo(expected));
        }
    }
}
