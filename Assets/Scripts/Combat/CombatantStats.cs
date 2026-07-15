using System;

public class CombatantStats
{
    private int maxHp;
    private int currentHp;
    private int attack;
    private int defense;
    private int speed;
    private int maxMp;
    private int currentMp;
    private int magicPower;
    private int magicDefense;

    public int MaxHp
    {
        get { return maxHp; }
        set
        {
            maxHp = Math.Max(1, value);
            currentHp = Clamp(currentHp, 0, maxHp);
        }
    }

    public int CurrentHp
    {
        get { return currentHp; }
        set { currentHp = Clamp(value, 0, MaxHp); }
    }

    public int Attack
    {
        get { return attack; }
        set { attack = Math.Max(0, value); }
    }

    public int Defense
    {
        get { return defense; }
        set { defense = Math.Max(0, value); }
    }

    public int Speed
    {
        get { return speed; }
        set { speed = Math.Max(0, value); }
    }

    public int MaxMp
    {
        get { return maxMp; }
        set
        {
            maxMp = Math.Max(0, value);
            currentMp = Clamp(currentMp, 0, maxMp);
        }
    }

    public int CurrentMp
    {
        get { return currentMp; }
        set { currentMp = Clamp(value, 0, MaxMp); }
    }

    public int MagicPower
    {
        get { return magicPower; }
        set { magicPower = Math.Max(0, value); }
    }

    public int MagicDefense
    {
        get { return magicDefense; }
        set { magicDefense = Math.Max(0, value); }
    }

    public CombatantStats(
        int maximumHp,
        int attack,
        int defense,
        int speed,
        int maximumMp,
        int magicPower,
        int magicDefense)
    {
        maxHp = Math.Max(1, maximumHp);
        currentHp = maxHp;
        this.attack = Math.Max(0, attack);
        this.defense = Math.Max(0, defense);
        this.speed = Math.Max(0, speed);
        maxMp = Math.Max(0, maximumMp);
        currentMp = maxMp;
        this.magicPower = Math.Max(0, magicPower);
        this.magicDefense = Math.Max(0, magicDefense);
    }

    public int ApplyDamage(int amount)
    {
        int previousHp = CurrentHp;
        CurrentHp -= Math.Max(0, amount);
        return previousHp - CurrentHp;
    }

    public int RestoreHp(int amount)
    {
        int previousHp = CurrentHp;
        CurrentHp += Math.Max(0, amount);
        return CurrentHp - previousHp;
    }

    public bool TrySpendMp(int amount)
    {
        if (amount < 0 || amount > CurrentMp)
        {
            return false;
        }

        CurrentMp -= amount;
        return true;
    }

    public int RestoreMp(int amount)
    {
        int previousMp = CurrentMp;
        CurrentMp += Math.Max(0, amount);
        return CurrentMp - previousMp;
    }

    private static int Clamp(int value, int minimum, int maximum)
    {
        return Math.Min(Math.Max(value, minimum), maximum);
    }
}
