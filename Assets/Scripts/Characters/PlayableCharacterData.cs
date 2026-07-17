using System;
using System.Collections.Generic;

public enum JobAffinity
{
    Low,
    Neutral,
    High
}

public class PlayableCharacterData
{
    private readonly Dictionary<JobId, JobAffinity> jobAffinities =
        new Dictionary<JobId, JobAffinity>();
    private readonly JobProgression jobProgression = new JobProgression();

    public string Id { get; }
    public string Name { get; }
    public JobId CurrentJob { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public CharacterRosterHistory RosterHistory { get; }
    public int JobPoints => jobProgression.JobPoints;
    public IReadOnlyCollection<string> LearnedJobNodeIds =>
        jobProgression.LearnedNodeIds;

    public PlayableCharacterData(
        string id,
        string name,
        JobId startingJob)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("A character ID is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A character name is required.", nameof(name));
        }

        if (!JobCatalog.Contains(startingJob))
        {
            throw new ArgumentOutOfRangeException(nameof(startingJob));
        }

        Id = id.Trim();
        Name = name.Trim();
        CurrentJob = startingJob;
        RosterHistory = new CharacterRosterHistory(Id);
    }

    public bool TryAssignJob(JobId jobId)
    {
        if (!IsAvailable || !JobCatalog.Contains(jobId))
        {
            return false;
        }

        CurrentJob = jobId;
        return true;
    }

    public bool TryAddJobPoints(int amount)
    {
        return IsAvailable && jobProgression.TryAddJobPoints(amount);
    }

    public bool TryPurchaseJobNode(string nodeId)
    {
        if (!IsAvailable || string.IsNullOrWhiteSpace(nodeId))
        {
            return false;
        }

        return jobProgression.TryPurchase(JobNodeCatalog.Get(nodeId));
    }

    public bool HasLearnedJobNode(string nodeId)
    {
        return jobProgression.HasLearned(nodeId);
    }

    public bool CanUseLearnedJobFeature(string nodeId)
    {
        return IsAvailable &&
            jobProgression.CanUseLearnedJobFeature(nodeId, CurrentJob);
    }

    public PermanentStatBonuses GetPermanentStatBonuses()
    {
        return jobProgression.GetPermanentStatBonuses();
    }

    public void SetJobAffinity(JobId jobId, JobAffinity affinity)
    {
        if (!JobCatalog.Contains(jobId))
        {
            throw new ArgumentOutOfRangeException(nameof(jobId));
        }

        jobAffinities[jobId] = affinity;
    }

    public JobAffinity GetJobAffinity(JobId jobId)
    {
        return jobAffinities.TryGetValue(jobId, out JobAffinity affinity)
            ? affinity
            : JobAffinity.Neutral;
    }

    public float GetJobAffinityMultiplier(JobId jobId)
    {
        switch (GetJobAffinity(jobId))
        {
            case JobAffinity.Low:
                return 0.9f;
            case JobAffinity.High:
                return 1.1f;
            default:
                return 1f;
        }
    }

    public void RemovePermanently()
    {
        IsAvailable = false;
    }
}
