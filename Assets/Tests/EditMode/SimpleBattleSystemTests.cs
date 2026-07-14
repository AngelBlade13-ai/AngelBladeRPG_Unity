using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class SimpleBattleSystemTests
    {
        private SimpleBattleSystem battleSystem;

        [SetUp]
        public void SetUp()
        {
            battleSystem = new SimpleBattleSystem();
        }

        [Test]
        public void PlayerAttackSubtractsAttackMinusDefense()
        {
            PlayerData player = new PlayerData("Angel");
            MonsterData monster = new MonsterData("Goblin", 35, 8, 2, 10, 15);

            string message = battleSystem.PlayerAttack(player, monster);

            Assert.That(monster.CurrentHp, Is.EqualTo(25));
            Assert.That(message, Is.EqualTo("Angel attacks Goblin for 10 damage."));
        }

        [Test]
        public void PlayerAttackDealsAtLeastOneDamageAndClampsHpAtZero()
        {
            PlayerData player = new PlayerData("Angel");
            MonsterData armoredMonster = new MonsterData("Guard", 5, 8, 100, 10, 15);
            MonsterData weakMonster = new MonsterData("Slime", 2, 1, 0, 1, 1);

            battleSystem.PlayerAttack(player, armoredMonster);
            battleSystem.PlayerAttack(player, weakMonster);

            Assert.That(armoredMonster.CurrentHp, Is.EqualTo(4));
            Assert.That(weakMonster.CurrentHp, Is.Zero);
        }

        [Test]
        public void MonsterAttackSubtractsAttackMinusDefense()
        {
            PlayerData player = new PlayerData("Angel");
            MonsterData monster = new MonsterData("Goblin", 35, 8, 1, 10, 15);

            string message = battleSystem.MonsterAttack(player, monster);

            Assert.That(player.CurrentHp, Is.EqualTo(95));
            Assert.That(message, Is.EqualTo("Goblin attacks Angel for 5 damage."));
        }

        [Test]
        public void MonsterAttackDealsAtLeastOneDamageAndClampsHpAtZero()
        {
            PlayerData durablePlayer = new PlayerData("Tank");
            durablePlayer.Defense = 100;
            PlayerData injuredPlayer = new PlayerData("Angel");
            injuredPlayer.CurrentHp = 2;
            MonsterData weakMonster = new MonsterData("Slime", 5, 1, 0, 1, 1);
            MonsterData strongMonster = new MonsterData("Ogre", 50, 100, 0, 20, 20);

            battleSystem.MonsterAttack(durablePlayer, weakMonster);
            battleSystem.MonsterAttack(injuredPlayer, strongMonster);

            Assert.That(durablePlayer.CurrentHp, Is.EqualTo(99));
            Assert.That(injuredPlayer.CurrentHp, Is.Zero);
        }
    }
}
