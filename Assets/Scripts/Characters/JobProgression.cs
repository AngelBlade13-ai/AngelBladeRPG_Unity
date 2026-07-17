using System;
using System.Collections.Generic;

public sealed class JobProgression
{
    private readonly HashSet<string> learnedNodeIds =
        new HashSet<string>(StringComparer.Ordinal);

    public int JobPoints { get; private set; }

    public IReadOnlyCollection<string> LearnedNodeIds =>
        new List<string>(learnedNodeIds).AsReadOnly();

    public bool TryAddJobPoints(int amount)
    {
        if (amount <= 0 || JobPoints > int.MaxValue - amount)
        {
            return false;
        }

        JobPoints += amount;
        return true;
    }

    public bool HasLearned(string nodeId)
    {
        return !string.IsNullOrWhiteSpace(nodeId) &&
            learnedNodeIds.Contains(nodeId.Trim());
    }

    public bool TryPurchase(JobNodeDefinition node)
    {
        if (node == null || learnedNodeIds.Contains(node.StableId) ||
            JobPoints < node.Cost)
        {
            return false;
        }

        JobDefinition job = JobCatalog.Get(node.JobId);
        if (job == null || node.Tier > job.DemoMaximumTier)
        {
            return false;
        }

        foreach (string prerequisiteId in node.PrerequisiteIds)
        {
            if (!learnedNodeIds.Contains(prerequisiteId))
            {
                return false;
            }
        }

        JobPoints -= node.Cost;
        learnedNodeIds.Add(node.StableId);
        return true;
    }

    public PermanentStatBonuses GetPermanentStatBonuses()
    {
        PermanentStatBonuses totals = new PermanentStatBonuses();

        foreach (string nodeId in learnedNodeIds)
        {
            JobNodeDefinition node = JobNodeCatalog.Get(nodeId);
            if (node == null || node.Kind != JobNodeKind.PermanentStat)
            {
                continue;
            }

            foreach (JobStatBonus bonus in node.StatBonuses)
            {
                totals.Add(bonus);
            }
        }

        return totals;
    }

    public bool CanUseLearnedJobFeature(string nodeId, JobId equippedJob)
    {
        JobNodeDefinition node = JobNodeCatalog.Get(nodeId);
        return node != null &&
            node.JobId == equippedJob &&
            node.Kind != JobNodeKind.PermanentStat &&
            learnedNodeIds.Contains(node.StableId);
    }
}
