using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BattlePrototypeBuilder
{
    private const string BattleScenePath = "Assets/Scenes/BattleScene.unity";
    private const string GuildHallScenePath =
        "Assets/Scenes/Suncrest/SuncrestGuildHallScene.unity";
    private const string PlaceholderSpritePath =
        "Assets/Tilemaps/Tiles/PlaceholderTileSprite.png";

    [MenuItem("Tools/AngelBlade RPG/Battle/Add Caravan Tutorial Test Encounter")]
    public static void AddCaravanTutorialTestEncounter()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(
            GuildHallScenePath,
            OpenSceneMode.Single);
        EnsureEncounterGroup(
            "CaravanTutorialTestEncounter",
            new Vector3(-2f, 2f, 0f),
            new Color(0.9f, 0.55f, 0.12f, 1f),
            BattleEncounterCatalog.CaravanTutorialId,
            "Default");
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AddSceneToBuildList(BattleScenePath);
        Selection.activeGameObject =
            GameObject.Find("CaravanTutorialTestEncounter");
        SceneView.lastActiveSceneView?.FrameSelected();
        EditorUtility.DisplayDialog(
            "Caravan Tutorial",
            "The orange tutorial test marker is ready in the Guild Hall district.",
            "OK");
    }

    [MenuItem("Tools/AngelBlade RPG/Battle/Repair Battle Scene Interface")]
    public static void BuildPlaceholderBattleScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        Scene previouslyActiveScene = EditorSceneManager.GetActiveScene();
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BattleScenePath) != null)
        {
            RepairBattleInterface(previouslyActiveScene);
            AddSceneToBuildList(BattleScenePath);
            EditorUtility.DisplayDialog(
                "Battle Scene Interface",
                "BattleScene combat commands and keyboard/gamepad navigation were repaired without changing any exploration scene.",
                "OK");
            return;
        }

        GameObject sourceEventSystem = GameObject.Find("EventSystem");
        Scene battleScene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene,
            NewSceneMode.Additive);
        EditorSceneManager.SetActiveScene(battleScene);

        CreateCamera();
        CreateEventSystem(sourceEventSystem, battleScene);
        CreateBattleInterface();

        EditorSceneManager.SaveScene(battleScene, BattleScenePath);
        AddSceneToBuildList(BattleScenePath);
        EditorSceneManager.SetActiveScene(previouslyActiveScene);
        EditorSceneManager.CloseScene(battleScene, true);

        EditorUtility.DisplayDialog(
            "Battle Scene Interface",
            "BattleScene was created and registered without changing any exploration scene.",
            "OK");
    }

    private static Transform FindChildByName(Transform parent, string objectName)
    {
        if (parent.name == objectName)
        {
            return parent;
        }

        foreach (Transform child in parent)
        {
            Transform result = FindChildByName(child, objectName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static void EnsureEncounterGroup(
        string objectName,
        Vector3 position,
        Color color,
        string encounterId,
        string returnSpawnId = "TownAfterBattle")
    {
        GameObject encounterObject = EnsureEncounterObject(
            objectName,
            position,
            color);
        BattleEncounterInteractable2D encounter =
            encounterObject.GetComponent<BattleEncounterInteractable2D>();
        if (encounter == null)
        {
            encounter =
                encounterObject.AddComponent<BattleEncounterInteractable2D>();
        }

        encounter.ConfigureEncounterGroup(
            "BattleScene",
            returnSpawnId,
            encounterId);
    }

    private static GameObject EnsureEncounterObject(
        string objectName,
        Vector3 position,
        Color color)
    {
        GameObject encounterObject = GameObject.Find(objectName);
        if (encounterObject == null)
        {
            encounterObject = new GameObject(objectName);
            encounterObject.transform.localScale =
                new Vector3(0.8f, 0.8f, 1f);
            encounterObject.AddComponent<BoxCollider2D>();
        }

        encounterObject.transform.position = position;
        SpriteRenderer renderer =
            encounterObject.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = encounterObject.AddComponent<SpriteRenderer>();
        }

        renderer.sprite = GetPlaceholderSprite();
        renderer.color = color;
        return encounterObject;
    }

    private static void CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.12f, 0.16f, 0.15f, 1f);
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
    }

    private static void CreateEventSystem(
        GameObject sourceEventSystem,
        Scene battleScene)
    {
        if (sourceEventSystem == null)
        {
            return;
        }

        GameObject eventSystem = Object.Instantiate(sourceEventSystem);
        eventSystem.name = "EventSystem";
        SceneManager.MoveGameObjectToScene(eventSystem, battleScene);
    }

    private static void CreateBattleInterface()
    {
        GameObject controllerObject = new GameObject("BattleController");
        BattleSceneController controller =
            controllerObject.AddComponent<BattleSceneController>();

        GameObject canvasObject = new GameObject(
            "BattleCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(320f, 180f);
        scaler.matchWidthOrHeight = 0.5f;

        CreateImage(
            "Backdrop",
            canvasObject.transform,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero,
            new Color(0.18f, 0.25f, 0.22f, 1f));

        CreateText(
            "BattleTitle",
            canvasObject.transform,
            "BATTLE",
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(0f, -7f),
            new Vector2(100f, 18f),
            11f,
            TextAlignmentOptions.Center);

        TextMeshProUGUI playerStatus = CreateText(
            "PlayerStatusText",
            canvasObject.transform,
            "> Hero  HP 100/100 MP 20/20",
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(8f, -25f),
            new Vector2(154f, 48f),
            7f,
            TextAlignmentOptions.TopLeft,
            new Vector2(0f, 1f));
        TextMeshProUGUI monsterStatus = CreateText(
            "MonsterStatusText",
            canvasObject.transform,
            "* Goblin  HP 35/35 MP 0/0",
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(-8f, -25f),
            new Vector2(142f, 48f),
            7f,
            TextAlignmentOptions.TopRight,
            Vector2.one);

        CreateImage(
            "PlayerPlaceholder",
            canvasObject.transform,
            new Vector2(0.25f, 0.57f),
            new Vector2(0.25f, 0.57f),
            Vector2.zero,
            new Vector2(34f, 48f),
            new Color(0.72f, 0.84f, 0.9f, 1f));
        CreateImage(
            "MonsterPlaceholder",
            canvasObject.transform,
            new Vector2(0.74f, 0.6f),
            new Vector2(0.74f, 0.6f),
            Vector2.zero,
            new Vector2(42f, 42f),
            new Color(0.72f, 0.25f, 0.22f, 1f));

        GameObject commandBand = CreateImage(
            "CommandBand",
            canvasObject.transform,
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            Vector2.zero,
            new Vector2(0f, 78f),
            new Color(0.08f, 0.09f, 0.1f, 0.96f),
            new Vector2(0.5f, 0f));
        TextMeshProUGUI battleLog = CreateText(
            "BattleLogText",
            commandBand.transform,
            "A monster appears!",
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero,
            7f,
            TextAlignmentOptions.TopLeft);
        RectTransform logRect = battleLog.rectTransform;
        logRect.offsetMin = new Vector2(8f, 7f);
        logRect.offsetMax = new Vector2(-180f, -7f);

        GameObject attackButton = CreateButton(
            "AttackButton",
            commandBand.transform,
            "Attack",
            new Vector2(1f, 0.5f),
            new Vector2(-158f, 12f),
            new Vector2(34f, 22f));
        GameObject abilityButton = CreateButton(
            "AbilityButton",
            commandBand.transform,
            "Ability",
            new Vector2(1f, 0.5f),
            new Vector2(-122f, 12f),
            new Vector2(34f, 22f));
        GameObject itemButton = CreateButton(
            "ItemButton",
            commandBand.transform,
            "Item",
            new Vector2(1f, 0.5f),
            new Vector2(-86f, 12f),
            new Vector2(34f, 22f));
        GameObject defendButton = CreateButton(
            "DefendButton",
            commandBand.transform,
            "Defend",
            new Vector2(1f, 0.5f),
            new Vector2(-50f, 12f),
            new Vector2(34f, 22f));
        GameObject escapeButton = CreateButton(
            "EscapeButton",
            commandBand.transform,
            "Escape",
            new Vector2(1f, 0.5f),
            new Vector2(-14f, 12f),
            new Vector2(34f, 22f));
        GameObject continueButton = CreateButton(
            "ContinueButton",
            commandBand.transform,
            "Return",
            new Vector2(1f, 0.5f),
            new Vector2(-46f, 12f),
            new Vector2(80f, 22f));
        TextMeshProUGUI continueLabel =
            continueButton.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI commandPrompt = CreateText(
            "CommandPromptText",
            commandBand.transform,
            "Hero -> Goblin",
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(-80f, 5f),
            new Vector2(100f, 17f),
            7f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0f));
        GameObject previousTargetButton = CreateButton(
            "PreviousTargetButton",
            commandBand.transform,
            "<",
            new Vector2(1f, 0f),
            new Vector2(-146f, 12f),
            new Vector2(24f, 18f));
        GameObject nextTargetButton = CreateButton(
            "NextTargetButton",
            commandBand.transform,
            ">",
            new Vector2(1f, 0f),
            new Vector2(-14f, 12f),
            new Vector2(24f, 18f));

        LinkNavigation(attackButton, null, previousTargetButton, null, abilityButton);
        LinkNavigation(abilityButton, null, nextTargetButton, attackButton, itemButton);
        LinkNavigation(itemButton, null, nextTargetButton, abilityButton, defendButton);
        LinkNavigation(defendButton, null, nextTargetButton, itemButton, escapeButton);
        LinkNavigation(escapeButton, null, nextTargetButton, defendButton, null);
        LinkNavigation(previousTargetButton, attackButton, null, null, nextTargetButton);
        LinkNavigation(nextTargetButton, itemButton, null, previousTargetButton, null);

        UnityEventTools.AddPersistentListener(
            attackButton.GetComponent<Button>().onClick,
            controller.Attack);
        UnityEventTools.AddPersistentListener(
            abilityButton.GetComponent<Button>().onClick,
            controller.Ability);
        UnityEventTools.AddPersistentListener(
            itemButton.GetComponent<Button>().onClick,
            controller.Item);
        UnityEventTools.AddPersistentListener(
            defendButton.GetComponent<Button>().onClick,
            controller.Defend);
        UnityEventTools.AddPersistentListener(
            escapeButton.GetComponent<Button>().onClick,
            controller.Escape);
        UnityEventTools.AddPersistentListener(
            previousTargetButton.GetComponent<Button>().onClick,
            controller.PreviousTarget);
        UnityEventTools.AddPersistentListener(
            nextTargetButton.GetComponent<Button>().onClick,
            controller.NextTarget);
        UnityEventTools.AddPersistentListener(
            continueButton.GetComponent<Button>().onClick,
            controller.Continue);

        continueButton.SetActive(false);
        controller.Configure(
            playerStatus,
            monsterStatus,
            battleLog,
            commandPrompt,
            continueLabel,
            attackButton,
            abilityButton,
            itemButton,
            defendButton,
            escapeButton,
            previousTargetButton,
            nextTargetButton,
            continueButton);
    }

    private static void RepairBattleInterface(Scene previouslyActiveScene)
    {
        bool battleSceneAlreadyActive =
            previouslyActiveScene.path == BattleScenePath;
        Scene battleScene = battleSceneAlreadyActive
            ? previouslyActiveScene
            : EditorSceneManager.OpenScene(
                BattleScenePath,
                OpenSceneMode.Additive);
        EditorSceneManager.SetActiveScene(battleScene);

        BattleSceneController controller =
            FindSceneObject(battleScene, "BattleController")
                .GetComponent<BattleSceneController>();
        Transform commandBand =
            FindSceneObject(battleScene, "CommandBand");
        RectTransform commandBandRect =
            commandBand.GetComponent<RectTransform>();
        commandBandRect.sizeDelta = new Vector2(
            commandBandRect.sizeDelta.x,
            78f);
        GameObject attackButton =
            FindSceneObject(battleScene, "AttackButton").gameObject;
        Transform abilityTransform =
            FindSceneObject(battleScene, "AbilityButton");
        bool createdAbilityButton = abilityTransform == null;
        GameObject abilityButton = createdAbilityButton
            ? CreateButton(
                "AbilityButton",
                commandBand,
                "Ability",
                new Vector2(1f, 0.5f),
                new Vector2(-106f, 12f),
                new Vector2(40f, 22f))
            : abilityTransform.gameObject;
        Transform itemTransform =
            FindSceneObject(battleScene, "ItemButton");
        bool createdItemButton = itemTransform == null;
        GameObject itemButton = createdItemButton
            ? CreateButton(
                "ItemButton",
                commandBand,
                "Item",
                new Vector2(1f, 0.5f),
                new Vector2(-86f, 12f),
                new Vector2(34f, 22f))
            : itemTransform.gameObject;
        Transform defendTransform =
            FindSceneObject(battleScene, "DefendButton");
        bool createdDefendButton = defendTransform == null;
        GameObject defendButton = createdDefendButton
            ? CreateButton(
                "DefendButton",
                commandBand,
                "Defend",
                new Vector2(1f, 0.5f),
                new Vector2(-50f, 12f),
                new Vector2(34f, 22f))
            : defendTransform.gameObject;
        GameObject escapeButton =
            FindSceneObject(battleScene, "EscapeButton").gameObject;
        GameObject continueButton =
            FindSceneObject(battleScene, "ContinueButton").gameObject;
        TextMeshProUGUI playerStatus =
            FindSceneObject(battleScene, "PlayerStatusText")
                .GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI monsterStatus =
            FindSceneObject(battleScene, "MonsterStatusText")
                .GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI battleLog =
            FindSceneObject(battleScene, "BattleLogText")
                .GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI continueLabel =
            continueButton.GetComponentInChildren<TextMeshProUGUI>();
        Transform promptTransform =
            FindSceneObject(battleScene, "CommandPromptText");
        TextMeshProUGUI commandPrompt = promptTransform == null
            ? CreateText(
                "CommandPromptText",
                commandBand,
                "Hero -> Goblin",
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-80f, 5f),
                new Vector2(100f, 17f),
                7f,
                TextAlignmentOptions.Center,
                new Vector2(0.5f, 0f))
            : promptTransform.GetComponent<TextMeshProUGUI>();
        Transform previousTargetTransform =
            FindSceneObject(battleScene, "PreviousTargetButton");
        bool createdPreviousTarget = previousTargetTransform == null;
        GameObject previousTargetButton = createdPreviousTarget
            ? CreateButton(
                "PreviousTargetButton",
                commandBand,
                "<",
                new Vector2(1f, 0f),
                new Vector2(-146f, 12f),
                new Vector2(24f, 18f))
            : previousTargetTransform.gameObject;
        Transform nextTargetTransform =
            FindSceneObject(battleScene, "NextTargetButton");
        bool createdNextTarget = nextTargetTransform == null;
        GameObject nextTargetButton = createdNextTarget
            ? CreateButton(
                "NextTargetButton",
                commandBand,
                ">",
                new Vector2(1f, 0f),
                new Vector2(-14f, 12f),
                new Vector2(24f, 18f))
            : nextTargetTransform.gameObject;

        SetRect(
            attackButton.GetComponent<RectTransform>(),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(-158f, 12f),
            new Vector2(34f, 22f),
            new Vector2(0.5f, 0.5f));
        SetRect(
            abilityButton.GetComponent<RectTransform>(),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(-122f, 12f),
            new Vector2(34f, 22f),
            new Vector2(0.5f, 0.5f));
        SetRect(
            itemButton.GetComponent<RectTransform>(),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(-86f, 12f),
            new Vector2(34f, 22f),
            new Vector2(0.5f, 0.5f));
        SetRect(
            defendButton.GetComponent<RectTransform>(),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(-50f, 12f),
            new Vector2(34f, 22f),
            new Vector2(0.5f, 0.5f));
        SetRect(
            escapeButton.GetComponent<RectTransform>(),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(-14f, 12f),
            new Vector2(34f, 22f),
            new Vector2(0.5f, 0.5f));
        battleLog.rectTransform.offsetMax = new Vector2(-180f, -7f);
        battleLog.fontSize = 7f;
        SetRect(
            playerStatus.rectTransform,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(8f, -25f),
            new Vector2(154f, 48f),
            new Vector2(0f, 1f));
        playerStatus.fontSize = 7f;
        SetRect(
            monsterStatus.rectTransform,
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(-8f, -25f),
            new Vector2(142f, 48f),
            Vector2.one);
        monsterStatus.fontSize = 7f;
        SetRect(
            commandPrompt.rectTransform,
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(-80f, 5f),
            new Vector2(100f, 17f),
            new Vector2(0.5f, 0f));
        commandPrompt.fontSize = 7f;
        SetRect(
            previousTargetButton.GetComponent<RectTransform>(),
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(-146f, 12f),
            new Vector2(24f, 18f),
            new Vector2(0.5f, 0.5f));
        SetRect(
            nextTargetButton.GetComponent<RectTransform>(),
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(-14f, 12f),
            new Vector2(24f, 18f),
            new Vector2(0.5f, 0.5f));

        if (createdDefendButton)
        {
            UnityEventTools.AddPersistentListener(
                defendButton.GetComponent<Button>().onClick,
                controller.Defend);
        }

        if (createdAbilityButton)
        {
            UnityEventTools.AddPersistentListener(
                abilityButton.GetComponent<Button>().onClick,
                controller.Ability);
        }

        if (createdItemButton)
        {
            UnityEventTools.AddPersistentListener(
                itemButton.GetComponent<Button>().onClick,
                controller.Item);
        }

        if (createdPreviousTarget)
        {
            UnityEventTools.AddPersistentListener(
                previousTargetButton.GetComponent<Button>().onClick,
                controller.PreviousTarget);
        }

        if (createdNextTarget)
        {
            UnityEventTools.AddPersistentListener(
                nextTargetButton.GetComponent<Button>().onClick,
                controller.NextTarget);
        }

        LinkNavigation(attackButton, null, previousTargetButton, null, abilityButton);
        LinkNavigation(abilityButton, null, nextTargetButton, attackButton, itemButton);
        LinkNavigation(itemButton, null, nextTargetButton, abilityButton, defendButton);
        LinkNavigation(defendButton, null, nextTargetButton, itemButton, escapeButton);
        LinkNavigation(escapeButton, null, nextTargetButton, defendButton, null);
        LinkNavigation(previousTargetButton, attackButton, null, null, nextTargetButton);
        LinkNavigation(nextTargetButton, itemButton, null, previousTargetButton, null);

        controller.Configure(
            playerStatus,
            monsterStatus,
            battleLog,
            commandPrompt,
            continueLabel,
            attackButton,
            abilityButton,
            itemButton,
            defendButton,
            escapeButton,
            previousTargetButton,
            nextTargetButton,
            continueButton);

        EditorSceneManager.MarkSceneDirty(battleScene);
        EditorSceneManager.SaveScene(battleScene);
        if (!battleSceneAlreadyActive)
        {
            EditorSceneManager.SetActiveScene(previouslyActiveScene);
            EditorSceneManager.CloseScene(battleScene, true);
        }
    }

    private static Transform FindSceneObject(Scene scene, string objectName)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform result = FindChildByName(root.transform, objectName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static GameObject CreateImage(
        string objectName,
        Transform parent,
        Vector2 anchorMinimum,
        Vector2 anchorMaximum,
        Vector2 anchoredPosition,
        Vector2 size,
        Color color,
        Vector2? pivot = null)
    {
        GameObject imageObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));
        imageObject.transform.SetParent(parent, false);
        SetRect(
            imageObject.GetComponent<RectTransform>(),
            anchorMinimum,
            anchorMaximum,
            anchoredPosition,
            size,
            pivot ?? new Vector2(0.5f, 0.5f));
        imageObject.GetComponent<Image>().color = color;
        return imageObject;
    }

    private static TextMeshProUGUI CreateText(
        string objectName,
        Transform parent,
        string text,
        Vector2 anchorMinimum,
        Vector2 anchorMaximum,
        Vector2 anchoredPosition,
        Vector2 size,
        float fontSize,
        TextAlignmentOptions alignment,
        Vector2? pivot = null)
    {
        GameObject textObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI textComponent =
            textObject.GetComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.font = TMP_Settings.defaultFontAsset;
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = Color.white;
        textComponent.raycastTarget = false;
        textComponent.textWrappingMode = TextWrappingModes.Normal;
        SetRect(
            textComponent.rectTransform,
            anchorMinimum,
            anchorMaximum,
            anchoredPosition,
            size,
            pivot ?? new Vector2(0.5f, 0.5f));
        return textComponent;
    }

    private static GameObject CreateButton(
        string objectName,
        Transform parent,
        string label,
        Vector2 anchor,
        Vector2 anchoredPosition,
        Vector2? size = null)
    {
        GameObject buttonObject = CreateImage(
            objectName,
            parent,
            anchor,
            anchor,
            anchoredPosition,
            size ?? new Vector2(48f, 22f),
            new Color(0.78f, 0.8f, 0.76f, 1f));
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();

        TextMeshProUGUI buttonText = CreateText(
            "Label",
            buttonObject.transform,
            label,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero,
            9f,
            TextAlignmentOptions.Center);
        buttonText.color = new Color(0.08f, 0.09f, 0.1f, 1f);
        buttonText.richText = false;
        return buttonObject;
    }

    private static void LinkNavigation(
        GameObject target,
        GameObject up,
        GameObject down,
        GameObject left,
        GameObject right)
    {
        if (target == null || !target.TryGetComponent(out Selectable selectable))
        {
            return;
        }

        Navigation navigation = selectable.navigation;
        navigation.mode = Navigation.Mode.Explicit;
        navigation.selectOnUp = up == null ? null : up.GetComponent<Selectable>();
        navigation.selectOnDown = down == null ? null : down.GetComponent<Selectable>();
        navigation.selectOnLeft = left == null ? null : left.GetComponent<Selectable>();
        navigation.selectOnRight = right == null ? null : right.GetComponent<Selectable>();
        selectable.navigation = navigation;
    }

    private static void SetRect(
        RectTransform rect,
        Vector2 anchorMinimum,
        Vector2 anchorMaximum,
        Vector2 anchoredPosition,
        Vector2 size,
        Vector2 pivot)
    {
        rect.anchorMin = anchorMinimum;
        rect.anchorMax = anchorMaximum;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
    }

    private static Sprite GetPlaceholderSprite()
    {
        Sprite placeholder = AssetDatabase.LoadAssetAtPath<Sprite>(
            PlaceholderSpritePath);
        if (placeholder != null)
        {
            return placeholder;
        }

        GameObject sign = GameObject.Find("TestSign");
        SpriteRenderer signRenderer = sign == null
            ? null
            : sign.GetComponent<SpriteRenderer>();
        return signRenderer == null ? null : signRenderer.sprite;
    }

    private static void AddSceneToBuildList(string scenePath)
    {
        List<EditorBuildSettingsScene> scenes =
            new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (scene.path == scenePath)
            {
                return;
            }
        }

        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
