public class PlayerData
{
    public string Name;
    public int Level;
    public int Gold;
    public int XP;
    public int XPToNextLevel;
    public CombatantStats Stats { get; }

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

    public PlayerData(string name)
    {
        Name = name;
        Level = 1;
        Stats = new CombatantStats(
            100,
            12,
            3,
            10,
            20,
            8,
            3);
        Gold = 0;
        XP = 0;
        XPToNextLevel = 50;
    }

    public bool GainXP(int amount)
    {
        XP += amount;
        bool leveledUp = false;

        while (XP >= XPToNextLevel)
        {
            XP -= XPToNextLevel;
            Level += 1;
            MaxHp += 20;
            CurrentHp = MaxHp;
            Attack += 3;
            Defense += 1;
            XPToNextLevel += 25;

            leveledUp = true;
        }

        return leveledUp;
    }
}
