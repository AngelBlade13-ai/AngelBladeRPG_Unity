using System;
using System.Collections.Generic;

public sealed class ShopDefinition
{
    private readonly HashSet<string> stockIds;

    public string Id { get; }
    public IReadOnlyCollection<string> StockIds => stockIds;

    public ShopDefinition(string id, IEnumerable<string> stockItemIds)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("A shop ID is required.", nameof(id));
        }

        stockIds = stockItemIds == null
            ? new HashSet<string>(StringComparer.Ordinal)
            : new HashSet<string>(stockItemIds, StringComparer.Ordinal);
        Id = id.Trim();
    }

    public bool Stocks(string itemId)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        return item != null && stockIds.Contains(item.Id);
    }
}

public static class ShopCatalog
{
    public const string WhisperMarketId = "shop_whisper_market";
    public const string IronforgeSmithyId = "shop_ironforge_smithy";

    private static readonly Dictionary<string, ShopDefinition> shops =
        new Dictionary<string, ShopDefinition>(StringComparer.Ordinal)
        {
            {
                WhisperMarketId,
                new ShopDefinition(
                    WhisperMarketId,
                    new[]
                    {
                        ItemCatalog.MinorPotionId,
                        ItemCatalog.FieldRemedyId,
                        ItemCatalog.CampRationId
                    })
            },
            {
                IronforgeSmithyId,
                new ShopDefinition(
                    IronforgeSmithyId,
                    new[]
                    {
                        ItemCatalog.IronHeavyBladeId,
                        ItemCatalog.AshBowId,
                        ItemCatalog.IronDaggerId,
                        ItemCatalog.OakStaffId,
                        ItemCatalog.TravelersLuteId,
                        ItemCatalog.ApprenticeTomeId,
                        ItemCatalog.CarvedHornId,
                        ItemCatalog.PaddedArmorId
                    })
            }
        };

    public static ShopDefinition Get(string shopId)
    {
        if (string.IsNullOrWhiteSpace(shopId))
        {
            return null;
        }

        return shops.TryGetValue(shopId.Trim(), out ShopDefinition shop)
            ? shop
            : null;
    }
}

public enum ShopTransactionStatus
{
    Completed,
    InvalidQuantity,
    ItemNotStocked,
    CannotSell,
    InsufficientGold,
    InsufficientItems,
    InventoryFull
}

public sealed class ShopService
{
    private readonly ShopDefinition shop;
    private readonly PlayerData player;
    private readonly Inventory inventory;

    public ShopService(
        ShopDefinition shop,
        PlayerData player,
        Inventory inventory)
    {
        this.shop = shop;
        this.player = player;
        this.inventory = inventory;
    }

    public ShopTransactionStatus TryBuy(string itemId, int quantity)
    {
        if (quantity <= 0)
        {
            return ShopTransactionStatus.InvalidQuantity;
        }

        ItemDefinition item = ItemCatalog.Get(itemId);
        if (shop == null || player == null || inventory == null ||
            item == null || !shop.Stocks(item.Id))
        {
            return ShopTransactionStatus.ItemNotStocked;
        }

        long total = (long)item.BuyPrice * quantity;
        if (total > player.Gold)
        {
            return ShopTransactionStatus.InsufficientGold;
        }

        if (!inventory.CanAdd(item.Id, quantity))
        {
            return ShopTransactionStatus.InventoryFull;
        }

        inventory.TryAdd(item.Id, quantity);
        player.Gold -= (int)total;
        return ShopTransactionStatus.Completed;
    }

    public ShopTransactionStatus TrySell(string itemId, int quantity)
    {
        if (quantity <= 0)
        {
            return ShopTransactionStatus.InvalidQuantity;
        }

        ItemDefinition item = ItemCatalog.Get(itemId);
        if (item == null || !item.CanSell)
        {
            return ShopTransactionStatus.CannotSell;
        }

        if (player == null || inventory == null ||
            inventory.GetQuantity(item.Id) < quantity)
        {
            return ShopTransactionStatus.InsufficientItems;
        }

        inventory.TryRemove(item.Id, quantity);
        player.Gold += item.SellPrice * quantity;
        return ShopTransactionStatus.Completed;
    }
}
