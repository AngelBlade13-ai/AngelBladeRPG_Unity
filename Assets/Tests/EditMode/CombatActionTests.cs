using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class CombatActionTests
    {
        [Test]
        public void PhysicalAttackMissDoesNotDealDamage()
        {
            PlayerData player = new PlayerData("Hero");
            MonsterData monster = CreateMonster();
            int startingHp = monster.CurrentHp;
            PhysicalAttackAction action = new PhysicalAttackAction();

            CombatActionResult result = action.Execute(
                Context(player, monster, new SequenceCombatRandom(false)));

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Damage, Is.Zero);
            Assert.That(monster.CurrentHp, Is.EqualTo(startingHp));
            Assert.That(result.Message, Does.Contain("misses"));
        }

        [Test]
        public void CriticalHitDoublesPhysicalDamage()
        {
            PlayerData player = new PlayerData("Hero");
            MonsterData monster = CreateMonster(defense: 2);
            PhysicalAttackAction action = new PhysicalAttackAction();

            CombatActionResult result = action.Execute(
                Context(
                    player,
                    monster,
                    new SequenceCombatRandom(true, true)));

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.WasCritical, Is.True);
            Assert.That(result.Damage, Is.EqualTo(20));
            Assert.That(result.Message, Does.StartWith("Critical hit!"));
        }

        [Test]
        public void GuardHalvesPhysicalDamageRoundedUp()
        {
            PlayerData player = new PlayerData("Hero");
            MonsterData monster = CreateMonster(attack: 12);
            PhysicalAttackAction action = new PhysicalAttackAction();

            CombatActionResult result = action.Execute(
                Context(
                    monster,
                    player,
                    new SequenceCombatRandom(true, false),
                    targetIsGuarding: true));

            Assert.That(result.WasGuarded, Is.True);
            Assert.That(result.Damage, Is.EqualTo(5));
            Assert.That(result.Message, Does.Contain("blocks part"));
        }

        [Test]
        public void AccuracyAndEvasionDetermineHitChance()
        {
            PlayerData player = new PlayerData("Hero");
            player.Accuracy = 80;
            MonsterData monster = CreateMonster();
            monster.Stats.Evasion = 15;
            PhysicalAttackAction action = new PhysicalAttackAction();

            CombatActionResult result = action.Execute(
                Context(player, monster, new SequenceCombatRandom(false)));

            Assert.That(result.ChancePercent, Is.EqualTo(65));
        }

        [Test]
        public void PhysicalHitChanceNeverDropsBelowFivePercent()
        {
            PlayerData player = new PlayerData("Hero");
            player.Accuracy = 0;
            MonsterData monster = CreateMonster();
            monster.Stats.Evasion = 95;

            CombatActionResult result = new PhysicalAttackAction().Execute(
                Context(player, monster, new SequenceCombatRandom(false)));

            Assert.That(result.ChancePercent, Is.EqualTo(5));
        }

        [TestCase(20, 5, 95)]
        [TestCase(10, 10, 50)]
        [TestCase(5, 20, 20)]
        public void EscapeChanceUsesSpeedAndClamps(
            int playerSpeed,
            int monsterSpeed,
            int expectedChance)
        {
            PlayerData player = new PlayerData("Hero");
            player.Speed = playerSpeed;
            MonsterData monster = CreateMonster(speed: monsterSpeed);

            CombatActionResult result = new EscapeAction().Execute(
                Context(player, monster, new SequenceCombatRandom(false)));

            Assert.That(result.ChancePercent, Is.EqualTo(expectedChance));
        }

        [TestCase(CombatActionType.PhysicalAttack)]
        [TestCase(CombatActionType.Magic)]
        [TestCase(CombatActionType.Healing)]
        [TestCase(CombatActionType.Item)]
        [TestCase(CombatActionType.Defend)]
        [TestCase(CombatActionType.Ability)]
        [TestCase(CombatActionType.Escape)]
        public void ActionContractIncludesCommandCategory(
            CombatActionType actionType)
        {
            Assert.That(
                System.Enum.IsDefined(typeof(CombatActionType), actionType),
                Is.True);
        }

        private static CombatActionContext Context(
            ICombatant actor,
            ICombatant target,
            ICombatRandom random,
            bool targetIsGuarding = false)
        {
            return new CombatActionContext(
                actor,
                target,
                random,
                targetIsGuarding);
        }

        private static MonsterData CreateMonster(
            int attack = 8,
            int defense = 1,
            int speed = 8)
        {
            return new MonsterData(
                "Goblin",
                100,
                attack,
                defense,
                10,
                15,
                speed);
        }

        private class SequenceCombatRandom : ICombatRandom
        {
            private readonly bool[] results;
            private int index;

            public SequenceCombatRandom(params bool[] results)
            {
                this.results = results;
            }

            public bool RollPercent(int chancePercent)
            {
                return results[index++];
            }
        }
    }
}
