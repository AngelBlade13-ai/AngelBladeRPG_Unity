public class MonsterData
{
    public string Id;
    public string Name;
    public int GoldReward;
    public int XPReward;
    public CombatantStats Stats { get; }

    public int MaxHp => Stats.MaxHp;

    public int CurrentHp
    {
        get { return Stats.CurrentHp; }
        set { Stats.CurrentHp = value; }
    }

    public int Attack => Stats.Attack;
    public int Defense => Stats.Defense;
    public int Speed => Stats.Speed;
    public int MaxMp => Stats.MaxMp;
    public int CurrentMp => Stats.CurrentMp;
    public int MagicPower => Stats.MagicPower;
    public int MagicDefense => Stats.MagicDefense;

    public MonsterData(
        string name,
        int hp,
        int attack,
        int defense,
        int goldReward,
        int xpReward,
        int speed = 8,
        int maxMp = 0,
        int magicPower = 0,
        int magicDefense = 0)
        : this(
            name,
            name,
            hp,
            attack,
            defense,
            goldReward,
            xpReward,
            speed,
            maxMp,
            magicPower,
            magicDefense)
    {
    }

    public MonsterData(
        string id,
        string name,
        int hp,
        int attack,
        int defense,
        int goldReward,
        int xpReward,
        int speed,
        int maxMp,
        int magicPower,
        int magicDefense)
    {
        Id = id;
        Name = name;
        Stats = new CombatantStats(
            hp,
            attack,
            defense,
            speed,
            maxMp,
            magicPower,
            magicDefense);
        GoldReward = goldReward;
        XPReward = xpReward;
    }
}
