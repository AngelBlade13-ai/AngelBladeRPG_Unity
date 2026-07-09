using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject titlePanel;
    public GameObject townPanel;
    public GameObject battlePanel;

    [Header("Town UI")]
    public TextMeshProUGUI townStatusText;

    [Header("Battle UI")]
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI battleLogText;

    private GameSession gameSession;
    private SimpleBattleSystem battleSystem;

    private void Start()
    {
        gameSession = new GameSession();
        battleSystem = new SimpleBattleSystem();

        ShowTitlePanel();
    }

    public void StartNewGame()
    {
        gameSession.StartNewGame("Hero");

        ShowTownPanel();
    }

    public void StartFight()
    {
        MonsterData goblin = new MonsterData("Goblin", 35, 8, 1, 10, 15);

        if (!gameSession.StartBattle(goblin))
        {
            return;
        }

        ShowBattlePanel();

        battleLogText.text = "A Goblin appears!";
        UpdateBattleUI();
    }

    public void Attack()
    {
        if (gameSession.Monster != null && gameSession.BattleIsOver)
        {
            battleLogText.text += "\nThe battle is already over.";
            return;
        }

        if (!gameSession.HasActiveBattle)
        {
            battleLogText.text = "Battle has not started.";
            return;
        }

        PlayerData player = gameSession.Player;
        MonsterData monster = gameSession.Monster;
        string playerMessage = battleSystem.PlayerAttack(player, monster);

        if (monster.CurrentHp <= 0)
        {
            if (!gameSession.TryCompleteVictory(out BattleRewardResult rewards))
            {
                battleLogText.text = "Rewards could not be granted.";
                return;
            }

            battleLogText.text =
                playerMessage +
                "\nYou won!" +
                $"\nGained {rewards.XP} XP and {rewards.Gold} gold.";

            if (rewards.PlayerLeveledUp)
            {
                battleLogText.text += $"\n{player.Name} leveled up to level {player.Level}!";
            }

            UpdateBattleUI();
            return;
        }

        string monsterMessage = battleSystem.MonsterAttack(player, monster);

        battleLogText.text = playerMessage + "\n" + monsterMessage;

        if (player.CurrentHp <= 0)
        {
            gameSession.CompleteDefeat();
            battleLogText.text += "\nYou were defeated.";
        }

        UpdateBattleUI();
    }

    public void ReturnToTown()
    {
        if (!gameSession.HasPlayer || !gameSession.BattleIsOver)
        {
            return;
        }

        ShowTownPanel();
    }

    private void ShowTitlePanel()
    {
        titlePanel.SetActive(true);
        townPanel.SetActive(false);
        battlePanel.SetActive(false);
    }

    private void ShowTownPanel()
    {
        titlePanel.SetActive(false);
        townPanel.SetActive(true);
        battlePanel.SetActive(false);

        UpdateTownUI();
    }

    private void ShowBattlePanel()
    {
        titlePanel.SetActive(false);
        townPanel.SetActive(false);
        battlePanel.SetActive(true);
    }

    private void UpdateTownUI()
    {
        if (!gameSession.HasPlayer)
        {
            townStatusText.text = "";
            return;
        }

        PlayerData player = gameSession.Player;
        townStatusText.text =
            $"{player.Name}\n" +
            $"Level: {player.Level}\n" +
            $"HP: {player.CurrentHp}/{player.MaxHp}\n" +
            $"XP: {player.XP}/{player.XPToNextLevel}\n" +
            $"Gold: {player.Gold}";
    }

    private void UpdateBattleUI()
    {
        if (!gameSession.HasPlayer || gameSession.Monster == null)
        {
            playerText.text = "";
            enemyText.text = "";
            return;
        }

        PlayerData player = gameSession.Player;
        MonsterData monster = gameSession.Monster;
        playerText.text = $"{player.Name}\nHP: {player.CurrentHp}/{player.MaxHp}";
        enemyText.text = $"{monster.Name}\nHP: {monster.CurrentHp}/{monster.MaxHp}";
    }
}
