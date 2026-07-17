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

public class JobDefinition
{
    public JobId Id { get; }
    public string StableId { get; }
    public string DisplayName { get; }
    public JobRole Roles { get; }
    public string Strength { get; }
    public string TradeOff { get; }
    public int DemoMaximumTier { get; }

    public JobDefinition(
        JobId id,
        string stableId,
        string displayName,
        JobRole roles,
        string strength,
        string tradeOff,
        int demoMaximumTier = 3)
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
    }
}
