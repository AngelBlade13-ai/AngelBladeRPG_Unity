using UnityEditor;
using UnityEngine;

public static class PlayModeTestUtilities
{
    private const string GrantGoldPath =
        "Tools/AngelBlade RPG/Testing/Grant 1000 Test Gold";

    [MenuItem(GrantGoldPath)]
    public static void GrantTestGold()
    {
        GameSession session = GameSessionStore.Current;
        if (!Application.isPlaying || session == null || !session.HasPlayer)
        {
            EditorUtility.DisplayDialog(
                "Play Mode Test Utilities",
                "Start a game in Play Mode before granting test gold.",
                "OK");
            return;
        }

        session.Player.Gold += 1000;
        Object.FindAnyObjectByType<ExplorationStatusHUD>()?.Refresh();
        Debug.Log(
            $"Granted 1000 temporary test gold. " +
            $"Current gold: {session.Player.Gold}.");
    }

    [MenuItem(GrantGoldPath, true)]
    private static bool CanGrantTestGold()
    {
        GameSession session = GameSessionStore.Current;
        return Application.isPlaying && session != null && session.HasPlayer;
    }
}
