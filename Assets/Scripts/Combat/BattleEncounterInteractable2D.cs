using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleEncounterInteractable2D : MonoBehaviour, IWorldInteractable
{
    [Header("Scenes")]
    [SerializeField] private string battleSceneName = "BattleScene";
    [SerializeField] private string returnSpawnId = "TownAfterBattle";

    [Header("Encounter")]
    [SerializeField] private string encounterId;

    [Header("Legacy Single Monster")]
    [SerializeField] private string monsterId = "monster_goblin";

    public bool CanInteract(GameObject interactor)
    {
        GameSession session = GameSessionStore.Current;
        return HasEncounterConfiguration(
                battleSceneName,
                returnSpawnId,
                encounterId,
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

        GameSession session = GameSessionStore.Current;
        BattleEncounterDefinition encounter =
            BattleEncounterCatalog.Get(encounterId);
        bool started = encounter != null
            ? session.StartEncounter(encounter)
            : session.StartBattle(MonsterCatalog.Get(monsterId).CreateMonster());

        if (!started ||
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
        encounterId = string.Empty;
        monsterId = configuredMonsterId;
    }

    public void ConfigureEncounterGroup(
        string sceneName,
        string spawnId,
        string configuredEncounterId)
    {
        battleSceneName = sceneName;
        returnSpawnId = spawnId;
        encounterId = configuredEncounterId;
        monsterId = string.Empty;
    }

    public static bool HasEncounterConfiguration(
        string sceneName,
        string spawnId,
        string configuredMonsterId)
    {
        return HasEncounterConfiguration(
            sceneName,
            spawnId,
            string.Empty,
            configuredMonsterId);
    }

    public static bool HasEncounterConfiguration(
        string sceneName,
        string spawnId,
        string configuredEncounterId,
        string configuredMonsterId)
    {
        return !string.IsNullOrWhiteSpace(sceneName) &&
            !string.IsNullOrWhiteSpace(spawnId) &&
            (BattleEncounterCatalog.Get(configuredEncounterId) != null ||
                MonsterCatalog.Get(configuredMonsterId) != null);
    }
}
