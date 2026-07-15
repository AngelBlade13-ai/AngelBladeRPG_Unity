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
    [SerializeField, Range(0f, 1f)] private float minimumFacingAlignment = 0.5f;
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
        Vector2 interactionOriginPosition = GetInteractionOrigin();
        Vector2 facingDirection = GetFacingDirection();
        Vector2 interactionPoint = GetInteractionPoint();
        Collider2D[] candidates = Physics2D.OverlapCircleAll(
            interactionPoint,
            interactionRadius,
            interactionLayers);
        IWorldInteractable interactable = WorldInteractionFinder2D.FindNearest(
            candidates,
            interactionPoint,
            gameObject,
            interactionOriginPosition,
            facingDirection,
            minimumFacingAlignment);

        if (interactable == null)
        {
            return false;
        }

        interactable.Interact(gameObject);
        return true;
    }

    public Vector2 GetInteractionPoint()
    {
        return GetInteractionOrigin() +
            GetFacingDirection() * interactionDistance;
    }

    public static Vector2 GetCardinalFacingDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            return Vector2.down;
        }

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0f ? Vector2.right : Vector2.left;
        }

        return direction.y > 0f ? Vector2.up : Vector2.down;
    }

    private Vector2 GetInteractionOrigin()
    {
        return interactionOrigin == null
            ? transform.position
            : interactionOrigin.position;
    }

    private Vector2 GetFacingDirection()
    {
        Vector2 lastDirection = movement == null
            ? Vector2.down
            : movement.LastMoveDirection;
        return GetCardinalFacingDirection(lastDirection);
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
