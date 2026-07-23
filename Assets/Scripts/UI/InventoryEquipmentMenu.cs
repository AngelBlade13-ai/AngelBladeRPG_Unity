using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class InventoryEquipmentMenu : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference menuAction;
    [SerializeField] private InputActionReference cancelAction;

    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject itemsView;
    [SerializeField] private GameObject equipmentView;
    [SerializeField] private TextMeshProUGUI characterText;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Tabs")]
    [SerializeField] private Button itemsTabButton;
    [SerializeField] private Button equipmentTabButton;
    [SerializeField] private Button closeButton;

    [Header("Character")]
    [SerializeField] private Button previousCharacterButton;
    [SerializeField] private Button nextCharacterButton;

    [Header("Items")]
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private Button previousItemButton;
    [SerializeField] private Button nextItemButton;
    [SerializeField] private Button useItemButton;

    [Header("Equipment")]
    [SerializeField] private TextMeshProUGUI slotText;
    [SerializeField] private TextMeshProUGUI equippedText;
    [SerializeField] private TextMeshProUGUI candidateText;
    [SerializeField] private Button previousSlotButton;
    [SerializeField] private Button nextSlotButton;
    [SerializeField] private Button previousEquipmentButton;
    [SerializeField] private Button nextEquipmentButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;

    private static readonly EquipmentSlot[] Slots =
        (EquipmentSlot[])System.Enum.GetValues(typeof(EquipmentSlot));

    private IReadOnlyList<PlayableCharacterData> characters;
    private IReadOnlyList<ItemDefinition> items;
    private IReadOnlyList<ItemDefinition> equipmentCandidates;
    private int characterIndex;
    private int itemIndex;
    private int slotIndex;
    private int equipmentIndex;
    private bool showingEquipment;
    private PlayerMovement2D movement;
    private PlayerInteraction2D interaction;
    private bool restoreMovement;
    private bool restoreInteraction;

    public bool IsOpen => panel != null && panel.activeSelf;

    private PlayableCharacterData CurrentCharacter =>
        characters != null && characterIndex >= 0 &&
        characterIndex < characters.Count
            ? characters[characterIndex]
            : null;

    private ItemDefinition CurrentItem =>
        items != null && itemIndex >= 0 && itemIndex < items.Count
            ? items[itemIndex]
            : null;

    private ItemDefinition CurrentEquipment =>
        equipmentCandidates != null && equipmentIndex >= 0 &&
        equipmentIndex < equipmentCandidates.Count
            ? equipmentCandidates[equipmentIndex]
            : null;

    private EquipmentSlot CurrentSlot => Slots[slotIndex];

    private void Awake()
    {
        panel?.SetActive(false);
    }

    private void OnEnable()
    {
        Subscribe(menuAction, HandleMenu);
        Subscribe(cancelAction, HandleCancel);
    }

    private void OnDisable()
    {
        Unsubscribe(menuAction, HandleMenu);
        Unsubscribe(cancelAction, HandleCancel);
        RestorePlayerControl();
    }

    private void Update()
    {
        if (IsOpen)
        {
            EnsureValidSelection();
        }
    }

    public void Open()
    {
        GameSession session = GameSessionStore.Current;
        if (panel == null || session == null || !session.HasPlayer)
        {
            return;
        }

        characters = new PartyManagementService(session.Party)
            .GetOrderedCharacters();
        if (characters.Count == 0)
        {
            return;
        }

        CaptureAndPausePlayer();
        characterIndex = Mathf.Clamp(characterIndex, 0, characters.Count - 1);
        showingEquipment = false;
        feedbackText.text = "";
        panel.SetActive(true);
        Refresh();
        UIFocusHelper.Select(itemsTabButton);
    }

    public void Close()
    {
        panel?.SetActive(false);
        UIFocusHelper.Clear();
        RestorePlayerControl();
    }

    public void ShowItems()
    {
        showingEquipment = false;
        feedbackText.text = "";
        Refresh();
        UIFocusHelper.SelectFirstAvailable(previousItemButton, nextItemButton, useItemButton);
    }

    public void ShowEquipment()
    {
        showingEquipment = true;
        feedbackText.text = "";
        Refresh();
        UIFocusHelper.SelectFirstAvailable(previousSlotButton, nextSlotButton, equipButton);
    }

    public void PreviousCharacter()
    {
        SelectCharacter(characterIndex - 1);
    }

    public void NextCharacter()
    {
        SelectCharacter(characterIndex + 1);
    }

    public void PreviousItem()
    {
        itemIndex = Wrap(itemIndex - 1, items.Count);
        feedbackText.text = "";
        Refresh();
    }

    public void NextItem()
    {
        itemIndex = Wrap(itemIndex + 1, items.Count);
        feedbackText.text = "";
        Refresh();
    }

    public void UseItem()
    {
        if (CurrentItem == null || CurrentCharacter == null)
        {
            return;
        }

        ItemUseResult result = new ItemUseService(
            GameSessionStore.Current.Inventory).TryUse(
                CurrentItem.Id,
                CurrentCharacter);
        feedbackText.text = InventoryEquipmentMenuData.GetUseMessage(result);
        Refresh();
        RefreshExplorationHud();
    }

    public void PreviousSlot()
    {
        slotIndex = Wrap(slotIndex - 1, Slots.Length);
        equipmentIndex = 0;
        feedbackText.text = "";
        Refresh();
    }

    public void NextSlot()
    {
        slotIndex = Wrap(slotIndex + 1, Slots.Length);
        equipmentIndex = 0;
        feedbackText.text = "";
        Refresh();
    }

    public void PreviousEquipment()
    {
        equipmentIndex = Wrap(
            equipmentIndex - 1,
            equipmentCandidates.Count);
        feedbackText.text = "";
        Refresh();
    }

    public void NextEquipment()
    {
        equipmentIndex = Wrap(
            equipmentIndex + 1,
            equipmentCandidates.Count);
        feedbackText.text = "";
        Refresh();
    }

    public void Equip()
    {
        if (CurrentCharacter == null || CurrentEquipment == null)
        {
            return;
        }

        bool equipped = CurrentCharacter.TryEquipItem(
            CurrentSlot,
            CurrentEquipment.Id,
            GameSessionStore.Current.Inventory);
        feedbackText.text = equipped
            ? $"Equipped {CurrentEquipment.DisplayName}."
            : "That item cannot be equipped.";
        Refresh();
        RefreshExplorationHud();
    }

    public void Unequip()
    {
        if (CurrentCharacter == null)
        {
            return;
        }

        ItemDefinition equipped = ItemCatalog.Get(
            CurrentCharacter.Equipment.GetItemId(CurrentSlot));
        bool removed = CurrentCharacter.TryUnequipItem(
            CurrentSlot,
            GameSessionStore.Current.Inventory);
        feedbackText.text = removed
            ? $"Unequipped {equipped.DisplayName}."
            : "There is nothing to unequip.";
        Refresh();
        RefreshExplorationHud();
    }

    public void Configure(
        InputActionReference openMenu,
        InputActionReference cancelMenu,
        GameObject panelObject,
        GameObject itemPanel,
        GameObject equipmentPanel,
        TextMeshProUGUI characterLabel,
        TextMeshProUGUI feedbackLabel,
        Button itemsTab,
        Button equipmentTab,
        Button close,
        Button previousCharacter,
        Button nextCharacter,
        TextMeshProUGUI itemLabel,
        Button previousItem,
        Button nextItem,
        Button useItem,
        TextMeshProUGUI slotLabel,
        TextMeshProUGUI equippedLabel,
        TextMeshProUGUI candidateLabel,
        Button previousSlot,
        Button nextSlot,
        Button previousEquipment,
        Button nextEquipment,
        Button equip,
        Button unequip)
    {
        menuAction = openMenu;
        cancelAction = cancelMenu;
        panel = panelObject;
        itemsView = itemPanel;
        equipmentView = equipmentPanel;
        characterText = characterLabel;
        feedbackText = feedbackLabel;
        itemsTabButton = itemsTab;
        equipmentTabButton = equipmentTab;
        closeButton = close;
        previousCharacterButton = previousCharacter;
        nextCharacterButton = nextCharacter;
        itemText = itemLabel;
        previousItemButton = previousItem;
        nextItemButton = nextItem;
        useItemButton = useItem;
        slotText = slotLabel;
        equippedText = equippedLabel;
        candidateText = candidateLabel;
        previousSlotButton = previousSlot;
        nextSlotButton = nextSlot;
        previousEquipmentButton = previousEquipment;
        nextEquipmentButton = nextEquipment;
        equipButton = equip;
        unequipButton = unequip;
    }

    private void Refresh()
    {
        GameSession session = GameSessionStore.Current;
        PlayableCharacterData character = CurrentCharacter;
        if (session == null || character == null)
        {
            return;
        }

        characterText.text =
            $"{characterIndex + 1}/{characters.Count}  {character.Name}  " +
            $"[{JobCatalog.Get(character.CurrentJob).DisplayName}]\n" +
            $"HP {character.Stats.CurrentHp}/{character.Stats.MaxHp}   " +
            $"MP {character.Stats.CurrentMp}/{character.Stats.MaxMp}";
        bool canChangeCharacter = characters.Count > 1;
        previousCharacterButton.interactable = canChangeCharacter;
        nextCharacterButton.interactable = canChangeCharacter;

        itemsView.SetActive(!showingEquipment);
        equipmentView.SetActive(showingEquipment);
        if (showingEquipment)
        {
            RefreshEquipment(session.Inventory, character);
        }
        else
        {
            RefreshItems(session.Inventory);
        }

        EnsureValidSelection();
    }

    private void RefreshItems(Inventory inventory)
    {
        items = InventoryEquipmentMenuData.GetConsumables(inventory);
        itemIndex = ClampIndex(itemIndex, items.Count);
        ItemDefinition item = CurrentItem;
        itemText.text = InventoryEquipmentMenuData.FormatItem(
            item,
            item == null ? 0 : inventory.GetQuantity(item.Id));
        bool hasItems = item != null;
        previousItemButton.interactable = items.Count > 1;
        nextItemButton.interactable = items.Count > 1;
        useItemButton.interactable = hasItems;
    }

    private void RefreshEquipment(
        Inventory inventory,
        PlayableCharacterData character)
    {
        equipmentCandidates =
            InventoryEquipmentMenuData.GetEquipmentCandidates(
                inventory,
                CurrentSlot,
                character.CurrentJob);
        equipmentIndex = ClampIndex(
            equipmentIndex,
            equipmentCandidates.Count);

        ItemDefinition equipped = ItemCatalog.Get(
            character.Equipment.GetItemId(CurrentSlot));
        slotText.text = $"SLOT  {GetSlotName(CurrentSlot)}";
        equippedText.text = equipped == null
            ? "Equipped: None"
            : $"Equipped: {equipped.DisplayName}";
        candidateText.text = InventoryEquipmentMenuData.FormatEquipment(
            CurrentEquipment);
        previousEquipmentButton.interactable = equipmentCandidates.Count > 1;
        nextEquipmentButton.interactable = equipmentCandidates.Count > 1;
        equipButton.interactable = CurrentEquipment != null;
        unequipButton.interactable = equipped != null;
    }

    private void SelectCharacter(int requestedIndex)
    {
        characterIndex = Wrap(requestedIndex, characters.Count);
        itemIndex = 0;
        equipmentIndex = 0;
        feedbackText.text = "";
        Refresh();
    }

    private void CaptureAndPausePlayer()
    {
        movement = Object.FindAnyObjectByType<PlayerMovement2D>();
        if (movement == null)
        {
            return;
        }

        interaction = movement.GetComponent<PlayerInteraction2D>();
        restoreMovement = movement.enabled;
        restoreInteraction = interaction != null && interaction.enabled;
        movement.enabled = false;
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

    private void EnsureValidSelection()
    {
        if (!UIFocusHelper.CurrentSelectionIsUsable())
        {
            UIFocusHelper.SelectFirstAvailable(
                itemsTabButton,
                equipmentTabButton,
                previousCharacterButton,
                nextCharacterButton,
                useItemButton,
                equipButton,
                unequipButton,
                closeButton);
        }

        UIFocusHelper.RefreshSelectionMarker();
    }

    private static void RefreshExplorationHud()
    {
        Object.FindAnyObjectByType<ExplorationStatusHUD>()?.Refresh();
    }

    private void HandleMenu(InputAction.CallbackContext context)
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void HandleCancel(InputAction.CallbackContext context)
    {
        if (IsOpen)
        {
            Close();
        }
    }

    private static void Subscribe(
        InputActionReference actionReference,
        System.Action<InputAction.CallbackContext> callback)
    {
        if (actionReference == null)
        {
            return;
        }

        actionReference.action.performed += callback;
        actionReference.action.Enable();
    }

    private static void Unsubscribe(
        InputActionReference actionReference,
        System.Action<InputAction.CallbackContext> callback)
    {
        if (actionReference == null)
        {
            return;
        }

        actionReference.action.performed -= callback;
        actionReference.action.Disable();
    }

    private static int Wrap(int value, int count)
    {
        return count <= 0 ? 0 : (value % count + count) % count;
    }

    private static int ClampIndex(int value, int count)
    {
        return count <= 0 ? 0 : Mathf.Clamp(value, 0, count - 1);
    }

    private static string GetSlotName(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Accessory1:
                return "Accessory 1";
            case EquipmentSlot.Accessory2:
                return "Accessory 2";
            default:
                return slot.ToString();
        }
    }
}
