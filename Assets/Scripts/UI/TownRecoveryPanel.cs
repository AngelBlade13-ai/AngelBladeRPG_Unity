using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class TownRecoveryPanel : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference cancelAction;

    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI detailsText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Button recoverButton;
    [SerializeField] private Button cancelButton;

    private readonly ExplorationModalControlLock controlLock =
        new ExplorationModalControlLock();

    private GameSession session;
    private int goldCost;

    public bool IsOpen => panel != null && panel.activeSelf;

    private void Awake()
    {
        panel?.SetActive(false);
    }

    private void OnEnable()
    {
        if (cancelAction == null)
        {
            return;
        }

        cancelAction.action.performed += HandleCancel;
        cancelAction.action.Enable();
    }

    private void OnDisable()
    {
        if (cancelAction != null)
        {
            cancelAction.action.performed -= HandleCancel;
            cancelAction.action.Disable();
        }

        ExplorationModalState.Release(this);
        controlLock.Restore();
    }

    private void Update()
    {
        if (IsOpen)
        {
            EnsureValidSelection();
        }
    }

    public void Open(
        GameObject interactor,
        string serviceName,
        int cost)
    {
        session = GameSessionStore.Current;
        if (panel == null || session == null || !session.HasPlayer)
        {
            return;
        }

        if (!ExplorationModalState.TryAcquire(this))
        {
            return;
        }

        goldCost = Mathf.Max(0, cost);
        titleText.text = serviceName;
        feedbackText.text = "";
        controlLock.Capture(interactor);
        panel.SetActive(true);
        Refresh();
        UIFocusHelper.Select(recoverButton);
    }

    public void PurchaseRecovery()
    {
        if (session == null)
        {
            return;
        }

        TownRecoveryStatus status = new TownRecoveryService(
            session.Player,
            session.Party).TryPurchaseFullRecovery(goldCost, true);
        feedbackText.text = TownServiceMenuData.GetRecoveryMessage(status);
        Refresh();
        Object.FindAnyObjectByType<ExplorationStatusHUD>()?.Refresh();
    }

    public void Close()
    {
        panel?.SetActive(false);
        UIFocusHelper.Clear();
        ExplorationModalState.Release(this);
        controlLock.Restore();
    }

    public void Configure(
        InputActionReference cancel,
        GameObject panelObject,
        TextMeshProUGUI titleLabel,
        TextMeshProUGUI detailsLabel,
        TextMeshProUGUI feedbackLabel,
        Button recover,
        Button cancelButtonControl)
    {
        cancelAction = cancel;
        panel = panelObject;
        titleText = titleLabel;
        detailsText = detailsLabel;
        feedbackText = feedbackLabel;
        recoverButton = recover;
        cancelButton = cancelButtonControl;
    }

    private void Refresh()
    {
        if (session == null)
        {
            return;
        }

        bool needsRecovery = PartyRecoveryService.NeedsFullRecovery(
            session.Party);
        detailsText.text =
            $"Full party recovery: {goldCost} gold\n" +
            $"Current gold: {session.Player.Gold}\n" +
            (needsRecovery
                ? "Restore HP and MP for all available party members?"
                : "The party is already fully recovered.");
        recoverButton.interactable = needsRecovery;
        EnsureValidSelection();
    }

    private void EnsureValidSelection()
    {
        if (!UIFocusHelper.CurrentSelectionIsUsable())
        {
            UIFocusHelper.SelectFirstAvailable(recoverButton, cancelButton);
        }

        UIFocusHelper.RefreshSelectionMarker();
    }

    private void HandleCancel(InputAction.CallbackContext context)
    {
        if (IsOpen)
        {
            Close();
        }
    }
}
