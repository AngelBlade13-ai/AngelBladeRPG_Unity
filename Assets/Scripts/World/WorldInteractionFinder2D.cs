using UnityEngine;

public static class WorldInteractionFinder2D
{
    public static IWorldInteractable FindNearest(
        Collider2D[] candidates,
        Vector2 interactionPoint,
        GameObject interactor,
        Vector2 interactionOrigin,
        Vector2 facingDirection,
        float minimumFacingAlignment)
    {
        IWorldInteractable nearest = null;
        float nearestDistance = float.PositiveInfinity;

        if (candidates == null)
        {
            return null;
        }

        foreach (Collider2D candidate in candidates)
        {
            if (candidate == null)
            {
                continue;
            }

            Vector2 targetPoint = candidate.ClosestPoint(interactionOrigin);
            Vector2 directionToTarget = targetPoint - interactionOrigin;
            if (directionToTarget.sqrMagnitude <= Mathf.Epsilon)
            {
                directionToTarget =
                    (Vector2)candidate.bounds.center - interactionOrigin;
            }

            if (!IsWithinFacingDirection(
                    directionToTarget,
                    facingDirection,
                    minimumFacingAlignment))
            {
                continue;
            }

            MonoBehaviour[] behaviours =
                candidate.GetComponentsInParent<MonoBehaviour>(true);

            foreach (MonoBehaviour behaviour in behaviours)
            {
                IWorldInteractable interactable =
                    behaviour as IWorldInteractable;

                if (interactable == null || !interactable.CanInteract(interactor))
                {
                    continue;
                }

                float distance = ((Vector2)behaviour.transform.position -
                    interactionPoint).sqrMagnitude;

                if (distance < nearestDistance)
                {
                    nearest = interactable;
                    nearestDistance = distance;
                }
            }
        }

        return nearest;
    }

    public static bool IsWithinFacingDirection(
        Vector2 directionToTarget,
        Vector2 facingDirection,
        float minimumFacingAlignment)
    {
        if (directionToTarget.sqrMagnitude <= Mathf.Epsilon ||
            facingDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            return false;
        }

        float alignment = Vector2.Dot(
            directionToTarget.normalized,
            facingDirection.normalized);
        return alignment >= Mathf.Clamp01(minimumFacingAlignment);
    }
}
