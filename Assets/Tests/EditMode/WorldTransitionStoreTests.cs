using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class WorldTransitionStoreTests
    {
        [TearDown]
        public void TearDown()
        {
            WorldTransitionStore.Clear();
        }

        [Test]
        public void RequestSpawnTrimsAndStoresSpawnId()
        {
            bool requested = WorldTransitionStore.RequestSpawn("  InnExit  ");

            bool consumed = WorldTransitionStore.TryConsumeSpawn(
                out string spawnId);

            Assert.That(requested, Is.True);
            Assert.That(consumed, Is.True);
            Assert.That(spawnId, Is.EqualTo("InnExit"));
        }

        [Test]
        public void TryConsumeSpawnClearsPendingRequest()
        {
            WorldTransitionStore.RequestSpawn("InnExit");

            WorldTransitionStore.TryConsumeSpawn(out _);
            bool consumedAgain = WorldTransitionStore.TryConsumeSpawn(out _);

            Assert.That(consumedAgain, Is.False);
            Assert.That(WorldTransitionStore.HasPendingSpawn, Is.False);
        }

        [Test]
        public void RequestSpawnRejectsBlankId()
        {
            bool requested = WorldTransitionStore.RequestSpawn("   ");

            Assert.That(requested, Is.False);
            Assert.That(WorldTransitionStore.HasPendingSpawn, Is.False);
        }
    }
}
