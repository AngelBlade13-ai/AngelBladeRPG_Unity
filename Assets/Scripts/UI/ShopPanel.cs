using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class ShopPanel : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference cancelAction;

    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Controls")]
    [SerializeField] private Button buyTabButton;
    [SerializeField] private Button sellTabButton;
    [SerializeField] private Button previousItemButton;
    [SerializeField] private Button nextItemButton;
    [SerializeField] private Button decreaseQuantityButton;
    [SerializeField] private Button increaseQuantityButton;
    [SerializeField] private Button transactButton;
    [SerializeField] private TextMeshProUGUI transactButtonText;
    [SerializeField] private Button closeButton;

    private readonly ExplorationModalControlLock controlLock =
        new ExplorationModalControlLock();

    private GameSession session;
    private ShopDefinition shop;
    private ShopService service;
    private IReadOnlyList<ItemDefinition> items;
    private bool selling;
    private int itemIndex;
    private int quantity = 1;

    public bool IsOpen => panel != null && panel.activeSelf;

    private ItemDefinition CurrentItem =>
        items != null && itemIndex >= 0 && itemIndex < items.Count
            ? items[itemIndex]
            : null;

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

    public void Open(GameObject interactor, string shopId)
    {
        session = GameSessionStore.Current;
        shop = ShopCatalog.Get(shopId);
        if (panel == null || session == null || !session.HasPlayer ||
            shop == null)
        {
            return;
        }

        if (!ExplorationModalState.TryAcquire(this))
        {
            return;
        }

        service = new ShopService(shop, session.Player, session.Inventory);
        controlLock.Capture(interactor);
        selling = false;
        itemIndex = 0;
        quantity = 1;
        feedbackText.text = "";
        titleText.text = GetShopTitle(shop.Id);
        panel.SetActive(true);
        Refresh();
        UIFocusHelper.Select(buyTabButton);
    }

    public void Close()
    {
        panel?.SetActive(false);
        UIFocusHelper.Clear();
        ExplorationModalState.Release(this);
        controlLock.Restore();
    }

    public void ShowBuy()
    {
        selling = false;
        ResetSelection();
        Refresh();
        UIFocusHelper.SelectFirstAvailable(
            previousItemButton,
            nextItemButton,
            transactButton,
            closeButton);
    }

    public void ShowSell()
    {
        selling = true;
        ResetSelection();
        Refresh();
        UIFocusHelper.SelectFirstAvailable(
            previousItemButton,
            nextItemButton,
            transactButton,
            closeButton);
    }

    public void PreviousItem()
    {
        itemIndex = Wrap(itemIndex - 1, items.Count);
        quantity = 1;
        feedbackText.text = "";
        Refresh();
    }

    public void NextItem()
    {
        itemIndex = Wrap(itemIndex + 1, items.Count);
        quantity = 1;
        feedbackText.text = "";
        Refresh();
    }

    public void DecreaseQuantity()
    {
        quantity = Mathf.Max(1, quantity - 1);
        feedbackText.text = "";
        Refresh();
    }

    public void IncreaseQuantity()
    {
        quantity = Mathf.Min(GetMaximumQuantity(), quantity + 1);
        feedbackText.text = "";
        Refresh();
    }

    public void Transact()
    {
        ItemDefinition item = CurrentItem;
        if (item == null || service == null)
        {
            return;
        }

        ShopTransactionStatus status = selling
            ? service.TrySell(item.Id, quantity)
            : service.TryBuy(item.Id, quantity);
        feedbackText.text = TownServiceMenuData.GetShopMessage(status);
        if (status == ShopTransactionStatus.Completed)
        {
            quantity = 1;
        }

        Refresh();
        Object.FindAnyObjectByType<ExplorationStatusHUD>()?.Refresh();
    }

    public void Configure(
        InputActionReference cancel,
        GameObject panelObject,
        TextMeshProUGUI titleLabel,
        TextMeshProUGUI goldLabel,
        TextMeshProUGUI itemLabel,
        TextMeshProUGUI feedbackLabel,
        Button buyTab,
        Button sellTab,
        Button previousItem,
        Button nextItem,
        Button decreaseQuantity,
        Button increaseQuantity,
        Button transact,
        TextMeshProUGUI transactLabel,
        Button close)
    {
        cancelAction = cancel;
        panel = panelObject;
        titleText = titleLabel;
        goldText = goldLabel;
        itemText = itemLabel;
        feedbackText = feedbackLabel;
        buyTabButton = buyTab;
        sellTabButton = sellTab;
        previousItemButton = previousItem;
        nextItemButton = nextItem;
        decreaseQuantityButton = decreaseQuantity;
        increaseQuantityButton = increaseQuantity;
        transactButton = transact;
        transactButtonText = transactLabel;
        closeButton = close;
    }

    private void Refresh()
    {
        if (session == null || shop == null)
        {
            return;
        }

        items = selling
            ? TownServiceMenuData.GetSellItems(session.Inventory)
            : TownServiceMenuData.GetBuyItems(shop);
        itemIndex = ClampIndex(itemIndex, items.Count);
        quantity = Mathf.Clamp(quantity, 1, GetMaximumQuantity());

        ItemDefinition item = CurrentItem;
        int owned = item == null
            ? 0
            : session.Inventory.GetQuantity(item.Id);
        goldText.text = $"Gold  {session.Player.Gold}";
        itemText.text = TownServiceMenuData.FormatShopItem(
            item,
            owned,
            quantity,
            selling);
        transactButtonText.text = selling ? "Sell" : "Buy";

        bool hasItem = item != null;
        previousItemButton.interactable = items.Count > 1;
        nextItemButton.interactable = items.Count > 1;
        decreaseQuantityButton.interactable = hasItem && quantity > 1;
        increaseQuantityButton.interactable =
            hasItem && quantity < GetMaximumQuantity();
        transactButton.interactable = hasItem;
        EnsureValidSelection();
    }

    private int GetMaximumQuantity()
    {
        ItemDefinition item = CurrentItem;
        if (item == null || session == null)
        {
            return 1;
        }

        if (selling)
        {
            return Mathf.Max(1, session.Inventory.GetQuantity(item.Id));
        }

        return Mathf.Max(
            1,
            item.MaximumStack - session.Inventory.GetQuantity(item.Id));
    }

    private void ResetSelection()
    {
        itemIndex = 0;
        quantity = 1;
        feedbackText.text = "";
    }

    private void EnsureValidSelection()
    {
        if (!UIFocusHelper.CurrentSelectionIsUsable())
        {
            UIFocusHelper.SelectFirstAvailable(
                buyTabButton,
                sellTabButton,
                previousItemButton,
                nextItemButton,
                decreaseQuantityButton,
                increaseQuantityButton,
                transactButton,
                closeButton);
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

    private static string GetShopTitle(string shopId)
    {
        return shopId == ShopCatalog.IronforgeSmithyId
            ? "IRONFORGE SMITHY"
            : "WHISPER MARKET";
    }

    private static int Wrap(int value, int count)
    {
        return count <= 0 ? 0 : (value % count + count) % count;
    }

    private static int ClampIndex(int value, int count)
    {
        return count <= 0 ? 0 : Mathf.Clamp(value, 0, count - 1);
    }
}
