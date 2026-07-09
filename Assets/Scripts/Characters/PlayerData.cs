public class PlayerData
{
    public string Name;
    public int Level;
    public int MaxHp;
    public int CurrentHp;
    public int Attack;
    public int Defense;
    public int Gold;
    public int XP;
    public int XPToNextLevel;

    public PlayerData(string name)
    {
        Name = name;
        Level = 1;
        MaxHp = 100;
        CurrentHp = 100;
        Attack = 12;
        Defense = 3;
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
