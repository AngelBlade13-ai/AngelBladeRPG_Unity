using System;
using System.Collections.Generic;

public sealed class CharacterEquipment
{
    private readonly Dictionary<EquipmentSlot, string> equippedItemIds =
        new Dictionary<EquipmentSlot, string>();

    public IReadOnlyDictionary<EquipmentSlot, string> EquippedItemIds =>
        equippedItemIds;

    public string GetItemId(EquipmentSlot slot)
    {
        return equippedItemIds.TryGetValue(slot, out string itemId)
            ? itemId
            : null;
    }

    public bool TryEquip(
        EquipmentSlot slot,
        string itemId,
        JobId currentJob,
        Inventory inventory)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        if (inventory == null || item == null || !item.CanEquipIn(slot) ||
            !item.IsCompatibleWith(currentJob))
        {
            return false;
        }

        string previousItemId = GetItemId(slot);
        if (string.Equals(previousItemId, item.Id, StringComparison.Ordinal))
        {
            return true;
        }

        if (inventory.GetQuantity(item.Id) < 1 ||
            (previousItemId != null && !inventory.CanAdd(previousItemId)))
        {
            return false;
        }

        inventory.TryRemove(item.Id);
        if (previousItemId != null)
        {
            inventory.TryAdd(previousItemId);
        }

        equippedItemIds[slot] = item.Id;
        return true;
    }

    public bool TryUnequip(EquipmentSlot slot, Inventory inventory)
    {
        string itemId = GetItemId(slot);
        if (itemId == null || inventory == null || !inventory.CanAdd(itemId))
        {
            return false;
        }

        inventory.TryAdd(itemId);
        equippedItemIds.Remove(slot);
        return true;
    }

    public bool WeaponIsCompatibleWith(JobId jobId)
    {
        string weaponId = GetItemId(EquipmentSlot.Weapon);
        ItemDefinition weapon = ItemCatalog.Get(weaponId);
        return weapon == null || weapon.IsCompatibleWith(jobId);
    }

    public EquipmentStatBonuses GetTotalStatBonuses()
    {
        int maxHp = 0;
        int attack = 0;
        int defense = 0;
        int speed = 0;
        int maxMp = 0;
        int magicPower = 0;
        int magicDefense = 0;
        int accuracy = 0;
        int evasion = 0;
        int criticalChance = 0;

        foreach (string itemId in equippedItemIds.Values)
        {
            EquipmentStatBonuses bonus = ItemCatalog.Get(itemId).StatBonuses;
            maxHp += bonus.MaxHp;
            attack += bonus.Attack;
            defense += bonus.Defense;
            speed += bonus.Speed;
            maxMp += bonus.MaxMp;
            magicPower += bonus.MagicPower;
            magicDefense += bonus.MagicDefense;
            accuracy += bonus.Accuracy;
            evasion += bonus.Evasion;
            criticalChance += bonus.CriticalChance;
        }

        return new EquipmentStatBonuses(
            maxHp,
            attack,
            defense,
            speed,
            maxMp,
            magicPower,
            magicDefense,
            accuracy,
            evasion,
            criticalChance);
    }

    public int DestroyAll()
    {
        int destroyedCount = equippedItemIds.Count;
        equippedItemIds.Clear();
        return destroyedCount;
    }
}
