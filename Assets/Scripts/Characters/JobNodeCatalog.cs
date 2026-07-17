using System;
using System.Collections.Generic;

public static class JobNodeCatalog
{
    private static readonly Dictionary<string, JobNodeDefinition> nodes =
        BuildCatalog();

    public static IReadOnlyCollection<JobNodeDefinition> All => nodes.Values;

    public static bool Contains(string stableId)
    {
        return !string.IsNullOrWhiteSpace(stableId) &&
            nodes.ContainsKey(stableId.Trim());
    }

    public static JobNodeDefinition Get(string stableId)
    {
        if (string.IsNullOrWhiteSpace(stableId))
        {
            return null;
        }

        return nodes.TryGetValue(
            stableId.Trim(),
            out JobNodeDefinition definition)
            ? definition
            : null;
    }

    public static IReadOnlyList<JobNodeDefinition> GetForJob(JobId jobId)
    {
        List<JobNodeDefinition> results = new List<JobNodeDefinition>();

        foreach (JobNodeDefinition node in nodes.Values)
        {
            if (node.JobId == jobId)
            {
                results.Add(node);
            }
        }

        results.Sort((left, right) =>
        {
            int tierComparison = left.Tier.CompareTo(right.Tier);
            return tierComparison != 0
                ? tierComparison
                : string.CompareOrdinal(left.StableId, right.StableId);
        });
        return results;
    }

    public static bool Validate(
        IEnumerable<JobNodeDefinition> definitions,
        out string error)
    {
        if (definitions == null)
        {
            error = "Job node definitions cannot be null.";
            return false;
        }

        Dictionary<string, JobNodeDefinition> knownNodes =
            new Dictionary<string, JobNodeDefinition>(StringComparer.Ordinal);

        foreach (JobNodeDefinition node in definitions)
        {
            if (node == null)
            {
                error = "Job node definitions cannot contain null entries.";
                return false;
            }

            if (knownNodes.ContainsKey(node.StableId))
            {
                error = $"Duplicate job node ID: {node.StableId}.";
                return false;
            }

            JobDefinition job = JobCatalog.Get(node.JobId);
            if (job == null)
            {
                error = $"Unknown job for node {node.StableId}.";
                return false;
            }

            if (node.Tier > job.DemoMaximumTier)
            {
                error = $"Node {node.StableId} exceeds the demo tier cap.";
                return false;
            }

            bool isPermanentStat = node.Kind == JobNodeKind.PermanentStat;
            if (isPermanentStat != (node.StatBonuses.Count > 0))
            {
                error = $"Node {node.StableId} has mismatched stat data.";
                return false;
            }

            knownNodes.Add(node.StableId, node);
        }

        foreach (JobNodeDefinition node in knownNodes.Values)
        {
            HashSet<string> uniquePrerequisites =
                new HashSet<string>(StringComparer.Ordinal);

            foreach (string prerequisiteId in node.PrerequisiteIds)
            {
                if (string.IsNullOrWhiteSpace(prerequisiteId) ||
                    !uniquePrerequisites.Add(prerequisiteId))
                {
                    error = $"Node {node.StableId} has an invalid prerequisite.";
                    return false;
                }

                if (!knownNodes.TryGetValue(
                    prerequisiteId,
                    out JobNodeDefinition prerequisite))
                {
                    error = $"Node {node.StableId} has a missing prerequisite.";
                    return false;
                }

                if (prerequisite.JobId != node.JobId)
                {
                    error = $"Node {node.StableId} links to another job.";
                    return false;
                }

                if (prerequisite.Tier >= node.Tier)
                {
                    error = $"Node {node.StableId} must depend on an earlier tier.";
                    return false;
                }
            }
        }

        error = string.Empty;
        return true;
    }

