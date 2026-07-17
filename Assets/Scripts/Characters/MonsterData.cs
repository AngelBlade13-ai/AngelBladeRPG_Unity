public class MonsterData : ICombatant
{
    public string Id;
    public string Name;
    public int GoldReward;
    public int XPReward;
    public int JobPointReward;
    public string DefinitionId { get; }
    public CombatantStats Stats { get; }
    public string CombatantId => Id;
    public string DisplayName => Name;

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
    public int Accuracy => Stats.Accuracy;
    public int Evasion => Stats.Evasion;
    public int CriticalChance => Stats.CriticalChance;

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
        int magicDefense = 0,
        int accuracy = 95,
        int evasion = 5,
        int criticalChance = 10,
        int jobPointReward = 1,
        string definitionId = null)
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
            magicDefense,
            accuracy,
            evasion,
            criticalChance,
            jobPointReward,
            definitionId)
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
        int magicDefense,
        int accuracy = 95,
        int evasion = 5,
        int criticalChance = 10,
        int jobPointReward = 1,
        string definitionId = null)
    {
        Id = id;
        DefinitionId = string.IsNullOrWhiteSpace(definitionId)
            ? id
            : definitionId.Trim();
        Name = name;
        Stats = new CombatantStats(
            hp,
            attack,
            defense,
            speed,
            maxMp,
            magicPower,
            magicDefense,
            accuracy,
            evasion,
            criticalChance);
        GoldReward = goldReward;
        XPReward = xpReward;
        JobPointReward = jobPointReward;
    }
}
