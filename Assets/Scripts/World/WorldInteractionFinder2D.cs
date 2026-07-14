using UnityEngine;

public static class WorldInteractionFinder2D
{
    public static IWorldInteractable FindNearest(
        Collider2D[] candidates,
        Vector2 interactionPoint,
        GameObject interactor)
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
}
