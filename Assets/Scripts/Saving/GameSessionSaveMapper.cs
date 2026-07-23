using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SaveCaptureContext
{
    public string SaveId { get; }
    public string GameVersion { get; }
    public string SavedAtUtc { get; }
    public double PlayTimeSeconds { get; }
    public string AppearanceId { get; }
    public string SceneName { get; }
    public string SpawnId { get; }

    public SaveCaptureContext(
        string saveId,
        string gameVersion,
        string savedAtUtc,
        double playTimeSeconds,
        string appearanceId,
        string sceneName,
        string spawnId)
    {
        SaveId = saveId?.Trim() ?? "";
        GameVersion = gameVersion?.Trim() ?? "";
        SavedAtUtc = savedAtUtc?.Trim() ?? "";
        PlayTimeSeconds = playTimeSeconds;
        AppearanceId = appearanceId?.Trim() ?? "";
        SceneName = sceneName?.Trim() ?? "";
        SpawnId = spawnId?.Trim() ?? "";
    }
}

public enum GameSessionRestoreStatus
{
    Success,
    InvalidSave,
    UnsupportedContent
}

public static class GameSessionSaveMapper
{
    public static GameSaveData Capture(
        GameSession session,
        SaveCaptureContext context)
    {
        if (session == null || !session.HasPlayer || context == null ||
            context.PlayTimeSeconds < 0 ||
            string.IsNullOrWhiteSpace(context.SceneName) ||
            string.IsNullOrWhiteSpace(context.SpawnId))
        {
            throw new ArgumentException(
                "A started session and explicit save location are required.");
        }

        var data = new GameSaveData
        {
            saveId = context.SaveId,
            gameVersion = context.GameVersion,
            savedAtUtc = context.SavedAtUtc,
            playTimeSeconds = context.PlayTimeSeconds,
            player = new PlayerSaveData
            {
                name = session.Player.Name,
                appearanceId = context.AppearanceId,
                gold = session.Player.Gold
            },
            location = new LocationSaveData
            {
                sceneName = context.SceneName,
                spawnId = context.SpawnId
            }
        };

        foreach (PlayableCharacterData character in session.Party.Characters
            .OrderBy(character => character.Id, StringComparer.Ordinal))
        {
            data.party.characters.Add(CaptureCharacter(
                character,
                session.Party.Characters));
        }

        data.party.activeCharacterIds.AddRange(
            session.Party.ActiveCharacterIds);
        foreach (KeyValuePair<string, int> entry in
            session.Inventory.Quantities.OrderBy(
                entry => entry.Key,
                StringComparer.Ordinal))
        {
            data.inventory.entries.Add(new InventoryEntrySaveData
            {
                itemId = entry.Key,
                quantity = entry.Value
            });
        }

        data.world.completedEncounterIds.AddRange(
            session.CompletedEncounterIds.OrderBy(
                id => id,
                StringComparer.Ordinal));
        data.world.claimedRewardIds.AddRange(
            session.RewardClaims.ClaimedRewardIds.OrderBy(
                id => id,
                StringComparer.Ordinal));
        data.camp.tutorialRestUsed =
            session.CampRestState.TutorialRestUsed;
        data.camp.completedRestCount =
            session.CampRestState.CompletedRestCount;
        return data;
    }

