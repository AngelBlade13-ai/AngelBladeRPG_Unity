using TMPro;
using UnityEngine;

public class SimpleDialoguePanel2D : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField, Min(0f)] private float maximumViewerDistance = 1.5f;

    private Transform messageSource;
    private Transform viewer;

    public bool IsOpen => dialoguePanel != null && dialoguePanel.activeSelf;

    private void Awake()
    {
        Hide();
    }

    private void Update()
    {
        if (IsOpen && IsOutsideRange(
            messageSource,
            viewer,
            maximumViewerDistance))
        {
            Hide();
        }
    }

    public void Show(string message, Transform source, Transform messageViewer)
    {
        if (dialoguePanel == null || dialogueText == null)
        {
            return;
        }

        dialogueText.text = message;
        messageSource = source;
        viewer = messageViewer;
        dialoguePanel.SetActive(true);
    }

    public void Hide()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        messageSource = null;
        viewer = null;
    }

    public void Toggle(string message, Transform source, Transform messageViewer)
    {
        if (IsOpen)
        {
            Hide();
        }
        else
        {
            Show(message, source, messageViewer);
        }
    }

    public static bool IsOutsideRange(
        Transform source,
        Transform messageViewer,
        float maximumDistance)
    {
        if (source == null || messageViewer == null)
        {
            return false;
        }

        float maximumDistanceSquared = maximumDistance * maximumDistance;
        return (source.position - messageViewer.position).sqrMagnitude >
            maximumDistanceSquared;
    }
}
