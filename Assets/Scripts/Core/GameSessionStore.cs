public static class GameSessionStore
{
    private static GameSession current = new GameSession();

    public static GameSession Current => current;

    public static GameSession BeginNewSession()
    {
        current = new GameSession();
        return current;
    }

    internal static void UseRestoredSession(GameSession restoredSession)
    {
        if (restoredSession == null)
        {
            throw new System.ArgumentNullException(
                nameof(restoredSession));
        }

        current = restoredSession;
    }
}
