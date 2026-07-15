public static class BattleReturnStore
{
    private static string sceneName;
    private static string spawnId;

    public static bool HasReturnDestination =>
        !string.IsNullOrWhiteSpace(sceneName) &&
        !string.IsNullOrWhiteSpace(spawnId);

    public static bool RequestReturn(string returnSceneName, string returnSpawnId)
    {
        if (string.IsNullOrWhiteSpace(returnSceneName) ||
            string.IsNullOrWhiteSpace(returnSpawnId))
        {
            return false;
        }

        sceneName = returnSceneName.Trim();
        spawnId = returnSpawnId.Trim();
        return true;
    }

    public static bool TryConsumeReturn(
        out string returnSceneName,
        out string returnSpawnId)
    {
        returnSceneName = sceneName;
        returnSpawnId = spawnId;
        Clear();
        return !string.IsNullOrEmpty(returnSceneName) &&
            !string.IsNullOrEmpty(returnSpawnId);
    }

    public static void Clear()
    {
        sceneName = null;
        spawnId = null;
    }
}
