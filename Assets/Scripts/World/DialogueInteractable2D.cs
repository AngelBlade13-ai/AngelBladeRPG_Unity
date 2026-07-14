using UnityEngine;

public class DialogueInteractable2D : MonoBehaviour, IWorldInteractable
{
    [SerializeField, TextArea(2, 4)] private string message =
        "A weathered sign.";
    [SerializeField] private SimpleDialoguePanel2D dialogue;

    public bool CanInteract(GameObject interactor)
    {
        return dialogue != null;
    }

    public void Interact(GameObject interactor)
    {
        dialogue.Toggle(message, transform, interactor.transform);
    }

    public void Configure(string interactionMessage, SimpleDialoguePanel2D panel)
    {
        message = interactionMessage;
        dialogue = panel;
    }
}
