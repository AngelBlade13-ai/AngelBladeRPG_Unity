using System;
using System.Collections.Generic;

public static class ItemCatalog
{
    public const string MinorPotionId = "consumable_minor_potion";
    public const string FieldRemedyId = "consumable_field_remedy";
    public const string CampRationId = "consumable_camp_ration";
    public const string IronHeavyBladeId = "weapon_iron_heavy_blade";
    public const string EnoraScytheId = "weapon_enora_scythe";
    public const string AshBowId = "weapon_ash_bow";
    public const string IronDaggerId = "weapon_iron_dagger";
    public const string OakStaffId = "weapon_oak_staff";
    public const string TravelersLuteId = "weapon_travelers_lute";
    public const string ApprenticeTomeId = "weapon_apprentice_tome";
    public const string CarvedHornId = "weapon_carved_horn";
    public const string PaddedArmorId = "armor_padded";
    public const string GuardCharmId = "accessory_guard_charm";
    public const string SunstoneNecklaceId = "necklace_sunstone";

    private static readonly Dictionary<string, ItemDefinition> items =
        CreateCatalog();

    public static IReadOnlyCollection<ItemDefinition> All => items.Values;

    public static ItemDefinition Get(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        return items.TryGetValue(itemId.Trim(), out ItemDefinition item)
            ? item
            : null;
    }

    public static bool Contains(string itemId)
    {
        return Get(itemId) != null;
    }

    private static Dictionary<string, ItemDefinition> CreateCatalog()
    {
        var catalog = new Dictionary<string, ItemDefinition>(
            StringComparer.Ordinal);

        Add(catalog, new ItemDefinition(
            MinorPotionId, "Minor Potion", ItemKind.Consumable,
            buyPrice: 30, sellPrice: 10));
        Add(catalog, new ItemDefinition(
            FieldRemedyId, "Field Remedy", ItemKind.Consumable,
            buyPrice: 45, sellPrice: 15));
        Add(catalog, new ItemDefinition(
            CampRationId, "Camp Ration", ItemKind.Consumable,
            buyPrice: 80, sellPrice: 25));

        Add(catalog, Weapon(
            IronHeavyBladeId, "Iron Heavy Blade", WeaponCategory.HeavyBlade,
            new[] { JobId.Knight, JobId.Reaver, JobId.Mercenary, JobId.Paladin },
            new EquipmentStatBonuses(attack: 4), 120, 40));
        Add(catalog, Weapon(
            EnoraScytheId, "Enora's Scythe", WeaponCategory.Scythe,
            new[] { JobId.BloodMage },
            new EquipmentStatBonuses(attack: 2, magicPower: 4), 0, 0,
            canSell: false));
        Add(catalog, Weapon(
            AshBowId, "Ash Bow", WeaponCategory.Bow,
            new[] { JobId.Ranger },
            new EquipmentStatBonuses(attack: 3, accuracy: 3), 110, 35));
        Add(catalog, Weapon(
            IronDaggerId, "Iron Dagger", WeaponCategory.Dagger,
            new[] { JobId.Rogue },
            new EquipmentStatBonuses(attack: 2, speed: 2), 100, 30));
        Add(catalog, Weapon(
            OakStaffId, "Oak Staff", WeaponCategory.Staff,
            new[] { JobId.Mage, JobId.WhiteMage },
            new EquipmentStatBonuses(magicPower: 4), 115, 35));
        Add(catalog, Weapon(
            TravelersLuteId, "Traveler's Lute", WeaponCategory.Instrument,
            new[] { JobId.Bard },
            new EquipmentStatBonuses(speed: 1, magicPower: 3), 115, 35));
        Add(catalog, Weapon(
            ApprenticeTomeId, "Apprentice Tome", WeaponCategory.TomeOrCharm,
            new[] { JobId.Tactician },
            new EquipmentStatBonuses(maxMp: 5, magicPower: 2), 110, 35));
        Add(catalog, Weapon(
            CarvedHornId, "Carved Horn", WeaponCategory.BondedTotemOrHorn,
            new[] { JobId.Summoner },
            new EquipmentStatBonuses(maxMp: 5, magicPower: 3), 120, 40));

        Add(catalog, new ItemDefinition(
            PaddedArmorId, "Padded Armor", ItemKind.Armor,
            buyPrice: 100, sellPrice: 35,
            statBonuses: new EquipmentStatBonuses(maxHp: 10, defense: 2)));
        Add(catalog, new ItemDefinition(
            GuardCharmId, "Guard Charm", ItemKind.Accessory,
            buyPrice: 90, sellPrice: 30,
            statBonuses: new EquipmentStatBonuses(defense: 1)));
        Add(catalog, new ItemDefinition(
            SunstoneNecklaceId, "Sunstone Necklace", ItemKind.Necklace,
            rarity: ItemRarity.Uncommon,
            buyPrice: 180, sellPrice: 60,
            statBonuses: new EquipmentStatBonuses(
                maxHp: 5,
                magicDefense: 2)));

        return catalog;
    }

    private static ItemDefinition Weapon(
        string id,
        string name,
        WeaponCategory category,
        JobId[] jobs,
        EquipmentStatBonuses bonuses,
        int buyPrice,
        int sellPrice,
        bool canSell = true)
    {
        return new ItemDefinition(
            id,
            name,
            ItemKind.Weapon,
            maximumStack: 99,
            buyPrice: buyPrice,
            sellPrice: sellPrice,
            canSell: canSell,
            canDiscard: canSell,
            weaponCategory: category,
            compatibleJobs: jobs,
            statBonuses: bonuses);
    }

    private static void Add(
        IDictionary<string, ItemDefinition> catalog,
        ItemDefinition item)
    {
        catalog.Add(item.Id, item);
    }
}
