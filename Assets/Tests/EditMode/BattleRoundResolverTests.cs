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

        private static BattleRoundResolver CreateResolver()
        {
            return new BattleRoundResolver(
                tieBreaker: new MinimumIndexRandom());
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
    }
}
