using System;
using System.Collections.Generic;

public static class MusicSceneCatalog
{
    public const string MainMenuSceneName = "MainGameScene";
    public const string BattleSceneName = "BattleScene";

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
        return GetCue(sceneName, null);
    }

    public static MusicCue GetCue(
        string sceneName,
        string encounterId)
    {
        if (string.Equals(
                sceneName,
                MainMenuSceneName,
                StringComparison.Ordinal))
        {
            return MusicCue.MainMenu;
        }

        if (string.Equals(
                sceneName,
                BattleSceneName,
                StringComparison.Ordinal))
        {
            BattleEncounterDefinition encounter =
                BattleEncounterCatalog.Get(encounterId);
            if (encounter == null)
            {
                return MusicCue.None;
            }

            if (string.Equals(
                    encounter.Id,
                    BattleEncounterCatalog.GoblinBossId,
                    StringComparison.Ordinal))
            {
                return MusicCue.GoblinBoss;
            }

            return MusicCue.StandardBattle;
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
