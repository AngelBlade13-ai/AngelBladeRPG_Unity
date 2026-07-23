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
                    "job_knight",
                    "Knight",
                    JobRole.Tank,
                    "Heavy defense and reliable protection.",
                    "Slow and deals modest damage.",
                    statModifiers: new JobStatModifiers(
                        maxHp: 20, defense: 5, magicDefense: 2))
            },
            {
                JobId.Reaver,
                new JobDefinition(
                    JobId.Reaver,
                    "job_reaver",
                    "Reaver",
                    JobRole.Tank | JobRole.PhysicalDamage,
                    "Absorbs punishment and converts risk into heavy damage.",
                    "Less reliable defense and performs best while endangered.",
                    statModifiers: new JobStatModifiers(
                        maxHp: 30, attack: 5, defense: 1))
            },
            {
                JobId.Mercenary,
                new JobDefinition(
                    JobId.Mercenary,
                    "job_mercenary",
                    "Mercenary",
                    JobRole.PhysicalDamage,
                    "Consistent direct melee damage.",
                    "Offers little healing or party utility.",
                    statModifiers: new JobStatModifiers(
                        attack: 5, criticalChance: 10))
            },
            {
                JobId.Rogue,
                new JobDefinition(
                    JobId.Rogue,
                    "job_rogue",
                    "Rogue",
                    JobRole.PhysicalDamage | JobRole.Support,
                    "Fast burst damage, evasion, and status pressure.",
                    "Fragile when focused by enemies.",
                    statModifiers: new JobStatModifiers(
                        speed: 5, evasion: 10, criticalChance: 5))
            },
            {
                JobId.Ranger,
                new JobDefinition(
                    JobId.Ranger,
                    "job_ranger",
                    "Ranger",
                    JobRole.PhysicalDamage | JobRole.Support,
                    "Ranged damage and tactical utility.",
                    "Weaker at close range or without setup.",
                    statModifiers: new JobStatModifiers(
                        attack: 4, speed: 2, accuracy: 5))
            },
            {
                JobId.Mage,
                new JobDefinition(
                    JobId.Mage,
                    "job_mage",
                    "Mage",
                    JobRole.MagicDamage,
                    "Powerful and flexible elemental magic.",
                    "Fragile and dependent on a limited resource.",
                    statModifiers: new JobStatModifiers(
                        maxMp: 20, magicPower: 5, magicDefense: 2))
            },
            {
                JobId.BloodMage,
                new JobDefinition(
                    JobId.BloodMage,
                    "job_blood_mage",
                    "Blood Mage",
                    JobRole.MagicDamage,
                    "Spends health to cast unusually powerful magic.",
                    "Self-damage puts pressure on party healing.",
                    statModifiers: new JobStatModifiers(
                        maxHp: 20, maxMp: 10, magicPower: 5))
            },
            {
                JobId.WhiteMage,
                new JobDefinition(
                    JobId.WhiteMage,
                    "job_white_mage",
                    "White Mage",
                    JobRole.Healer | JobRole.Support,
                    "Strong restoration and protective support.",
                    "Has minimal personal offense.",
                    statModifiers: new JobStatModifiers(
                        maxMp: 20, magicPower: 5, magicDefense: 3))
            },
            {
                JobId.Paladin,
                new JobDefinition(
                    JobId.Paladin,
                    "job_paladin",
                    "Paladin",
                    JobRole.Tank | JobRole.PhysicalDamage |
                        JobRole.Healer | JobRole.Support,
                    "Combines front-line damage, protection, limited healing, and leadership support.",
                    "Splits resources across roles and cannot match a dedicated specialist.",
                    statModifiers: new JobStatModifiers(
                        maxHp: 20,
                        attack: 3,
                        defense: 4,
                        maxMp: 10,
                        magicPower: 3,
                        magicDefense: 3))
            },
            {
                JobId.Bard,
                new JobDefinition(
                    JobId.Bard,
                    "job_bard",
                    "Bard",
                    JobRole.Support | JobRole.Healer,
                    "Buffs allies, hinders enemies, and provides minor healing.",
                    "Deals little damage and has low durability.",
                    statModifiers: new JobStatModifiers(
                        speed: 3,
                        maxMp: 15,
                        magicPower: 3,
                        magicDefense: 4))
            },
            {
                JobId.Tactician,
                new JobDefinition(
                    JobId.Tactician,
                    "job_tactician",
                    "Tactician",
                    JobRole.Support,
                    "Controls battlefield conditions and action timing.",
                    "Has minimal direct damage and durability.",
                    statModifiers: new JobStatModifiers(
                        speed: 5, maxMp: 15, accuracy: 5, evasion: 5))
            },
            {
                JobId.Summoner,
                new JobDefinition(
                    JobId.Summoner,
                    "job_summoner",
                    "Summoner",
                    JobRole.MagicDamage | JobRole.Support,
                    "Calls a temporary creature ally for flexible pressure.",
                    "The summon is temporary and vulnerable to disruption.",
                    statModifiers: new JobStatModifiers(
                        maxMp: 20, magicPower: 4, magicDefense: 4))
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

    public static JobDefinition Get(string stableId)
    {
        if (string.IsNullOrWhiteSpace(stableId))
        {
            return null;
        }

        string requestedId = stableId.Trim();
        foreach (JobDefinition definition in jobs.Values)
        {
            if (string.Equals(
                definition.StableId,
                requestedId,
                System.StringComparison.Ordinal))
            {
                return definition;
            }
        }

        return null;
    }
}
