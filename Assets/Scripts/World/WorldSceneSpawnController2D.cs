using UnityEngine;

public class WorldSceneSpawnController2D : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private PlayerSpawnPoint2D defaultSpawnPoint;

    private void Start()
    {
        PlacePlayerAtDefaultSpawn();
    }

    public void Configure(
        Transform playerTransform,
        PlayerSpawnPoint2D spawnPoint)
    {
        player = playerTransform;
        defaultSpawnPoint = spawnPoint;
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
}
