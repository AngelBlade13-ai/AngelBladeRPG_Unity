using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0f)] private float followSpeed = 12f;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        transform.position = CalculatePosition(
            transform.position,
            target.position,
            followSpeed,
            Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public static Vector3 CalculatePosition(
        Vector3 currentPosition,
        Vector3 targetPosition,
        float speed,
        float deltaTime)
    {
        Vector3 destination = new Vector3(
            targetPosition.x,
            targetPosition.y,
            currentPosition.z);

        if (speed <= 0f)
        {
            return destination;
        }

        float followAmount = 1f - Mathf.Exp(-speed * deltaTime);
        return Vector3.Lerp(currentPosition, destination, followAmount);
    }
}
