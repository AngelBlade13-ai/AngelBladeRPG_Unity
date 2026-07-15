using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string townSceneName = "TownScene";

    [Header("Panels")]
    public GameObject titlePanel;
    public GameObject characterCreationPanel;

    [Header("Character Creation UI")]
    public TMP_InputField playerNameInput;
    public TextMeshProUGUI characterCreationErrorText;

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

        SceneManager.LoadScene(townSceneName);
    }

    public void ReturnToTitle()
    {
        ShowTitlePanel();
    }

    private void ShowTitlePanel()
    {
        titlePanel.SetActive(true);
        characterCreationPanel.SetActive(false);
    }

    private void ShowCharacterCreationPanel()
    {
        titlePanel.SetActive(false);
        characterCreationPanel.SetActive(true);

        playerNameInput.Select();
        playerNameInput.ActivateInputField();
    }
}
