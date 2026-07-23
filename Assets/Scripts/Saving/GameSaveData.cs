using System;
using System.Collections.Generic;

public static class SaveSchema
{
    public const int CurrentVersion = 1;
}

[Serializable]
public sealed class GameSaveData
{
    public int schemaVersion = SaveSchema.CurrentVersion;
    public string saveId = "";
    public string gameVersion = "";
    public string savedAtUtc = "";
    public double playTimeSeconds;
    public PlayerSaveData player = new PlayerSaveData();
    public PartySaveData party = new PartySaveData();
    public InventorySaveData inventory = new InventorySaveData();
    public QuestSaveData quests = new QuestSaveData();
    public WorldSaveData world = new WorldSaveData();
    public CampSaveData camp = new CampSaveData();
    public LocationSaveData location = new LocationSaveData();
}

[Serializable]
public sealed class PlayerSaveData
{
    public string name = "";
    public string appearanceId = "";
    public int gold;
}

[Serializable]
public sealed class PartySaveData
{
    public List<CharacterSaveData> characters = new List<CharacterSaveData>();
    public List<string> activeCharacterIds = new List<string>();
}

[Serializable]
public sealed class CharacterSaveData
{
    public string characterId = "";
    public string displayName = "";
    public bool isAvailable = true;
    public string currentJobId = "";
    public int level = 1;
    public int xp;
    public int xpToNextLevel = 50;
    public int jobPoints;
    public CharacterStatsSaveData stats = new CharacterStatsSaveData();
    public List<string> learnedJobNodeIds = new List<string>();
    public List<JobAffinitySaveData> jobAffinities =
        new List<JobAffinitySaveData>();
    public List<EquipmentSaveData> equipment =
        new List<EquipmentSaveData>();
    public RosterHistorySaveData rosterHistory =
        new RosterHistorySaveData();
}

[Serializable]
public sealed class CharacterStatsSaveData
{
    public int maxHp;
    public int currentHp;
    public int attack;
    public int defense;
    public int speed;
    public int maxMp;
    public int currentMp;
    public int magicPower;
    public int magicDefense;
    public int accuracy;
    public int evasion;
    public int criticalChance;
}

[Serializable]
public sealed class JobAffinitySaveData
{
    public string jobId = "";
    public string affinityId = "";
}

[Serializable]
public sealed class EquipmentSaveData
{
    public string slotId = "";
    public string itemId = "";
}

[Serializable]
public sealed class RosterHistorySaveData
{
    public int battlesActive;
    public int battlesBenched;
    public int consecutiveBenchedBattles;
    public List<BondSaveData> bonds = new List<BondSaveData>();
}

[Serializable]
public sealed class BondSaveData
{
    public string otherCharacterId = "";
    public int points;
}

[Serializable]
public sealed class InventorySaveData
{
    public List<InventoryEntrySaveData> entries =
        new List<InventoryEntrySaveData>();
}

[Serializable]
public sealed class InventoryEntrySaveData
{
    public string itemId = "";
    public int quantity;
}

[Serializable]
public sealed class QuestSaveData
{
    public List<QuestEntrySaveData> entries = new List<QuestEntrySaveData>();
    public int completedQuestCount;
}

[Serializable]
public sealed class QuestEntrySaveData
{
    public string questId = "";
    public string stateId = "";
    public int objectiveIndex;
    public List<string> completedObjectiveIds = new List<string>();
}

[Serializable]
public sealed class WorldSaveData
{
    public List<string> completedEncounterIds = new List<string>();
    public List<string> claimedRewardIds = new List<string>();
    public List<WorldFlagSaveData> flags = new List<WorldFlagSaveData>();
}

[Serializable]
public sealed class WorldFlagSaveData
{
    public string flagId = "";
    public string value = "";
}

[Serializable]
public sealed class CampSaveData
{
    public string activeCampId = "";
    public bool tutorialRestUsed;
    public int completedRestCount;
    public List<string> consumedEventIds = new List<string>();
    public LocationSaveData returnLocation = new LocationSaveData();
}

[Serializable]
public sealed class LocationSaveData
{
    public string sceneName = "";
    public string spawnId = "";
}
