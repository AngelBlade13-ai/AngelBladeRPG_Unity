using System;
using System.Collections.Generic;

public sealed class Inventory
{
    private readonly Dictionary<string, int> quantities =
        new Dictionary<string, int>(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, int> Quantities => quantities;

    public int GetQuantity(string itemId)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        return item != null && quantities.TryGetValue(item.Id, out int quantity)
            ? quantity
            : 0;
    }

    public bool CanAdd(string itemId, int amount = 1)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        if (item == null || amount <= 0)
        {
            return false;
        }

        return GetQuantity(item.Id) <= item.MaximumStack - amount;
    }

    public bool TryAdd(string itemId, int amount = 1)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        if (item == null || !CanAdd(item.Id, amount))
        {
            return false;
        }

        quantities[item.Id] = GetQuantity(item.Id) + amount;
        return true;
    }

    public bool TryRemove(string itemId, int amount = 1)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        int current = GetQuantity(itemId);
        if (item == null || amount <= 0 || current < amount)
        {
            return false;
        }

        int remaining = current - amount;
        if (remaining == 0)
        {
            quantities.Remove(item.Id);
        }
        else
        {
            quantities[item.Id] = remaining;
        }

        return true;
    }

    public bool TryDiscard(string itemId, int amount = 1)
    {
        ItemDefinition item = ItemCatalog.Get(itemId);
        return item != null && item.CanDiscard && TryRemove(item.Id, amount);
    }

    internal bool TryRestore(IEnumerable<KeyValuePair<string, int>> entries)
    {
        if (entries == null)
        {
            return false;
        }

        Dictionary<string, int> restored =
            new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (KeyValuePair<string, int> entry in entries)
        {
            ItemDefinition item = ItemCatalog.Get(entry.Key);
            if (item == null || entry.Value <= 0 ||
                entry.Value > item.MaximumStack ||
                restored.ContainsKey(item.Id))
            {
                return false;
            }

            restored.Add(item.Id, entry.Value);
        }

        quantities.Clear();
        foreach (KeyValuePair<string, int> entry in restored)
        {
            quantities.Add(entry.Key, entry.Value);
        }

        return true;
    }
}
