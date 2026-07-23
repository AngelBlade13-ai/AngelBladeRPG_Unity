using System;
using System.Collections.Generic;

public static class InventoryEquipmentMenuData
{
    public static IReadOnlyList<ItemDefinition> GetConsumables(
        Inventory inventory)
    {
        return GetInventoryItems(
            inventory,
            item => item.Kind == ItemKind.Consumable);
    }

    public static IReadOnlyList<ItemDefinition> GetEquipmentCandidates(
        Inventory inventory,
        EquipmentSlot slot,
        JobId jobId)
    {
        return GetInventoryItems(
            inventory,
            item => item.CanEquipIn(slot) && item.IsCompatibleWith(jobId));
    }

    public static string FormatItem(ItemDefinition item, int quantity)
    {
        if (item == null)
        {
            return "No usable items in the inventory.";
        }

        string effect;
        switch (item.ConsumableEffect)
        {
            case ConsumableEffect.RestoreHp:
                effect = $"Restores {item.ConsumablePotency} HP.";
                break;
            case ConsumableEffect.RemoveNegativeStatus:
                effect = "Removes a negative status.";
                break;
            case ConsumableEffect.FullRest:
                effect = "Used when making camp.";
                break;
            default:
                effect = "No field effect.";
                break;
        }

        return $"{item.DisplayName}  x{quantity}\n{effect}";
    }

    public static string FormatEquipment(ItemDefinition item)
    {
        if (item == null)
        {
            return "No compatible equipment in the inventory.";
        }

        EquipmentStatBonuses stats = item.StatBonuses;
        List<string> bonuses = new List<string>();
        AddBonus(bonuses, "HP", stats.MaxHp);
        AddBonus(bonuses, "MP", stats.MaxMp);
        AddBonus(bonuses, "ATK", stats.Attack);
        AddBonus(bonuses, "DEF", stats.Defense);
        AddBonus(bonuses, "MAG", stats.MagicPower);
        AddBonus(bonuses, "MDEF", stats.MagicDefense);
        AddBonus(bonuses, "SPD", stats.Speed);
        AddBonus(bonuses, "ACC", stats.Accuracy);
        AddBonus(bonuses, "EVA", stats.Evasion);
        AddBonus(bonuses, "CRIT", stats.CriticalChance);

        string bonusText = bonuses.Count == 0
            ? "No stat bonuses"
            : string.Join("  ", bonuses);
        return $"{item.DisplayName}  [{item.Rarity}]\n{bonusText}";
    }

    public static string GetUseMessage(ItemUseResult result)
    {
        if (result == null)
        {
            return "The item could not be used.";
        }

        switch (result.Status)
        {
            case ItemUseStatus.Used:
                return result.RestoredHp > 0
                    ? $"Restored {result.RestoredHp} HP."
                    : "Item used.";
            case ItemUseStatus.NoItem:
                return "That item is no longer available.";
            case ItemUseStatus.InvalidTarget:
                return "That character cannot use this item.";
            case ItemUseStatus.NoEffect:
                return "That character does not need it.";
            case ItemUseStatus.RequiresRestService:
                return "Camp Rations are used when making camp.";
            case ItemUseStatus.UnsupportedEffect:
                return "That effect is not available yet.";
            default:
                return "That item cannot be used here.";
        }
    }

    private static IReadOnlyList<ItemDefinition> GetInventoryItems(
        Inventory inventory,
        Predicate<ItemDefinition> include)
    {
        List<ItemDefinition> results = new List<ItemDefinition>();
        if (inventory == null)
        {
            return results.AsReadOnly();
        }

        foreach (ItemDefinition item in ItemCatalog.All)
        {
            if (inventory.GetQuantity(item.Id) > 0 && include(item))
            {
                results.Add(item);
            }
        }

        results.Sort((left, right) => string.Compare(
            left.DisplayName,
            right.DisplayName,
            StringComparison.Ordinal));
        return results.AsReadOnly();
    }

    private static void AddBonus(List<string> bonuses, string label, int value)
    {
        if (value > 0)
        {
            bonuses.Add($"{label} +{value}");
        }
    }
}
