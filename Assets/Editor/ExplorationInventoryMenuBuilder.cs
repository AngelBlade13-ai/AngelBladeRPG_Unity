using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class ExplorationInventoryMenuBuilder
{
    private const string InputActionsPath =
        "Assets/Settings/InputSystem_Actions.inputactions";
    private const string CanvasName = "ExplorationMenuCanvas";

    [MenuItem("Tools/AngelBlade RPG/UI/Install Inventory Menu In Exploration Scenes")]
    public static void InstallInExplorationScenes()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        InputActionReference menuAction = FindActionReference("Player/Menu");
        InputActionReference cancelAction = FindActionReference("UI/Cancel");
        if (menuAction == null || cancelAction == null)
        {
            EditorUtility.DisplayDialog(
                "Exploration Inventory Menu",
                "Unity has not imported the Player/Menu and UI/Cancel action " +
                "references yet. Focus Unity, wait for compilation to finish, " +
                "then run this command again.",
                "OK");
            return;
        }

        string originalScenePath = SceneManager.GetActiveScene().path;
        string[] scenePaths = AssetDatabase.FindAssets(
                "t:Scene",
                new[] { "Assets/Scenes" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        int installedCount = 0;

        try
        {
            foreach (string scenePath in scenePaths)
            {
                Scene scene = EditorSceneManager.OpenScene(
                    scenePath,
                    OpenSceneMode.Single);
                if (Object.FindAnyObjectByType<PlayerMovement2D>() == null ||
                    Object.FindAnyObjectByType<ExplorationStatusHUD>() == null)
                {
                    continue;
                }

                EnsureEventSystem();
                DestroyIfPresent(CanvasName);
                CreateInterface(menuAction, cancelAction);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                installedCount += 1;
            }
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(originalScenePath))
            {
                EditorSceneManager.OpenScene(
                    originalScenePath,
                    OpenSceneMode.Single);
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog(
            "Exploration Inventory Menu",
            $"Installed the inventory and equipment menu in " +
            $"{installedCount} exploration scenes.",
            "OK");
    }

    private static void CreateInterface(
        InputActionReference menuAction,
        InputActionReference cancelAction)
    {
        GameObject canvasObject = new GameObject(
            CanvasName,
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 110;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(320f, 180f);
        scaler.matchWidthOrHeight = 0.5f;

        InventoryEquipmentMenu controller =
            canvasObject.AddComponent<InventoryEquipmentMenu>();
        GameObject panel = CreateImage(
            "InventoryEquipmentPanel",
            canvasObject.transform,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero,
            new Color(0.035f, 0.045f, 0.05f, 0.98f));

        CreateText(
            "TitleText",
            panel.transform,
            "INVENTORY & EQUIPMENT",
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, 78f),
            new Vector2(220f, 14f),
            10f,
            TextAlignmentOptions.Center);
        Button close = CreateButton(
            "CloseButton", panel.transform, "X",
            new Vector2(0.5f, 0.5f), new Vector2(146f, 78f),
            new Vector2(20f, 16f));
        Button itemsTab = CreateButton(
            "ItemsTabButton", panel.transform, "Items",
            new Vector2(0.5f, 0.5f), new Vector2(-38f, 59f),
            new Vector2(72f, 18f));
        Button equipmentTab = CreateButton(
            "EquipmentTabButton", panel.transform, "Equipment",
            new Vector2(0.5f, 0.5f), new Vector2(41f, 59f),
            new Vector2(82f, 18f));

        Button previousCharacter = CreateButton(
            "PreviousCharacterButton", panel.transform, "<",
            new Vector2(0.5f, 0.5f), new Vector2(-139f, 38f),
            new Vector2(18f, 26f));
        Button nextCharacter = CreateButton(
            "NextCharacterButton", panel.transform, ">",
            new Vector2(0.5f, 0.5f), new Vector2(139f, 38f),
            new Vector2(18f, 26f));
        TextMeshProUGUI characterText = CreateText(
            "CharacterText", panel.transform,
            "1/1  Hero  [Mercenary]\nHP 100/100   MP 20/20",
            new Vector2(0.5f, 0.5f), new Vector2(0f, 38f),
            new Vector2(252f, 28f), 7f,
            TextAlignmentOptions.Center);

        GameObject itemsView = CreateRect("ItemsView", panel.transform);
        Button previousItem = CreateButton(
            "PreviousItemButton", itemsView.transform, "<",
            new Vector2(0.5f, 0.5f), new Vector2(-139f, 4f),
            new Vector2(18f, 42f));
        Button nextItem = CreateButton(
            "NextItemButton", itemsView.transform, ">",
            new Vector2(0.5f, 0.5f), new Vector2(139f, 4f),
            new Vector2(18f, 42f));
        TextMeshProUGUI itemText = CreateText(
            "ItemText", itemsView.transform,
            "No usable items in the inventory.",
            new Vector2(0.5f, 0.5f), new Vector2(0f, 4f),
            new Vector2(250f, 42f), 8f,
            TextAlignmentOptions.Center);
        Button useItem = CreateButton(
            "UseItemButton", itemsView.transform, "Use",
            new Vector2(0.5f, 0.5f), new Vector2(0f, -32f),
            new Vector2(88f, 20f));

        GameObject equipmentView = CreateRect(
            "EquipmentView", panel.transform);
        Button previousSlot = CreateButton(
            "PreviousSlotButton", equipmentView.transform, "< Slot",
            new Vector2(0.5f, 0.5f), new Vector2(-91f, 14f),
            new Vector2(54f, 18f));
        Button nextSlot = CreateButton(
            "NextSlotButton", equipmentView.transform, "Slot >",
            new Vector2(0.5f, 0.5f), new Vector2(91f, 14f),
            new Vector2(54f, 18f));
        TextMeshProUGUI slotText = CreateText(
            "SlotText", equipmentView.transform, "SLOT  Weapon",
            new Vector2(0.5f, 0.5f), new Vector2(0f, 14f),
            new Vector2(110f, 16f), 8f,
            TextAlignmentOptions.Center);
        TextMeshProUGUI equippedText = CreateText(
            "EquippedText", equipmentView.transform, "Equipped: None",
            new Vector2(0.5f, 0.5f), new Vector2(0f, -4f),
            new Vector2(250f, 14f), 7f,
            TextAlignmentOptions.Center);
        Button previousEquipment = CreateButton(
            "PreviousEquipmentButton", equipmentView.transform, "<",
            new Vector2(0.5f, 0.5f), new Vector2(-139f, -25f),
            new Vector2(18f, 28f));
        Button nextEquipment = CreateButton(
            "NextEquipmentButton", equipmentView.transform, ">",
            new Vector2(0.5f, 0.5f), new Vector2(139f, -25f),
            new Vector2(18f, 28f));
        TextMeshProUGUI candidateText = CreateText(
            "CandidateText", equipmentView.transform,
            "No compatible equipment in the inventory.",
            new Vector2(0.5f, 0.5f), new Vector2(0f, -25f),
            new Vector2(250f, 28f), 7f,
            TextAlignmentOptions.Center);
        Button equip = CreateButton(
            "EquipButton", equipmentView.transform, "Equip",
            new Vector2(0.5f, 0.5f), new Vector2(-48f, -49f),
            new Vector2(84f, 18f));
        Button unequip = CreateButton(
            "UnequipButton", equipmentView.transform, "Unequip",
            new Vector2(0.5f, 0.5f), new Vector2(48f, -49f),
            new Vector2(84f, 18f));

        TextMeshProUGUI feedbackText = CreateText(
            "FeedbackText", panel.transform, "",
            new Vector2(0.5f, 0.5f), new Vector2(0f, -74f),
            new Vector2(290f, 14f), 7f,
            TextAlignmentOptions.Center);

        controller.Configure(
            menuAction, cancelAction, panel, itemsView, equipmentView,
            characterText, feedbackText, itemsTab, equipmentTab, close,
            previousCharacter, nextCharacter, itemText, previousItem,
            nextItem, useItem, slotText, equippedText, candidateText,
            previousSlot, nextSlot, previousEquipment, nextEquipment,
            equip, unequip);

        UnityEventTools.AddPersistentListener(itemsTab.onClick, controller.ShowItems);
        UnityEventTools.AddPersistentListener(equipmentTab.onClick, controller.ShowEquipment);
        UnityEventTools.AddPersistentListener(close.onClick, controller.Close);
        UnityEventTools.AddPersistentListener(previousCharacter.onClick, controller.PreviousCharacter);
        UnityEventTools.AddPersistentListener(nextCharacter.onClick, controller.NextCharacter);
        UnityEventTools.AddPersistentListener(previousItem.onClick, controller.PreviousItem);
        UnityEventTools.AddPersistentListener(nextItem.onClick, controller.NextItem);
        UnityEventTools.AddPersistentListener(useItem.onClick, controller.UseItem);
        UnityEventTools.AddPersistentListener(previousSlot.onClick, controller.PreviousSlot);
        UnityEventTools.AddPersistentListener(nextSlot.onClick, controller.NextSlot);
        UnityEventTools.AddPersistentListener(previousEquipment.onClick, controller.PreviousEquipment);
        UnityEventTools.AddPersistentListener(nextEquipment.onClick, controller.NextEquipment);
        UnityEventTools.AddPersistentListener(equip.onClick, controller.Equip);
        UnityEventTools.AddPersistentListener(unequip.onClick, controller.Unequip);

        equipmentView.SetActive(false);
        panel.SetActive(false);
    }

    private static InputActionReference FindActionReference(string actionPath)
    {
        InputActionAsset inputAsset =
            AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
        InputAction action = inputAsset?.FindAction(actionPath);
        if (action == null)
        {
            return null;
        }

        return AssetDatabase.LoadAllAssetsAtPath(InputActionsPath)
            .OfType<InputActionReference>()
            .FirstOrDefault(reference => reference.action != null &&
                reference.action.id == action.id);
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

        new GameObject(
            "EventSystem",
            typeof(EventSystem),
            typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
    }

    private static GameObject CreateRect(string name, Transform parent)
    {
        GameObject result = new GameObject(name, typeof(RectTransform));
        result.transform.SetParent(parent, false);
        SetRect(
            result.GetComponent<RectTransform>(),
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero);
        return result;
    }

    private static GameObject CreateImage(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 position,
        Vector2 size,
        Color color)
    {
        GameObject result = new GameObject(
            name,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));
        result.transform.SetParent(parent, false);
        SetRect(
            result.GetComponent<RectTransform>(),
            anchorMin,
            anchorMax,
            position,
            size);
        result.GetComponent<Image>().color = color;
        return result;
    }

    private static TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        string text,
        Vector2 anchor,
        Vector2 position,
        Vector2 size,
        float fontSize,
        TextAlignmentOptions alignment)
    {
        GameObject result = new GameObject(
            name,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        result.transform.SetParent(parent, false);
        TextMeshProUGUI label = result.GetComponent<TextMeshProUGUI>();
        label.text = text;
        label.font = TMP_Settings.defaultFontAsset;
        label.fontSize = fontSize;
        label.alignment = alignment;
        label.color = Color.white;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;
        SetRect(label.rectTransform, anchor, anchor, position, size);
        return label;
    }

    private static Button CreateButton(
        string name,
        Transform parent,
        string text,
        Vector2 anchor,
        Vector2 position,
        Vector2 size)
    {
        GameObject result = CreateImage(
            name,
            parent,
            anchor,
            anchor,
            position,
            size,
            new Color(0.78f, 0.8f, 0.76f, 1f));
        Button button = result.AddComponent<Button>();
        button.targetGraphic = result.GetComponent<Image>();
        TextMeshProUGUI label = CreateText(
            "Label",
            result.transform,
            text,
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            size,
            7f,
            TextAlignmentOptions.Center);
        label.color = new Color(0.06f, 0.07f, 0.08f, 1f);
        label.richText = false;
        return button;
    }

    private static void SetRect(
        RectTransform rect,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 position,
        Vector2 size)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void DestroyIfPresent(string objectName)
    {
        GameObject target = GameObject.Find(objectName);
        if (target != null)
        {
            Object.DestroyImmediate(target);
        }
    }
}