    private static Dictionary<string, JobNodeDefinition> BuildCatalog()
    {
        List<JobNodeDefinition> definitions = new List<JobNodeDefinition>();

        AddJob(definitions, JobId.Knight, "job_knight",
            Feature("shield_bash", "Shield Bash"),
            Feature("interpose", "Interpose"),
            Feature("hold_the_line", "Hold the Line"),
            Stat("fortitude", "Fortitude", PermanentStatType.MaxHp, 10),
            Stat("guard_training", "Guard Training", PermanentStatType.Defense, 1),
            Mastery("foundation", "Knight's Foundation",
                Bonus(PermanentStatType.MaxHp, 10),
                Bonus(PermanentStatType.Defense, 1)));

        AddJob(definitions, JobId.Reaver, "job_reaver",
            Feature("reckless_strike", "Reckless Strike"),
            Feature("feed_the_pain", "Feed the Pain", JobNodeKind.Passive),
            Feature("ruinous_blow", "Ruinous Blow"),
            Stat("scarred_endurance", "Scarred Endurance", PermanentStatType.MaxHp, 10),
            Stat("ferocity", "Ferocity", PermanentStatType.Attack, 1),
            Mastery("painforged", "Painforged",
                Bonus(PermanentStatType.MaxHp, 10),
                Bonus(PermanentStatType.Attack, 1)));

        AddJob(definitions, JobId.Mercenary, "job_mercenary",
            Feature("armor_break", "Armor Break"),
            Feature("second_wind", "Second Wind"),
            Feature("finishing_blow", "Finishing Blow"),
            Stat("arms_training", "Arms Training", PermanentStatType.Attack, 1),
            Stat("killer_instinct", "Killer Instinct", PermanentStatType.CriticalChance, 2),
            Mastery("seasoned_fighter", "Seasoned Fighter",
                Bonus(PermanentStatType.Attack, 1),
                Bonus(PermanentStatType.CriticalChance, 2)));

        AddJob(definitions, JobId.Rogue, "job_rogue",
            Feature("venom_blade", "Venom Blade"),
            Feature("smoke_veil", "Smoke Veil"),
            Feature("exploit_opening", "Exploit Opening"),
            Stat("agility", "Agility", PermanentStatType.Speed, 1),
            Stat("footwork", "Footwork", PermanentStatType.Evasion, 2),
            Mastery("untouchable", "Untouchable",
                Bonus(PermanentStatType.Speed, 1),
                Bonus(PermanentStatType.Evasion, 1),
                Bonus(PermanentStatType.CriticalChance, 1)));

        AddJob(definitions, JobId.Ranger, "job_ranger",
            Feature("piercing_shot", "Piercing Shot"),
            Feature("pinning_shot", "Pinning Shot"),
            Feature("predators_shot", "Predator's Shot"),
            Stat("draw_strength", "Draw Strength", PermanentStatType.Attack, 1),
            Stat("true_aim", "True Aim", PermanentStatType.Accuracy, 1),
            Mastery("master_hunter", "Master Hunter",
                Bonus(PermanentStatType.Attack, 1),
                Bonus(PermanentStatType.CriticalChance, 2)));

        AddJob(definitions, JobId.Mage, "job_mage",
            Feature("frostbind", "Frostbind"),
            Feature("arc_spark", "Arc Spark"),
            Feature("arcane_burst", "Arcane Burst"),
            Stat("arcane_reserves", "Arcane Reserves", PermanentStatType.MaxMp, 5),
            Stat("spellcraft", "Spellcraft", PermanentStatType.MagicPower, 1),
            Mastery("arcane_scholar", "Arcane Scholar",
                Bonus(PermanentStatType.MaxMp, 5),
                Bonus(PermanentStatType.MagicPower, 1)));

        AddJob(definitions, JobId.BloodMage, "job_blood_mage",
            Feature("siphon", "Siphon"),
            Feature("hemorrhage", "Hemorrhage"),
            Feature("red_deluge", "Red Deluge"),
            Stat("vital_reservoir", "Vital Reservoir", PermanentStatType.MaxHp, 10),
            Stat("bloodcraft", "Bloodcraft", PermanentStatType.MagicPower, 1),
            Mastery("crimson_vessel", "Crimson Vessel",
                Bonus(PermanentStatType.MaxHp, 10),
                Bonus(PermanentStatType.MagicPower, 1)));

        AddJob(definitions, JobId.WhiteMage, "job_white_mage",
            Feature("purify", "Purify"),
            Feature("ward", "Ward"),
            Feature("healing_light", "Healing Light"),
            Stat("sacred_reserves", "Sacred Reserves", PermanentStatType.MaxMp, 5),
            Stat("healing_study", "Healing Study", PermanentStatType.MagicPower, 1),
            Mastery("blessed_spirit", "Blessed Spirit",
                Bonus(PermanentStatType.MaxMp, 5),
                Bonus(PermanentStatType.MagicPower, 1)));

        AddJob(definitions, JobId.Paladin, "job_paladin",
            Feature("smite", "Smite"),
            Feature("guardian_oath", "Guardian Oath"),
            Feature("rallying_light", "Rallying Light"),
            Stat("sacred_vigor", "Sacred Vigor", PermanentStatType.MaxHp, 10),
            Stat("conviction", "Conviction", PermanentStatType.MagicPower, 1),
            Mastery("oathbound", "Oathbound",
                Bonus(PermanentStatType.Defense, 1),
                Bonus(PermanentStatType.MagicDefense, 1)));

        AddJob(definitions, JobId.Bard, "job_bard",
            Feature("soothing_melody", "Soothing Melody"),
            Feature("cutting_verse", "Cutting Verse"),
            Feature("chorus_of_resolve", "Chorus of Resolve"),
            Stat("breath_control", "Breath Control", PermanentStatType.MaxMp, 5),
            Stat("tempo", "Tempo", PermanentStatType.Speed, 1),
            Mastery("virtuoso", "Virtuoso",
                Bonus(PermanentStatType.MaxMp, 5),
                Bonus(PermanentStatType.MagicDefense, 1)));

        AddJob(definitions, JobId.Tactician, "job_tactician",
            Feature("hasten", "Hasten"),
            Feature("disrupt", "Disrupt"),
            Feature("coordinated_assault", "Coordinated Assault"),
            Stat("preparation", "Preparation", PermanentStatType.MaxMp, 5),
            Stat("initiative", "Initiative", PermanentStatType.Speed, 1),
            Mastery("mastermind", "Mastermind",
                Bonus(PermanentStatType.MaxMp, 5),
                Bonus(PermanentStatType.Speed, 1)));

        AddJob(definitions, JobId.Summoner, "job_summoner",
            Feature("wisps_aid", "Wisp's Aid"),
            Feature("reinforce_bond", "Reinforce Bond"),
            Feature("spirit_flare", "Spirit Flare"),
            Stat("bond_reserves", "Bond Reserves", PermanentStatType.MaxMp, 5),
            Stat("invocation", "Invocation", PermanentStatType.MagicPower, 1),
            Mastery("spirit_keeper", "Spirit Keeper",
                Bonus(PermanentStatType.MaxMp, 5),
                Bonus(PermanentStatType.MagicDefense, 1)));

        if (!Validate(definitions, out string error))
        {
            throw new InvalidOperationException(error);
        }

        Dictionary<string, JobNodeDefinition> catalog =
            new Dictionary<string, JobNodeDefinition>(StringComparer.Ordinal);
        foreach (JobNodeDefinition definition in definitions)
        {
            catalog.Add(definition.StableId, definition);
        }

        return catalog;
    }

