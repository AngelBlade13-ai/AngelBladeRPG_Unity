public class GameSession
{
    public PlayerData Player { get; private set; }
    public MonsterData Monster { get; private set; }
    public bool BattleIsOver { get; private set; }

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
    }

    public void StartNewGame(string playerName)
    {
        Player = new PlayerData(playerName);
        Monster = null;
        BattleIsOver = true;
    }

    public bool StartBattle(MonsterData newMonster)
    {
        if (Player == null || Player.CurrentHp <= 0 || newMonster == null)
        {
            return false;
        }

        Monster = newMonster;
        BattleIsOver = false;
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

        rewards = new BattleRewardResult(
            Monster.GoldReward,
            Monster.XPReward,
            playerLeveledUp);

        return true;
    }

    public void CompleteDefeat()
    {
        if (Player != null && Player.CurrentHp <= 0)
        {
            BattleIsOver = true;
        }
    }
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
