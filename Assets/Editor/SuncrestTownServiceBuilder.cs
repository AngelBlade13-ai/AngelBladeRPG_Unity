using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SuncrestTownServiceBuilder
{
    private const string InputActionsPath =
        "Assets/Settings/InputSystem_Actions.inputactions";
    private const string PlaceholderSpritePath =
        "Assets/Tilemaps/Tiles/PlaceholderTileSprite.png";
    private const string ShopCanvasName = "TownShopCanvas";
    private const string RecoveryCanvasName = "TownRecoveryCanvas";
    private const string TestFixtureName = "TEMP_TEST_TownServicePoint";

    private static readonly ServiceFixture[] TestFixtures =
    {
        ServiceFixture.Shop(
            "Assets/Scenes/Suncrest/SuncrestWhisperMarketScene.unity",
            ShopCatalog.WhisperMarketId,
            new Vector3(-4f, 2f, 0f),
            new Color(0.15f, 0.8f, 0.85f, 1f)),
        ServiceFixture.Shop(
            "Assets/Scenes/Suncrest/SuncrestIronforgeScene.unity",
            ShopCatalog.IronforgeSmithyId,
            new Vector3(0f, 2f, 0f),
            new Color(0.95f, 0.5f, 0.15f, 1f)),
        ServiceFixture.Recovery(
            "Assets/Scenes/Suncrest/SuncrestInnScene.unity",
            "SUNCREST INN",
            DemoEconomyCatalog.TownRecoveryPrice,
            new Vector3(0f, 2f, 0f),
            new Color(0.25f, 0.55f, 0.95f, 1f)),
        ServiceFixture.Recovery(
            "Assets/Scenes/Suncrest/SuncrestShrineScene.unity",
            "SUNWELL SHRINE",
            DemoEconomyCatalog.TownRecoveryPrice,
            new Vector3(0f, 2f, 0f),
            new Color(0.95f, 0.85f, 0.35f, 1f))
    };

    [MenuItem(
        "Tools/AngelBlade RPG/World/Town Services/" +
        "Install Temporary Test Fixtures")]
    public static void InstallTemporaryTestFixtures()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        InputActionReference cancelAction = FindActionReference("UI/Cancel");
        if (cancelAction == null)
        {
            ShowResult(
                "Unity has not imported the UI/Cancel action reference. " +
                "Wait for compilation to finish and run this command again.");
            return;
        }

        foreach (ServiceFixture fixture in TestFixtures)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(fixture.ScenePath) ==
                null)
            {
                ShowResult($"Scene not found: {fixture.ScenePath}");
                return;
            }
        }

        string originalScenePath = SceneManager.GetActiveScene().path;
        try
        {
            foreach (ServiceFixture fixture in TestFixtures)
            {
                InstallTestFixture(fixture, cancelAction);
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

        ShowResult(
            "Installed four clearly labeled temporary service fixtures. " +
            "They are functional test objects, not permanent level design.");
    }

    private static void InstallTestFixture(
        ServiceFixture fixture,
        InputActionReference cancelAction)
    {
        Scene scene = EditorSceneManager.OpenScene(
            fixture.ScenePath,
            OpenSceneMode.Single);
        DestroyImmediateIfPresent(TestFixtureName);
        DestroyImmediateIfPresent(ShopCanvasName);
        DestroyImmediateIfPresent(RecoveryCanvasName);

        GameObject marker = CreateTestMarker(fixture);
        if (fixture.IsShop)
        {
            ShopPanel panel = CreateShopInterface(cancelAction);
            ShopInteractable2D interactable =
                marker.AddComponent<ShopInteractable2D>();
            interactable.Configure(fixture.ShopId, panel);
        }
        else
        {
            TownRecoveryPanel panel = CreateRecoveryInterface(cancelAction);
            TownRecoveryInteractable2D interactable =
                marker.AddComponent<TownRecoveryInteractable2D>();
            interactable.Configure(
                fixture.DisplayName,
                fixture.GoldCost,
                panel);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static GameObject CreateTestMarker(ServiceFixture fixture)
    {
        GameObject marker = new GameObject(TestFixtureName);
        marker.transform.position = fixture.Position;
        marker.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
        renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            PlaceholderSpritePath);
        renderer.color = fixture.MarkerColor;
        renderer.sortingOrder = 5;
        marker.AddComponent<BoxCollider2D>();
        return marker;
    }

    [MenuItem(
        "Tools/AngelBlade RPG/World/Town Services/" +
        "Wire Selected as Whisper Market Shop")]
    public static void WireWhisperMarketShop()
    {
        WireSelectedShop(ShopCatalog.WhisperMarketId, "Whisper Market");
    }

    [MenuItem(
        "Tools/AngelBlade RPG/World/Town Services/" +
        "Wire Selected as Ironforge Shop")]
    public static void WireIronforgeShop()
    {
        WireSelectedShop(ShopCatalog.IronforgeSmithyId, "Ironforge");
    }

    [MenuItem(
        "Tools/AngelBlade RPG/World/Town Services/" +
        "Wire Selected as Suncrest Inn Recovery")]
    public static void WireSuncrestInnRecovery()
    {
        WireSelectedRecovery(
            "SUNCREST INN",
            DemoEconomyCatalog.TownRecoveryPrice);
    }

    [MenuItem(
        "Tools/AngelBlade RPG/World/Town Services/" +
        "Wire Selected as Sunwell Shrine Recovery")]
    public static void WireSunwellShrineRecovery()
    {
        WireSelectedRecovery(
            "SUNWELL SHRINE",
            DemoEconomyCatalog.TownRecoveryPrice);
    }

    private static void WireSelectedShop(string shopId, string displayName)
    {
        GameObject target = GetSelectedSceneObject();
        if (target == null || HasConflictingRecoveryService(target) ||
            HasAnotherServiceInScene<ShopInteractable2D>(target, "shop"))
        {
            return;
        }

        InputActionReference cancelAction = FindActionReference("UI/Cancel");
        if (cancelAction == null)
        {
            ShowResult(
                "Unity has not imported the UI/Cancel action reference. " +
                "Wait for compilation to finish and run this command again.");
            return;
        }

        DestroyGeneratedCanvas(ShopCanvasName);
        ShopPanel panel = CreateShopInterface(cancelAction);
        ShopInteractable2D interactable =
            target.GetComponent<ShopInteractable2D>() ??
            Undo.AddComponent<ShopInteractable2D>(target);
        EnsureInteractionCollider(target);
        interactable.Configure(shopId, panel);
        EditorUtility.SetDirty(interactable);
        FinishWiring(target, $"{displayName} shop");
    }

    private static void WireSelectedRecovery(string displayName, int goldCost)
    {
        GameObject target = GetSelectedSceneObject();
        if (target == null || HasConflictingShopService(target) ||
            HasAnotherServiceInScene<TownRecoveryInteractable2D>(
                target,
                "recovery service"))
        {
            return;
        }

        InputActionReference cancelAction = FindActionReference("UI/Cancel");
        if (cancelAction == null)
        {
            ShowResult(
                "Unity has not imported the UI/Cancel action reference. " +
                "Wait for compilation to finish and run this command again.");
            return;
        }

        DestroyGeneratedCanvas(RecoveryCanvasName);
        TownRecoveryPanel panel = CreateRecoveryInterface(cancelAction);
        TownRecoveryInteractable2D interactable =
            target.GetComponent<TownRecoveryInteractable2D>() ??
            Undo.AddComponent<TownRecoveryInteractable2D>(target);
        EnsureInteractionCollider(target);
        interactable.Configure(displayName, goldCost, panel);
        EditorUtility.SetDirty(interactable);
        FinishWiring(target, $"{displayName} recovery service");
    }

    private static GameObject GetSelectedSceneObject()
    {
        GameObject target = Selection.activeGameObject;
        if (target != null && target.scene.IsValid() && target.scene.isLoaded)
        {
            return target;
        }

        ShowResult(
            "Select the scene object that should provide the service, then " +
            "run the command again. The tool will not place or style it.");
        return null;
    }

    private static bool HasConflictingRecoveryService(GameObject target)
    {
        if (target.GetComponent<TownRecoveryInteractable2D>() == null)
        {
            return false;
        }

        ShowResult(
            $"'{target.name}' is already a recovery service. Select a " +
            "different object or remove that component deliberately.");
        return true;
    }

    private static bool HasConflictingShopService(GameObject target)
    {
        if (target.GetComponent<ShopInteractable2D>() == null)
        {
            return false;
        }

        ShowResult(
            $"'{target.name}' is already a shop. Select a different object " +
            "or remove that component deliberately.");
        return true;
    }

    private static bool HasAnotherServiceInScene<T>(
        GameObject target,
        string serviceType)
        where T : Component
    {
        T existing = UnityEngine.Object.FindObjectsByType<T>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
            .FirstOrDefault(component =>
                component.gameObject != target &&
                component.gameObject.scene == target.scene);
        if (existing == null)
        {
            return false;
        }

        ShowResult(
            $"'{existing.gameObject.name}' is already wired as a " +
            $"{serviceType} in this scene. Select that object to update it, " +
            "or deliberately remove its component before choosing another.");
        return true;
    }

    private static void EnsureInteractionCollider(GameObject target)
    {
        if (target.GetComponent<Collider2D>() == null)
        {
            Undo.AddComponent<BoxCollider2D>(target);
        }
    }

    private static void FinishWiring(GameObject target, string serviceName)
    {
        EditorSceneManager.MarkSceneDirty(target.scene);
        Selection.activeGameObject = target;
        ShowResult(
            $"Wired '{target.name}' as the {serviceName}. Its transform and " +
            "visual components were not changed. Review the collider, test " +
            "the interaction, and save the scene when satisfied.");
    }

    private static ShopPanel CreateShopInterface(
        InputActionReference cancelAction)
    {
        GameObject canvasObject = CreateCanvas(ShopCanvasName);
        ShopPanel controller = canvasObject.AddComponent<ShopPanel>();
        GameObject panel = CreatePanel(
            "ShopPanel",
            canvasObject.transform);

        TextMeshProUGUI title = CreateText(
            "TitleText", panel.transform, "SHOP",
            new Vector2(0.5f, 0.5f), new Vector2(0f, 77f),
            new Vector2(220f, 14f), 10f, TextAlignmentOptions.Center);
        TextMeshProUGUI gold = CreateText(
            "GoldText", panel.transform, "Gold  0",
            new Vector2(0.5f, 0.5f), new Vector2(0f, 61f),
            new Vector2(220f, 12f), 8f, TextAlignmentOptions.Center);
        Button close = CreateButton(
            "CloseButton", panel.transform, "X",
            new Vector2(0.5f, 0.5f), new Vector2(146f, 77f),
            new Vector2(20f, 16f));
        Button buyTab = CreateButton(
            "BuyTabButton", panel.transform, "Buy",
            new Vector2(0.5f, 0.5f), new Vector2(-38f, 44f),
            new Vector2(72f, 18f));
        Button sellTab = CreateButton(
            "SellTabButton", panel.transform, "Sell",
            new Vector2(0.5f, 0.5f), new Vector2(38f, 44f),
            new Vector2(72f, 18f));

        Button previousItem = CreateButton(
            "PreviousItemButton", panel.transform, "<",
            new Vector2(0.5f, 0.5f), new Vector2(-139f, 5f),
            new Vector2(18f, 56f));
        Button nextItem = CreateButton(
            "NextItemButton", panel.transform, ">",
            new Vector2(0.5f, 0.5f), new Vector2(139f, 5f),
            new Vector2(18f, 56f));
        TextMeshProUGUI item = CreateText(
            "ItemText", panel.transform, "Shop stock",
            new Vector2(0.5f, 0.5f), new Vector2(0f, 5f),
            new Vector2(250f, 56f), 7f, TextAlignmentOptions.Center);

        Button decreaseQuantity = CreateButton(
            "DecreaseQuantityButton", panel.transform, "-",
            new Vector2(0.5f, 0.5f), new Vector2(-70f, -37f),
            new Vector2(24f, 18f));
        Button increaseQuantity = CreateButton(
            "IncreaseQuantityButton", panel.transform, "+",
            new Vector2(0.5f, 0.5f), new Vector2(-40f, -37f),
            new Vector2(24f, 18f));
        Button transact = CreateButton(
            "TransactButton", panel.transform, "Buy",
            new Vector2(0.5f, 0.5f), new Vector2(45f, -37f),
            new Vector2(100f, 20f));
        TextMeshProUGUI transactText =
            transact.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI feedback = CreateText(
            "FeedbackText", panel.transform, "",
            new Vector2(0.5f, 0.5f), new Vector2(0f, -68f),
            new Vector2(290f, 18f), 7f, TextAlignmentOptions.Center);

        controller.Configure(
            cancelAction,
            panel,
            title,
            gold,
            item,
            feedback,
            buyTab,
            sellTab,
            previousItem,
            nextItem,
            decreaseQuantity,
            increaseQuantity,
            transact,
            transactText,
            close);

        UnityEventTools.AddPersistentListener(
            buyTab.onClick, controller.ShowBuy);
        UnityEventTools.AddPersistentListener(
            sellTab.onClick, controller.ShowSell);
        UnityEventTools.AddPersistentListener(
            previousItem.onClick, controller.PreviousItem);
        UnityEventTools.AddPersistentListener(
            nextItem.onClick, controller.NextItem);
        UnityEventTools.AddPersistentListener(
            decreaseQuantity.onClick, controller.DecreaseQuantity);
        UnityEventTools.AddPersistentListener(
            increaseQuantity.onClick, controller.IncreaseQuantity);
        UnityEventTools.AddPersistentListener(
            transact.onClick, controller.Transact);
        UnityEventTools.AddPersistentListener(close.onClick, controller.Close);

        panel.SetActive(false);
        return controller;
    }

    private static TownRecoveryPanel CreateRecoveryInterface(
        InputActionReference cancelAction)
    {
        GameObject canvasObject = CreateCanvas(RecoveryCanvasName);
        TownRecoveryPanel controller =
            canvasObject.AddComponent<TownRecoveryPanel>();
        GameObject panel = CreatePanel(
            "RecoveryPanel",
            canvasObject.transform);

        TextMeshProUGUI title = CreateText(
            "TitleText", panel.transform, "RECOVERY",
            new Vector2(0.5f, 0.5f), new Vector2(0f, 56f),
            new Vector2(240f, 16f), 11f, TextAlignmentOptions.Center);
        TextMeshProUGUI details = CreateText(
            "DetailsText", panel.transform,
            $"Full party recovery: {DemoEconomyCatalog.TownRecoveryPrice} gold",
            new Vector2(0.5f, 0.5f), new Vector2(0f, 10f),
            new Vector2(270f, 64f), 8f, TextAlignmentOptions.Center);
        Button recover = CreateButton(
            "RecoverButton", panel.transform, "Recover",
            new Vector2(0.5f, 0.5f), new Vector2(-53f, -38f),
            new Vector2(96f, 22f));
        Button cancel = CreateButton(
            "CancelButton", panel.transform, "Cancel",
            new Vector2(0.5f, 0.5f), new Vector2(53f, -38f),
            new Vector2(96f, 22f));
        TextMeshProUGUI feedback = CreateText(
            "FeedbackText", panel.transform, "",
            new Vector2(0.5f, 0.5f), new Vector2(0f, -68f),
            new Vector2(280f, 18f), 7f, TextAlignmentOptions.Center);

        controller.Configure(
            cancelAction,
            panel,
            title,
            details,
            feedback,
            recover,
            cancel);
        UnityEventTools.AddPersistentListener(
            recover.onClick, controller.PurchaseRecovery);
        UnityEventTools.AddPersistentListener(cancel.onClick, controller.Close);

        panel.SetActive(false);
        return controller;
    }

    private static GameObject CreateCanvas(string name)
    {
        GameObject canvasObject = new GameObject(
            name,
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(
            canvasObject,
            $"Create {name}");
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 120;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(320f, 180f);
        scaler.matchWidthOrHeight = 0.5f;
        return canvasObject;
    }

    private static GameObject CreatePanel(string name, Transform parent)
    {
        return CreateImage(
            name,
            parent,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero,
            new Color(0.035f, 0.045f, 0.05f, 0.98f));
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

    private static void DestroyGeneratedCanvas(string objectName)
    {
        GameObject target = GameObject.Find(objectName);
        if (target != null)
        {
            Undo.DestroyObjectImmediate(target);
        }
    }

    private static void DestroyImmediateIfPresent(string objectName)
    {
        GameObject target = GameObject.Find(objectName);
        if (target != null)
        {
            UnityEngine.Object.DestroyImmediate(target);
        }
    }

    private static void ShowResult(string message)
    {
        Debug.Log(message);
        EditorUtility.DisplayDialog("Suncrest Town Services", message, "OK");
    }

    private sealed class ServiceFixture
    {
        public string ScenePath { get; }
        public string ShopId { get; }
        public string DisplayName { get; }
        public int GoldCost { get; }
        public Vector3 Position { get; }
        public Color MarkerColor { get; }
        public bool IsShop => ShopId != null;

        private ServiceFixture(
            string scenePath,
            string shopId,
            string displayName,
            int goldCost,
            Vector3 position,
            Color markerColor)
        {
            ScenePath = scenePath;
            ShopId = shopId;
            DisplayName = displayName;
            GoldCost = goldCost;
            Position = position;
            MarkerColor = markerColor;
        }

        public static ServiceFixture Shop(
            string scenePath,
            string shopId,
            Vector3 position,
            Color markerColor)
        {
            return new ServiceFixture(
                scenePath,
                shopId,
                null,
                0,
                position,
                markerColor);
        }

        public static ServiceFixture Recovery(
            string scenePath,
            string displayName,
            int goldCost,
            Vector3 position,
            Color markerColor)
        {
            return new ServiceFixture(
                scenePath,
                null,
                displayName,
                goldCost,
                position,
                markerColor);
        }
    }
}
