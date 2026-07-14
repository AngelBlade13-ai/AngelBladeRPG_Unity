using UnityEngine;

public interface IWorldInteractable
{
    bool CanInteract(GameObject interactor);

    void Interact(GameObject interactor);
}
