using UnityEngine;

public class PlaceholderDirectionVisual2D : MonoBehaviour
{
    [SerializeField] private PlayerMovement2D movement;
    [SerializeField] private Transform visual;
    [SerializeField, Min(0f)] private float bobHeight = 0.05f;
    [SerializeField, Min(0f)] private float bobSpeed = 8f;

    private Vector3 restingLocalPosition;

    private void Awake()
    {
        if (visual != null)
        {
            restingLocalPosition = visual.localPosition;
        }
    }

    private void LateUpdate()
    {
        if (movement == null || visual == null)
        {
            return;
        }

        visual.localRotation = Quaternion.Euler(
            0f,
            0f,
            GetFacingAngle(movement.LastMoveDirection));

        float bobOffset = movement.Movement.sqrMagnitude > 0f
            ? Mathf.Abs(Mathf.Sin(Time.time * bobSpeed)) * bobHeight
            : 0f;

        visual.localPosition = restingLocalPosition + Vector3.up * bobOffset;
    }

    public static float GetFacingAngle(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0f ? -90f : 90f;
        }

        return direction.y >= 0f ? 0f : 180f;
    }
}
