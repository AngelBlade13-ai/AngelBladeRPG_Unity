using System;
using System.Collections.Generic;

public enum ItemKind
{
    Consumable,
    Weapon,
    Armor,
    Accessory,
    Necklace,
    KeyItem
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum EquipmentSlot
{
    Weapon,
    Armor,
    Accessory1,
    Accessory2,
    Necklace
}

public enum WeaponCategory
{
    None,
    HeavyBlade,
    Scythe,
    Bow,
    Dagger,
    Staff,
    Instrument,
    TomeOrCharm,
    BondedTotemOrHorn
}

public sealed class EquipmentStatBonuses
{
    public static EquipmentStatBonuses None { get; } =
        new EquipmentStatBonuses();

    public int MaxHp { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int Speed { get; }
    public int MaxMp { get; }
    public int MagicPower { get; }
    public int MagicDefense { get; }
    public int Accuracy { get; }
    public int Evasion { get; }
    public int CriticalChance { get; }

    public EquipmentStatBonuses(
        int maxHp = 0,
        int attack = 0,
        int defense = 0,
        int speed = 0,
        int maxMp = 0,
        int magicPower = 0,
        int magicDefense = 0,
        int accuracy = 0,
        int evasion = 0,
        int criticalChance = 0)
    {
        MaxHp = RequireNonNegative(maxHp, nameof(maxHp));
        Attack = RequireNonNegative(attack, nameof(attack));
        Defense = RequireNonNegative(defense, nameof(defense));
        Speed = RequireNonNegative(speed, nameof(speed));
        MaxMp = RequireNonNegative(maxMp, nameof(maxMp));
        MagicPower = RequireNonNegative(magicPower, nameof(magicPower));
        MagicDefense = RequireNonNegative(magicDefense, nameof(magicDefense));
        Accuracy = RequireNonNegative(accuracy, nameof(accuracy));
        Evasion = RequireNonNegative(evasion, nameof(evasion));
        CriticalChance = RequireNonNegative(
            criticalChance,
            nameof(criticalChance));
    }

    private static int RequireNonNegative(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }

        return value;
    }
}

public sealed class ItemDefinition
{
    private readonly HashSet<JobId> compatibleJobs;

    public string Id { get; }
    public string DisplayName { get; }
    public ItemKind Kind { get; }
    public ItemRarity Rarity { get; }
    public int MaximumStack { get; }
    public int BuyPrice { get; }
    public int SellPrice { get; }
    public bool CanSell { get; }
    public bool CanDiscard { get; }
    public WeaponCategory WeaponCategory { get; }
    public EquipmentStatBonuses StatBonuses { get; }
    public bool IsEquipment =>
        Kind == ItemKind.Weapon || Kind == ItemKind.Armor ||
        Kind == ItemKind.Accessory || Kind == ItemKind.Necklace;
    public IReadOnlyCollection<JobId> CompatibleJobs => compatibleJobs;

    public ItemDefinition(
        string id,
        string displayName,
        ItemKind kind,
        ItemRarity rarity = ItemRarity.Common,
        int maximumStack = 99,
        int buyPrice = 0,
        int sellPrice = 0,
        bool canSell = true,
        bool canDiscard = true,
        WeaponCategory weaponCategory = WeaponCategory.None,
        IEnumerable<JobId> compatibleJobs = null,
        EquipmentStatBonuses statBonuses = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("An item ID is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException(
                "An item display name is required.",
                nameof(displayName));
        }

        if (maximumStack < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumStack));
        }

        if (buyPrice < 0 || sellPrice < 0)
        {
            throw new ArgumentOutOfRangeException(
                buyPrice < 0 ? nameof(buyPrice) : nameof(sellPrice));
        }

        this.compatibleJobs = compatibleJobs == null
            ? new HashSet<JobId>()
            : new HashSet<JobId>(compatibleJobs);
        if (kind == ItemKind.Weapon &&
            (weaponCategory == WeaponCategory.None ||
                this.compatibleJobs.Count == 0))
        {
            throw new ArgumentException(
                "Weapons require a category and at least one compatible job.");
        }

        if (kind != ItemKind.Weapon && weaponCategory != WeaponCategory.None)
        {
            throw new ArgumentException(
                "Only weapons can declare a weapon category.");
        }

        Id = id.Trim();
        DisplayName = displayName.Trim();
        Kind = kind;
        Rarity = rarity;
        MaximumStack = maximumStack;
        BuyPrice = buyPrice;
        SellPrice = sellPrice;
        CanSell = canSell && kind != ItemKind.KeyItem;
        CanDiscard = canDiscard && kind != ItemKind.KeyItem;
        WeaponCategory = weaponCategory;
        StatBonuses = statBonuses ?? EquipmentStatBonuses.None;
    }

    public bool CanEquipIn(EquipmentSlot slot)
    {
        switch (Kind)
        {
            case ItemKind.Weapon:
                return slot == EquipmentSlot.Weapon;
            case ItemKind.Armor:
                return slot == EquipmentSlot.Armor;
            case ItemKind.Accessory:
                return slot == EquipmentSlot.Accessory1 ||
                    slot == EquipmentSlot.Accessory2;
            case ItemKind.Necklace:
                return slot == EquipmentSlot.Necklace;
            default:
                return false;
        }
    }

    public bool IsCompatibleWith(JobId jobId)
    {
        return Kind != ItemKind.Weapon || compatibleJobs.Contains(jobId);
    }
}
