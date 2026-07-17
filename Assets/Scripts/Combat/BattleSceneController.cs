using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleSceneController : MonoBehaviour
{
    [Header("Fallback Scene")]
    [SerializeField] private string titleSceneName = "MainGameScene";

    [Header("Status UI")]
    [SerializeField] private TextMeshProUGUI playerStatusText;
    [SerializeField] private TextMeshProUGUI monsterStatusText;
    [SerializeField] private TextMeshProUGUI battleLogText;
    [SerializeField] private TextMeshProUGUI commandPromptText;
    [SerializeField] private TextMeshProUGUI continueButtonText;

    [Header("Commands")]
    [SerializeField] private GameObject attackButton;
    [SerializeField] private GameObject defendButton;
    [SerializeField] private GameObject escapeButton;
    [SerializeField] private GameObject previousTargetButton;
    [SerializeField] private GameObject nextTargetButton;
    [SerializeField] private GameObject continueButton;

    private GameSession session;
    private BattleRoundResolver legacyRoundResolver;
    private PartyBattleRoundResolver partyRoundResolver;
    private PartyCommandSelection commandSelection;

    private void Start()
    {
        session = GameSessionStore.Current;
        legacyRoundResolver = new BattleRoundResolver();
        partyRoundResolver = new PartyBattleRoundResolver();

        if (!session.HasActiveBattle || session.PartyBattle == null)
        {
            ShowUnavailableBattle();
            return;
        }

        battleLogText.text = BuildEncounterOpening(session.PartyBattle.Enemies);
        BeginCommandSelection();
    }

    public void Attack()
    {
        if (!session.HasActiveBattle)
        {
            return;
        }

        if (commandSelection == null ||
            !commandSelection.TryQueueAttack())
        {
            return;
        }

        AdvanceOrResolveRound();
    }

    public void Defend()
    {
        if (!session.HasActiveBattle)
        {
            return;
        }

        if (commandSelection == null ||
            !commandSelection.TryQueueDefend())
        {
            return;
        }

        AdvanceOrResolveRound();
    }

    public void PreviousTarget()
    {
        if (commandSelection != null && commandSelection.CycleTarget(-1))
        {
            RefreshStatus();
        }
    }

    public void NextTarget()
    {
        if (commandSelection != null && commandSelection.CycleTarget(1))
        {
            RefreshStatus();
        }
    }

    public void Escape()
    {
        if (!session.HasActiveBattle || commandSelection == null ||
            commandSelection.HasQueuedCommands ||
            session.PartyBattle.PartyMembers.Count != 1)
        {
            return;
        }

        BattleRoundResult round = legacyRoundResolver.ResolveEscapeRound(
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

    public static string FormatCombatantGroup(
        IReadOnlyList<ICombatant> combatants,
        string actingId,
        string targetId)
    {
        List<string> lines = new List<string>();
        foreach (ICombatant combatant in combatants)
        {
            string marker = combatant.CombatantId == actingId
                ? ">"
                : combatant.CombatantId == targetId ? "*" : " ";
            string state = combatant.Stats.CurrentHp > 0
                ? $"HP {combatant.Stats.CurrentHp}/{combatant.Stats.MaxHp} " +
                    $"MP {combatant.Stats.CurrentMp}/{combatant.Stats.MaxMp}"
                : "INCAPACITATED";
            lines.Add($"{marker} {combatant.DisplayName}  {state}");
        }

        return string.Join("\n", lines);
    }

    public static string FormatCommandPrompt(
        ICombatant actor,
        ICombatant target)
    {
        if (actor == null)
        {
            return string.Empty;
        }

        return target == null
            ? $"Choose {actor.DisplayName}'s command"
            : $"{actor.DisplayName} -> {target.DisplayName}";
    }

    public void Configure(
        TextMeshProUGUI playerStatus,
        TextMeshProUGUI monsterStatus,
        TextMeshProUGUI battleLog,
        TextMeshProUGUI commandPrompt,
        TextMeshProUGUI continueLabel,
        GameObject attackCommand,
        GameObject defendCommand,
        GameObject escapeCommand,
        GameObject previousTargetCommand,
        GameObject nextTargetCommand,
        GameObject continueCommand)
    {
        playerStatusText = playerStatus;
        monsterStatusText = monsterStatus;
        battleLogText = battleLog;
        commandPromptText = commandPrompt;
        continueButtonText = continueLabel;
        attackButton = attackCommand;
        defendButton = defendCommand;
        escapeButton = escapeCommand;
        previousTargetButton = previousTargetCommand;
        nextTargetButton = nextTargetCommand;
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
        escapeButton.SetActive(
            commandsAreActive && session != null &&
            session.PartyBattle != null &&
            session.PartyBattle.PartyMembers.Count == 1);
        if (previousTargetButton != null)
        {
            previousTargetButton.SetActive(commandsAreActive);
        }

        if (nextTargetButton != null)
        {
            nextTargetButton.SetActive(commandsAreActive);
        }

        if (commandPromptText != null)
        {
            commandPromptText.gameObject.SetActive(commandsAreActive);
        }

        continueButton.SetActive(false);
    }

    private void CompleteDefeat()
    {
        session.CompleteDefeat();
        battleLogText.text += "\nYou were defeated.";
        FinishBattle("Return to Title");
        RefreshStatus();
    }

    private void RefreshStatus()
    {
        if (session.PartyBattle == null)
        {
            return;
        }

        ICombatant actor = commandSelection?.CurrentActor;
        ICombatant target = commandSelection?.SelectedTarget;
        playerStatusText.text = FormatCombatantGroup(
            session.PartyBattle.PartyMembers,
            actor?.CombatantId,
            null);
        monsterStatusText.text = FormatCombatantGroup(
            session.PartyBattle.Enemies,
            null,
            target?.CombatantId);

        if (commandPromptText != null)
        {
            commandPromptText.text = FormatCommandPrompt(actor, target);
        }

        int targetCount = actor == null
            ? 0
            : session.PartyBattle.GetValidTargets(
                actor.CombatantId,
                BattleTargetType.SingleEnemy).Count;
        SetTargetButtonInteractable(previousTargetButton, targetCount > 1);
        SetTargetButtonInteractable(nextTargetButton, targetCount > 1);
    }

    private void BeginCommandSelection()
    {
        commandSelection = new PartyCommandSelection(session.PartyBattle);
        SetCommandState(true);
        RefreshStatus();
    }

    private void AdvanceOrResolveRound()
    {
        if (!commandSelection.IsComplete)
        {
            RefreshStatus();
            return;
        }

        PartyBattleRoundResult round = partyRoundResolver.ResolveRound(
            session.PartyBattle,
            commandSelection.Commands);
        string roundMessages = string.Join("\n", round.Messages);
        battleLogText.text = roundMessages;

        if (round.EnemiesWereDefeated)
        {
            CompleteVictory(roundMessages);
            return;
        }

        if (round.PartyWasDefeated)
        {
            CompleteDefeat();
            return;
        }

        BeginCommandSelection();
    }

    private static string BuildEncounterOpening(
        IReadOnlyList<ICombatant> enemies)
    {
        List<string> names = new List<string>();
        foreach (ICombatant enemy in enemies)
        {
            names.Add(enemy.DisplayName);
        }

        return names.Count == 1
            ? $"A {names[0]} appears!"
            : $"Enemies appear: {string.Join(", ", names)}!";
    }

    private static void SetTargetButtonInteractable(
        GameObject buttonObject,
        bool interactable)
    {
        if (buttonObject != null &&
            buttonObject.TryGetComponent(out Button button))
        {
            button.interactable = interactable;
        }
    }
}
