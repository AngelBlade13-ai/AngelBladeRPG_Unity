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

        [Test]
        public void CharacterIsACombatantWithStableIdentityAndOwnedStats()
        {
            PlayableCharacterData character = CreateCharacter();

            Assert.That(character.CombatantId, Is.EqualTo("hero"));
            Assert.That(character.DisplayName, Is.EqualTo("Angel"));
            Assert.That(character.Stats, Is.Not.Null);
            Assert.That(character.IsIncapacitated, Is.False);

            character.Stats.CurrentHp = 0;
            Assert.That(character.IsIncapacitated, Is.True);
        }

        [Test]
        public void CharacterCanShareExistingPersistentCombatStats()
        {
            CombatantStats stats = new CombatantStats(80, 9, 4, 7, 10, 5, 3);
            PlayableCharacterData character = new PlayableCharacterData(
                "hero",
                "Angel",
                JobId.Mercenary,
                stats);

            character.Stats.ApplyDamage(12);

            Assert.That(character.Stats, Is.SameAs(stats));
            Assert.That(stats.CurrentHp, Is.EqualTo(68));
        }

        private static PlayableCharacterData CreateCharacter()
        {
            return new PlayableCharacterData("hero", "Angel", JobId.Knight);
        }
    }
}
