using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class NewGameConfirmationStateTests
    {
        [Test]
        public void NewGameWithoutExistingSaveStartsImmediately()
        {
            var state = new NewGameConfirmationState();
            state.Begin(existingSaveFound: false);

            Assert.That(state.TryConfirm(), Is.True);
            Assert.That(state.ConfirmationRequired, Is.False);
        }

        [Test]
        public void ExistingSaveRequiresTwoConfirmAttempts()
        {
            var state = new NewGameConfirmationState();
            state.Begin(existingSaveFound: true);

            Assert.That(state.TryConfirm(), Is.False);
            Assert.That(state.ConfirmationAcknowledged, Is.True);
            Assert.That(state.TryConfirm(), Is.True);
        }

        [Test]
        public void BeginningOrResettingClearsPriorAcknowledgement()
        {
            var state = new NewGameConfirmationState();
            state.Begin(existingSaveFound: true);
            state.TryConfirm();

            state.Begin(existingSaveFound: true);

            Assert.That(state.ConfirmationAcknowledged, Is.False);
            Assert.That(state.TryConfirm(), Is.False);
            state.Reset();
            Assert.That(state.TryConfirm(), Is.True);
        }
    }
}