    public static GameSessionRestoreStatus TryRestore(
        GameSaveData data,
        out GameSession session,
        out LocationSaveData location)
    {
        session = null;
        location = null;
        if (!HasRequiredSaveData(data))
        {
            return GameSessionRestoreStatus.InvalidSave;
        }

        if (HasUnsupportedFutureContent(data))
        {
            return GameSessionRestoreStatus.UnsupportedContent;
        }

        if (!TryRestoreInventory(data.inventory, out Inventory inventory) ||
            !TryRestorePlayer(data, out PlayerData player) ||
            !TryRestoreParty(data.party, player, out PartyRoster party) ||
            !TryRestoreClaims(
                data.world.claimedRewardIds,
                out DemoRewardClaimState claims) ||
            !TryRestoreCamp(data.camp, out CampRestState camp) ||
            !TryRestoreEncounterIds(
                data.world.completedEncounterIds,
                out HashSet<string> encounterIds))
        {
            return GameSessionRestoreStatus.InvalidSave;
        }

        var restored = new GameSession();
        restored.RestorePersistentState(
            player,
            party,
            inventory,
            claims,
            camp,
            encounterIds);
        session = restored;
        location = new LocationSaveData
        {
            sceneName = data.location.sceneName.Trim(),
            spawnId = data.location.spawnId.Trim()
        };
        return GameSessionRestoreStatus.Success;
    }

    private static CharacterSaveData CaptureCharacter(
        PlayableCharacterData character,
        IEnumerable<PlayableCharacterData> roster)
    {
        CombatantStats stats = character.Stats;
        var data = new CharacterSaveData
        {
            characterId = character.Id,
            displayName = character.Name,
            isAvailable = character.IsAvailable,
            currentJobId = JobCatalog.Get(character.CurrentJob).StableId,
            level = character.Level,
            xp = character.XP,
            xpToNextLevel = character.XPToNextLevel,
            jobPoints = character.JobPoints,
            stats = CaptureStats(stats),
            rosterHistory = new RosterHistorySaveData
            {
                battlesActive = character.RosterHistory.BattlesActive,
                battlesBenched = character.RosterHistory.BattlesBenched,
                consecutiveBenchedBattles =
                    character.RosterHistory.ConsecutiveBenchedBattles
            }
        };

        data.learnedJobNodeIds.AddRange(
            character.LearnedJobNodeIds.OrderBy(
                id => id,
                StringComparer.Ordinal));
        foreach (JobDefinition job in JobCatalog.All.OrderBy(
            job => job.StableId,
            StringComparer.Ordinal))
        {
            JobAffinity affinity = character.GetJobAffinity(job.Id);
            if (affinity != JobAffinity.Neutral)
            {
                data.jobAffinities.Add(new JobAffinitySaveData
                {
                    jobId = job.StableId,
                    affinityId = AffinityToStableId(affinity)
                });
            }
        }

        foreach (KeyValuePair<EquipmentSlot, string> entry in
            character.Equipment.EquippedItemIds.OrderBy(
                entry => (int)entry.Key))
        {
            data.equipment.Add(new EquipmentSaveData
            {
                slotId = SlotToStableId(entry.Key),
                itemId = entry.Value
            });
        }

        foreach (PlayableCharacterData other in roster.OrderBy(
            other => other.Id,
            StringComparer.Ordinal))
        {
            int points = character.RosterHistory.GetBondPoints(other.Id);
            if (points > 0)
            {
                data.rosterHistory.bonds.Add(new BondSaveData
                {
                    otherCharacterId = other.Id,
                    points = points
                });
            }
        }

        return data;
    }

    private static CharacterStatsSaveData CaptureStats(CombatantStats stats)
    {
        return new CharacterStatsSaveData
        {
            maxHp = stats.MaxHp,
            currentHp = stats.CurrentHp,
            attack = stats.Attack,
            defense = stats.Defense,
            speed = stats.Speed,
            maxMp = stats.MaxMp,
            currentMp = stats.CurrentMp,
            magicPower = stats.MagicPower,
            magicDefense = stats.MagicDefense,
            accuracy = stats.Accuracy,
            evasion = stats.Evasion,
            criticalChance = stats.CriticalChance
        };
    }

