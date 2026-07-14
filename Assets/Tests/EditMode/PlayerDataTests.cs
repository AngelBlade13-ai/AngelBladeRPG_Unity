using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class PlayerDataTests
    {
        [Test]
        public void NewPlayerStartsWithExpectedStats()
        {
            PlayerData player = new PlayerData("Angel");

            Assert.That(player.Name, Is.EqualTo("Angel"));
            Assert.That(player.Level, Is.EqualTo(1));
            Assert.That(player.MaxHp, Is.EqualTo(100));
            Assert.That(player.CurrentHp, Is.EqualTo(100));
            Assert.That(player.Attack, Is.EqualTo(12));
            Assert.That(player.Defense, Is.EqualTo(3));
            Assert.That(player.Gold, Is.Zero);
            Assert.That(player.XP, Is.Zero);
            Assert.That(player.XPToNextLevel, Is.EqualTo(50));
        }

        [Test]
        public void GainXPBelowThresholdStoresXPWithoutLeveling()
        {
            PlayerData player = new PlayerData("Angel");

            bool leveledUp = player.GainXP(49);

            Assert.That(leveledUp, Is.False);
            Assert.That(player.Level, Is.EqualTo(1));
            Assert.That(player.XP, Is.EqualTo(49));
            Assert.That(player.XPToNextLevel, Is.EqualTo(50));
        }

        [Test]
        public void GainXPAtThresholdLevelsUpAndImprovesStats()
        {
            PlayerData player = new PlayerData("Angel");
            player.CurrentHp = 1;

            bool leveledUp = player.GainXP(50);

            Assert.That(leveledUp, Is.True);
            Assert.That(player.Level, Is.EqualTo(2));
            Assert.That(player.XP, Is.Zero);
            Assert.That(player.XPToNextLevel, Is.EqualTo(75));
            Assert.That(player.MaxHp, Is.EqualTo(120));
            Assert.That(player.CurrentHp, Is.EqualTo(120));
            Assert.That(player.Attack, Is.EqualTo(15));
            Assert.That(player.Defense, Is.EqualTo(4));
        }

        [Test]
        public void GainXPPreservesOverflowAcrossMultipleLevels()
        {
            PlayerData player = new PlayerData("Angel");

            bool leveledUp = player.GainXP(135);

            Assert.That(leveledUp, Is.True);
            Assert.That(player.Level, Is.EqualTo(3));
            Assert.That(player.XP, Is.EqualTo(10));
            Assert.That(player.XPToNextLevel, Is.EqualTo(100));
            Assert.That(player.MaxHp, Is.EqualTo(140));
            Assert.That(player.CurrentHp, Is.EqualTo(140));
            Assert.That(player.Attack, Is.EqualTo(18));
            Assert.That(player.Defense, Is.EqualTo(5));
        }
    }
}
