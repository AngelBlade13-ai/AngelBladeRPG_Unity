using UnityEngine;

public class PlayerSpawnPoint2D : MonoBehaviour
{
    [SerializeField] private string spawnId = "TownEntrance";

    public string SpawnId => spawnId;
}