    private static bool TryRestorePlayer(
        GameSaveData data,
        out PlayerData player)
    {
        player = null;
        if (string.IsNullOrWhiteSpace(data.player.name) ||
            data.player.gold < 0)
        {
            return false;
        }

        CharacterSaveData protagonist = data.party.characters.FirstOrDefault(
            character => character != null &&
                character.characterId == PlayableCharacterData.ProtagonistId);
        if (protagonist == null ||
            !string.Equals(
                protagonist.displayName?.Trim(),
                data.player.name.Trim(),
                StringComparison.Ordinal) ||
            !TryCreateStats(protagonist.stats, out CombatantStats stats))
        {
            return false;
        }

        player = new PlayerData(data.player.name.Trim())
        {
            Gold = data.player.gold
        };
        if (!player.Progression.TryRestore(
                protagonist.level,
                protagonist.xp,
                protagonist.xpToNextLevel))
        {
            player = null;
            return false;
        }

        CopyStats(stats, player.Stats);
        return true;
    }

    private static bool TryRestoreParty(
        PartySaveData data,
        PlayerData player,
        out PartyRoster party)
    {
        party = null;
        if (data?.characters == null || data.activeCharacterIds == null ||
            data.characters.Count == 0)
        {
            return false;
        }

        var restored = new PartyRoster();
        HashSet<string> ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (CharacterSaveData characterData in data.characters)
        {
            if (!TryRestoreCharacter(
                    characterData,
                    player,
                    out PlayableCharacterData character) ||
                !ids.Add(character.Id) ||
                !restored.TryAddCharacter(character))
            {
                return false;
            }
        }

        foreach (CharacterSaveData characterData in data.characters)
        {
            foreach (BondSaveData bond in characterData.rosterHistory.bonds)
            {
                if (!ids.Contains(bond.otherCharacterId?.Trim()))
                {
                    return false;
                }
            }
        }

        if (!ids.Contains(PlayableCharacterData.ProtagonistId) ||
            !restored.TrySetActiveParty(data.activeCharacterIds))
        {
            return false;
        }

        party = restored;
        return true;
    }

    private static bool TryRestoreCharacter(
        CharacterSaveData data,
        PlayerData player,
        out PlayableCharacterData character)
    {
        character = null;
        JobDefinition job = JobCatalog.Get(data?.currentJobId);
        if (data == null || string.IsNullOrWhiteSpace(data.characterId) ||
            string.IsNullOrWhiteSpace(data.displayName) || job == null ||
            data.learnedJobNodeIds == null || data.jobAffinities == null ||
            data.equipment == null || data.rosterHistory == null ||
            data.rosterHistory.bonds == null ||
            !TryCreateStats(data.stats, out CombatantStats savedStats))
        {
            return false;
        }

        bool isProtagonist = data.characterId.Trim() ==
            PlayableCharacterData.ProtagonistId;
        character = new PlayableCharacterData(
            data.characterId,
            data.displayName,
            job.Id,
            isProtagonist ? player.Stats : null,
            characterProgression:
                isProtagonist ? player.Progression : null);
        if (!character.TryRestoreProgression(
                data.level,
                data.xp,
                data.xpToNextLevel,
                data.jobPoints,
                data.learnedJobNodeIds) ||
            !TryRestoreAffinities(character, data.jobAffinities) ||
            !TryRestoreEquipment(character, data.equipment) ||
            !TryRestoreHistory(character, data.rosterHistory))
        {
            character = null;
            return false;
        }

        character.RestoreStats(savedStats);
        if (!data.isAvailable)
        {
            if (data.equipment.Count > 0)
            {
                character = null;
                return false;
            }

            character.RemovePermanently();
        }

        return true;
    }

    private static bool TryRestoreAffinities(
        PlayableCharacterData character,
        IEnumerable<JobAffinitySaveData> entries)
    {
        HashSet<string> jobs = new HashSet<string>(StringComparer.Ordinal);
        foreach (JobAffinitySaveData entry in entries)
        {
            JobDefinition job = JobCatalog.Get(entry?.jobId);
            if (job == null || !jobs.Add(job.StableId) ||
                !TryParseAffinity(entry.affinityId, out JobAffinity affinity))
            {
                return false;
            }

            character.SetJobAffinity(job.Id, affinity);
        }

        return true;
    }

