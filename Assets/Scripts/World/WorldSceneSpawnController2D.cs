using System;
using UnityEngine;

public class WorldSceneSpawnController2D : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private PlayerSpawnPoint2D defaultSpawnPoint;
    [SerializeField] private PlayerSpawnPoint2D[] additionalSpawnPoints;

    private void Start()
    {
        if (WorldTransitionStore.TryConsumeSpawn(out string spawnId) &&
            PlacePlayerAtSpawn(spawnId))
        {
            RegisterSafeLocation(spawnId);
            return;
        }

        if (PlacePlayerAtDefaultSpawn())
        {
            RegisterSafeLocation(defaultSpawnPoint.SpawnId);
        }
    }

    public void Configure(
        Transform playerTransform,
        PlayerSpawnPoint2D spawnPoint,
        PlayerSpawnPoint2D[] otherSpawnPoints = null)
    {
        player = playerTransform;
        defaultSpawnPoint = spawnPoint;
        additionalSpawnPoints = otherSpawnPoints;
    }

    public bool PlacePlayerAtDefaultSpawn()
    {
        if (player == null || defaultSpawnPoint == null)
        {
            return false;
        }

        Vector3 spawnPosition = defaultSpawnPoint.transform.position;
        player.position = new Vector3(
            spawnPosition.x,
            spawnPosition.y,
            player.position.z);

        return true;
    }

    public bool PlacePlayerAtSpawn(string spawnId)
    {
        if (string.IsNullOrWhiteSpace(spawnId))
        {
            return false;
        }

        PlayerSpawnPoint2D spawnPoint = FindSpawnPoint(spawnId);
        if (spawnPoint == null || player == null)
        {
            return false;
        }

        Vector3 spawnPosition = spawnPoint.transform.position;
        player.position = new Vector3(
            spawnPosition.x,
            spawnPosition.y,
            player.position.z);
        return true;
    }

    private PlayerSpawnPoint2D FindSpawnPoint(string spawnId)
    {
        if (MatchesSpawnId(defaultSpawnPoint, spawnId))
        {
            return defaultSpawnPoint;
        }

        if (additionalSpawnPoints == null)
        {
            return null;
        }

        foreach (PlayerSpawnPoint2D spawnPoint in additionalSpawnPoints)
        {
            if (MatchesSpawnId(spawnPoint, spawnId))
            {
                return spawnPoint;
            }
        }

        return null;
    }

    private static bool MatchesSpawnId(
        PlayerSpawnPoint2D spawnPoint,
        string spawnId)
    {
        return spawnPoint != null && string.Equals(
            spawnPoint.SpawnId,
            spawnId,
            StringComparison.Ordinal);
    }

    private void RegisterSafeLocation(string spawnId)
    {
        string sceneName = gameObject.scene.name;
        GameSaveRuntime.RememberLocation(sceneName, spawnId);
        GameSaveRuntime.SaveAutosave(sceneName, spawnId);
    }
}
