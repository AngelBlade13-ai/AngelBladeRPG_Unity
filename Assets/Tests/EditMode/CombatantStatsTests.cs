using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class CombatantStatsTests
    {
        [Test]
        public void NewStatsBeginWithFullHpAndMp()
        {
            CombatantStats stats = CreateStats();

            Assert.That(stats.CurrentHp, Is.EqualTo(100));
            Assert.That(stats.CurrentMp, Is.EqualTo(20));
        }

        [Test]
        public void DamageClampsAtZeroAndReportsActualDamage()
        {
            CombatantStats stats = CreateStats();

            int damage = stats.ApplyDamage(150);

            Assert.That(damage, Is.EqualTo(100));
            Assert.That(stats.CurrentHp, Is.Zero);
        }

        [Test]
        public void HealingClampsAtMaximumHp()
        {
            CombatantStats stats = CreateStats();
            stats.CurrentHp = 90;

            int restored = stats.RestoreHp(50);

            Assert.That(restored, Is.EqualTo(10));
            Assert.That(stats.CurrentHp, Is.EqualTo(100));
        }

        [Test]
        public void SpendingMpRejectsUnaffordableCost()
        {
            CombatantStats stats = CreateStats();

            bool spent = stats.TrySpendMp(25);

            Assert.That(spent, Is.False);
            Assert.That(stats.CurrentMp, Is.EqualTo(20));
        }

        [Test]
        public void SpendingAndRestoringMpStayWithinLimits()
        {
            CombatantStats stats = CreateStats();

            bool spent = stats.TrySpendMp(8);
            int restored = stats.RestoreMp(20);

            Assert.That(spent, Is.True);
            Assert.That(restored, Is.EqualTo(8));
            Assert.That(stats.CurrentMp, Is.EqualTo(20));
        }

        [Test]
        public void PlayerExposesSharedSpeedAndMagicStats()
        {
            PlayerData player = new PlayerData("Angel");

            Assert.That(player.Speed, Is.EqualTo(10));
            Assert.That(player.MaxMp, Is.EqualTo(20));
            Assert.That(player.MagicPower, Is.EqualTo(8));
            Assert.That(player.Stats.CurrentHp, Is.EqualTo(player.CurrentHp));
        }

        [Test]
        public void MonsterAcceptsOptionalSpeedAndMagicStats()
        {
            MonsterData monster = new MonsterData(
                "Wisp",
                30,
                4,
                2,
                5,
                8,
                14,
                12,
                10,
                9);

            Assert.That(monster.Speed, Is.EqualTo(14));
            Assert.That(monster.MaxMp, Is.EqualTo(12));
            Assert.That(monster.MagicPower, Is.EqualTo(10));
            Assert.That(monster.MagicDefense, Is.EqualTo(9));
        }

        [Test]
        public void AccuracyEvasionAndCriticalChanceAreClampedPercentages()
        {
            CombatantStats stats = CreateStats();

            stats.Accuracy = 150;
            stats.Evasion = -5;
            stats.CriticalChance = 125;

            Assert.That(stats.Accuracy, Is.EqualTo(100));
            Assert.That(stats.Evasion, Is.Zero);
            Assert.That(stats.CriticalChance, Is.EqualTo(100));
        }

        [Test]
        public void PlayerAndMonsterExposeCombatRollStats()
        {
            PlayerData player = new PlayerData("Angel");
            MonsterData monster = new MonsterData(
                "Goblin", 35, 8, 1, 10, 15);

            Assert.That(player.Accuracy, Is.EqualTo(95));
            Assert.That(player.Evasion, Is.EqualTo(5));
            Assert.That(player.CriticalChance, Is.EqualTo(10));
            Assert.That(monster.Accuracy, Is.EqualTo(95));
        }

        private static CombatantStats CreateStats()
        {
            return new CombatantStats(100, 12, 3, 10, 20, 8, 3);
        }
    }
}
