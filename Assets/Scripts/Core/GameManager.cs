using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public const string DefaultStartingSceneName = "SuncrestGuildHallScene";

    [Header("Scenes")]
    [FormerlySerializedAs("townSceneName")]
    [SerializeField] private string startingSceneName =
        DefaultStartingSceneName;

    [Header("Panels")]
    public GameObject titlePanel;
    public GameObject characterCreationPanel;

    [Header("Character Creation UI")]
    public TMP_InputField playerNameInput;
    public TextMeshProUGUI characterCreationErrorText;

    [Header("Save UI (Optional)")]
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI titleMessageText;

    [Header("Focus (Keyboard/Controller Navigation)")]
    [Tooltip("Selected automatically when the title panel opens.")]
    public GameObject titleFirstSelected;

    private GameSession gameSession;

    private void Start()
    {
        gameSession = GameSessionStore.Current;

        ShowTitlePanel();
        RefreshContinueButton();
    }

    public void StartNewGame()
    {
        gameSession = GameSessionStore.BeginNewSession();
        playerNameInput.text = "";
        characterCreationErrorText.text = "";
        SetTitleMessage("");

        ShowCharacterCreationPanel();
    }

    public void ConfirmCharacterCreation()
    {
        if (!gameSession.TryStartNewGame(playerNameInput.text))
        {
            characterCreationErrorText.text = "Please enter a hero name.";
            return;
        }

        GameSaveRuntime.BeginNewGame();
        SceneManager.LoadScene(startingSceneName);
    }

    public void ContinueGame()
    {
        PlayerContinueStatus status =
            GameSaveRuntime.Continue(out LocationSaveData location);
        if ((status != PlayerContinueStatus.Success &&
                status != PlayerContinueStatus.RecoveredBackup) ||
            location == null)
        {
            SetTitleMessage(status == PlayerContinueStatus.NoSave
                ? "No valid save was found."
                : "The save could not be loaded.");
            RefreshContinueButton();
            return;
        }

        WorldTransitionStore.RequestSpawn(location.spawnId);
        SceneManager.LoadScene(location.sceneName);
    }

    public void ReturnToTitle()
    {
        ShowTitlePanel();
    }

    private void ShowTitlePanel()
    {
        titlePanel.SetActive(true);
        characterCreationPanel.SetActive(false);

        RefreshContinueButton();
        UIFocusHelper.SelectFirstAvailable(
            continueButton,
            titleFirstSelected == null
                ? null
                : titleFirstSelected.GetComponent<Selectable>());
    }

    private void ShowCharacterCreationPanel()
    {
        titlePanel.SetActive(false);
        characterCreationPanel.SetActive(true);

        playerNameInput.Select();
        playerNameInput.ActivateInputField();
    }

    private void RefreshContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.interactable = GameSaveRuntime.HasContinue;
        }
    }

    private void SetTitleMessage(string message)
    {
        if (titleMessageText != null)
        {
            titleMessageText.text = message;
        }
    }
}
