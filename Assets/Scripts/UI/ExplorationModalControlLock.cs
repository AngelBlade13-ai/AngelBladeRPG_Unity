using UnityEngine;

public sealed class ExplorationModalControlLock
{
    private PlayerMovement2D movement;
    private PlayerInteraction2D interaction;
    private bool restoreMovement;
    private bool restoreInteraction;

    public void Capture(GameObject interactor)
    {
        Restore();
        if (interactor == null)
        {
            return;
        }

        movement = interactor.GetComponent<PlayerMovement2D>();
        interaction = interactor.GetComponent<PlayerInteraction2D>();
        restoreMovement = movement != null && movement.enabled;
        restoreInteraction = interaction != null && interaction.enabled;

        if (movement != null)
        {
            movement.enabled = false;
        }

        if (interaction != null)
        {
            interaction.enabled = false;
        }
    }

    public void Restore()
    {
        if (movement != null)
        {
            movement.enabled = restoreMovement;
        }

        if (interaction != null)
        {
            interaction.enabled = restoreInteraction;
        }

        movement = null;
        interaction = null;
        restoreMovement = false;
        restoreInteraction = false;
    }
}
