using System;
using System.Collections.Generic;

public static class TownServiceMenuData
{
    public static IReadOnlyList<ItemDefinition> GetBuyItems(
        ShopDefinition shop)
    {
        List<ItemDefinition> items = new List<ItemDefinition>();
        if (shop == null)
        {
            return items.AsReadOnly();
        }

        foreach (string itemId in shop.StockIds)
        {
            ItemDefinition item = ItemCatalog.Get(itemId);
            if (item != null)
            {
                items.Add(item);
            }
        }

        SortItems(items);
        return items.AsReadOnly();
    }

    public static IReadOnlyList<ItemDefinition> GetSellItems(
        Inventory inventory)
    {
        List<ItemDefinition> items = new List<ItemDefinition>();
        if (inventory == null)
        {
            return items.AsReadOnly();
        }

        foreach (ItemDefinition item in ItemCatalog.All)
        {
            if (item.CanSell && inventory.GetQuantity(item.Id) > 0)
            {
                items.Add(item);
            }
        }

        SortItems(items);
        return items.AsReadOnly();
    }

    public static string FormatShopItem(
        ItemDefinition item,
        int ownedQuantity,
        int transactionQuantity,
        bool selling)
    {
        if (item == null)
        {
            return selling
                ? "No sellable items in the inventory."
                : "This shop has no stock.";
        }

        int unitPrice = selling ? item.SellPrice : item.BuyPrice;
        string action = selling ? "Sell" : "Buy";
        return
            $"{item.DisplayName}  [{item.Rarity}]\n" +
            $"Owned {ownedQuantity}   {action} {transactionQuantity}   " +
            $"Total {unitPrice * transactionQuantity} gold\n" +
            FormatItemEffect(item);
    }

    public static string GetShopMessage(ShopTransactionStatus status)
    {
        switch (status)
        {
            case ShopTransactionStatus.Completed:
                return "Transaction complete.";
            case ShopTransactionStatus.InvalidQuantity:
                return "Choose a valid quantity.";
            case ShopTransactionStatus.ItemNotStocked:
                return "That item is not sold here.";
            case ShopTransactionStatus.CannotSell:
                return "That item cannot be sold.";
            case ShopTransactionStatus.InsufficientGold:
                return "Not enough gold.";
            case ShopTransactionStatus.InsufficientItems:
                return "You do not have enough of that item.";
            case ShopTransactionStatus.InventoryFull:
                return "That item stack is full.";
            default:
                return "The transaction could not be completed.";
        }
    }

    public static string GetRecoveryMessage(TownRecoveryStatus status)
    {
        switch (status)
        {
            case TownRecoveryStatus.Recovered:
                return "The party is fully recovered.";
            case TownRecoveryStatus.Cancelled:
                return "Recovery cancelled.";
            case TownRecoveryStatus.AlreadyRecovered:
                return "The party is already fully recovered.";
            case TownRecoveryStatus.InsufficientGold:
                return "Not enough gold.";
            default:
                return "Recovery is unavailable.";
        }
    }

    private static string FormatItemEffect(ItemDefinition item)
    {
        if (item.Kind == ItemKind.Consumable)
        {
            return InventoryEquipmentMenuData.FormatItem(item, 1)
                .Split('\n')[1];
        }

        if (item.IsEquipment)
        {
            return InventoryEquipmentMenuData.FormatEquipment(item)
                .Split('\n')[1];
        }

        return item.Kind.ToString();
    }

    private static void SortItems(List<ItemDefinition> items)
    {
        items.Sort((left, right) => string.Compare(
            left.DisplayName,
            right.DisplayName,
            StringComparison.Ordinal));
    }
}
