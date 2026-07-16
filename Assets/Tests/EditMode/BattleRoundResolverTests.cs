using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class BattleRoundResolverTests
    {
        [Test]
        public void FasterPlayerAttacksFirst()
        {
            PlayerData player = CreatePlayer(speed: 15);
            MonsterData monster = CreateMonster(speed: 5);

            BattleRoundResult result = CreateResolver().ResolveAttackRound(
                player,
                monster);

            Assert.That(result.Messages[0], Does.StartWith("Hero attacks"));
            Assert.That(result.Messages[1], Does.StartWith("Goblin attacks"));
        }

        [Test]
        public void FasterMonsterAttacksFirst()
        {
            PlayerData player = CreatePlayer(speed: 5);
            MonsterData monster = CreateMonster(speed: 15);

            BattleRoundResult result = CreateResolver().ResolveAttackRound(
                player,
                monster);

            Assert.That(result.Messages[0], Does.StartWith("Goblin attacks"));
            Assert.That(result.Messages[1], Does.StartWith("Hero attacks"));
        }

        [Test]
        public void DefeatedMonsterDoesNotRetaliate()
        {
            PlayerData player = CreatePlayer(speed: 15);
            MonsterData monster = CreateMonster(hp: 1, speed: 5);

            BattleRoundResult result = CreateResolver().ResolveAttackRound(
                player,
                monster);

            Assert.That(result.Messages, Has.Count.EqualTo(1));
            Assert.That(result.MonsterWasDefeated, Is.True);
            Assert.That(player.CurrentHp, Is.EqualTo(player.MaxHp));
        }

        [Test]
        public void DefeatedPlayerDoesNotAttack()
        {
            PlayerData player = CreatePlayer(speed: 5);
            player.CurrentHp = 1;
            MonsterData monster = CreateMonster(attack: 100, speed: 15);

            BattleRoundResult result = CreateResolver().ResolveAttackRound(
                player,
                monster);

            Assert.That(result.Messages, Has.Count.EqualTo(1));
            Assert.That(result.PlayerWasDefeated, Is.True);
            Assert.That(monster.CurrentHp, Is.EqualTo(monster.MaxHp));
        }

        [Test]
        public void EqualSpeedUsesProvidedTieBreaker()
        {
            PlayerData player = CreatePlayer(speed: 10);
            MonsterData monster = CreateMonster(speed: 10);

            BattleRoundResult result = CreateResolver().ResolveAttackRound(
                player,
                monster);

            Assert.That(result.Messages[0], Does.StartWith("Goblin attacks"));
        }

        [Test]
        public void RoundExposesStructuredActionResults()
        {
            BattleRoundResult result = CreateResolver().ResolveAttackRound(
                CreatePlayer(speed: 15),
                CreateMonster(speed: 5));

            Assert.That(result.Actions, Has.Count.EqualTo(2));
            Assert.That(
                result.Actions[0].Type,
                Is.EqualTo(CombatActionType.PhysicalAttack));
            Assert.That(result.Actions[0].Damage, Is.GreaterThan(0));
        }

        [Test]
        public void DefendReducesMonsterDamageForOneRound()
        {
            PlayerData player = CreatePlayer(speed: 10);
            MonsterData monster = CreateMonster(attack: 13, speed: 8);

            BattleRoundResult result = CreateResolver().ResolveDefendRound(
                player,
                monster);

            Assert.That(result.Actions[0].Type, Is.EqualTo(CombatActionType.Defend));
            Assert.That(result.Actions[1].WasGuarded, Is.True);
            Assert.That(result.Actions[1].Damage, Is.EqualTo(5));
            Assert.That(player.CurrentHp, Is.EqualTo(95));
        }

        [Test]
        public void SuccessfulEscapeDoesNotAllowMonsterAttack()
        {
            BattleRoundResolver resolver = new BattleRoundResolver(
                tieBreaker: new MinimumIndexRandom(),
                combatRandom: new AlwaysCombatRandom(true));

            BattleRoundResult result = resolver.ResolveEscapeRound(
                CreatePlayer(speed: 10),
                CreateMonster(speed: 8));

            Assert.That(result.EscapeSucceeded, Is.True);
            Assert.That(result.Actions, Has.Count.EqualTo(1));
        }

        [Test]
        public void FailedEscapeAllowsMonsterCounterattack()
        {
            PlayerData player = CreatePlayer(speed: 10);
            BattleRoundResolver resolver = new BattleRoundResolver(
                tieBreaker: new MinimumIndexRandom(),
                combatRandom: new SequenceCombatRandom(false, true, false));

            BattleRoundResult result = resolver.ResolveEscapeRound(
                player,
                CreateMonster(speed: 8));

            Assert.That(result.EscapeSucceeded, Is.False);
            Assert.That(result.Actions, Has.Count.EqualTo(2));
            Assert.That(result.Actions[1].Type, Is.EqualTo(CombatActionType.PhysicalAttack));
            Assert.That(player.CurrentHp, Is.LessThan(player.MaxHp));
        }

        private static BattleRoundResolver CreateResolver()
        {
            return new BattleRoundResolver(
                tieBreaker: new MinimumIndexRandom(),
                combatRandom: new NormalHitCombatRandom());
        }

        private static PlayerData CreatePlayer(int speed)
        {
            PlayerData player = new PlayerData("Hero");
            player.Speed = speed;
            return player;
        }

        private static MonsterData CreateMonster(
            int hp = 35,
            int attack = 8,
            int speed = 8)
        {
            return new MonsterData(
                "Goblin",
                hp,
                attack,
                1,
                10,
                15,
                speed);
        }

        private class MinimumIndexRandom : ITurnOrderRandom
        {
            public int NextIndex(int minimumInclusive, int maximumExclusive)
            {
                return minimumInclusive;
            }
        }

        private class NormalHitCombatRandom : ICombatRandom
        {
            public bool RollPercent(int chancePercent)
            {
                return chancePercent > 50;
            }
        }

        private class AlwaysCombatRandom : ICombatRandom
        {
            private readonly bool result;

            public AlwaysCombatRandom(bool result)
            {
                this.result = result;
            }

            public bool RollPercent(int chancePercent)
            {
                return result;
            }
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
