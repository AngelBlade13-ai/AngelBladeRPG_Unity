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

        [Test]
        public void EquippedJobAddsItsNeutralStatModifiers()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Mercenary);

            Assert.That(character.Stats.Attack, Is.EqualTo(17));
            Assert.That(character.Stats.CriticalChance, Is.EqualTo(20));
        }

        [Test]
        public void AffinityScalesOnlyTheEquippedJobContribution()
        {
            PlayableCharacterData high = CreateCharacter(JobId.Mercenary);
            PlayableCharacterData low = CreateCharacter(JobId.Mercenary);

            high.SetJobAffinity(JobId.Mercenary, JobAffinity.High);
            low.SetJobAffinity(JobId.Mercenary, JobAffinity.Low);

            Assert.That(high.Stats.Attack, Is.EqualTo(18));
            Assert.That(low.Stats.Attack, Is.EqualTo(16));
            Assert.That(high.Stats.CriticalChance, Is.EqualTo(21));
            Assert.That(low.Stats.CriticalChance, Is.EqualTo(19));
        }

        [Test]
        public void SwitchingJobsReplacesBonusesWithoutStackingOrHealing()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Mercenary);
            character.Stats.ApplyDamage(30);

            character.TryAssignJob(JobId.Knight);

            Assert.That(character.Stats.MaxHp, Is.EqualTo(120));
            Assert.That(character.Stats.CurrentHp, Is.EqualTo(90));
            Assert.That(character.Stats.Attack, Is.EqualTo(12));
            Assert.That(character.Stats.Defense, Is.EqualTo(8));

            character.TryAssignJob(JobId.Mercenary);

            Assert.That(character.Stats.MaxHp, Is.EqualTo(100));
            Assert.That(character.Stats.CurrentHp, Is.EqualTo(70));
            Assert.That(character.Stats.Attack, Is.EqualTo(17));
        }

        [Test]
        public void PermanentMaxHpNodePreservesMissingHpAcrossJobs()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Knight);
            character.Stats.ApplyDamage(30);
            character.TryAddJobPoints(1);

            bool purchased = character.TryPurchaseJobNode(
                "job_knight_fortitude_1");

            Assert.That(purchased, Is.True);
            Assert.That(character.Stats.MaxHp, Is.EqualTo(130));
            Assert.That(character.Stats.CurrentHp, Is.EqualTo(100));

            character.TryAssignJob(JobId.Mage);

            Assert.That(character.Stats.MaxHp, Is.EqualTo(110));
            Assert.That(character.Stats.CurrentHp, Is.EqualTo(80));
        }

        [Test]
        public void ExternalLevelGrowthSurvivesLaterJobChanges()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Mercenary);
            character.Stats.MaxHp += 20;
            character.Stats.CurrentHp = character.Stats.MaxHp;
            character.Stats.Attack += 3;

            character.TryAssignJob(JobId.Knight);
            character.TryAssignJob(JobId.Mercenary);

            Assert.That(character.Stats.MaxHp, Is.EqualTo(120));
            Assert.That(character.Stats.CurrentHp, Is.EqualTo(120));
            Assert.That(character.Stats.Attack, Is.EqualTo(20));
        }

        [Test]
        public void RecalculatingMaximumHpDoesNotReviveACharacter()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Mercenary);
            character.Stats.CurrentHp = 0;

            character.TryAssignJob(JobId.Knight);

            Assert.That(character.Stats.CurrentHp, Is.Zero);
        }

        [Test]
        public void CharacterStartsWithIndependentLevelProgression()
        {
            PlayableCharacterData character = CreateCharacter();

            Assert.That(character.Level, Is.EqualTo(1));
            Assert.That(character.XP, Is.Zero);
            Assert.That(character.XPToNextLevel, Is.EqualTo(50));
        }

        [Test]
        public void GainXPLevelsCompanionAndRestoresHp()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Mercenary);
            character.Stats.CurrentHp = 1;

            int levelsGained = character.GainXP(50);

            Assert.That(levelsGained, Is.EqualTo(1));
            Assert.That(character.Level, Is.EqualTo(2));
            Assert.That(character.XP, Is.Zero);
            Assert.That(character.Stats.MaxHp, Is.EqualTo(120));
            Assert.That(character.Stats.CurrentHp, Is.EqualTo(120));
            Assert.That(character.Stats.Attack, Is.EqualTo(20));
            Assert.That(character.Stats.Defense, Is.EqualTo(4));
        }

        [Test]
        public void LevelGrowthSurvivesJobChangesWithoutStacking()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Mercenary);
            character.GainXP(50);

            character.TryAssignJob(JobId.Knight);
            character.TryAssignJob(JobId.Mercenary);

            Assert.That(character.Stats.MaxHp, Is.EqualTo(120));
            Assert.That(character.Stats.Attack, Is.EqualTo(20));
            Assert.That(character.Stats.Defense, Is.EqualTo(4));
        }

        [Test]
        public void PermanentlyRemovedCharacterCannotGainXP()
        {
            PlayableCharacterData character = CreateCharacter();
            character.RemovePermanently();

            int levelsGained = character.GainXP(50);

            Assert.That(levelsGained, Is.Zero);
            Assert.That(character.Level, Is.EqualTo(1));
            Assert.That(character.XP, Is.Zero);
        }

        private static PlayableCharacterData CreateCharacter()
        {
            return new PlayableCharacterData("hero", "Angel", JobId.Knight);
        }

        private static PlayableCharacterData CreateCharacter(JobId jobId)
        {
            return new PlayableCharacterData("hero", "Angel", jobId);
        }
    }
}
