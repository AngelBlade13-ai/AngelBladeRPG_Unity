using System;
using System.Collections.Generic;

public enum JobNodeKind
{
    Ability,
    Passive,
    PermanentStat
}

public enum PermanentStatType
{
    MaxHp,
    Attack,
    Defense,
    Speed,
    MaxMp,
    MagicPower,
    MagicDefense,
    Accuracy,
    Evasion,
    CriticalChance
}

public sealed class JobStatBonus
{
    public PermanentStatType Stat { get; }
    public int Amount { get; }

    public JobStatBonus(PermanentStatType stat, int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        Stat = stat;
        Amount = amount;
    }
}

public sealed class JobNodeDefinition
{
    private readonly string[] prerequisiteIds;
    private readonly JobStatBonus[] statBonuses;

    public string StableId { get; }
    public JobId JobId { get; }
    public string DisplayName { get; }
    public JobNodeKind Kind { get; }
    public int Tier { get; }
    public int Cost { get; }
    public IReadOnlyList<string> PrerequisiteIds => prerequisiteIds;
    public IReadOnlyList<JobStatBonus> StatBonuses => statBonuses;

    public JobNodeDefinition(
        string stableId,
        JobId jobId,
        string displayName,
        JobNodeKind kind,
        int tier,
        int cost,
        string[] prerequisites = null,
        JobStatBonus[] permanentStatBonuses = null)
    {
        if (string.IsNullOrWhiteSpace(stableId))
        {
            throw new ArgumentException(
                "A stable node ID is required.",
                nameof(stableId));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException(
                "A node display name is required.",
                nameof(displayName));
        }

        if (tier < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(tier));
        }

        if (cost < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(cost));
        }

        StableId = stableId.Trim();
        JobId = jobId;
        DisplayName = displayName.Trim();
        Kind = kind;
        Tier = tier;
        Cost = cost;
        prerequisiteIds = prerequisites == null
            ? Array.Empty<string>()
            : (string[])prerequisites.Clone();
        statBonuses = permanentStatBonuses == null
            ? Array.Empty<JobStatBonus>()
            : (JobStatBonus[])permanentStatBonuses.Clone();
    }
}

public sealed class PermanentStatBonuses
{
    public int MaxHp { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public int MaxMp { get; private set; }
    public int MagicPower { get; private set; }
    public int MagicDefense { get; private set; }
    public int Accuracy { get; private set; }
    public int Evasion { get; private set; }
    public int CriticalChance { get; private set; }

    internal void Add(JobStatBonus bonus)
    {
        switch (bonus.Stat)
        {
            case PermanentStatType.MaxHp:
                MaxHp += bonus.Amount;
                break;
            case PermanentStatType.Attack:
                Attack += bonus.Amount;
                break;
            case PermanentStatType.Defense:
                Defense += bonus.Amount;
                break;
            case PermanentStatType.Speed:
                Speed += bonus.Amount;
                break;
            case PermanentStatType.MaxMp:
                MaxMp += bonus.Amount;
                break;
            case PermanentStatType.MagicPower:
                MagicPower += bonus.Amount;
                break;
            case PermanentStatType.MagicDefense:
                MagicDefense += bonus.Amount;
                break;
            case PermanentStatType.Accuracy:
                Accuracy += bonus.Amount;
                break;
            case PermanentStatType.Evasion:
                Evasion += bonus.Amount;
                break;
            case PermanentStatType.CriticalChance:
                CriticalChance += bonus.Amount;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bonus));
        }
    }
}
