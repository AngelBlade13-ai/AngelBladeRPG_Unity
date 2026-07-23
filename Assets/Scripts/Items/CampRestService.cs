public enum CampRestStatus
{
    Rested,
    Cancelled,
    AlreadyRecovered,
    MissingRation,
    InvalidParty
}

public sealed class CampRestState
{
    public bool TutorialRestUsed { get; private set; }
    public int CompletedRestCount { get; private set; }

    internal void RecordRest(bool wasTutorialRest)
    {
        if (wasTutorialRest)
        {
            TutorialRestUsed = true;
        }

        CompletedRestCount += 1;
    }

    internal bool TryRestore(bool tutorialRestUsed, int completedRestCount)
    {
        if (completedRestCount < 0 ||
            (tutorialRestUsed && completedRestCount == 0))
        {
            return false;
        }

        TutorialRestUsed = tutorialRestUsed;
        CompletedRestCount = completedRestCount;
        return true;
    }
}

public sealed class CampRestResult
{
    public CampRestStatus Status { get; }
    public bool ConsumedRation { get; }
    public int RestoredCharacterCount { get; }
    public bool Succeeded => Status == CampRestStatus.Rested;

    public CampRestResult(
        CampRestStatus status,
        bool consumedRation = false,
        int restoredCharacterCount = 0)
    {
        Status = status;
        ConsumedRation = consumedRation;
        RestoredCharacterCount = restoredCharacterCount;
    }
}

public sealed class CampRestService
{
    private readonly PartyRoster roster;
    private readonly Inventory inventory;
    private readonly CampRestState state;

    public CampRestService(
        PartyRoster roster,
        Inventory inventory,
        CampRestState state)
    {
        this.roster = roster;
        this.inventory = inventory;
        this.state = state;
    }

    public CampRestResult TryFullRest(
        bool confirmed,
        bool allowFreeTutorialRest)
    {
        if (!confirmed)
        {
            return new CampRestResult(CampRestStatus.Cancelled);
        }

        if (roster == null || inventory == null || state == null)
        {
            return new CampRestResult(CampRestStatus.InvalidParty);
        }

        if (!PartyRecoveryService.NeedsFullRecovery(roster))
        {
            return new CampRestResult(CampRestStatus.AlreadyRecovered);
        }

        bool freeTutorialRest =
            allowFreeTutorialRest && !state.TutorialRestUsed;
        if (!freeTutorialRest &&
            inventory.GetQuantity(ItemCatalog.CampRationId) < 1)
        {
            return new CampRestResult(CampRestStatus.MissingRation);
        }

        if (!freeTutorialRest)
        {
            inventory.TryRemove(ItemCatalog.CampRationId);
        }

        int restored = PartyRecoveryService.FullRestore(roster);
        state.RecordRest(freeTutorialRest);
        return new CampRestResult(
            CampRestStatus.Rested,
            !freeTutorialRest,
            restored);
    }
}
