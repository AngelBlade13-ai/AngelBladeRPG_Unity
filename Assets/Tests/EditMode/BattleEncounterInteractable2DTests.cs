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

        [Test]
        public void HasEncounterConfigurationAcceptsKnownEncounterGroup()
        {
            bool valid = BattleEncounterInteractable2D.HasEncounterConfiguration(
                "BattleScene",
                "TownReturn",
                BattleEncounterCatalog.Quest1SkirmishAId,
                string.Empty);

            Assert.That(valid, Is.True);
        }

        [Test]
        public void HasEncounterConfigurationRejectsUnknownGroupAndMonster()
        {
            bool valid = BattleEncounterInteractable2D.HasEncounterConfiguration(
                "BattleScene",
                "TownReturn",
                "encounter_unknown",
                "monster_unknown");

            Assert.That(valid, Is.False);
        }
    }
}