    private static void AddJob(
        ICollection<JobNodeDefinition> definitions,
        JobId jobId,
        string prefix,
        FeatureSeed firstFeature,
        FeatureSeed secondFeature,
        FeatureSeed endpoint,
        StatSeed firstStat,
        StatSeed secondStat,
        MasterySeed mastery)
    {
        string firstFeatureId = $"{prefix}_{firstFeature.Suffix}";
        string secondFeatureId = $"{prefix}_{secondFeature.Suffix}";
        string firstStatOneId = $"{prefix}_{firstStat.Suffix}_1";
        string firstStatTwoId = $"{prefix}_{firstStat.Suffix}_2";
        string secondStatOneId = $"{prefix}_{secondStat.Suffix}_1";
        string secondStatTwoId = $"{prefix}_{secondStat.Suffix}_2";

        definitions.Add(new JobNodeDefinition(
            firstFeatureId, jobId, firstFeature.Name,
            firstFeature.Kind, 1, 1));
        definitions.Add(new JobNodeDefinition(
            secondFeatureId, jobId, secondFeature.Name,
            secondFeature.Kind, 1, 1));
        definitions.Add(new JobNodeDefinition(
            $"{prefix}_{endpoint.Suffix}", jobId, endpoint.Name,
            endpoint.Kind, 2, 2,
            new[] { firstFeatureId, secondFeatureId }));

        AddStatTrack(definitions, jobId, prefix, firstStat);
        AddStatTrack(definitions, jobId, prefix, secondStat);

        definitions.Add(new JobNodeDefinition(
            $"{prefix}_{mastery.Suffix}", jobId, mastery.Name,
            JobNodeKind.PermanentStat, 3, 2,
            new[] { firstStatTwoId, secondStatTwoId },
            mastery.Bonuses));
    }

