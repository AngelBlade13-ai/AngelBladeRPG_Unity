using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class JobProgressionTests
    {
        [Test]
        public void PurchaseSpendsPointsAndCannotBeRepeated()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Knight);
            character.TryAddJobPoints(3);

            bool purchased = character.TryPurchaseJobNode(
                "job_knight_shield_bash");
            bool repeated = character.TryPurchaseJobNode(
                "job_knight_shield_bash");

            Assert.That(purchased, Is.True);
            Assert.That(repeated, Is.False);
            Assert.That(character.JobPoints, Is.EqualTo(2));
            Assert.That(
                character.HasLearnedJobNode("job_knight_shield_bash"),
                Is.True);
        }

        [Test]
        public void FailedPurchaseDoesNotSpendPoints()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Knight);
            character.TryAddJobPoints(5);

            bool purchased = character.TryPurchaseJobNode(
                "job_knight_hold_the_line");

            Assert.That(purchased, Is.False);
            Assert.That(character.JobPoints, Is.EqualTo(5));
            Assert.That(character.LearnedJobNodeIds, Is.Empty);
        }

        [Test]
        public void EndpointUnlocksAfterBothPrerequisites()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Knight);
            character.TryAddJobPoints(4);
            character.TryPurchaseJobNode("job_knight_shield_bash");
            character.TryPurchaseJobNode("job_knight_interpose");

            bool purchased = character.TryPurchaseJobNode(
                "job_knight_hold_the_line");

            Assert.That(purchased, Is.True);
            Assert.That(character.JobPoints, Is.Zero);
        }

        [Test]
        public void LowAffinityDoesNotBlockLearningAnotherJob()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Knight);
            character.SetJobAffinity(JobId.Mage, JobAffinity.Low);
            character.TryAddJobPoints(1);

            bool purchased = character.TryPurchaseJobNode(
                "job_mage_frostbind");

            Assert.That(purchased, Is.True);
        }

        [Test]
        public void LearnedFeatureOnlyWorksWhileItsJobIsEquipped()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Knight);
            character.TryAddJobPoints(1);
            character.TryPurchaseJobNode("job_knight_shield_bash");

            Assert.That(
                character.CanUseLearnedJobFeature("job_knight_shield_bash"),
                Is.True);

            character.TryAssignJob(JobId.Mage);
            Assert.That(
                character.CanUseLearnedJobFeature("job_knight_shield_bash"),
                Is.False);

            character.TryAssignJob(JobId.Knight);
            Assert.That(
                character.CanUseLearnedJobFeature("job_knight_shield_bash"),
                Is.True);
        }

        [Test]
        public void PermanentStatsRemainAfterChangingJobs()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Knight);
            character.TryAddJobPoints(3);
            character.TryPurchaseJobNode("job_knight_fortitude_1");
            character.TryPurchaseJobNode("job_knight_fortitude_2");
            character.TryPurchaseJobNode("job_mage_spellcraft_1");

            character.TryAssignJob(JobId.Rogue);
            PermanentStatBonuses bonuses =
                character.GetPermanentStatBonuses();

            Assert.That(bonuses.MaxHp, Is.EqualTo(20));
            Assert.That(bonuses.MagicPower, Is.EqualTo(1));
        }

        [Test]
        public void PermanentStatNodeIsNotAnEquippedJobFeature()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Knight);
            character.TryAddJobPoints(1);
            character.TryPurchaseJobNode("job_knight_fortitude_1");

            Assert.That(
                character.CanUseLearnedJobFeature("job_knight_fortitude_1"),
                Is.False);
        }

        [Test]
        public void TierCapRejectsFutureNodeWithoutSpendingPoints()
        {
            JobProgression progression = new JobProgression();
            progression.TryAddJobPoints(3);
            JobNodeDefinition futureNode = new JobNodeDefinition(
                "future_knight_node",
                JobId.Knight,
                "Future Knight Node",
                JobNodeKind.Ability,
                4,
                2);

            bool purchased = progression.TryPurchase(futureNode);

            Assert.That(purchased, Is.False);
            Assert.That(progression.JobPoints, Is.EqualTo(3));
        }

        [Test]
        public void RemovedCharacterCannotGainOrSpendJobPoints()
        {
            PlayableCharacterData character = CreateCharacter(JobId.Knight);
            character.TryAddJobPoints(2);
            character.RemovePermanently();

            Assert.That(character.TryAddJobPoints(1), Is.False);
            Assert.That(
                character.TryPurchaseJobNode("job_knight_shield_bash"),
                Is.False);
            Assert.That(character.JobPoints, Is.EqualTo(2));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void JobPointAwardsMustBePositive(int amount)
        {
            PlayableCharacterData character = CreateCharacter(JobId.Knight);

            Assert.That(character.TryAddJobPoints(amount), Is.False);
            Assert.That(character.JobPoints, Is.Zero);
        }

        private static PlayableCharacterData CreateCharacter(JobId jobId)
        {
            return new PlayableCharacterData("test-member", "Test Member", jobId);
        }
    }
}
