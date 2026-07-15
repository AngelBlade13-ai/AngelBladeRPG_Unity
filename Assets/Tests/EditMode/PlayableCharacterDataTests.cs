using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class PlayableCharacterDataTests
    {
        [Test]
        public void CharacterCanUseAJobWithLowAffinity()
        {
            PlayableCharacterData character = CreateCharacter();
            character.SetJobAffinity(JobId.BloodMage, JobAffinity.Low);

            bool assigned = character.TryAssignJob(JobId.BloodMage);

            Assert.That(assigned, Is.True);
            Assert.That(character.CurrentJob, Is.EqualTo(JobId.BloodMage));
            Assert.That(
                character.GetJobAffinityMultiplier(JobId.BloodMage),
                Is.EqualTo(0.9f));
        }

        [Test]
        public void UnspecifiedAffinityIsNeutral()
        {
            PlayableCharacterData character = CreateCharacter();

            Assert.That(
                character.GetJobAffinity(JobId.WhiteMage),
                Is.EqualTo(JobAffinity.Neutral));
            Assert.That(
                character.GetJobAffinityMultiplier(JobId.WhiteMage),
                Is.EqualTo(1f));
        }

        [Test]
        public void RespecChangesAssignmentWithoutChangingIdentity()
        {
            PlayableCharacterData character = CreateCharacter();

            bool assigned = character.TryAssignJob(JobId.Paladin);

            Assert.That(assigned, Is.True);
            Assert.That(character.Id, Is.EqualTo("hero"));
            Assert.That(character.Name, Is.EqualTo("Angel"));
            Assert.That(character.CurrentJob, Is.EqualTo(JobId.Paladin));
        }

        [Test]
        public void PermanentlyRemovedCharacterCannotChangeJobs()
        {
            PlayableCharacterData character = CreateCharacter();
            character.RemovePermanently();

            bool assigned = character.TryAssignJob(JobId.Reaver);

            Assert.That(assigned, Is.False);
            Assert.That(character.IsAvailable, Is.False);
        }

        private static PlayableCharacterData CreateCharacter()
        {
            return new PlayableCharacterData("hero", "Angel", JobId.Knight);
        }
    }
}
