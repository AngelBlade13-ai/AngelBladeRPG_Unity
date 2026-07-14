using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class ExplorationStatusHUDTests
    {
        [Test]
        public void FormatPlayerStatusReturnsEmptyTextWithoutPlayer()
        {
            string status = ExplorationStatusHUD.FormatPlayerStatus(null);

            Assert.That(status, Is.Empty);
        }

        [Test]
        public void FormatPlayerStatusIncludesCurrentProgress()
        {
            PlayerData player = new PlayerData("Angel");
            player.CurrentHp = 75;
            player.Gold = 12;
            player.XP = 15;

            string status = ExplorationStatusHUD.FormatPlayerStatus(player);

            Assert.That(status, Does.Contain("Angel  Lv 1"));
            Assert.That(status, Does.Contain("HP 75/100"));
            Assert.That(status, Does.Contain("XP 15/50"));
            Assert.That(status, Does.Contain("Gold 12"));
        }
    }
}
