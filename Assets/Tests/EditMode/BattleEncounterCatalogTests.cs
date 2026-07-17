using System;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class BattleEncounterCatalogTests
    {
        [Test]
        public void CatalogContainsTenUniqueGrasslandGroups()
        {
            BattleEncounterDefinition[] encounters =
                BattleEncounterCatalog.All.ToArray();

            Assert.That(encounters, Has.Length.EqualTo(10));
            Assert.That(
                encounters.Select(encounter => encounter.Id).Distinct().Count(),
                Is.EqualTo(encounters.Length));
        }

        [TestCase(BattleEncounterCatalog.Quest1SkirmishAId, 2)]
        [TestCase(BattleEncounterCatalog.Quest2RescueId, 3)]
        [TestCase(BattleEncounterCatalog.Quest3PatrolAId, 3)]
        [TestCase(BattleEncounterCatalog.AmbientBoarId, 1)]
        [TestCase(BattleEncounterCatalog.GoblinBossId, 3)]
        public void AuthoredGroupCreatesExpectedEnemyCount(
            string encounterId,
            int expectedCount)
        {
            BattleEncounterDefinition encounter =
                BattleEncounterCatalog.Get(encounterId);

            Assert.That(encounter.CreateEnemies(), Has.Count.EqualTo(expectedCount));
        }

        [Test]
        public void DuplicateEnemyRolesReceiveUniqueRuntimeIdentityAndLabels()
        {
            BattleEncounterDefinition encounter = BattleEncounterCatalog.Get(
                BattleEncounterCatalog.Quest1SkirmishAId);
            MonsterData[] enemies = encounter.CreateEnemies().ToArray();

            Assert.That(enemies[0].Id, Is.Not.EqualTo(enemies[1].Id));
            Assert.That(
                enemies.Select(enemy => enemy.Id).Distinct().Count(),
                Is.EqualTo(2));
            Assert.That(
                enemies.Select(enemy => enemy.DefinitionId),
                Is.All.EqualTo("monster_goblin_skirmisher"));
            Assert.That(enemies[0].Name, Is.EqualTo("Goblin Skirmisher 1"));
            Assert.That(enemies[1].Name, Is.EqualTo("Goblin Skirmisher 2"));
        }

        [Test]
        public void GoblinBossUsesBossLayoutAndDisablesEscape()
        {
            BattleEncounterDefinition encounter = BattleEncounterCatalog.Get(
                BattleEncounterCatalog.GoblinBossId);

            Assert.That(encounter.LayoutId, Is.EqualTo(BattleLayoutCatalog.BossId));
            Assert.That(encounter.EscapeAllowed, Is.False);
            Assert.That(
                encounter.MonsterDefinitionIds[0],
                Is.EqualTo("boss_grassland_goblin"));
        }

        [Test]
        public void EncounterRejectsUnknownMonsterAndOversizedGroup()
        {
            Assert.Throws<ArgumentException>(() => new BattleEncounterDefinition(
                "encounter_test",
                "Test",
                BattleLayoutCatalog.StandardId,
                new[] { "monster_unknown" }));
            Assert.Throws<ArgumentException>(() => new BattleEncounterDefinition(
                "encounter_test",
                "Test",
                BattleLayoutCatalog.StandardId,
                Enumerable.Repeat("monster_slime", 5)));
        }

        [Test]
        public void CatalogReturnsNullForUnknownEncounter()
        {
            Assert.That(BattleEncounterCatalog.Get("encounter_unknown"), Is.Null);
        }
    }
}
