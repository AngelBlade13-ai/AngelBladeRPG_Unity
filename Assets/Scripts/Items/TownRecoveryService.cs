public enum TownRecoveryStatus
{
    Recovered,
    Cancelled,
    AlreadyRecovered,
    InsufficientGold,
    InvalidParty
}

public sealed class TownRecoveryService
{
    private readonly PlayerData player;
    private readonly PartyRoster roster;

    public TownRecoveryService(PlayerData player, PartyRoster roster)
    {
        this.player = player;
        this.roster = roster;
    }

    public TownRecoveryStatus TryPurchaseFullRecovery(
        int goldCost,
        bool confirmed)
    {
        if (!confirmed)
        {
            return TownRecoveryStatus.Cancelled;
        }

        if (player == null || roster == null || goldCost < 0)
        {
            return TownRecoveryStatus.InvalidParty;
        }

        if (!PartyRecoveryService.NeedsFullRecovery(roster))
        {
            return TownRecoveryStatus.AlreadyRecovered;
        }

        if (player.Gold < goldCost)
        {
            return TownRecoveryStatus.InsufficientGold;
        }

        player.Gold -= goldCost;
        PartyRecoveryService.FullRestore(roster);
        return TownRecoveryStatus.Recovered;
    }
}
