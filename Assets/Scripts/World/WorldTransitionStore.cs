public static class WorldTransitionStore
{
    private static string pendingSpawnId;

    public static bool HasPendingSpawn => !string.IsNullOrWhiteSpace(pendingSpawnId);

    public static bool RequestSpawn(string spawnId)
    {
        if (string.IsNullOrWhiteSpace(spawnId))
        {
            return false;
        }

        pendingSpawnId = spawnId.Trim();
        return true;
    }

    public static bool TryConsumeSpawn(out string spawnId)
    {
        spawnId = pendingSpawnId;
        pendingSpawnId = null;
        return !string.IsNullOrEmpty(spawnId);
    }

    public static void Clear()
    {
        pendingSpawnId = null;
    }
}
