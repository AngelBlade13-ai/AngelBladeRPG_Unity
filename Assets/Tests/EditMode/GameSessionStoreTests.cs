using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class GameSessionStoreTests
    {
        [Test]
        public void BeginNewSessionReplacesPreviousSession()
        {
            GameSession previous = GameSessionStore.BeginNewSession();
            previous.TryStartNewGame("Old Hero");

            GameSession current = GameSessionStore.BeginNewSession();

            Assert.That(current, Is.SameAs(GameSessionStore.Current));
            Assert.That(current, Is.Not.SameAs(previous));
            Assert.That(current.HasPlayer, Is.False);
        }
    }
}
