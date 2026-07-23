using System;
using System.Collections.Generic;

public sealed class BattleItemService
{
    private readonly Inventory inventory;

    public BattleItemService(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public static IReadOnlyList<ItemDefinition> GetUsableItems(
        Inventory inventory)
    {
        List<ItemDefinition> items = new List<ItemDefinition>();
        if (inventory == null)
        {
            return items.AsReadOnly();
        }

        foreach (ItemDefinition item in ItemCatalog.All)
        {
            if (IsUsableInBattle(item) &&
                inventory.GetQuantity(item.Id) > 0)
            {
                items.Add(item);
            }
        }

        items.Sort((left, right) => string.Compare(
            left.DisplayName,
            right.DisplayName,
            StringComparison.Ordinal));
        return items.AsReadOnly();
    }

    public static bool IsUsableInBattle(ItemDefinition item)
    {
        return item != null &&
            item.Kind == ItemKind.Consumable &&
            item.ConsumableEffect == ConsumableEffect.RestoreHp;
    }

    public static bool CanTarget(
        ItemDefinition item,
        ICombatant target)
    {
        return IsUsableInBattle(item) &&
            target is PlayableCharacterData character &&
            character.IsAvailable &&
            character.Stats.CurrentHp > 0 &&
            character.Stats.CurrentHp < character.Stats.MaxHp;
    }

    public bool CanUse(string itemId, ICombatant target)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        return inventory != null &&
            inventory.GetQuantity(itemId) > 0 &&
            CanTarget(item, target);
    }

    public CombatActionResult Use(
        ICombatant actor,
        ICombatant target,
        string itemId)
    {
        if (actor == null)
        {
            throw new ArgumentNullException(nameof(actor));
        }

        ItemDefinition item = ItemCatalog.Get(itemId);
        if (!CanUse(itemId, target))
        {
            throw new ArgumentException(
                $"{itemId} cannot be used on this battle target.",
                nameof(itemId));
        }

        PlayableCharacterData character =
            (PlayableCharacterData)target;
        ItemUseResult useResult =
            new ItemUseService(inventory).TryUse(item.Id, character);
        if (!useResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Battle item {item.Id} passed validation but was not used.");
        }

        return new CombatActionResult(
            CombatActionType.Item,
            actor.CombatantId,
            target.CombatantId,
            true,
            $"{actor.DisplayName} uses {item.DisplayName} on " +
                $"{target.DisplayName}, restoring {useResult.RestoredHp} HP.",
            healing: useResult.RestoredHp);
    }
}
