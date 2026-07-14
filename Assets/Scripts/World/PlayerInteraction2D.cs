using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement2D))]
public class PlayerInteraction2D : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Detection")]
    [SerializeField] private Transform interactionOrigin;
    [SerializeField, Min(0f)] private float interactionDistance = 0.65f;
    [SerializeField, Min(0.01f)] private float interactionRadius = 0.35f;
    [SerializeField] private LayerMask interactionLayers = ~0;

    private PlayerMovement2D movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement2D>();
    }

    private void OnEnable()
    {
        if (interactAction == null)
        {
            return;
        }

        interactAction.action.performed += HandleInteract;
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction == null)
        {
            return;
        }

        interactAction.action.performed -= HandleInteract;
        interactAction.action.Disable();
    }

    public bool TryInteract()
    {
        Vector2 interactionPoint = GetInteractionPoint();
        Collider2D[] candidates = Physics2D.OverlapCircleAll(
            interactionPoint,
            interactionRadius,
            interactionLayers);
        IWorldInteractable interactable = WorldInteractionFinder2D.FindNearest(
            candidates,
            interactionPoint,
            gameObject);

        if (interactable == null)
        {
            return false;
        }

        interactable.Interact(gameObject);
        return true;
    }

    public Vector2 GetInteractionPoint()
    {
        Vector2 origin = interactionOrigin == null
            ? transform.position
            : interactionOrigin.position;
        Vector2 facing = movement == null
            ? Vector2.down
            : movement.LastMoveDirection;

        return origin + facing.normalized * interactionDistance;
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        TryInteract();
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            movement = GetComponent<PlayerMovement2D>();
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetInteractionPoint(), interactionRadius);
    }
}
