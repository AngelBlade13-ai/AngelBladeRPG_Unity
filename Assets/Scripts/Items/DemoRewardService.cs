using System;
using System.Collections.Generic;

public enum DemoRewardStatus
{
    Granted,
    UnknownReward,
    AlreadyGranted,
    InventoryFull,
    InvalidState
}

public sealed class DemoRewardClaimState
{
    private readonly HashSet<string> claimedRewardIds =
        new HashSet<string>(StringComparer.Ordinal);

    public bool IsClaimed(string rewardId)
    {
        return !string.IsNullOrWhiteSpace(rewardId) &&
            claimedRewardIds.Contains(rewardId.Trim());
    }

    internal void Record(string rewardId)
    {
        claimedRewardIds.Add(rewardId);
    }
}

public sealed class DemoRewardGrantResult
{
    public int Gold { get; }
    public IReadOnlyList<ItemGrant> Items { get; }

    public DemoRewardGrantResult(
        int gold,
        IReadOnlyList<ItemGrant> items)
    {
        Gold = gold;
        Items = items ?? Array.Empty<ItemGrant>();
    }
}

public static class DemoRewardService
{
    public static DemoRewardStatus TryGrant(
        string rewardId,
        PlayerData player,
        Inventory inventory,
        DemoRewardClaimState claimState,
        out DemoRewardGrantResult result)
    {
        result = null;
        DemoRewardDefinition reward =
            DemoEconomyCatalog.GetReward(rewardId);
        if (reward == null)
        {
            return DemoRewardStatus.UnknownReward;
        }

        if (player == null || inventory == null || claimState == null)
        {
            return DemoRewardStatus.InvalidState;
        }

        if (claimState.IsClaimed(reward.Id))
        {
            return DemoRewardStatus.AlreadyGranted;
        }

        foreach (ItemGrant item in reward.Items)
        {
            if (!inventory.CanAdd(item.ItemId, item.Quantity))
            {
                return DemoRewardStatus.InventoryFull;
            }
        }

        foreach (ItemGrant item in reward.Items)
        {
            inventory.TryAdd(item.ItemId, item.Quantity);
        }

        player.Gold += reward.Gold;
        claimState.Record(reward.Id);
        result = new DemoRewardGrantResult(reward.Gold, reward.Items);
        return DemoRewardStatus.Granted;
    }
}
