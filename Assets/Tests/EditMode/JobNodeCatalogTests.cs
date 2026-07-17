using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class JobNodeCatalogTests
    {
        [Test]
        public void CatalogDefinesEightNodesForEachJob()
        {
            JobNodeDefinition[] nodes = JobNodeCatalog.All.ToArray();

            Assert.That(nodes, Has.Length.EqualTo(96));
            foreach (JobDefinition job in JobCatalog.All)
            {
                Assert.That(
                    JobNodeCatalog.GetForJob(job.Id),
                    Has.Count.EqualTo(8),
                    job.DisplayName);
            }
        }

        [Test]
        public void EveryJobTreeCostsTenPointsToComplete()
        {
            foreach (JobDefinition job in JobCatalog.All)
            {
                int totalCost = JobNodeCatalog.GetForJob(job.Id)
                    .Sum(node => node.Cost);

                Assert.That(totalCost, Is.EqualTo(10), job.DisplayName);
            }
        }

        [Test]
        public void EveryJobTreeHasTheExpectedDemoShape()
        {
            foreach (JobDefinition job in JobCatalog.All)
            {
                JobNodeDefinition[] nodes =
                    JobNodeCatalog.GetForJob(job.Id).ToArray();

                Assert.That(nodes.Count(node => node.Tier == 1), Is.EqualTo(4));
                Assert.That(nodes.Count(node => node.Tier == 2), Is.EqualTo(3));
                Assert.That(nodes.Count(node => node.Tier == 3), Is.EqualTo(1));
                Assert.That(
                    nodes.Count(node => node.Kind == JobNodeKind.PermanentStat),
                    Is.EqualTo(5));
            }
        }

        [Test]
        public void AuthoredCatalogPassesRelationshipValidation()
        {
            bool valid = JobNodeCatalog.Validate(
                JobNodeCatalog.All,
                out string error);

            Assert.That(valid, Is.True, error);
        }

        [Test]
        public void ValidationRejectsDuplicateStableIds()
        {
            JobNodeDefinition first = Ability(
                "duplicate", JobId.Knight, 1);
            JobNodeDefinition second = Ability(
                "duplicate", JobId.Knight, 1);

            bool valid = JobNodeCatalog.Validate(
                new[] { first, second },
                out string error);

            Assert.That(valid, Is.False);
            Assert.That(error, Does.Contain("Duplicate"));
        }

        [Test]
        public void ValidationRejectsMissingPrerequisites()
        {
            JobNodeDefinition node = new JobNodeDefinition(
                "job_knight_missing_test",
                JobId.Knight,
                "Missing Test",
                JobNodeKind.Ability,
                2,
                1,
                new[] { "does_not_exist" });

            bool valid = JobNodeCatalog.Validate(
                new[] { node },
                out string error);

            Assert.That(valid, Is.False);
            Assert.That(error, Does.Contain("missing prerequisite"));
        }

        [Test]
        public void ValidationRejectsCyclicRelationships()
        {
            JobNodeDefinition first = new JobNodeDefinition(
                "cycle_a", JobId.Knight, "Cycle A",
                JobNodeKind.Ability, 2, 1, new[] { "cycle_b" });
            JobNodeDefinition second = new JobNodeDefinition(
                "cycle_b", JobId.Knight, "Cycle B",
                JobNodeKind.Ability, 2, 1, new[] { "cycle_a" });

            bool valid = JobNodeCatalog.Validate(
                new[] { first, second },
                out string error);

            Assert.That(valid, Is.False);
            Assert.That(error, Does.Contain("earlier tier"));
        }

        [Test]
        public void ValidationRejectsNodesBeyondDemoTierCap()
        {
            bool valid = JobNodeCatalog.Validate(
                new[] { Ability("tier_four", JobId.Knight, 4) },
                out string error);

            Assert.That(valid, Is.False);
            Assert.That(error, Does.Contain("tier cap"));
        }

        [Test]
        public void UnknownNodeIdDoesNotResolve()
        {
            Assert.That(JobNodeCatalog.Get("missing_node"), Is.Null);
            Assert.That(JobNodeCatalog.Contains("missing_node"), Is.False);
        }

        private static JobNodeDefinition Ability(
            string id,
            JobId jobId,
            int tier)
        {
            return new JobNodeDefinition(
                id,
                jobId,
                "Test Ability",
                JobNodeKind.Ability,
                tier,
                1);
        }
    }
}
