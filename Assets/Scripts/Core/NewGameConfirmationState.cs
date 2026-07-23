public sealed class NewGameConfirmationState
{
    private bool confirmationRequired;
    private bool confirmationAcknowledged;

    public bool ConfirmationRequired => confirmationRequired;
    public bool ConfirmationAcknowledged => confirmationAcknowledged;

    public void Begin(bool existingSaveFound)
    {
        confirmationRequired = existingSaveFound;
        confirmationAcknowledged = false;
    }

    public bool TryConfirm()
    {
        if (!confirmationRequired || confirmationAcknowledged)
        {
            return true;
        }

        confirmationAcknowledged = true;
        return false;
    }

    public void Reset()
    {
        confirmationRequired = false;
        confirmationAcknowledged = false;
    }
}
