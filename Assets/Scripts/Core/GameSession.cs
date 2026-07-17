public class GameSession
{
    public PlayerData Player { get; private set; }
    public PartyRoster Party { get; private set; }
    public MonsterData Monster { get; private set; }
    public PartyBattleState PartyBattle { get; private set; }
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
        Party = new PartyRoster();
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
        Party = new PartyRoster();
        PlayableCharacterData protagonist = new PlayableCharacterData(
            PlayableCharacterData.ProtagonistId,
            Player.Name,
            JobId.Mercenary,
            Player.Stats);
        Party.TryAddCharacter(protagonist);
        Party.TrySetActiveParty(new[] { protagonist.Id });
        Monster = null;
        PartyBattle = null;
        BattleIsOver = true;
        BattleOutcome = BattleOutcome.None;
        return true;
    }

    public bool StartBattle(MonsterData newMonster)
    {
        if (Player == null || Player.CurrentHp <= 0 || newMonster == null ||
            HasActiveBattle || Party.GetActiveCharacters().Count < 1)
        {
            return false;
        }

        PartyBattle = PartyBattleState.FromRoster(
            Party,
            new ICombatant[] { newMonster });
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
        if (HasActiveBattle && PartyBattle != null &&
            PartyBattle.IsPartyDefeated)
        {
            BattleIsOver = true;
            BattleOutcome = BattleOutcome.Defeat;
        }
    }

    public bool CompleteEscape()
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
