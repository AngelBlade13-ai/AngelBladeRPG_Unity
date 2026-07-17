using System;

public class CharacterProgression
{
    public int Level { get; private set; } = 1;
    public int XP { get; private set; }
    public int XPToNextLevel { get; private set; } = 50;

    public int GainXP(int amount, CombatantStats stats)
    {
        if (amount <= 0)
        {
            return 0;
        }

        if (stats == null)
        {
            throw new ArgumentNullException(nameof(stats));
        }

        XP += amount;
        int levelsGained = 0;

        while (XP >= XPToNextLevel)
        {
            XP -= XPToNextLevel;
            Level += 1;
            XPToNextLevel += 25;
            stats.MaxHp += 20;
            stats.CurrentHp = stats.MaxHp;
            stats.Attack += 3;
            stats.Defense += 1;
            levelsGained += 1;
        }

        return levelsGained;
    }

    public void SetXP(int amount)
    {
        if (amount < 0 || amount >= XPToNextLevel)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        XP = amount;
    }
}

public class PlayerData : ICombatant
{
    private readonly CharacterProgression progression;

    public string Name;
    public int Gold;
    public int Level => progression.Level;
    public int XP
    {
        get { return progression.XP; }
        set { progression.SetXP(value); }
    }
    public int XPToNextLevel => progression.XPToNextLevel;
    public CharacterProgression Progression => progression;
    public CombatantStats Stats { get; }
    public string CombatantId => "player";
    public string DisplayName => Name;

    public int MaxHp
    {
        get { return Stats.MaxHp; }
        set { Stats.MaxHp = value; }
    }

    public int CurrentHp
    {
        get { return Stats.CurrentHp; }
        set { Stats.CurrentHp = value; }
    }

    public int Attack
    {
        get { return Stats.Attack; }
        set { Stats.Attack = value; }
    }

    public int Defense
    {
        get { return Stats.Defense; }
        set { Stats.Defense = value; }
    }

    public int Speed
    {
        get { return Stats.Speed; }
        set { Stats.Speed = value; }
    }

    public int MaxMp
    {
        get { return Stats.MaxMp; }
        set { Stats.MaxMp = value; }
    }

    public int CurrentMp
    {
        get { return Stats.CurrentMp; }
        set { Stats.CurrentMp = value; }
    }

    public int MagicPower
    {
        get { return Stats.MagicPower; }
        set { Stats.MagicPower = value; }
    }

    public int MagicDefense
    {
        get { return Stats.MagicDefense; }
        set { Stats.MagicDefense = value; }
    }

    public int Accuracy
    {
        get { return Stats.Accuracy; }
        set { Stats.Accuracy = value; }
    }

    public int Evasion
    {
        get { return Stats.Evasion; }
        set { Stats.Evasion = value; }
    }

    public int CriticalChance
    {
        get { return Stats.CriticalChance; }
        set { Stats.CriticalChance = value; }
    }

    public PlayerData(string name)
    {
        Name = name;
        progression = new CharacterProgression();
        Stats = new CombatantStats(
            100,
            12,
            3,
            10,
            20,
            8,
            3);
        Gold = 0;
    }

    public bool GainXP(int amount)
    {
        return progression.GainXP(amount, Stats) > 0;
    }
}
