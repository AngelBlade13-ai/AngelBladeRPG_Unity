using System.Collections.Generic;

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
            Player.Stats,
            characterProgression: Player.Progression);
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

        List<CharacterBattleReward> characterRewards =
            new List<CharacterBattleReward>();

        foreach (ICombatant combatant in PartyBattle.PartyMembers)
        {
            PlayableCharacterData character =
                combatant as PlayableCharacterData;
            if (character == null || !character.IsAvailable)
            {
                continue;
            }

            int levelsGained = character.GainXP(Monster.XPReward);
            characterRewards.Add(new CharacterBattleReward(
                character.Id,
                character.Name,
                Monster.XPReward,
                levelsGained,
                character.Level));
        }

        Player.Gold += Monster.GoldReward;
        int jobPointRecipients =
            Party.GrantJobPointsToAvailableCharacters(Monster.JobPointReward);
        Party.RecordBattleParticipation();
        BattleIsOver = true;
        BattleOutcome = BattleOutcome.Victory;

        rewards = new BattleRewardResult(
            Monster.GoldReward,
            Monster.XPReward,
            Monster.JobPointReward,
            jobPointRecipients,
            characterRewards);

        return true;
    }

    public bool CompleteDefeat()
    {
        if (!HasActiveBattle || PartyBattle == null ||
            !PartyBattle.IsPartyDefeated)
        {
            return false;
        }

        Party.RecordBattleParticipation();
        BattleIsOver = true;
        BattleOutcome = BattleOutcome.Defeat;
        return true;
    }

    public bool CompleteEscape()
    {
        if (!HasActiveBattle)
        {
            return false;
        }

        Party.RecordBattleParticipation();
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
    public int JobPoints { get; private set; }
    public int JobPointRecipients { get; private set; }
    public IReadOnlyList<CharacterBattleReward> CharacterRewards
    {
        get;
        private set;
    }
    public bool PlayerLeveledUp
    {
        get
        {
            foreach (CharacterBattleReward reward in CharacterRewards)
            {
                if (reward.CharacterId == PlayableCharacterData.ProtagonistId)
                {
                    return reward.LevelsGained > 0;
                }
            }

            return false;
        }
    }

    public BattleRewardResult(
        int gold,
        int xp,
        int jobPoints,
        int jobPointRecipients,
        IList<CharacterBattleReward> characterRewards)
    {
        Gold = gold;
        XP = xp;
        JobPoints = jobPoints;
        JobPointRecipients = jobPointRecipients;
        CharacterRewards = new List<CharacterBattleReward>(
            characterRewards).AsReadOnly();
    }
}

public class CharacterBattleReward
{
    public string CharacterId { get; private set; }
    public string CharacterName { get; private set; }
    public int XP { get; private set; }
    public int LevelsGained { get; private set; }
    public int NewLevel { get; private set; }

    public CharacterBattleReward(
        string characterId,
        string characterName,
        int xp,
        int levelsGained,
        int newLevel)
    {
        CharacterId = characterId;
        CharacterName = characterName;
        XP = xp;
        LevelsGained = levelsGained;
        NewLevel = newLevel;
    }
}
