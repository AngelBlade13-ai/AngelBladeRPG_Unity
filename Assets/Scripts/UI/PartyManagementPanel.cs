using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartyManagementPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI characterText;
    [SerializeField] private TextMeshProUGUI jobText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Controls")]
    [SerializeField] private Button previousCharacterButton;
    [SerializeField] private Button nextCharacterButton;
    [SerializeField] private Button partyButton;
    [SerializeField] private TextMeshProUGUI partyButtonText;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button previousJobButton;
    [SerializeField] private Button nextJobButton;
    [SerializeField] private Button applyJobButton;
    [SerializeField] private Button closeButton;

    private readonly JobId[] jobs =
        (JobId[])Enum.GetValues(typeof(JobId));

    private PartyManagementService service;
    private IReadOnlyList<PlayableCharacterData> characters;
    private int characterIndex;
    private int jobIndex;
    private PlayerMovement2D movement;
    private PlayerInteraction2D interaction;
    private bool restoreMovement;
    private bool restoreInteraction;

    public bool IsOpen => panel != null && panel.activeSelf;

    private void Awake()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void OnDisable()
    {
        RestorePlayerControl();
    }

    public void Open(GameObject interactor)
    {
        GameSession session = GameSessionStore.Current;
        if (panel == null || session == null || !session.HasPlayer)
        {
            return;
        }

        service = new PartyManagementService(session.Party);
        characters = service.GetOrderedCharacters();
        if (characters.Count == 0)
        {
            return;
        }

        CaptureAndPausePlayer(interactor);
        characterIndex = 0;
        jobIndex = GetJobIndex(characters[0].CurrentJob);
        SetFeedback("Choose a formation position or preview any job.");
        panel.SetActive(true);
        Refresh();

        if (EventSystem.current != null && nextJobButton != null)
        {
            EventSystem.current.SetSelectedGameObject(
                nextJobButton.gameObject);
        }
    }

    public void Close()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        RestorePlayerControl();
    }

    public void PreviousCharacter()
    {
        SelectCharacter(characterIndex - 1);
    }

    public void NextCharacter()
    {
        SelectCharacter(characterIndex + 1);
    }

    public void TogglePartyStatus()
    {
        PlayableCharacterData character = CurrentCharacter;
        if (character == null)
        {
            return;
        }

        PartyManagementResult result = service.IsActive(character.Id)
            ? service.MoveToReserve(character.Id)
            : service.AddToActiveParty(character.Id);
        RefreshAfterRosterChange(character.Id, result);
    }

    public void MoveUp()
    {
        MoveSelectedCharacter(-1);
    }

    public void MoveDown()
    {
        MoveSelectedCharacter(1);
    }

    public void PreviousJob()
    {
        SelectJob(jobIndex - 1);
    }

    public void NextJob()
    {
        SelectJob(jobIndex + 1);
    }

    public void ApplyJob()
    {
        PlayableCharacterData character = CurrentCharacter;
        if (character == null || jobs.Length == 0)
        {
            return;
        }

        PartyManagementResult result = service.AssignJob(
            character.Id,
            jobs[jobIndex]);
        SetFeedback(GetResultMessage(result));
        Refresh();
    }

    public void Configure(
        GameObject panelObject,
        TextMeshProUGUI characterLabel,
        TextMeshProUGUI jobLabel,
        TextMeshProUGUI statsLabel,
        TextMeshProUGUI feedbackLabel,
        Button previousCharacter,
        Button nextCharacter,
        Button partyToggle,
        TextMeshProUGUI partyToggleLabel,
        Button formationUp,
        Button formationDown,
        Button previousJob,
        Button nextJob,
        Button applyJob,
        Button close)
    {
        panel = panelObject;
        characterText = characterLabel;
        jobText = jobLabel;
        statsText = statsLabel;
        feedbackText = feedbackLabel;
        previousCharacterButton = previousCharacter;
        nextCharacterButton = nextCharacter;
        partyButton = partyToggle;
        partyButtonText = partyToggleLabel;
        moveUpButton = formationUp;
        moveDownButton = formationDown;
        previousJobButton = previousJob;
        nextJobButton = nextJob;
        applyJobButton = applyJob;
        closeButton = close;
    }

    public static string GetResultMessage(PartyManagementResult result)
    {
        switch (result)
        {
            case PartyManagementResult.Success:
                return "Changes applied.";
            case PartyManagementResult.ActivePartyFull:
                return "The active party already has four members.";
            case PartyManagementResult.LastActiveMember:
                return "At least one character must remain active.";
            case PartyManagementResult.AlreadyActive:
                return "That character is already active.";
            case PartyManagementResult.AlreadyInReserve:
                return "That character is already in reserve.";
            case PartyManagementResult.InvalidMove:
                return "That formation move is not available.";
            case PartyManagementResult.InvalidJob:
                return "That job is not available.";
            case PartyManagementResult.CharacterUnavailable:
                return "That character is unavailable.";
            default:
                return "Character data could not be found.";
        }
    }

    private PlayableCharacterData CurrentCharacter =>
        characters != null && characterIndex >= 0 &&
        characterIndex < characters.Count
            ? characters[characterIndex]
            : null;

    private void SelectCharacter(int requestedIndex)
    {
        if (characters == null || characters.Count == 0)
        {
            return;
        }

        characterIndex = Wrap(requestedIndex, characters.Count);
        jobIndex = GetJobIndex(CurrentCharacter.CurrentJob);
        SetFeedback("");
        Refresh();
    }

    private void SelectJob(int requestedIndex)
    {
        if (jobs.Length == 0)
        {
            return;
        }

        jobIndex = Wrap(requestedIndex, jobs.Length);
        SetFeedback("");
        Refresh();
    }

    private void MoveSelectedCharacter(int direction)
    {
        PlayableCharacterData character = CurrentCharacter;
        if (character == null)
        {
            return;
        }

        PartyManagementResult result = service.MoveActiveCharacter(
            character.Id,
            direction);
        RefreshAfterRosterChange(character.Id, result);
    }

    private void RefreshAfterRosterChange(
        string selectedCharacterId,
        PartyManagementResult result)
    {
        characters = service.GetOrderedCharacters();
        characterIndex = FindCharacterIndex(selectedCharacterId);
        SetFeedback(GetResultMessage(result));
        Refresh();
    }

    private void Refresh()
    {
        PlayableCharacterData character = CurrentCharacter;
        if (character == null || jobs.Length == 0)
        {
            return;
        }

        int activeIndex = GetActiveIndex(character.Id);
        bool isActive = activeIndex >= 0;
        JobDefinition previewJob = JobCatalog.Get(jobs[jobIndex]);
        JobAffinity affinity = character.GetJobAffinity(previewJob.Id);
        CombatantStats stats = character.Stats;

        characterText.text =
            $"{characterIndex + 1}/{characters.Count}  {character.Name}\n" +
            (isActive
                ? $"ACTIVE - FORMATION {activeIndex + 1}"
                : "RESERVE") +
            $"   JP {character.JobPoints}";
        jobText.text =
            $"{previewJob.DisplayName}  [{affinity} affinity]\n" +
            $"{previewJob.Strength}\nTrade-off: {previewJob.TradeOff}";
        statsText.text =
            $"HP {stats.CurrentHp}/{stats.MaxHp}   " +
            $"MP {stats.CurrentMp}/{stats.MaxMp}\n" +
            $"ATK {stats.Attack}  DEF {stats.Defense}  " +
            $"MAG {stats.MagicPower}  MDEF {stats.MagicDefense}\n" +
            $"SPD {stats.Speed}  ACC {stats.Accuracy}  " +
            $"EVA {stats.Evasion}  CRIT {stats.CriticalChance}";

        bool multipleCharacters = characters.Count > 1;
        previousCharacterButton.interactable = multipleCharacters;
        nextCharacterButton.interactable = multipleCharacters;
        partyButtonText.text = isActive ? "Move to Reserve" : "Add to Party";
        partyButton.interactable = isActive
            ? service.GetOrderedCharacters().Count > 0 &&
                GameSessionStore.Current.Party.ActiveCharacterIds.Count > 1
            : GameSessionStore.Current.Party.ActiveCharacterIds.Count <
                PartyRoster.MaximumActiveMembers;
        moveUpButton.interactable = isActive && activeIndex > 0;
        moveDownButton.interactable = isActive && activeIndex <
            GameSessionStore.Current.Party.ActiveCharacterIds.Count - 1;
        previousJobButton.interactable = jobs.Length > 1;
        nextJobButton.interactable = jobs.Length > 1;
        applyJobButton.interactable = character.CurrentJob != previewJob.Id;
    }

    private int GetActiveIndex(string characterId)
    {
        IReadOnlyList<string> activeIds =
            GameSessionStore.Current.Party.ActiveCharacterIds;
        for (int index = 0; index < activeIds.Count; index++)
        {
            if (string.Equals(activeIds[index], characterId, StringComparison.Ordinal))
            {
                return index;
            }
        }

        return -1;
    }

    private int FindCharacterIndex(string characterId)
    {
        for (int index = 0; index < characters.Count; index++)
        {
            if (string.Equals(
                characters[index].Id,
                characterId,
                StringComparison.Ordinal))
            {
                return index;
            }
        }

        return 0;
    }

    private int GetJobIndex(JobId jobId)
    {
        for (int index = 0; index < jobs.Length; index++)
        {
            if (jobs[index] == jobId)
            {
                return index;
            }
        }

        return 0;
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }

    private void CaptureAndPausePlayer(GameObject interactor)
    {
        if (interactor == null)
        {
            return;
        }

        movement = interactor.GetComponent<PlayerMovement2D>();
        interaction = interactor.GetComponent<PlayerInteraction2D>();
        restoreMovement = movement != null && movement.enabled;
        restoreInteraction = interaction != null && interaction.enabled;

        if (movement != null)
        {
            movement.enabled = false;
        }

        if (interaction != null)
        {
            interaction.enabled = false;
        }
    }

    private void RestorePlayerControl()
    {
        if (movement != null)
        {
            movement.enabled = restoreMovement;
        }

        if (interaction != null)
        {
            interaction.enabled = restoreInteraction;
        }

        movement = null;
        interaction = null;
        restoreMovement = false;
        restoreInteraction = false;
    }

    private static int Wrap(int value, int count)
    {
        return (value % count + count) % count;
    }
}
