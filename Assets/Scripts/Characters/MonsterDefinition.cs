using System;

public class MonsterDefinition
{
    public string Id { get; }
    public string DisplayName { get; }
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
    public int GoldReward { get; }
    public int XPReward { get; }

    public MonsterDefinition(
        string id,
        string displayName,
        int maxHp,
        int attack,
        int defense,
        int speed,
        int maxMp,
        int magicPower,
        int magicDefense,
        int goldReward,
        int xpReward,
        int accuracy = 95,
        int evasion = 5,
        int criticalChance = 10)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Monster ID is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException(
                "Monster display name is required.",
                nameof(displayName));
        }

        if (maxHp <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxHp),
                "Maximum HP must be greater than zero.");
        }

        ValidateNonNegative(attack, nameof(attack));
        ValidateNonNegative(defense, nameof(defense));
        ValidateNonNegative(speed, nameof(speed));
        ValidateNonNegative(maxMp, nameof(maxMp));
        ValidateNonNegative(magicPower, nameof(magicPower));
        ValidateNonNegative(magicDefense, nameof(magicDefense));
        ValidateNonNegative(goldReward, nameof(goldReward));
        ValidateNonNegative(xpReward, nameof(xpReward));
        ValidatePercentage(accuracy, nameof(accuracy), 100);
        ValidatePercentage(evasion, nameof(evasion), 95);
        ValidatePercentage(
            criticalChance,
            nameof(criticalChance),
            100);

        Id = id.Trim();
        DisplayName = displayName.Trim();
        MaxHp = maxHp;
        Attack = attack;
        Defense = defense;
        Speed = speed;
        MaxMp = maxMp;
        MagicPower = magicPower;
        MagicDefense = magicDefense;
        Accuracy = accuracy;
        Evasion = evasion;
        CriticalChance = criticalChance;
        GoldReward = goldReward;
        XPReward = xpReward;
    }

    public MonsterData CreateMonster()
    {
        return new MonsterData(
            Id,
            DisplayName,
            MaxHp,
            Attack,
            Defense,
            GoldReward,
            XPReward,
            Speed,
            MaxMp,
            MagicPower,
            MagicDefense,
            Accuracy,
            Evasion,
            CriticalChance);
    }

    private static void ValidateNonNegative(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Monster values cannot be negative.");
        }
    }

    private static void ValidatePercentage(
        int value,
        string parameterName,
        int maximum)
    {
        if (value < 0 || value > maximum)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                $"Monster percentage must be between 0 and {maximum}.");
        }
    }
}
