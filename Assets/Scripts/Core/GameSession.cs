using System.Collections.Generic;

public class GameSession
{
    private readonly HashSet<string> completedEncounterIds =
        new HashSet<string>(System.StringComparer.Ordinal);
    private readonly List<MonsterData> battleEnemies =
        new List<MonsterData>();

    public PlayerData Player { get; private set; }
    public PartyRoster Party { get; private set; }
    public MonsterData Monster { get; private set; }
    public PartyBattleState PartyBattle { get; private set; }
    public BattleEncounterDefinition Encounter { get; private set; }
    public BattleLayoutDefinition BattleLayout { get; private set; }
    public bool EscapeAllowed { get; private set; }
    public BattleTimingMode BattleTimingMode { get; private set; }
    public CaravanTutorialBattle CaravanTutorial { get; private set; }
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
        BattleTimingMode = BattleTimingMode.Wait;
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
        Encounter = null;
        BattleLayout = null;
        EscapeAllowed = true;
        CaravanTutorial = null;
        completedEncounterIds.Clear();
        battleEnemies.Clear();
        BattleIsOver = true;
        BattleOutcome = BattleOutcome.None;
        return true;
    }

    public bool StartBattle(MonsterData newMonster)
    {
        if (newMonster == null)
        {
            return false;
        }

        return StartBattle(
            new[] { newMonster },
            null,
            BattleLayoutCatalog.Get(BattleLayoutCatalog.StandardId),
            true);
    }

    public void SetBattleTimingMode(BattleTimingMode timingMode)
    {
        BattleTimingMode = timingMode;
    }

    public bool StartEncounter(BattleEncounterDefinition encounter)
    {
        if (encounter == null ||
            (!encounter.IsRepeatable && IsEncounterCompleted(encounter.Id)))
        {
            return false;
        }

        return StartBattle(
            encounter.CreateEnemies(),
            encounter,
            BattleLayoutCatalog.Get(encounter.LayoutId),
            encounter.EscapeAllowed);
    }

    private bool StartBattle(
        IReadOnlyList<MonsterData> enemies,
        BattleEncounterDefinition encounter,
        BattleLayoutDefinition layout,
        bool escapeAllowed)
    {
        if (Player == null || Player.CurrentHp <= 0 || enemies == null ||
            enemies.Count < 1 || layout == null || HasActiveBattle ||
            Party.GetActiveCharacters().Count < 1 ||
            enemies.Count > layout.EnemySlots.Count)
        {
            return false;
        }

        foreach (MonsterData enemy in enemies)
        {
            if (enemy == null || enemy.CurrentHp <= 0)
            {
                return false;
            }
        }

        PartyBattle = PartyBattleState.FromRoster(
            Party,
            enemies);
        battleEnemies.Clear();
        battleEnemies.AddRange(enemies);
        Monster = enemies[0];
        Encounter = encounter;
        BattleLayout = layout;
        EscapeAllowed = escapeAllowed;
        CaravanTutorial = encounter != null &&
            encounter.Id == BattleEncounterCatalog.CaravanTutorialId
            ? new CaravanTutorialBattle(this)
            : null;
        BattleIsOver = false;
        BattleOutcome = BattleOutcome.InProgress;
        return true;
    }

    public bool TryCompleteVictory(out BattleRewardResult rewards)
    {
        rewards = null;

        if (!HasActiveBattle || !PartyBattle.AreEnemiesDefeated ||
            (CaravanTutorial != null &&
                CaravanTutorial.Stage != CaravanTutorialStage.Completed))
        {
            return false;
        }

        int goldReward = 0;
        int xpReward = 0;
        int jobPointReward = 0;
        foreach (MonsterData enemy in battleEnemies)
        {
            goldReward += enemy.GoldReward;
            xpReward += enemy.XPReward;
            jobPointReward += enemy.JobPointReward;
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

            int levelsGained = character.GainXP(xpReward);
            characterRewards.Add(new CharacterBattleReward(
                character.Id,
                character.Name,
                xpReward,
                levelsGained,
                character.Level));
        }

        Player.Gold += goldReward;
        int jobPointRecipients =
            Party.GrantJobPointsToAvailableCharacters(jobPointReward);
        Party.RecordBattleParticipation();
        BattleIsOver = true;
        BattleOutcome = BattleOutcome.Victory;
        if (Encounter != null && !Encounter.IsRepeatable)
        {
            completedEncounterIds.Add(Encounter.Id);
        }

        rewards = new BattleRewardResult(
            goldReward,
            xpReward,
            jobPointReward,
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
        if (!HasActiveBattle || !EscapeAllowed)
        {
            return false;
        }

        Party.RecordBattleParticipation();
        BattleIsOver = true;
        BattleOutcome = BattleOutcome.Escaped;
        return true;
    }

    public bool IsEncounterCompleted(string encounterId)
    {
        return !string.IsNullOrWhiteSpace(encounterId) &&
            completedEncounterIds.Contains(encounterId.Trim());
    }

    public PartyBattleRoundResolver CreatePartyRoundResolver(
        ITurnOrderRandom tieBreaker = null,
        ICombatRandom combatRandom = null)
    {
        return new PartyBattleRoundResolver(
            tieBreaker,
            combatRandom,
            CaravanTutorial,
            CaravanTutorial);
    }

    public IReadOnlyList<string> AdvanceTutorialAfterRound(
        PartyBattleRoundResult round)
    {
        return CaravanTutorial == null
            ? System.Array.Empty<string>()
            : CaravanTutorial.AdvanceAfterRound(round);
    }

    public ActionGaugeBattle CreateActionGaugeBattle()
    {
        return new ActionGaugeBattle(PartyBattle, BattleTimingMode);
    }

    public PartyBattleActionResolver CreatePartyActionResolver(
        ICombatRandom combatRandom = null)
    {
        return new PartyBattleActionResolver(
            combatRandom,
            CaravanTutorial,
            CaravanTutorial,
            CaravanTutorial);
    }

    public IReadOnlyList<string> AdvanceTutorialAfterAction(
        CombatActionResult action)
    {
        return CaravanTutorial == null
            ? System.Array.Empty<string>()
            : CaravanTutorial.AdvanceAfterAction(action);
    }

    internal PlayableCharacterData EnsureTutorialCompanion(
        string characterId)
    {
        PlayableCharacterData character = Party.GetCharacter(characterId);
        if (character == null)
        {
            PartyMemberDefinition definition =
                PartyMemberCatalog.Get(characterId);
            if (definition == null)
            {
                return null;
            }

            character = definition.CreateCharacter();
            if (!Party.TryAddCharacter(character))
            {
                return null;
            }
        }

        List<string> activeIds = new List<string>(Party.ActiveCharacterIds);
        if (!activeIds.Contains(character.Id))
        {
            activeIds.Add(character.Id);
            if (!Party.TrySetActiveParty(activeIds))
            {
                return null;
            }
        }

        return character;
    }

    internal bool ReplaceTutorialEnemies(
        IReadOnlyList<MonsterData> enemies)
    {
        if (PartyBattle == null || enemies == null || enemies.Count < 1 ||
            !PartyBattle.TryReplaceEnemies(enemies))
        {
            return false;
        }

        battleEnemies.AddRange(enemies);
        Monster = enemies[0];
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
