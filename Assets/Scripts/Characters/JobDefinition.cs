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
    public string DisplayName { get; }
    public JobRole Roles { get; }
    public string Strength { get; }
    public string TradeOff { get; }

    public JobDefinition(
        JobId id,
        string displayName,
        JobRole roles,
        string strength,
        string tradeOff)
    {
        Id = id;
        DisplayName = displayName;
        Roles = roles;
        Strength = strength;
        TradeOff = tradeOff;
    }
}
