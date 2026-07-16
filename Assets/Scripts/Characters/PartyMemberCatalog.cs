using System.Collections.Generic;

public static class PartyMemberCatalog
{
    private static readonly Dictionary<string, PartyMemberDefinition> members =
        new Dictionary<string, PartyMemberDefinition>
        {
            {
                "pc_01",
                new PartyMemberDefinition(
                    "pc_01",
                    "Iona",
                    true,
                    JobId.WhiteMage,
                    "Warm, steady, easygoing, and inclined to put others first.",
                    "Prioritizes keeping allies alive over dealing damage.",
                    HighAffinity(JobId.WhiteMage))
            },
            {
                "pc_02",
                new PartyMemberDefinition(
                    "pc_02",
                    "Damari",
                    false,
                    JobId.Reaver,
                    "Chaotic, crude, and unwilling to hold back.",
                    "Favors aggressive front-line fighting, taking hits, and dealing heavy damage.",
                    HighAffinity(JobId.Reaver))
            },
            {
                "pc_03",
                new PartyMemberDefinition(
                    "pc_03",
                    "Enora",
                    false,
                    JobId.BloodMage,
                    "Bold, dramatic, and drawn to high-risk, high-impact choices.",
                    "Comfortably spends her own HP for power and favors a scythe.",
                    HighAffinity(JobId.BloodMage))
            },
            {
                "pc_04",
                new PartyMemberDefinition(
                    "pc_04",
                    "Lysander",
                    true,
                    JobId.Paladin,
                    "Grizzled, independent, experienced, measured, and protective.",
                    "Balances front-line fighting with protection and rallying support.",
                    HighAffinity(JobId.Paladin))
            }
        };

    public static IReadOnlyCollection<PartyMemberDefinition> All =>
        members.Values;

    public static PartyMemberDefinition Get(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
        {
            return null;
        }

        return members.TryGetValue(
            characterId.Trim(),
            out PartyMemberDefinition member)
            ? member
            : null;
    }

    private static Dictionary<JobId, JobAffinity> HighAffinity(JobId jobId)
    {
        return new Dictionary<JobId, JobAffinity>
        {
            { jobId, JobAffinity.High }
        };
    }
}
