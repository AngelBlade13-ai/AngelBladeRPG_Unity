using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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

    [Header("Focus (Keyboard/Controller Navigation)")]
    [Tooltip("Selected automatically when the title panel opens.")]
    public GameObject titleFirstSelected;

    private GameSession gameSession;

    private void Start()
    {
        gameSession = GameSessionStore.Current;

        ShowTitlePanel();
    }

    public void StartNewGame()
    {
        gameSession = GameSessionStore.BeginNewSession();
        playerNameInput.text = "";
        characterCreationErrorText.text = "";

        ShowCharacterCreationPanel();
    }

    public void ConfirmCharacterCreation()
    {
        if (!gameSession.TryStartNewGame(playerNameInput.text))
        {
            characterCreationErrorText.text = "Please enter a hero name.";
            return;
        }

        SceneManager.LoadScene(startingSceneName);
    }

    public void ReturnToTitle()
    {
        ShowTitlePanel();
    }

    private void ShowTitlePanel()
    {
        titlePanel.SetActive(true);
        characterCreationPanel.SetActive(false);

        UIFocusHelper.Select(titleFirstSelected);
    }

    private void ShowCharacterCreationPanel()
    {
        titlePanel.SetActive(false);
        characterCreationPanel.SetActive(true);

        playerNameInput.Select();
        playerNameInput.ActivateInputField();
    }
}
