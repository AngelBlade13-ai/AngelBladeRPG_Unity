using System;

public interface ICombatRandom
{
    bool RollPercent(int chancePercent);
}

public class SystemCombatRandom : ICombatRandom
{
    private readonly Random random;

    public SystemCombatRandom()
    {
        random = new Random();
    }

    public bool RollPercent(int chancePercent)
    {
        int clampedChance = Math.Min(Math.Max(chancePercent, 0), 100);
        return random.Next(0, 100) < clampedChance;
    }
}
