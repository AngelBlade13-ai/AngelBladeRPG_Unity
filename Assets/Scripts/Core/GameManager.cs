using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject titlePanel;
    public GameObject townPanel;
    public GameObject battlePanel;

    [Header("Battle UI")]
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI battleLogText;

    private PlayerData player;
    private MonsterData monster;
    private SimpleBattleSystem battleSystem;

    private void Start()
    {
        battleSystem = new SimpleBattleSystem();

        ShowTitlePanel();
    }

    public void StartNewGame()
    {
        player = new PlayerData("Hero");

        ShowTownPanel();
    }

    public void StartFight()
    {
        monster = new MonsterData("Goblin", 35, 8, 1);

        ShowBattlePanel();

        battleLogText.text = "A Goblin appears!";
        UpdateBattleUI();
    }

    public void Attack()
    {
        if (player == null || monster == null)
        {
            battleLogText.text = "Battle has not started.";
            return;
        }

        string playerMessage = battleSystem.PlayerAttack(player, monster);

        if (monster.CurrentHp <= 0)
        {
            battleLogText.text = playerMessage + "\nYou won!";
            UpdateBattleUI();
            return;
        }

        string monsterMessage = battleSystem.MonsterAttack(player, monster);

        battleLogText.text = playerMessage + "\n" + monsterMessage;

        if (player.CurrentHp <= 0)
        {
            battleLogText.text += "\nYou were defeated.";
        }

        UpdateBattleUI();
    }

    public void ReturnToTown()
    {
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
    }

    private void ShowBattlePanel()
    {
        titlePanel.SetActive(false);
        townPanel.SetActive(false);
        battlePanel.SetActive(true);
    }

    private void UpdateBattleUI()
    {
        playerText.text = $"{player.Name}\nHP: {player.CurrentHp}/{player.MaxHp}";
        enemyText.text = $"{monster.Name}\nHP: {monster.CurrentHp}/{monster.MaxHp}";
    }
}