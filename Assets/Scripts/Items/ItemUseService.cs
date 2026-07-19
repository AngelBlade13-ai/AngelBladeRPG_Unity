public enum ItemUseStatus
{
    Used,
    InvalidItem,
    NoItem,
    InvalidTarget,
    NoEffect,
    RequiresRestService,
    UnsupportedEffect
}

public sealed class ItemUseResult
{
    public ItemUseStatus Status { get; }
    public int RestoredHp { get; }
    public bool Succeeded => Status == ItemUseStatus.Used;

    public ItemUseResult(ItemUseStatus status, int restoredHp = 0)
    {
        Status = status;
        RestoredHp = restoredHp;
    }
}

public sealed class ItemUseService
{
    private readonly Inventory inventory;

    public ItemUseService(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public ItemUseResult TryUse(string itemId, PlayableCharacterData target)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        if (item == null || item.Kind != ItemKind.Consumable)
        {
            return new ItemUseResult(ItemUseStatus.InvalidItem);
        }

        if (inventory == null || inventory.GetQuantity(item.Id) < 1)
        {
            return new ItemUseResult(ItemUseStatus.NoItem);
        }

        if (target == null || !target.IsAvailable || target.IsIncapacitated)
        {
            return new ItemUseResult(ItemUseStatus.InvalidTarget);
        }

        switch (item.ConsumableEffect)
        {
            case ConsumableEffect.RestoreHp:
                int restored = target.Stats.RestoreHp(item.ConsumablePotency);
                if (restored <= 0)
                {
                    return new ItemUseResult(ItemUseStatus.NoEffect);
                }

                inventory.TryRemove(item.Id);
                return new ItemUseResult(ItemUseStatus.Used, restored);
            case ConsumableEffect.FullRest:
                return new ItemUseResult(ItemUseStatus.RequiresRestService);
            default:
                return new ItemUseResult(ItemUseStatus.UnsupportedEffect);
        }
    }
}