    private static void AddStatTrack(
        ICollection<JobNodeDefinition> definitions,
        JobId jobId,
        string prefix,
        StatSeed stat)
    {
        string firstId = $"{prefix}_{stat.Suffix}_1";
        definitions.Add(new JobNodeDefinition(
            firstId, jobId, $"{stat.Name} I",
            JobNodeKind.PermanentStat, 1, 1,
            permanentStatBonuses: new[] { Bonus(stat.Stat, stat.Amount) }));
        definitions.Add(new JobNodeDefinition(
            $"{prefix}_{stat.Suffix}_2", jobId, $"{stat.Name} II",
            JobNodeKind.PermanentStat, 2, 1,
            new[] { firstId },
            new[] { Bonus(stat.Stat, stat.Amount) }));
    }

    private static FeatureSeed Feature(
        string suffix,
        string name,
        JobNodeKind kind = JobNodeKind.Ability)
    {
        return new FeatureSeed(suffix, name, kind);
    }

    private static StatSeed Stat(
        string suffix,
        string name,
        PermanentStatType stat,
        int amount)
    {
        return new StatSeed(suffix, name, stat, amount);
    }

    private static MasterySeed Mastery(
        string suffix,
        string name,
        params JobStatBonus[] bonuses)
    {
        return new MasterySeed(suffix, name, bonuses);
    }

    private static JobStatBonus Bonus(PermanentStatType stat, int amount)
    {
        return new JobStatBonus(stat, amount);
    }

    private sealed class FeatureSeed
    {
        public string Suffix { get; }
        public string Name { get; }
        public JobNodeKind Kind { get; }

        public FeatureSeed(string suffix, string name, JobNodeKind kind)
        {
            Suffix = suffix;
            Name = name;
            Kind = kind;
        }
    }

    private sealed class StatSeed
    {
        public string Suffix { get; }
        public string Name { get; }
        public PermanentStatType Stat { get; }
        public int Amount { get; }

        public StatSeed(
            string suffix,
            string name,
            PermanentStatType stat,
            int amount)
        {
            Suffix = suffix;
            Name = name;
            Stat = stat;
            Amount = amount;
        }
    }

    private sealed class MasterySeed
    {
        public string Suffix { get; }
        public string Name { get; }
        public JobStatBonus[] Bonuses { get; }

        public MasterySeed(
            string suffix,
            string name,
            JobStatBonus[] bonuses)
        {
            Suffix = suffix;
            Name = name;
            Bonuses = bonuses;
        }
    }
}
