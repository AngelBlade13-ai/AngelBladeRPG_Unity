using System;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class JobCatalogTests
    {
        [Test]
        public void CatalogDefinesAllTwelveJobs()
        {
            JobDefinition[] jobs = JobCatalog.All.ToArray();

            Assert.That(jobs, Has.Length.EqualTo(12));
            Assert.That(
                jobs.Select(job => job.Id).Distinct().Count(),
                Is.EqualTo(12));
            Assert.That(
                jobs.Select(job => job.StableId).Distinct().Count(),
                Is.EqualTo(12));
            Assert.That(
                jobs.All(job => job.DemoMaximumTier == 3),
                Is.True);
        }

        [TestCase(JobId.Reaver)]
        [TestCase(JobId.BloodMage)]
        [TestCase(JobId.WhiteMage)]
        [TestCase(JobId.Paladin)]
        public void TentativeCharacterArchetypesHaveDefinitions(JobId jobId)
        {
            JobDefinition job = JobCatalog.Get(jobId);

            Assert.That(job, Is.Not.Null);
            Assert.That(job.Strength, Is.Not.Empty);
            Assert.That(job.TradeOff, Is.Not.Empty);
        }

        [Test]
        public void PaladinIsAHybridRatherThanAHealingSpecialist()
        {
            JobDefinition paladin = JobCatalog.Get(JobId.Paladin);

            Assert.That(paladin.Roles.HasFlag(JobRole.Tank), Is.True);
            Assert.That(
                paladin.Roles.HasFlag(JobRole.PhysicalDamage),
                Is.True);
            Assert.That(paladin.Roles.HasFlag(JobRole.Healer), Is.True);
            Assert.That(paladin.TradeOff, Does.Contain("specialist"));
        }

        [Test]
        public void UnknownJobDoesNotHaveDefinition()
        {
            JobDefinition job = JobCatalog.Get((JobId)999);

            Assert.That(job, Is.Null);
        }
    }
}