    private static bool TryRestoreEquipment(
        PlayableCharacterData character,
        IEnumerable<EquipmentSaveData> entries)
    {
        var restored =
            new List<KeyValuePair<EquipmentSlot, string>>();
        foreach (EquipmentSaveData entry in entries)
        {
            if (entry == null ||
                !TryParseSlot(entry.slotId, out EquipmentSlot slot))
            {
                return false;
            }

            restored.Add(new KeyValuePair<EquipmentSlot, string>(
                slot,
                entry.itemId));
        }

        return character.TryRestoreEquipment(restored);
    }

    private static bool TryRestoreHistory(
        PlayableCharacterData character,
        RosterHistorySaveData history)
    {
        var bonds = new List<KeyValuePair<string, int>>();
        foreach (BondSaveData bond in history.bonds)
        {
            if (bond == null)
            {
                return false;
            }

            bonds.Add(new KeyValuePair<string, int>(
                bond.otherCharacterId,
                bond.points));
        }

        return character.TryRestoreRosterHistory(
            history.battlesActive,
            history.battlesBenched,
            history.consecutiveBenchedBattles,
            bonds);
    }

    private static bool TryRestoreInventory(
        InventorySaveData data,
        out Inventory inventory)
    {
        inventory = null;
        if (data?.entries == null)
        {
            return false;
        }

        var entries = new List<KeyValuePair<string, int>>();
        foreach (InventoryEntrySaveData entry in data.entries)
        {
            if (entry == null)
            {
                return false;
            }

            entries.Add(new KeyValuePair<string, int>(
                entry.itemId,
                entry.quantity));
        }

        var restored = new Inventory();
        if (!restored.TryRestore(entries))
        {
            return false;
        }

        inventory = restored;
        return true;
    }

    private static bool TryRestoreClaims(
        IEnumerable<string> rewardIds,
        out DemoRewardClaimState claims)
    {
        claims = new DemoRewardClaimState();
        if (claims.TryRestore(rewardIds))
        {
            return true;
        }

        claims = null;
        return false;
    }

    private static bool TryRestoreCamp(
        CampSaveData data,
        out CampRestState camp)
    {
        camp = null;
        if (data == null)
        {
            return false;
        }

        var restored = new CampRestState();
        if (!restored.TryRestore(
                data.tutorialRestUsed,
                data.completedRestCount))
        {
            return false;
        }

        camp = restored;
        return true;
    }

    private static bool TryRestoreEncounterIds(
        IEnumerable<string> ids,
        out HashSet<string> restored)
    {
        restored = new HashSet<string>(StringComparer.Ordinal);
        if (ids == null)
        {
            return false;
        }

        foreach (string id in ids)
        {
            BattleEncounterDefinition encounter =
                BattleEncounterCatalog.Get(id);
            if (encounter == null || !restored.Add(encounter.Id))
            {
                restored = null;
                return false;
            }
        }

        return true;
    }

    private static bool TryCreateStats(
        CharacterStatsSaveData data,
        out CombatantStats stats)
    {
        stats = null;
        if (data == null || data.maxHp < 1 || data.currentHp < 0 ||
            data.currentHp > data.maxHp || data.attack < 0 ||
            data.defense < 0 || data.speed < 0 || data.maxMp < 0 ||
            data.currentMp < 0 || data.currentMp > data.maxMp ||
            data.magicPower < 0 || data.magicDefense < 0 ||
            data.accuracy < 0 || data.accuracy > 100 ||
            data.evasion < 0 || data.evasion > 95 ||
            data.criticalChance < 0 || data.criticalChance > 100)
        {
            return false;
        }

        stats = new CombatantStats(
            data.maxHp,
            data.attack,
            data.defense,
            data.speed,
            data.maxMp,
            data.magicPower,
            data.magicDefense,
            data.accuracy,
            data.evasion,
            data.criticalChance)
        {
            CurrentHp = data.currentHp,
            CurrentMp = data.currentMp
        };
        return true;
    }

