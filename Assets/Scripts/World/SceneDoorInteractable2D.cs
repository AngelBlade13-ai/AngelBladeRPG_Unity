using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoorInteractable2D : MonoBehaviour, IWorldInteractable
{
    [SerializeField] private string destinationSceneName;
    [SerializeField] private string destinationSpawnId;

    public bool CanInteract(GameObject interactor)
    {
        return HasDestination(destinationSceneName, destinationSpawnId);
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract(interactor))
        {
            return;
        }

        WorldTransitionStore.RequestSpawn(destinationSpawnId);
        GameSaveRuntime.SaveAutosave(
            destinationSceneName,
            destinationSpawnId);
        SceneManager.LoadScene(destinationSceneName);
    }

    public void Configure(string sceneName, string spawnId)
    {
        destinationSceneName = sceneName;
        destinationSpawnId = spawnId;
    }

    public static bool HasDestination(string sceneName, string spawnId)
    {
        return !string.IsNullOrWhiteSpace(sceneName) &&
            !string.IsNullOrWhiteSpace(spawnId);
    }
}
