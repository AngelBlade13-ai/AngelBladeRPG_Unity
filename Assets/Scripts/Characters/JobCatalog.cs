using System.Collections.Generic;

public static class JobCatalog
{
    private static readonly Dictionary<JobId, JobDefinition> jobs =
        new Dictionary<JobId, JobDefinition>
        {
            {
                JobId.Knight,
                new JobDefinition(
                    JobId.Knight,
                    "Knight",
                    JobRole.Tank,
                    "Heavy defense and reliable protection.",
                    "Slow and deals modest damage.")
            },
            {
                JobId.Reaver,
                new JobDefinition(
                    JobId.Reaver,
                    "Reaver",
                    JobRole.Tank | JobRole.PhysicalDamage,
                    "Absorbs punishment and converts risk into heavy damage.",
                    "Less reliable defense and performs best while endangered.")
            },
            {
                JobId.Mercenary,
                new JobDefinition(
                    JobId.Mercenary,
                    "Mercenary",
                    JobRole.PhysicalDamage,
                    "Consistent direct melee damage.",
                    "Offers little healing or party utility.")
            },
            {
                JobId.Rogue,
                new JobDefinition(
                    JobId.Rogue,
                    "Rogue",
                    JobRole.PhysicalDamage | JobRole.Support,
                    "Fast burst damage, evasion, and status pressure.",
                    "Fragile when focused by enemies.")
            },
            {
                JobId.Ranger,
                new JobDefinition(
                    JobId.Ranger,
                    "Ranger",
                    JobRole.PhysicalDamage | JobRole.Support,
                    "Ranged damage and tactical utility.",
                    "Weaker at close range or without setup.")
            },
            {
                JobId.Mage,
                new JobDefinition(
                    JobId.Mage,
                    "Mage",
                    JobRole.MagicDamage,
                    "Powerful and flexible elemental magic.",
                    "Fragile and dependent on a limited resource.")
            },
            {
                JobId.BloodMage,
                new JobDefinition(
                    JobId.BloodMage,
                    "Blood Mage",
                    JobRole.MagicDamage,
                    "Spends health to cast unusually powerful magic.",
                    "Self-damage puts pressure on party healing.")
            },
            {
                JobId.WhiteMage,
                new JobDefinition(
                    JobId.WhiteMage,
                    "White Mage",
                    JobRole.Healer | JobRole.Support,
                    "Strong restoration and protective support.",
                    "Has minimal personal offense.")
            },
            {
                JobId.Paladin,
                new JobDefinition(
                    JobId.Paladin,
                    "Paladin",
                    JobRole.Tank | JobRole.PhysicalDamage |
                        JobRole.Healer | JobRole.Support,
                    "Combines front-line damage, protection, limited healing, and leadership support.",
                    "Splits resources across roles and cannot match a dedicated specialist.")
            },
            {
                JobId.Bard,
                new JobDefinition(
                    JobId.Bard,
                    "Bard",
                    JobRole.Support | JobRole.Healer,
                    "Buffs allies, hinders enemies, and provides minor healing.",
                    "Deals little damage and has low durability.")
            },
            {
                JobId.Tactician,
                new JobDefinition(
                    JobId.Tactician,
                    "Tactician",
                    JobRole.Support,
                    "Controls battlefield conditions and action timing.",
                    "Has minimal direct damage and durability.")
            },
            {
                JobId.Summoner,
                new JobDefinition(
                    JobId.Summoner,
                    "Summoner",
                    JobRole.MagicDamage | JobRole.Support,
                    "Calls a temporary creature ally for flexible pressure.",
                    "The summon is temporary and vulnerable to disruption.")
            }
        };

    public static IReadOnlyCollection<JobDefinition> All => jobs.Values;

    public static bool Contains(JobId jobId)
    {
        return jobs.ContainsKey(jobId);
    }

    public static JobDefinition Get(JobId jobId)
    {
        return jobs.TryGetValue(jobId, out JobDefinition definition)
            ? definition
            : null;
    }
}
