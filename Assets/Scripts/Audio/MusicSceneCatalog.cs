using System;
using System.Collections.Generic;

public static class MusicSceneCatalog
{
    public const string MainMenuSceneName = "MainGameScene";

    private static readonly HashSet<string> SuncrestScenes =
        new HashSet<string>(StringComparer.Ordinal)
        {
            "SuncrestAmberRowScene",
            "SuncrestGroveScene",
            "SuncrestGuildHallScene",
            "SuncrestInnScene",
            "SuncrestIronforgeScene",
            "SuncrestShrineScene",
            "SuncrestWatchScene",
            "SuncrestWhisperMarketScene"
        };

    public static MusicCue GetCue(string sceneName)
    {
        if (string.Equals(
                sceneName,
                MainMenuSceneName,
                StringComparison.Ordinal))
        {
            return MusicCue.MainMenu;
        }

        return SuncrestScenes.Contains(sceneName)
            ? MusicCue.Suncrest
            : MusicCue.None;
    }

    public static IReadOnlyCollection<string> GetSuncrestSceneNames()
    {
        return SuncrestScenes;
    }
}
