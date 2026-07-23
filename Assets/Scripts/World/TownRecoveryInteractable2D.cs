using UnityEngine;

public sealed class TownRecoveryInteractable2D :
    MonoBehaviour,
    IWorldInteractable
{
    [SerializeField] private string serviceName = "Recovery";
    [SerializeField, Min(0)] private int goldCost = 25;
    [SerializeField] private TownRecoveryPanel panel;

    public bool CanInteract(GameObject interactor)
    {
        return panel != null && GameSessionStore.Current.HasPlayer;
    }

    public void Interact(GameObject interactor)
    {
        panel.Open(interactor, serviceName, goldCost);
    }

    public void Configure(
        string displayName,
        int cost,
        TownRecoveryPanel servicePanel)
    {
        serviceName = displayName;
        goldCost = Mathf.Max(0, cost);
        panel = servicePanel;
    }
}
