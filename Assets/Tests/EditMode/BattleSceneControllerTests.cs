using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class BattleSceneControllerTests
    {
        [Test]
        public void FormatCombatantStatusIncludesNameAndHp()
        {
            string status = BattleSceneController.FormatCombatantStatus(
                "Goblin",
                12,
                35);

            Assert.That(status, Is.EqualTo("Goblin\nHP 12/35"));
        }
    }
}
