using System.Collections.Generic;

public class PartyMemberDefinition
{
    private readonly Dictionary<JobId, JobAffinity> jobAffinities;

    public string Id { get; }
    public string DisplayName { get; }
    public bool HasPlaceholderName { get; }
    public JobId NaturalJob { get; }
    public string PersonalitySummary { get; }
    public string CombatPreference { get; }

    public PartyMemberDefinition(
        string id,
        string displayName,
        bool hasPlaceholderName,
        JobId naturalJob,
        string personalitySummary,
        string combatPreference,
        Dictionary<JobId, JobAffinity> affinities)
    {
        Id = id;
        DisplayName = displayName;
        HasPlaceholderName = hasPlaceholderName;
        NaturalJob = naturalJob;
        PersonalitySummary = personalitySummary;
        CombatPreference = combatPreference;
        jobAffinities = new Dictionary<JobId, JobAffinity>(affinities);
    }

    public JobAffinity GetJobAffinity(JobId jobId)
    {
        return jobAffinities.TryGetValue(jobId, out JobAffinity affinity)
            ? affinity
            : JobAffinity.Neutral;
    }

    public PlayableCharacterData CreateCharacter()
    {
        PlayableCharacterData character = new PlayableCharacterData(
            Id,
            DisplayName,
            NaturalJob);

        foreach (KeyValuePair<JobId, JobAffinity> affinity in jobAffinities)
        {
            character.SetJobAffinity(affinity.Key, affinity.Value);
        }

        return character;
    }
}
