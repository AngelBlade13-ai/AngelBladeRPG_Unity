public class GameSession
{
    public PlayerData Player { get; private set; }
    public MonsterData Monster { get; private set; }
    public bool BattleIsOver { get; private set; }
    public BattleOutcome BattleOutcome { get; private set; }

    public bool HasPlayer
    {
        get { return Player != null; }
    }

    public bool HasActiveBattle
    {
        get { return Player != null && Monster != null && !BattleIsOver; }
    }

    public GameSession()
    {
        BattleIsOver = true;
        BattleOutcome = BattleOutcome.None;
    }

    public bool TryStartNewGame(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return false;
        }

        Player = new PlayerData(playerName.Trim());
        Monster = null;
        BattleIsOver = true;
        BattleOutcome = BattleOutcome.None;
        return true;
    }

    public bool StartBattle(MonsterData newMonster)
    {
        if (Player == null || Player.CurrentHp <= 0 || newMonster == null ||
            HasActiveBattle)
        {
            return false;
        }

        Monster = newMonster;
        BattleIsOver = false;
        BattleOutcome = BattleOutcome.InProgress;
        return true;
    }

    public bool TryCompleteVictory(out BattleRewardResult rewards)
    {
        rewards = null;

        if (!HasActiveBattle || Monster.CurrentHp > 0)
        {
            return false;
        }

        Player.Gold += Monster.GoldReward;
        bool playerLeveledUp = Player.GainXP(Monster.XPReward);
        BattleIsOver = true;
        BattleOutcome = BattleOutcome.Victory;

        rewards = new BattleRewardResult(
            Monster.GoldReward,
            Monster.XPReward,
            playerLeveledUp);

        return true;
    }

    public void CompleteDefeat()
    {
        if (HasActiveBattle && Player.CurrentHp <= 0)
        {
            BattleIsOver = true;
            BattleOutcome = BattleOutcome.Defeat;
        }
    }

    public bool TryEscapeBattle()
    {
        if (!HasActiveBattle)
        {
            return false;
        }

        BattleIsOver = true;
        BattleOutcome = BattleOutcome.Escaped;
        return true;
    }
}

public enum BattleOutcome
{
    None,
    InProgress,
    Victory,
    Defeat,
    Escaped
}

public class BattleRewardResult
{
    public int Gold { get; private set; }
    public int XP { get; private set; }
    public bool PlayerLeveledUp { get; private set; }

    public BattleRewardResult(int gold, int xp, bool playerLeveledUp)
    {
        Gold = gold;
        XP = xp;
        PlayerLeveledUp = playerLeveledUp;
    }
}
