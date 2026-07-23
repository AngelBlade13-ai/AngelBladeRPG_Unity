using System;
using System.Collections.Generic;

public sealed class ItemGrant
{
    public string ItemId { get; }
    public int Quantity { get; }

    public ItemGrant(string itemId, int quantity)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        if (item == null)
        {
            throw new ArgumentException("A known item ID is required.", nameof(itemId));
        }

        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        ItemId = item.Id;
        Quantity = quantity;
    }
}

public sealed class DemoRewardDefinition
{
    private readonly List<ItemGrant> items;

    public string Id { get; }
    public int Gold { get; }
    public IReadOnlyList<ItemGrant> Items => items.AsReadOnly();

    public DemoRewardDefinition(
        string id,
        int gold,
        IEnumerable<ItemGrant> itemGrants = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("A reward ID is required.", nameof(id));
        }

        if (gold < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(gold));
        }

        items = itemGrants == null
            ? new List<ItemGrant>()
            : new List<ItemGrant>(itemGrants);
        var itemIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (ItemGrant item in items)
        {
            if (item == null || !itemIds.Add(item.ItemId))
            {
                throw new ArgumentException(
                    "Reward items must be non-null and unique.",
                    nameof(itemGrants));
            }
        }

        Id = id.Trim();
        Gold = gold;
    }
}

public sealed class MonsterLootEntry
{
    public string ItemId { get; }
    public int ChancePercent { get; }
    public int Quantity { get; }

    public MonsterLootEntry(
        string itemId,
        int chancePercent,
        int quantity = 1)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        if (item == null)
        {
            throw new ArgumentException("A known item ID is required.", nameof(itemId));
        }

        if (chancePercent < 1 || chancePercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(chancePercent));
        }

        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        ItemId = item.Id;
        ChancePercent = chancePercent;
        Quantity = quantity;
    }
}

public sealed class MonsterLootTable
{
    private readonly List<MonsterLootEntry> entries;

    public string MonsterId { get; }
    public IReadOnlyList<MonsterLootEntry> Entries => entries.AsReadOnly();

    public MonsterLootTable(
        string monsterId,
        IEnumerable<MonsterLootEntry> lootEntries)
    {
        MonsterDefinition monster = MonsterCatalog.Get(monsterId);
        if (monster == null)
        {
            throw new ArgumentException(
                "A known monster ID is required.",
                nameof(monsterId));
        }

        entries = lootEntries == null
            ? new List<MonsterLootEntry>()
            : new List<MonsterLootEntry>(lootEntries);
        MonsterId = monster.Id;
    }
}

public static class DemoEconomyCatalog
{
    public const int TownRecoveryPrice = 25;

    public const string Quest1EvidenceRewardId =
        "reward_main_q1_trail_evidence";
    public const string Quest1BaseRewardId = "reward_main_q1_base";
    public const string Quest1ThirdSpotRewardId = "reward_main_q1_spot_03";
    public const string Quest1FullClearRewardId = "reward_main_q1_full_clear";
    public const string Quest2RewardId = "reward_main_q2";
    public const string Quest3BaseRewardId = "reward_main_q3_base";
    public const string Quest3FullRescueRewardId =
        "reward_main_q3_full_rescue";
    public const string GoblinBossReturnRewardId =
        "reward_grassland_boss_return";
    public const string TasteOfHomeRewardId =
        "reward_side_taste_of_home";
    public const string RoadsideChimesRewardId =
        "reward_side_roadside_chimes";
    public const string PaintersViewRewardId =
        "reward_side_painters_view";
    public const string HerdWontGrazeRewardId =
        "reward_side_herd_wont_graze";

    private static readonly Dictionary<string, DemoRewardDefinition> rewards =
        CreateRewards();
    private static readonly Dictionary<string, MonsterLootTable> lootTables =
        CreateLootTables();

    private static readonly string[] criticalPathRewardIds =
    {
        Quest1BaseRewardId,
        Quest2RewardId,
        Quest3BaseRewardId,
        GoblinBossReturnRewardId
    };

    private static readonly string[] minimumCriticalPathEncounterIds =
    {
        BattleEncounterCatalog.CaravanTutorialId,
        BattleEncounterCatalog.Quest2RescueId,
        BattleEncounterCatalog.Quest3PatrolAId,
        BattleEncounterCatalog.Quest3PatrolCId,
        BattleEncounterCatalog.GoblinBossId
    };

    public static IReadOnlyCollection<DemoRewardDefinition> Rewards =>
        rewards.Values;
    public static IReadOnlyCollection<MonsterLootTable> LootTables =>
        lootTables.Values;
    public static IReadOnlyList<string> CriticalPathRewardIds =>
        Array.AsReadOnly(criticalPathRewardIds);
    public static IReadOnlyList<string> MinimumCriticalPathEncounterIds =>
        Array.AsReadOnly(minimumCriticalPathEncounterIds);

    public static DemoRewardDefinition GetReward(string rewardId)
    {
        if (string.IsNullOrWhiteSpace(rewardId))
        {
            return null;
        }

        return rewards.TryGetValue(
            rewardId.Trim(),
            out DemoRewardDefinition reward)
            ? reward
            : null;
    }

    public static MonsterLootTable GetLootTable(string monsterId)
    {
        if (string.IsNullOrWhiteSpace(monsterId))
        {
            return null;
        }

        return lootTables.TryGetValue(
            monsterId.Trim(),
            out MonsterLootTable table)
            ? table
            : null;
    }

