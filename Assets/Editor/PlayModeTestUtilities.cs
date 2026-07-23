using UnityEditor;
using UnityEngine;

public static class PlayModeTestUtilities
{
    private const string GrantGoldPath =
        "Tools/AngelBlade RPG/Testing/Grant 1000 Test Gold";
    private const string MusicVolumeQuarterPath =
        "Tools/AngelBlade RPG/Testing/Set Music Volume To 25 Percent";
    private const string MusicVolumeFullPath =
        "Tools/AngelBlade RPG/Testing/Set Music Volume To 100 Percent";

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

    [MenuItem(MusicVolumeQuarterPath)]
    public static void SetMusicVolumeToQuarter()
    {
        SetMusicVolume(0.25f);
    }

    [MenuItem(MusicVolumeQuarterPath, true)]
    private static bool CanSetMusicVolumeToQuarter()
    {
        return Application.isPlaying;
    }

    [MenuItem(MusicVolumeFullPath)]
    public static void SetMusicVolumeToFull()
    {
        SetMusicVolume(1f);
    }

    [MenuItem(MusicVolumeFullPath, true)]
    private static bool CanSetMusicVolumeToFull()
    {
        return Application.isPlaying;
    }

    private static void SetMusicVolume(float volume)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        GameAudioSettingsRuntime.SetMusicVolume(volume);
        Debug.Log($"Music volume set to {volume:P0}.");
    }
}
