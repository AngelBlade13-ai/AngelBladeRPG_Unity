using System;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class MonsterCatalogTests
    {
        [Test]
        public void CatalogContainsFourUniqueMonsterDefinitions()
        {
            MonsterDefinition[] definitions = MonsterCatalog.All.ToArray();

            Assert.That(definitions, Has.Length.EqualTo(4));
            Assert.That(
                definitions.Select(monster => monster.Id).Distinct().Count(),
                Is.EqualTo(4));
        }

        [TestCase("monster_goblin", "Goblin")]
        [TestCase("monster_ogre", "Ogre")]
        [TestCase("monster_slime", "Slime")]
        [TestCase("monster_wisp", "Wisp")]
        public void DefinitionCreatesExpectedMonster(
            string monsterId,
            string expectedName)
        {
            MonsterData monster = MonsterCatalog.Get(monsterId).CreateMonster();

            Assert.That(monster.Id, Is.EqualTo(monsterId));
            Assert.That(monster.Name, Is.EqualTo(expectedName));
            Assert.That(monster.MaxHp, Is.GreaterThan(0));
            Assert.That(monster.XPReward, Is.GreaterThanOrEqualTo(0));
            Assert.That(monster.Accuracy, Is.InRange(0, 100));
            Assert.That(monster.Evasion, Is.InRange(0, 95));
            Assert.That(monster.CriticalChance, Is.InRange(0, 100));
        }

        [Test]
        public void UnknownMonsterIdReturnsNull()
        {
            Assert.That(MonsterCatalog.Get("monster_unknown"), Is.Null);
        }

        [Test]
        public void DefinitionCreatesIndependentRuntimeMonsters()
        {
            MonsterDefinition definition = MonsterCatalog.Get("monster_goblin");
            MonsterData first = definition.CreateMonster();
            MonsterData second = definition.CreateMonster();

            first.CurrentHp = 1;

            Assert.That(second.CurrentHp, Is.EqualTo(second.MaxHp));
        }

        [Test]
        public void StableIdIsIndependentFromDisplayName()
        {
            MonsterData goblin =
                MonsterCatalog.Get("monster_goblin").CreateMonster();

            Assert.That(goblin.Id, Is.EqualTo("monster_goblin"));
            Assert.That(goblin.Name, Is.EqualTo("Goblin"));
            Assert.That(goblin.Id, Is.Not.EqualTo(goblin.Name));
        }

        [Test]
        public void DefinitionRejectsBlankStableId()
        {
            Assert.Throws<ArgumentException>(() => CreateDefinition(id: " "));
        }

        [Test]
        public void DefinitionRejectsInvalidCombatValues()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => CreateDefinition(maxHp: 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => CreateDefinition(goldReward: -1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => CreateDefinition(accuracy: 101));
        }

        private static MonsterDefinition CreateDefinition(
            string id = "monster_test",
            int maxHp = 10,
            int goldReward = 1,
            int accuracy = 95)
        {
            return new MonsterDefinition(
                id,
                "Test Monster",
                maxHp,
                1,
                0,
                1,
                0,
                0,
                0,
                goldReward,
                1,
                accuracy: accuracy);
        }
    }
}
