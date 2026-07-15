using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleEncounterInteractable2D : MonoBehaviour, IWorldInteractable
{
    [Header("Scenes")]
    [SerializeField] private string battleSceneName = "BattleScene";
    [SerializeField] private string returnSpawnId = "TownAfterBattle";

    [Header("Monster")]
    [SerializeField] private string monsterId = "monster_goblin";

    public bool CanInteract(GameObject interactor)
    {
        GameSession session = GameSessionStore.Current;
        return HasEncounterConfiguration(
                battleSceneName,
                returnSpawnId,
                monsterId) &&
            session.HasPlayer &&
            session.Player.CurrentHp > 0 &&
            !session.HasActiveBattle;
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract(interactor))
        {
            return;
        }

        MonsterData monster = MonsterCatalog.Get(monsterId).CreateMonster();
        GameSession session = GameSessionStore.Current;

        if (!session.StartBattle(monster) ||
            !BattleReturnStore.RequestReturn(
                gameObject.scene.name,
                returnSpawnId))
        {
            return;
        }

        SceneManager.LoadScene(battleSceneName);
    }

    public void Configure(
        string sceneName,
        string spawnId,
        string configuredMonsterId)
    {
        battleSceneName = sceneName;
        returnSpawnId = spawnId;
        monsterId = configuredMonsterId;
    }

    public static bool HasEncounterConfiguration(
        string sceneName,
        string spawnId,
        string configuredMonsterId)
    {
        return !string.IsNullOrWhiteSpace(sceneName) &&
            !string.IsNullOrWhiteSpace(spawnId) &&
            MonsterCatalog.Get(configuredMonsterId) != null;
    }
}
