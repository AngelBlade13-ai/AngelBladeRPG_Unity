using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class BattleReturnStoreTests
    {
        [TearDown]
        public void TearDown()
        {
            BattleReturnStore.Clear();
        }

        [Test]
        public void RequestReturnTrimsAndStoresDestination()
        {
            bool requested = BattleReturnStore.RequestReturn(
                "  TownScene  ",
                "  TownAfterBattle  ");

            bool consumed = BattleReturnStore.TryConsumeReturn(
                out string sceneName,
                out string spawnId);

            Assert.That(requested, Is.True);
            Assert.That(consumed, Is.True);
            Assert.That(sceneName, Is.EqualTo("TownScene"));
            Assert.That(spawnId, Is.EqualTo("TownAfterBattle"));
        }

        [Test]
        public void TryConsumeReturnClearsDestination()
        {
            BattleReturnStore.RequestReturn("TownScene", "TownAfterBattle");

            BattleReturnStore.TryConsumeReturn(out _, out _);
            bool consumedAgain =
                BattleReturnStore.TryConsumeReturn(out _, out _);

            Assert.That(consumedAgain, Is.False);
            Assert.That(BattleReturnStore.HasReturnDestination, Is.False);
        }

        [TestCase("", "TownAfterBattle")]
        [TestCase("TownScene", "")]
        [TestCase("   ", "   ")]
        public void RequestReturnRejectsBlankDestination(
            string sceneName,
            string spawnId)
        {
            bool requested = BattleReturnStore.RequestReturn(
                sceneName,
                spawnId);

            Assert.That(requested, Is.False);
            Assert.That(BattleReturnStore.HasReturnDestination, Is.False);
        }
    }
}
