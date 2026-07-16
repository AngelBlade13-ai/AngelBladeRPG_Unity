using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSceneController : MonoBehaviour
{
    [Header("Fallback Scene")]
    [SerializeField] private string titleSceneName = "MainGameScene";

    [Header("Status UI")]
    [SerializeField] private TextMeshProUGUI playerStatusText;
    [SerializeField] private TextMeshProUGUI monsterStatusText;
    [SerializeField] private TextMeshProUGUI battleLogText;
    [SerializeField] private TextMeshProUGUI continueButtonText;

    [Header("Commands")]
    [SerializeField] private GameObject attackButton;
    [SerializeField] private GameObject defendButton;
    [SerializeField] private GameObject escapeButton;
    [SerializeField] private GameObject continueButton;

    private GameSession session;
    private BattleRoundResolver roundResolver;

    private void Start()
    {
        session = GameSessionStore.Current;
        roundResolver = new BattleRoundResolver();

        if (!session.HasActiveBattle)
        {
            ShowUnavailableBattle();
            return;
        }

        battleLogText.text = $"A {session.Monster.Name} appears!";
        SetCommandState(true);
        RefreshStatus();
    }

    public void Attack()
    {
        if (!session.HasActiveBattle)
        {
            return;
        }

        PlayerData player = session.Player;
        MonsterData monster = session.Monster;
        BattleRoundResult round =
            roundResolver.ResolveAttackRound(player, monster);
        string roundMessages = string.Join("\n", round.Messages);

        if (round.MonsterWasDefeated)
        {
            CompleteVictory(roundMessages);
            return;
        }

        ShowRoundResult(round, roundMessages);
        RefreshStatus();
    }

    public void Defend()
    {
        if (!session.HasActiveBattle)
        {
            return;
        }

        BattleRoundResult round = roundResolver.ResolveDefendRound(
            session.Player,
            session.Monster);
        ShowRoundResult(round, string.Join("\n", round.Messages));
        RefreshStatus();
    }

    public void Escape()
    {
        if (!session.HasActiveBattle)
        {
            return;
        }

        BattleRoundResult round = roundResolver.ResolveEscapeRound(
            session.Player,
            session.Monster);
        battleLogText.text = string.Join("\n", round.Messages);

        if (round.EscapeSucceeded && session.CompleteEscape())
        {
            FinishBattle("Return");
        }
        else if (round.PlayerWasDefeated)
        {
            CompleteDefeat();
        }

        RefreshStatus();
    }

    public void Continue()
    {
        if (session.BattleOutcome == BattleOutcome.Defeat ||
            (session.BattleOutcome != BattleOutcome.Victory &&
                session.BattleOutcome != BattleOutcome.Escaped))
        {
            BattleReturnStore.Clear();
            SceneManager.LoadScene(titleSceneName);
            return;
        }

        if ((session.BattleOutcome == BattleOutcome.Victory ||
                session.BattleOutcome == BattleOutcome.Escaped) &&
            BattleReturnStore.TryConsumeReturn(
                out string returnSceneName,
                out string returnSpawnId))
        {
            WorldTransitionStore.RequestSpawn(returnSpawnId);
            SceneManager.LoadScene(returnSceneName);
        }
    }

    public static string FormatCombatantStatus(
        string combatantName,
        int currentHp,
        int maximumHp)
    {
        return $"{combatantName}\nHP {currentHp}/{maximumHp}";
    }

    public void Configure(
        TextMeshProUGUI playerStatus,
        TextMeshProUGUI monsterStatus,
        TextMeshProUGUI battleLog,
        TextMeshProUGUI continueLabel,
        GameObject attackCommand,
        GameObject defendCommand,
        GameObject escapeCommand,
        GameObject continueCommand)
    {
        playerStatusText = playerStatus;
        monsterStatusText = monsterStatus;
        battleLogText = battleLog;
        continueButtonText = continueLabel;
        attackButton = attackCommand;
        defendButton = defendCommand;
        escapeButton = escapeCommand;
        continueButton = continueCommand;
    }

    private void CompleteVictory(string playerMessage)
    {
        if (!session.TryCompleteVictory(out BattleRewardResult rewards))
        {
            battleLogText.text = "Rewards could not be granted.";
            return;
        }

        battleLogText.text =
            playerMessage +
            "\nVictory!" +
            $"\nGained {rewards.XP} XP and {rewards.Gold} gold.";

        if (rewards.PlayerLeveledUp)
        {
            battleLogText.text +=
                $"\n{session.Player.Name} reached level {session.Player.Level}!";
        }

        FinishBattle("Return");
        RefreshStatus();
    }

    private void ShowUnavailableBattle()
    {
        playerStatusText.text = "";
        monsterStatusText.text = "";
        battleLogText.text = "No active encounter was provided.";
        SetCommandState(false);
        continueButton.SetActive(true);
        continueButtonText.text = "Return to Title";
    }

    private void FinishBattle(string continueLabel)
    {
        SetCommandState(false);
        continueButton.SetActive(true);
        continueButtonText.text = continueLabel;
    }

    private void SetCommandState(bool commandsAreActive)
    {
        attackButton.SetActive(commandsAreActive);
        defendButton.SetActive(commandsAreActive);
        escapeButton.SetActive(commandsAreActive);
        continueButton.SetActive(false);
    }

    private void ShowRoundResult(
        BattleRoundResult round,
        string roundMessages)
    {
        battleLogText.text = roundMessages;
        if (round.PlayerWasDefeated)
        {
            CompleteDefeat();
        }
    }

    private void CompleteDefeat()
    {
        session.CompleteDefeat();
        battleLogText.text += "\nYou were defeated.";
        FinishBattle("Return to Title");
    }

    private void RefreshStatus()
    {
        playerStatusText.text = FormatCombatantStatus(
            session.Player.Name,
            session.Player.CurrentHp,
            session.Player.MaxHp);
        monsterStatusText.text = FormatCombatantStatus(
            session.Monster.Name,
            session.Monster.CurrentHp,
            session.Monster.MaxHp);
    }
}