    public static int CalculateEncounterGold(string encounterId)
    {
        BattleEncounterDefinition encounter =
            BattleEncounterCatalog.Get(encounterId);
        if (encounter == null)
        {
            return 0;
        }

        int total = 0;
        foreach (string monsterId in encounter.MonsterDefinitionIds)
        {
            total += MonsterCatalog.Get(monsterId).GoldReward;
        }

        if (encounter.Id == BattleEncounterCatalog.CaravanTutorialId)
        {
            total += MonsterCatalog.Get(
                "monster_tutorial_hobgoblin").GoldReward;
        }

        return total;
    }

    public static int CalculateMinimumCriticalPathGold()
    {
        int total = 0;
        foreach (string rewardId in criticalPathRewardIds)
        {
            total += GetReward(rewardId).Gold;
        }

        foreach (string encounterId in minimumCriticalPathEncounterIds)
        {
            total += CalculateEncounterGold(encounterId);
        }

        return total;
    }

    public static int CalculateRecommendedCriticalPathSpend()
    {
        return
            ItemCatalog.Get(ItemCatalog.IronHeavyBladeId).BuyPrice * 2 +
            ItemCatalog.Get(ItemCatalog.OakStaffId).BuyPrice +
            ItemCatalog.Get(ItemCatalog.PaddedArmorId).BuyPrice +
            ItemCatalog.Get(ItemCatalog.MinorPotionId).BuyPrice * 3 +
            TownRecoveryPrice * 2;
    }

    private static Dictionary<string, DemoRewardDefinition> CreateRewards()
    {
        var catalog = new Dictionary<string, DemoRewardDefinition>(
            StringComparer.Ordinal);

        Add(catalog, Reward(
            Quest1EvidenceRewardId,
            0,
            new ItemGrant(ItemCatalog.TrailKnifeFragmentId, 1)));
        Add(catalog, Reward(
            Quest1BaseRewardId,
            60,
            new ItemGrant(ItemCatalog.MinorPotionId, 2)));
        Add(catalog, Reward(
            Quest1ThirdSpotRewardId,
            15,
            new ItemGrant(ItemCatalog.MinorPotionId, 1)));
        Add(catalog, Reward(
            Quest1FullClearRewardId,
            15,
            new ItemGrant(ItemCatalog.MarlowsTradeCharmId, 1)));
        Add(catalog, Reward(
            Quest2RewardId,
            90,
            new ItemGrant(ItemCatalog.IronforgeFieldguardId, 1)));
        Add(catalog, Reward(
            Quest3BaseRewardId,
            120,
            new ItemGrant(ItemCatalog.MinorPotionId, 2),
            new ItemGrant(ItemCatalog.FieldRemedyId, 1),
            new ItemGrant(ItemCatalog.CampRationId, 1)));
        Add(catalog, Reward(
            Quest3FullRescueRewardId,
            30,
            new ItemGrant(ItemCatalog.SuncrestWatchInsigniaId, 1)));
        Add(catalog, Reward(
            GoblinBossReturnRewardId,
            120,
            new ItemGrant(ItemCatalog.CampRationId, 1)));
        Add(catalog, Reward(
            TasteOfHomeRewardId,
            0,
            new ItemGrant(ItemCatalog.SuncrestSupperId, 3)));
        Add(catalog, Reward(
            RoadsideChimesRewardId,
            0,
            new ItemGrant(ItemCatalog.FieldRemedyId, 2)));
        Add(catalog, Reward(
            PaintersViewRewardId,
            60,
            new ItemGrant(ItemCatalog.TravelersTonicId, 2)));
        Add(catalog, Reward(
            HerdWontGrazeRewardId,
            0,
            new ItemGrant(ItemCatalog.SettlementTeaId, 2),
            new ItemGrant(ItemCatalog.NomadsWovenCordId, 1)));

        return catalog;
    }

    private static Dictionary<string, MonsterLootTable> CreateLootTables()
    {
        var catalog = new Dictionary<string, MonsterLootTable>(
            StringComparer.Ordinal);

        Add(catalog, Loot(
            "monster_slime",
            new MonsterLootEntry(ItemCatalog.MinorPotionId, 20)));
        Add(catalog, Loot(
            "monster_wild_boar",
            new MonsterLootEntry(ItemCatalog.CampRationId, 10)));
        Add(catalog, Loot(
            "monster_goblin_skirmisher",
            new MonsterLootEntry(ItemCatalog.MinorPotionId, 8)));
        Add(catalog, Loot(
            "monster_goblin_slinger",
            new MonsterLootEntry(ItemCatalog.FieldRemedyId, 10)));
        Add(catalog, Loot(
            "monster_goblin_guard",
            new MonsterLootEntry(ItemCatalog.MinorPotionId, 12)));
        Add(catalog, Loot(
            "monster_goblin_raider",
            new MonsterLootEntry(ItemCatalog.MinorPotionId, 15)));

        return catalog;
    }

    private static DemoRewardDefinition Reward(
        string id,
        int gold,
        params ItemGrant[] items)
    {
        return new DemoRewardDefinition(id, gold, items);
    }

    private static MonsterLootTable Loot(
        string monsterId,
        params MonsterLootEntry[] entries)
    {
        return new MonsterLootTable(monsterId, entries);
    }

    private static void Add(
        IDictionary<string, DemoRewardDefinition> catalog,
        DemoRewardDefinition reward)
    {
        catalog.Add(reward.Id, reward);
    }

    private static void Add(
        IDictionary<string, MonsterLootTable> catalog,
        MonsterLootTable table)
    {
        catalog.Add(table.MonsterId, table);
    }
}