    private static void CopyStats(
        CombatantStats source,
        CombatantStats destination)
    {
        destination.MaxHp = source.MaxHp;
        destination.MaxMp = source.MaxMp;
        destination.Attack = source.Attack;
        destination.Defense = source.Defense;
        destination.Speed = source.Speed;
        destination.MagicPower = source.MagicPower;
        destination.MagicDefense = source.MagicDefense;
        destination.Accuracy = source.Accuracy;
        destination.Evasion = source.Evasion;
        destination.CriticalChance = source.CriticalChance;
        destination.CurrentHp = source.CurrentHp;
        destination.CurrentMp = source.CurrentMp;
    }

    private static bool HasRequiredSaveData(GameSaveData data)
    {
        return data != null &&
            data.schemaVersion == SaveSchema.CurrentVersion &&
            data.player != null &&
            data.party != null &&
            data.inventory != null &&
            data.quests?.entries != null &&
            data.world?.completedEncounterIds != null &&
            data.world.claimedRewardIds != null &&
            data.world.flags != null &&
            data.camp?.consumedEventIds != null &&
            data.location != null &&
            string.IsNullOrWhiteSpace(data.location.sceneName) == false &&
            string.IsNullOrWhiteSpace(data.location.spawnId) == false &&
            data.playTimeSeconds >= 0;
    }

    private static bool HasUnsupportedFutureContent(GameSaveData data)
    {
        return data.quests.entries.Count > 0 ||
            data.quests.completedQuestCount != 0 ||
            data.world.flags.Count > 0 ||
            !string.IsNullOrWhiteSpace(data.camp.activeCampId) ||
            data.camp.consumedEventIds.Count > 0 ||
            !string.IsNullOrWhiteSpace(data.camp.returnLocation?.sceneName) ||
            !string.IsNullOrWhiteSpace(data.camp.returnLocation?.spawnId);
    }

    private static string SlotToStableId(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon:
                return "slot_weapon";
            case EquipmentSlot.Armor:
                return "slot_armor";
            case EquipmentSlot.Accessory1:
                return "slot_accessory_1";
            case EquipmentSlot.Accessory2:
                return "slot_accessory_2";
            case EquipmentSlot.Necklace:
                return "slot_necklace";
            default:
                throw new ArgumentOutOfRangeException(nameof(slot));
        }
    }

    private static bool TryParseSlot(
        string stableId,
        out EquipmentSlot slot)
    {
        switch (stableId?.Trim())
        {
            case "slot_weapon":
                slot = EquipmentSlot.Weapon;
                return true;
            case "slot_armor":
                slot = EquipmentSlot.Armor;
                return true;
            case "slot_accessory_1":
                slot = EquipmentSlot.Accessory1;
                return true;
            case "slot_accessory_2":
                slot = EquipmentSlot.Accessory2;
                return true;
            case "slot_necklace":
                slot = EquipmentSlot.Necklace;
                return true;
            default:
                slot = default;
                return false;
        }
    }

    private static string AffinityToStableId(JobAffinity affinity)
    {
        switch (affinity)
        {
            case JobAffinity.Low:
                return "affinity_low";
            case JobAffinity.Neutral:
                return "affinity_neutral";
            case JobAffinity.High:
                return "affinity_high";
            default:
                throw new ArgumentOutOfRangeException(nameof(affinity));
        }
    }

    private static bool TryParseAffinity(
        string stableId,
        out JobAffinity affinity)
    {
        switch (stableId?.Trim())
        {
            case "affinity_low":
                affinity = JobAffinity.Low;
                return true;
            case "affinity_neutral":
                affinity = JobAffinity.Neutral;
                return true;
            case "affinity_high":
                affinity = JobAffinity.High;
                return true;
            default:
                affinity = default;
                return false;
        }
    }
}
