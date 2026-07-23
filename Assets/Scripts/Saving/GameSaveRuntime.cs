using System;
using UnityEngine;

public static class GameSaveRuntime
{
    private static GameSaveService service;

    public static bool HasContinue => Service.HasContinue();

    private static GameSaveService Service
    {
        get
        {
            if (service == null)
            {
                service = new GameSaveService(
                    SaveFileStorage.CreateDefault(),
                    () => Time.realtimeSinceStartupAsDouble,
                    () => DateTime.UtcNow.ToString("O"),
                    () => Application.version);
            }

            return service;
        }
    }

    public static void BeginNewGame(string appearanceId = "")
    {
        Service.BeginNewGame(appearanceId);
    }

    public static bool RememberLocation(string sceneName, string spawnId)
    {
        return Service.RememberLocation(sceneName, spawnId);
    }

    public static PlayerSaveStatus SaveManual()
    {
        return Service.SaveManual();
    }

    public static PlayerSaveStatus SaveAutosave(
        string sceneName,
        string spawnId)
    {
        return Service.SaveAutosave(sceneName, spawnId);
    }

    public static PlayerContinueStatus Continue(
        out LocationSaveData location)
    {
        return Service.Continue(out location);
    }
}
