public static class ExplorationModalState
{
    private static object owner;

    public static bool IsOpen => owner != null;

    public static bool TryAcquire(object requestedOwner)
    {
        if (requestedOwner == null ||
            (owner != null && !object.ReferenceEquals(owner, requestedOwner)))
        {
            return false;
        }

        owner = requestedOwner;
        return true;
    }

    public static void Release(object requestedOwner)
    {
        if (object.ReferenceEquals(owner, requestedOwner))
        {
            owner = null;
        }
    }
}
