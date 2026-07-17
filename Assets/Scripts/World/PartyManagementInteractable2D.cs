using UnityEngine;

public class PartyManagementInteractable2D : MonoBehaviour, IWorldInteractable
{
    [SerializeField] private PartyManagementPanel partyManagementPanel;

    public bool CanInteract(GameObject interactor)
    {
        return partyManagementPanel != null &&
            GameSessionStore.Current.HasPlayer;
    }

    public void Interact(GameObject interactor)
    {
        partyManagementPanel.Open(interactor);
    }

    public void Configure(PartyManagementPanel panel)
    {
        partyManagementPanel = panel;
    }
}
