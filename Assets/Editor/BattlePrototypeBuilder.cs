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
    private const string TownScenePath = "Assets/Scenes/TownScene.unity";
    private const string BattleScenePath = "Assets/Scenes/BattleScene.unity";
    private const string MainScenePath = "Assets/Scenes/MainGameScene.unity";

    [MenuItem("Tools/AngelBlade RPG/Build Placeholder Battle Scene")]
    public static void BuildPlaceholderBattleScene()
    {
        Scene townScene = EditorSceneManager.GetActiveScene();
        if (townScene.path != TownScenePath)
        {
            EditorUtility.DisplayDialog(
                "Placeholder Battle Scene",
                "Open TownScene before building the battle prototype.",
                "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        EnsureTownEncounter();
        EditorSceneManager.MarkSceneDirty(townScene);
        EditorSceneManager.SaveScene(townScene);
        RemoveLegacyPanels(townScene);

        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BattleScenePath) != null)
        {
            RepairBattleInterface(townScene);
            AddSceneToBuildList(BattleScenePath);
            EditorUtility.DisplayDialog(
                "Placeholder Battle Scene",
                "BattleScene already exists. Encounters, return spawn, and combat commands were repaired without overwriting the scene.",
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
        EditorSceneManager.SetActiveScene(townScene);
        EditorSceneManager.CloseScene(battleScene, true);

        EditorUtility.DisplayDialog(
            "Placeholder Battle Scene",
            "BattleScene and the town Goblin encounter are ready. Start from MainGameScene to test the full battle loop.",
            "OK");
    }

    private static void RemoveLegacyPanels(Scene townScene)
    {
        Scene mainScene = EditorSceneManager.OpenScene(
            MainScenePath,
            OpenSceneMode.Additive);
        DestroySceneObject(mainScene, "TownPanel");
        DestroySceneObject(mainScene, "BattlePanel");
        EditorSceneManager.MarkSceneDirty(mainScene);
        EditorSceneManager.SaveScene(mainScene);
        EditorSceneManager.SetActiveScene(townScene);
        EditorSceneManager.CloseScene(mainScene, true);
    }

    private static void DestroySceneObject(Scene scene, string objectName)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform target = FindChildByName(root.transform, objectName);
            if (target != null)
            {
                Object.DestroyImmediate(target.gameObject);
                return;
            }
        }
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

    private static void EnsureTownEncounter()
    {
        PlayerSpawnPoint2D returnSpawn = EnsureReturnSpawn();
        AddSpawnToController(returnSpawn);

        EnsureEncounter(
            "BattleTestEncounter",
            new Vector3(0f, 3f, 0f),
            new Color(0.75f, 0.2f, 0.22f, 1f),
            "monster_goblin");
        EnsureEncounter(
            "BattleDefeatTestEncounter",
            new Vector3(3f, 3f, 0f),
            new Color(0.45f, 0.16f, 0.55f, 1f),
            "monster_ogre");
        EnsureEncounter(
            "BattleSpeedTestEncounter",
            new Vector3(-3f, 3f, 0f),
            new Color(0.16f, 0.7f, 0.78f, 1f),
            "monster_wisp");
    }

    private static void EnsureEncounter(
        string objectName,
        Vector3 position,
        Color color,
        string monsterId)
    {
        GameObject encounterObject = GameObject.Find(objectName);
        if (encounterObject == null)
        {
            encounterObject = new GameObject(objectName);
            encounterObject.transform.position = position;
            encounterObject.transform.localScale =
                new Vector3(0.8f, 0.8f, 1f);

            SpriteRenderer renderer =
                encounterObject.AddComponent<SpriteRenderer>();
            renderer.sprite = GetPlaceholderSprite();
            renderer.color = color;
            encounterObject.AddComponent<BoxCollider2D>();
        }

        BattleEncounterInteractable2D encounter =
            encounterObject.GetComponent<BattleEncounterInteractable2D>();
        if (encounter == null)
        {
            encounter =
                encounterObject.AddComponent<BattleEncounterInteractable2D>();
        }

        encounter.Configure(
            "BattleScene",
            "TownAfterBattle",
            monsterId);
    }

    private static PlayerSpawnPoint2D EnsureReturnSpawn()
    {
        GameObject spawnObject = GameObject.Find("TownAfterBattleSpawn");
        if (spawnObject == null)
        {
            spawnObject = new GameObject("TownAfterBattleSpawn");
            spawnObject.AddComponent<PlayerSpawnPoint2D>();
        }

        spawnObject.transform.position = new Vector3(0f, 1.5f, 0f);
        PlayerSpawnPoint2D spawn =
            spawnObject.GetComponent<PlayerSpawnPoint2D>();
        spawn.Configure("TownAfterBattle");
        return spawn;
    }

    private static void AddSpawnToController(PlayerSpawnPoint2D spawnPoint)
    {
        WorldSceneSpawnController2D controller =
            Object.FindFirstObjectByType<WorldSceneSpawnController2D>();
        SerializedObject serializedController = new SerializedObject(controller);
        SerializedProperty spawns =
            serializedController.FindProperty("additionalSpawnPoints");

        for (int index = 0; index < spawns.arraySize; index++)
        {
            if (spawns.GetArrayElementAtIndex(index).objectReferenceValue ==
                spawnPoint)
            {
                return;
            }
        }

        int newIndex = spawns.arraySize;
        spawns.InsertArrayElementAtIndex(newIndex);
        spawns.GetArrayElementAtIndex(newIndex).objectReferenceValue =
            spawnPoint;
        serializedController.ApplyModifiedPropertiesWithoutUndo();
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
            "Hero\nHP 100/100",
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(8f, -8f),
            new Vector2(120f, 32f),
            9f,
            TextAlignmentOptions.TopLeft,
            new Vector2(0f, 1f));
        TextMeshProUGUI monsterStatus = CreateText(
            "MonsterStatusText",
            canvasObject.transform,
            "Goblin\nHP 35/35",
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(-8f, -8f),
            new Vector2(120f, 32f),
            9f,
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
            new Vector2(0f, 62f),
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
            9f,
            TextAlignmentOptions.TopLeft);
        RectTransform logRect = battleLog.rectTransform;
        logRect.offsetMin = new Vector2(8f, 7f);
        logRect.offsetMax = new Vector2(-168f, -7f);

        GameObject attackButton = CreateButton(
            "AttackButton",
            commandBand.transform,
            "Attack",
            new Vector2(1f, 0.5f),
            new Vector2(-134f, 12f));
        GameObject defendButton = CreateButton(
            "DefendButton",
            commandBand.transform,
            "Defend",
            new Vector2(1f, 0.5f),
            new Vector2(-82f, 12f));
        GameObject escapeButton = CreateButton(
            "EscapeButton",
            commandBand.transform,
            "Escape",
            new Vector2(1f, 0.5f),
            new Vector2(-30f, 12f));
        GameObject continueButton = CreateButton(
            "ContinueButton",
            commandBand.transform,
            "Return",
            new Vector2(1f, 0.5f),
            new Vector2(-46f, 12f),
            new Vector2(80f, 22f));
        TextMeshProUGUI continueLabel =
            continueButton.GetComponentInChildren<TextMeshProUGUI>();

        UnityEventTools.AddPersistentListener(
            attackButton.GetComponent<Button>().onClick,
            controller.Attack);
        UnityEventTools.AddPersistentListener(
            defendButton.GetComponent<Button>().onClick,
            controller.Defend);
        UnityEventTools.AddPersistentListener(
            escapeButton.GetComponent<Button>().onClick,
            controller.Escape);
        UnityEventTools.AddPersistentListener(
            continueButton.GetComponent<Button>().onClick,
            controller.Continue);

        continueButton.SetActive(false);
        controller.Configure(
            playerStatus,
            monsterStatus,
            battleLog,
            continueLabel,
            attackButton,
            defendButton,
            escapeButton,
            continueButton);
    }

    private static void RepairBattleInterface(Scene townScene)
    {
        Scene battleScene = EditorSceneManager.OpenScene(
            BattleScenePath,
            OpenSceneMode.Additive);
        EditorSceneManager.SetActiveScene(battleScene);

        BattleSceneController controller =
            FindSceneObject(battleScene, "BattleController")
                .GetComponent<BattleSceneController>();
        Transform commandBand =
            FindSceneObject(battleScene, "CommandBand");
        GameObject attackButton =
            FindSceneObject(battleScene, "AttackButton").gameObject;
        Transform defendTransform =
            FindSceneObject(battleScene, "DefendButton");
        bool createdDefendButton = defendTransform == null;
        GameObject defendButton = createdDefendButton
            ? CreateButton(
                "DefendButton",
                commandBand,
                "Defend",
                new Vector2(1f, 0.5f),
                new Vector2(-82f, 12f))
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

        SetRect(
            attackButton.GetComponent<RectTransform>(),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(-134f, 12f),
            new Vector2(48f, 22f),
            new Vector2(0.5f, 0.5f));
        SetRect(
            defendButton.GetComponent<RectTransform>(),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(-82f, 12f),
            new Vector2(48f, 22f),
            new Vector2(0.5f, 0.5f));
        SetRect(
            escapeButton.GetComponent<RectTransform>(),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(-30f, 12f),
            new Vector2(48f, 22f),
            new Vector2(0.5f, 0.5f));
        battleLog.rectTransform.offsetMax = new Vector2(-168f, -7f);

        if (createdDefendButton)
        {
            UnityEventTools.AddPersistentListener(
                defendButton.GetComponent<Button>().onClick,
                controller.Defend);
        }

        controller.Configure(
            playerStatus,
            monsterStatus,
            battleLog,
            continueLabel,
            attackButton,
            defendButton,
            escapeButton,
            continueButton);

        EditorSceneManager.MarkSceneDirty(battleScene);
        EditorSceneManager.SaveScene(battleScene);
        EditorSceneManager.SetActiveScene(townScene);
        EditorSceneManager.CloseScene(battleScene, true);
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
        textComponent.enableWordWrapping = true;
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
        return buttonObject;
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
