using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int LastMoveX = Animator.StringToHash("LastMoveX");
    private static readonly int LastMoveY = Animator.StringToHash("LastMoveY");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 4f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody2D body;
    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.down;

    public Vector2 Movement => movement;
    public Vector2 LastMoveDirection => lastMoveDirection;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        movement = Vector2.zero;

        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    private void Update()
    {
        Vector2 input = moveAction == null
            ? Vector2.zero
            : moveAction.action.ReadValue<Vector2>();

        movement = ClampMovementInput(input);

        if (movement.sqrMagnitude > 0f)
        {
            lastMoveDirection = movement.normalized;
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public static Vector2 ClampMovementInput(Vector2 input)
    {
        return Vector2.ClampMagnitude(input, 1f);
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat(MoveX, movement.x);
        animator.SetFloat(MoveY, movement.y);
        animator.SetFloat(LastMoveX, lastMoveDirection.x);
        animator.SetFloat(LastMoveY, lastMoveDirection.y);
        animator.SetBool(IsMoving, movement.sqrMagnitude > 0f);
    }
}
