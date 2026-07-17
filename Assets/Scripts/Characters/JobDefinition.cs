using System;

[Flags]
public enum JobRole
{
    None = 0,
    Tank = 1,
    PhysicalDamage = 2,
    MagicDamage = 4,
    Healer = 8,
    Support = 16
}

public enum JobId
{
    Knight,
    Reaver,
    Mercenary,
    Rogue,
    Ranger,
    Mage,
    BloodMage,
    WhiteMage,
    Paladin,
    Bard,
    Tactician,
    Summoner
}

public sealed class JobStatModifiers
{
    public static JobStatModifiers None { get; } = new JobStatModifiers();

    public int MaxHp { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int Speed { get; }
    public int MaxMp { get; }
    public int MagicPower { get; }
    public int MagicDefense { get; }
    public int Accuracy { get; }
    public int Evasion { get; }
    public int CriticalChance { get; }

    public bool HasAnyBonus =>
        MaxHp > 0 || Attack > 0 || Defense > 0 || Speed > 0 ||
        MaxMp > 0 || MagicPower > 0 || MagicDefense > 0 ||
        Accuracy > 0 || Evasion > 0 || CriticalChance > 0;

    public JobStatModifiers(
        int maxHp = 0,
        int attack = 0,
        int defense = 0,
        int speed = 0,
        int maxMp = 0,
        int magicPower = 0,
        int magicDefense = 0,
        int accuracy = 0,
        int evasion = 0,
        int criticalChance = 0)
    {
        MaxHp = RequireNonNegative(maxHp, nameof(maxHp));
        Attack = RequireNonNegative(attack, nameof(attack));
        Defense = RequireNonNegative(defense, nameof(defense));
        Speed = RequireNonNegative(speed, nameof(speed));
        MaxMp = RequireNonNegative(maxMp, nameof(maxMp));
        MagicPower = RequireNonNegative(magicPower, nameof(magicPower));
        MagicDefense = RequireNonNegative(magicDefense, nameof(magicDefense));
        Accuracy = RequireNonNegative(accuracy, nameof(accuracy));
        Evasion = RequireNonNegative(evasion, nameof(evasion));
        CriticalChance = RequireNonNegative(
            criticalChance,
            nameof(criticalChance));
    }

    private static int RequireNonNegative(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }

        return value;
    }
}

public class JobDefinition
{
    public JobId Id { get; }
    public string StableId { get; }
    public string DisplayName { get; }
    public JobRole Roles { get; }
    public string Strength { get; }
    public string TradeOff { get; }
    public int DemoMaximumTier { get; }
    public JobStatModifiers StatModifiers { get; }

    public JobDefinition(
        JobId id,
        string stableId,
        string displayName,
        JobRole roles,
        string strength,
        string tradeOff,
        int demoMaximumTier = 3,
        JobStatModifiers statModifiers = null)
    {
        if (string.IsNullOrWhiteSpace(stableId))
        {
            throw new ArgumentException(
                "A stable job ID is required.",
                nameof(stableId));
        }

        if (demoMaximumTier < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(demoMaximumTier));
        }

        Id = id;
        StableId = stableId.Trim();
        DisplayName = displayName;
        Roles = roles;
        Strength = strength;
        TradeOff = tradeOff;
        DemoMaximumTier = demoMaximumTier;
        StatModifiers = statModifiers ?? JobStatModifiers.None;
    }
}
