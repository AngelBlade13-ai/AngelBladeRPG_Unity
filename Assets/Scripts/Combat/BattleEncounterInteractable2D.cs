using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleEncounterInteractable2D : MonoBehaviour, IWorldInteractable
{
    [Header("Scenes")]
    [SerializeField] private string battleSceneName = "BattleScene";
    [SerializeField] private string returnSpawnId = "TownAfterBattle";

    [Header("Monster")]
    [SerializeField] private string monsterName = "Goblin";
    [SerializeField, Min(1)] private int monsterHp = 35;
    [SerializeField, Min(0)] private int monsterAttack = 8;
    [SerializeField, Min(0)] private int monsterDefense = 1;
    [SerializeField, Min(0)] private int goldReward = 10;
    [SerializeField, Min(0)] private int xpReward = 15;

    public bool CanInteract(GameObject interactor)
    {
        GameSession session = GameSessionStore.Current;
        return HasEncounterConfiguration(
                battleSceneName,
                returnSpawnId,
                monsterName,
                monsterHp) &&
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

        MonsterData monster = new MonsterData(
            monsterName,
            monsterHp,
            monsterAttack,
            monsterDefense,
            goldReward,
            xpReward);
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
        string configuredMonsterName,
        int hp,
        int attack,
        int defense,
        int gold,
        int xp)
    {
        battleSceneName = sceneName;
        returnSpawnId = spawnId;
        monsterName = configuredMonsterName;
        monsterHp = hp;
        monsterAttack = attack;
        monsterDefense = defense;
        goldReward = gold;
        xpReward = xp;
    }

    public static bool HasEncounterConfiguration(
        string sceneName,
        string spawnId,
        string configuredMonsterName,
        int configuredMonsterHp)
    {
        return !string.IsNullOrWhiteSpace(sceneName) &&
            !string.IsNullOrWhiteSpace(spawnId) &&
            !string.IsNullOrWhiteSpace(configuredMonsterName) &&
            configuredMonsterHp > 0;
    }
}
