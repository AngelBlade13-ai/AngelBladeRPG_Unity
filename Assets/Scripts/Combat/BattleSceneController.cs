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
    [SerializeField] private GameObject abilityButton;
    [SerializeField] private GameObject defendButton;
    [SerializeField] private GameObject escapeButton;
    [SerializeField] private GameObject previousTargetButton;
    [SerializeField] private GameObject nextTargetButton;
    [SerializeField] private GameObject continueButton;

    [Header("Action Pacing")]
    [SerializeField, Min(0f)] private float actionPresentationSeconds = 0.65f;

    private GameSession session;
    private ActionGaugeBattle actionGauges;
    private PartyBattleActionResolver actionResolver;
    private PartyCommandSelection commandSelection;
    private float actionPresentationRemaining;

    private void Start()
    {
        session = GameSessionStore.Current;

        if (!session.HasActiveBattle || session.PartyBattle == null)
        {
            ShowUnavailableBattle();
            return;
        }

        actionGauges = session.CreateActionGaugeBattle();
        actionResolver = session.CreatePartyActionResolver();
        ArrangeFormationPlaceholders();
        battleLogText.text = BuildEncounterOpening(session.PartyBattle.Enemies);
        SetCommandState(false);
        RefreshStatus();
    }

    private void Update()
    {
        if (session == null || !session.HasActiveBattle ||
            actionGauges == null || actionResolver == null)
        {
            return;
        }

        if (actionPresentationRemaining > 0f)
        {
            actionPresentationRemaining = Mathf.Max(
                0f,
                actionPresentationRemaining - Time.deltaTime);
            RefreshStatus();
            return;
        }

        bool menuIsOpen = commandSelection != null &&
            !commandSelection.IsComplete;
        actionGauges.Tick(Time.deltaTime, menuIsOpen);

        if (menuIsOpen)
        {
            ICombatant currentActor = commandSelection.CurrentActor;
            if (currentActor == null || currentActor.Stats.CurrentHp <= 0)
            {
                commandSelection = null;
                SetCommandState(false);
            }
            else if (actionGauges.TimingMode == BattleTimingMode.Active)
            {
                ICombatant readyEnemy = actionGauges.GetNextReadyEnemy();
                if (readyEnemy != null)
                {
                    ResolveEnemyAction(readyEnemy);
                }
            }

            RefreshStatus();
            return;
        }

        ICombatant readyActor = actionGauges.GetNextReadyCombatant();
        if (readyActor == null)
        {
            RefreshStatus();
            return;
        }

        if (IsInGroup(session.PartyBattle.PartyMembers, readyActor))
        {
            BeginCommandSelection(readyActor);
            return;
        }

        ResolveEnemyAction(readyActor);
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

        ResolveSelectedCommand();
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

        ResolveSelectedCommand();
    }

    public void Ability()
    {
        if (!session.HasActiveBattle || commandSelection == null)
        {
            return;
        }

        if (!commandSelection.IsChoosingAbility)
        {
            if (commandSelection.TryBeginCoreAbility())
            {
                RefreshStatus();
            }

            return;
        }

        if (commandSelection.TryQueuePendingAbility())
        {
            ResolveSelectedCommand();
        }
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
            commandSelection.IsChoosingAbility ||
            session.PartyBattle.PartyMembers.Count != 1 ||
            session.PartyBattle.Enemies.Count != 1 ||
            !session.EscapeAllowed)
        {
            return;
        }

        ICombatant actor = commandSelection.CurrentActor;
        CombatActionResult result = actionResolver.ResolveEscape(
            session.PartyBattle,
            actor);
        actionGauges.ConsumeTurn(actor.CombatantId);
        commandSelection = null;
        battleLogText.text = result.Message;

        if (result.Succeeded && session.CompleteEscape())
        {
            FinishBattle("Return");
        }
        else
        {
            SetCommandState(false);
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
            bool isActor = combatant.CombatantId == actingId;
            bool isTarget = combatant.CombatantId == targetId;
            string marker = isActor && isTarget
                ? ">*"
                : isActor ? ">" : isTarget ? "*" : " ";
            string state = combatant.Stats.CurrentHp > 0
                ? $"HP {combatant.Stats.CurrentHp}/{combatant.Stats.MaxHp} " +
                    $"MP {combatant.Stats.CurrentMp}/{combatant.Stats.MaxMp}"
                : "INCAPACITATED";
            lines.Add($"{marker} {combatant.DisplayName}  {state}");
        }

        return string.Join("\n", lines);
    }

    public static string FormatCombatantGroupWithGauges(
        IReadOnlyList<ICombatant> combatants,
        string actingId,
        string targetId,
        ActionGaugeBattle gauges)
    {
        if (gauges == null)
        {
            return FormatCombatantGroup(combatants, actingId, targetId);
        }

        List<string> lines = new List<string>();
        foreach (ICombatant combatant in combatants)
        {
            bool isActor = combatant.CombatantId == actingId;
            bool isTarget = combatant.CombatantId == targetId;
            string marker = isActor && isTarget
                ? ">*"
                : isActor ? ">" : isTarget ? "*" : " ";
            string state = combatant.Stats.CurrentHp > 0
                ? $"HP {combatant.Stats.CurrentHp}/{combatant.Stats.MaxHp} " +
                    $"MP {combatant.Stats.CurrentMp}/{combatant.Stats.MaxMp} " +
                    $"AT {gauges.GetGaugePercent(combatant.CombatantId)}%"
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

    public static string FormatAbilityPrompt(
        ICombatant actor,
        ICombatant target,
        CombatAbilityDefinition ability)
    {
        if (actor == null || ability == null)
        {
            return string.Empty;
        }

        string costType = ability.CostType == CombatAbilityCostType.Mp
            ? "MP"
            : "HP";
        string action =
            $"{ability.DisplayName} ({ability.ResourceCost} {costType})";
        return target == null
            ? $"{action}: choose a target"
            : $"{action}: {actor.DisplayName} -> " +
                $"{target.DisplayName}";
    }

    public void Configure(
        TextMeshProUGUI playerStatus,
        TextMeshProUGUI monsterStatus,
        TextMeshProUGUI battleLog,
        TextMeshProUGUI commandPrompt,
        TextMeshProUGUI continueLabel,
        GameObject attackCommand,
        GameObject abilityCommand,
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
        abilityButton = abilityCommand;
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
            $"\nActive party gained {rewards.XP} XP each." +
            $"\nRoster gained {rewards.JobPoints} JP each; " +
            $"gained {rewards.Gold} gold.";

        foreach (CharacterBattleReward reward in rewards.CharacterRewards)
        {
            if (reward.LevelsGained > 0)
            {
                battleLogText.text +=
                    $"\n{reward.CharacterName} reached level " +
                    $"{reward.NewLevel}!";
            }
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
        UIFocusHelper.Select(continueButton);
    }

    private void FinishBattle(string continueLabel)
    {
        SetCommandState(false);
        continueButton.SetActive(true);
        continueButtonText.text = continueLabel;
        UIFocusHelper.Select(continueButton);
    }

    private void SetCommandState(bool commandsAreActive)
    {
        attackButton.SetActive(commandsAreActive);
        if (abilityButton != null)
        {
            abilityButton.SetActive(commandsAreActive);
        }

        defendButton.SetActive(commandsAreActive);
        RefreshEscapeButton(commandsAreActive);
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

        if (commandsAreActive)
        {
            UIFocusHelper.SelectFirstAvailable(
                attackButton,
                abilityButton,
                defendButton);
        }
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
        string partyTargetId = IsInGroup(
            session.PartyBattle.PartyMembers,
            target)
            ? target?.CombatantId
            : null;
        string enemyTargetId = IsInGroup(
            session.PartyBattle.Enemies,
            target)
            ? target?.CombatantId
            : null;
        playerStatusText.text = FormatCombatantGroupWithGauges(
            session.PartyBattle.PartyMembers,
            actor?.CombatantId,
            partyTargetId,
            actionGauges);
        monsterStatusText.text = FormatCombatantGroupWithGauges(
            session.PartyBattle.Enemies,
            null,
            enemyTargetId,
            actionGauges);

        if (commandPromptText != null)
        {
            commandPromptText.text = commandSelection != null &&
                commandSelection.IsChoosingAbility
                ? FormatAbilityPrompt(
                    actor,
                    target,
                    commandSelection.PendingAbility)
                : FormatCommandPrompt(actor, target);
        }

        int targetCount = commandSelection?.GetCurrentTargetCount() ?? 0;
        SetTargetButtonInteractable(previousTargetButton, targetCount > 1);
        SetTargetButtonInteractable(nextTargetButton, targetCount > 1);
        RefreshAbilityButton();

        if (attackButton.activeInHierarchy &&
            !UIFocusHelper.CurrentSelectionIsUsable())
        {
            UIFocusHelper.SelectFirstAvailable(
                attackButton,
                abilityButton,
                defendButton,
                previousTargetButton,
                nextTargetButton);
        }

        UIFocusHelper.RefreshSelectionMarker();
    }

    private void BeginCommandSelection(ICombatant readyActor)
    {
        commandSelection = new PartyCommandSelection(
            session.PartyBattle,
            readyActor);
        SetCommandState(true);
        RefreshStatus();
    }

    private void ResolveSelectedCommand()
    {
        if (commandSelection == null || !commandSelection.IsComplete ||
            commandSelection.Commands.Count != 1)
        {
            return;
        }

        PartyBattleCommand command = commandSelection.Commands[0];
        CombatActionResult action = actionResolver.ResolveCommand(
            session.PartyBattle,
            command);
        actionGauges.ConsumeTurn(command.ActorId);
        commandSelection = null;
        CompleteResolvedAction(action);
    }

    private void ResolveEnemyAction(ICombatant enemy)
    {
        CombatActionResult action = actionResolver.ResolveEnemyAction(
            session.PartyBattle,
            enemy);
        actionGauges.ConsumeTurn(enemy.CombatantId);
        CompleteResolvedAction(action);
    }

    private void CompleteResolvedAction(CombatActionResult action)
    {
        string actionMessages = action.Message;
        IReadOnlyList<string> tutorialMessages =
            session.AdvanceTutorialAfterAction(action);
        if (tutorialMessages.Count > 0)
        {
            actionMessages += "\n" + string.Join("\n", tutorialMessages);
            actionGauges.ResetAll();
            ArrangeFormationPlaceholders();
        }

        battleLogText.text = actionMessages;
        bool tutorialIsComplete = session.CaravanTutorial == null ||
            session.CaravanTutorial.Stage == CaravanTutorialStage.Completed;
        if (session.PartyBattle.AreEnemiesDefeated && tutorialIsComplete)
        {
            CompleteVictory(actionMessages);
            return;
        }

        if (session.PartyBattle.IsPartyDefeated)
        {
            CompleteDefeat();
            return;
        }

        actionPresentationRemaining = actionPresentationSeconds;

        bool keepCommandMenuOpen = commandSelection != null &&
            !commandSelection.IsComplete &&
            commandSelection.CurrentActor != null;
        SetCommandState(keepCommandMenuOpen);
        RefreshStatus();
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

    private void ArrangeFormationPlaceholders()
    {
        if (session.BattleLayout == null)
        {
            return;
        }

        ArrangePlaceholderGroup(
            "PlayerPlaceholder",
            session.PartyBattle.PartyMembers.Count,
            session.BattleLayout.PartySlots);
        ArrangePlaceholderGroup(
            "MonsterPlaceholder",
            session.PartyBattle.Enemies.Count,
            session.BattleLayout.EnemySlots);
    }

    private static void ArrangePlaceholderGroup(
        string objectName,
        int visibleCount,
        IReadOnlyList<BattleSlotPosition> slots)
    {
        GameObject source = GameObject.Find(objectName);
        if (source == null || slots == null || visibleCount < 1)
        {
            return;
        }

        Transform parent = source.transform.parent;
        for (int childIndex = parent.childCount - 1; childIndex >= 0; childIndex--)
        {
            GameObject child = parent.GetChild(childIndex).gameObject;
            if (child != source && child.name.StartsWith(objectName + "_"))
            {
                Destroy(child);
            }
        }

        int count = Mathf.Min(visibleCount, slots.Count);
        for (int index = 0; index < count; index++)
        {
            GameObject placeholder = index == 0
                ? source
                : Instantiate(source, source.transform.parent);
            placeholder.name = index == 0
                ? objectName
                : $"{objectName}_{index + 1}";
            RectTransform rect = placeholder.GetComponent<RectTransform>();
            BattleSlotPosition slot = slots[index];
            rect.anchorMin = new Vector2(slot.X, slot.Y);
            rect.anchorMax = rect.anchorMin;
            rect.anchoredPosition = Vector2.zero;
        }
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

    private void RefreshAbilityButton()
    {
        if (abilityButton == null || commandSelection == null)
        {
            return;
        }

        if (abilityButton.TryGetComponent(out Button button))
        {
            button.interactable = commandSelection.IsChoosingAbility ||
                commandSelection.CanChooseCurrentCoreAbility();
        }

        TextMeshProUGUI label =
            abilityButton.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = commandSelection.IsChoosingAbility
                ? "Confirm"
                : "Ability";
        }
    }

    private void RefreshEscapeButton(bool commandsAreActive)
    {
        if (escapeButton == null)
        {
            return;
        }

        bool isTutorial = session != null && session.CaravanTutorial != null;
        bool canAttemptEscape = commandsAreActive && session != null &&
            session.PartyBattle != null &&
            session.PartyBattle.PartyMembers.Count == 1 &&
            session.PartyBattle.Enemies.Count == 1 &&
            session.EscapeAllowed;
        escapeButton.SetActive(canAttemptEscape ||
            (commandsAreActive && isTutorial));
        if (escapeButton.TryGetComponent(out Button button))
        {
            button.interactable = canAttemptEscape;
        }

        TextMeshProUGUI label =
            escapeButton.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = isTutorial ? "No Escape" : "Escape";
        }
    }

    private static bool IsInGroup(
        IReadOnlyList<ICombatant> combatants,
        ICombatant target)
    {
        if (target == null)
        {
            return false;
        }

        foreach (ICombatant combatant in combatants)
        {
            if (combatant.CombatantId == target.CombatantId)
            {
                return true;
            }
        }

        return false;
    }
}
