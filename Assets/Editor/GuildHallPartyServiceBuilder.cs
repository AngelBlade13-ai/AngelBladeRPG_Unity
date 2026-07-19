using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GuildHallPartyServiceBuilder
{
    private const string GuildHallScenePath =
        "Assets/Scenes/Suncrest/SuncrestGuildHallScene.unity";
    private const string PlaceholderSpritePath =
        "Assets/Tilemaps/Tiles/PlaceholderTileSprite.png";
    private const string ServiceRootName = "GuildHallPartyService";
    private const string CanvasName = "PartyManagementCanvas";

    [MenuItem(
        "Tools/AngelBlade RPG/World/Build Guild Hall Party Service")]
    public static void BuildGuildHallPartyService()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(GuildHallScenePath) == null)
        {
            ShowResult("SuncrestGuildHallScene could not be found.");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(
            GuildHallScenePath,
            OpenSceneMode.Single);
        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            ShowResult(
                "The Guild Hall scene needs an EventSystem before the party service can be built.");
            return;
        }

        DestroyIfPresent(ServiceRootName);
        DestroyIfPresent(CanvasName);

        PartyManagementPanel panel = CreatePartyManagementInterface();
        CreateServicePoint(panel);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = GameObject.Find(ServiceRootName);
        SceneView.lastActiveSceneView?.FrameSelected();
        ShowResult(
            "The Guild Hall party service is ready. Enter from MainGameScene, " +
            "walk to the gold marker, face it, and press the interact button.");
    }

    private static PartyManagementPanel CreatePartyManagementInterface()
    {
        GameObject canvasObject = new GameObject(
            CanvasName,
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(320f, 180f);
        scaler.matchWidthOrHeight = 0.5f;

        PartyManagementPanel controller =
            canvasObject.AddComponent<PartyManagementPanel>();
        GameObject panel = CreateImage(
            "PartyManagementPanel",
            canvasObject.transform,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero,
            new Color(0.035f, 0.045f, 0.05f, 0.98f));

        CreateText(
            "TitleText",
            panel.transform,
            "GUILD HALL - PARTY & JOBS",
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, 76f),
            new Vector2(230f, 14f),
            10f,
            TextAlignmentOptions.Center);

        Button closeButton = CreateButton(
            "CloseButton",
            panel.transform,
            "X",
            new Vector2(0.5f, 0.5f),
            new Vector2(146f, 76f),
            new Vector2(20f, 16f));

        TextMeshProUGUI characterText = CreateText(
            "CharacterText",
            panel.transform,
            "1/1  Hero\nACTIVE - FORMATION 1   JP 0",
            new Vector2(0.5f, 0.5f),
            new Vector2(-76f, 53f),
            new Vector2(104f, 28f),
            7f,
            TextAlignmentOptions.Center);
        Button previousCharacter = CreateButton(
            "PreviousCharacterButton",
            panel.transform,
            "<",
            new Vector2(0.5f, 0.5f),
            new Vector2(-139f, 53f),
            new Vector2(18f, 20f));
        Button nextCharacter = CreateButton(
            "NextCharacterButton",
            panel.transform,
            ">",
            new Vector2(0.5f, 0.5f),
            new Vector2(-13f, 53f),
            new Vector2(18f, 20f));

        TextMeshProUGUI statsText = CreateText(
            "StatsText",
            panel.transform,
            "HP 100/100   MP 20/20\nATK 10  DEF 8  MAG 3  MDEF 3\nSPD 10  ACC 95  EVA 5  CRIT 10",
            new Vector2(0.5f, 0.5f),
            new Vector2(-76f, 10f),
            new Vector2(126f, 48f),
            7f,
            TextAlignmentOptions.TopLeft);

        Button moveUp = CreateButton(
            "MoveUpButton",
            panel.transform,
            "Up",
            new Vector2(0.5f, 0.5f),
            new Vector2(-116f, -45f),
            new Vector2(48f, 18f));
        Button moveDown = CreateButton(
            "MoveDownButton",
            panel.transform,
            "Down",
            new Vector2(0.5f, 0.5f),
            new Vector2(-61f, -45f),
            new Vector2(48f, 18f));
        Button partyButton = CreateButton(
            "PartyToggleButton",
            panel.transform,
            "Move to Reserve",
            new Vector2(0.5f, 0.5f),
            new Vector2(-76f, -69f),
            new Vector2(128f, 20f));
        TextMeshProUGUI partyButtonText =
            partyButton.GetComponentInChildren<TextMeshProUGUI>();

        TextMeshProUGUI jobText = CreateText(
            "JobText",
            panel.transform,
            "Mercenary  [Neutral affinity]\nConsistent direct melee damage.\nTrade-off: Offers little healing or party utility.",
            new Vector2(0.5f, 0.5f),
            new Vector2(71f, 24f),
            new Vector2(132f, 74f),
            7f,
            TextAlignmentOptions.TopLeft);
        Button previousJob = CreateButton(
            "PreviousJobButton",
            panel.transform,
            "< Job",
            new Vector2(0.5f, 0.5f),
            new Vector2(39f, -25f),
            new Vector2(56f, 18f));
        Button nextJob = CreateButton(
            "NextJobButton",
            panel.transform,
            "Job >",
            new Vector2(0.5f, 0.5f),
            new Vector2(103f, -25f),
            new Vector2(56f, 18f));
        Button applyJob = CreateButton(
            "ApplyJobButton",
            panel.transform,
            "Assign Job",
            new Vector2(0.5f, 0.5f),
            new Vector2(71f, -49f),
            new Vector2(120f, 20f));
        TextMeshProUGUI feedbackText = CreateText(
            "FeedbackText",
            panel.transform,
            "Choose a formation position or preview any job.",
            new Vector2(0.5f, 0.5f),
            new Vector2(71f, -72f),
            new Vector2(136f, 16f),
            6f,
            TextAlignmentOptions.Center);

        LinkNavigation(closeButton.gameObject, null, previousCharacter.gameObject, null, null);
        LinkNavigation(previousCharacter.gameObject, closeButton.gameObject, moveUp.gameObject, null, nextCharacter.gameObject);
        LinkNavigation(nextCharacter.gameObject, closeButton.gameObject, moveDown.gameObject, previousCharacter.gameObject, previousJob.gameObject);
        LinkNavigation(moveUp.gameObject, previousCharacter.gameObject, partyButton.gameObject, null, moveDown.gameObject);
        LinkNavigation(moveDown.gameObject, nextCharacter.gameObject, partyButton.gameObject, moveUp.gameObject, applyJob.gameObject);
        LinkNavigation(partyButton.gameObject, moveUp.gameObject, null, null, applyJob.gameObject);
        LinkNavigation(previousJob.gameObject, nextCharacter.gameObject, applyJob.gameObject, null, nextJob.gameObject);
        LinkNavigation(nextJob.gameObject, closeButton.gameObject, applyJob.gameObject, previousJob.gameObject, null);
        LinkNavigation(applyJob.gameObject, nextJob.gameObject, null, partyButton.gameObject, null);

        controller.Configure(
            panel,
            characterText,
            jobText,
            statsText,
            feedbackText,
            previousCharacter,
            nextCharacter,
            partyButton,
            partyButtonText,
            moveUp,
            moveDown,
            previousJob,
            nextJob,
            applyJob,
            closeButton);

        UnityEventTools.AddPersistentListener(
            previousCharacter.onClick,
            controller.PreviousCharacter);
        UnityEventTools.AddPersistentListener(
            nextCharacter.onClick,
            controller.NextCharacter);
        UnityEventTools.AddPersistentListener(
            partyButton.onClick,
            controller.TogglePartyStatus);
        UnityEventTools.AddPersistentListener(moveUp.onClick, controller.MoveUp);
        UnityEventTools.AddPersistentListener(
            moveDown.onClick,
            controller.MoveDown);
        UnityEventTools.AddPersistentListener(
            previousJob.onClick,
            controller.PreviousJob);
        UnityEventTools.AddPersistentListener(
            nextJob.onClick,
            controller.NextJob);
        UnityEventTools.AddPersistentListener(
            applyJob.onClick,
            controller.ApplyJob);
        UnityEventTools.AddPersistentListener(closeButton.onClick, controller.Close);

        panel.SetActive(false);
        return controller;
    }

    private static void CreateServicePoint(PartyManagementPanel panel)
    {
        GameObject service = new GameObject(ServiceRootName);
        service.transform.position = new Vector3(2f, 2f, 0f);
        service.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        SpriteRenderer renderer = service.AddComponent<SpriteRenderer>();
        renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            PlaceholderSpritePath);
        renderer.color = new Color(0.95f, 0.72f, 0.2f, 1f);
        renderer.sortingOrder = 5;
        service.AddComponent<BoxCollider2D>();

        PartyManagementInteractable2D interactable =
            service.AddComponent<PartyManagementInteractable2D>();
        interactable.Configure(panel);
    }

    private static GameObject CreateImage(
        string objectName,
        Transform parent,
        Vector2 anchorMinimum,
        Vector2 anchorMaximum,
        Vector2 anchoredPosition,
        Vector2 size,
        Color color)
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
            size);
        imageObject.GetComponent<Image>().color = color;
        return imageObject;
    }

    private static TextMeshProUGUI CreateText(
        string objectName,
        Transform parent,
        string text,
        Vector2 anchor,
        Vector2 anchoredPosition,
        Vector2 size,
        float fontSize,
        TextAlignmentOptions alignment)
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
            anchor,
            anchor,
            anchoredPosition,
            size);
        return textComponent;
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

    private static Button CreateButton(
        string objectName,
        Transform parent,
        string label,
        Vector2 anchor,
        Vector2 anchoredPosition,
        Vector2 size)
    {
        GameObject buttonObject = CreateImage(
            objectName,
            parent,
            anchor,
            anchor,
            anchoredPosition,
            size,
            new Color(0.78f, 0.8f, 0.76f, 1f));
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();

        TextMeshProUGUI labelText = CreateText(
            "Label",
            buttonObject.transform,
            label,
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            size,
            7f,
            TextAlignmentOptions.Center);
        labelText.color = new Color(0.06f, 0.07f, 0.08f, 1f);
        labelText.richText = false;
        return button;
    }

    private static void SetRect(
        RectTransform rect,
        Vector2 anchorMinimum,
        Vector2 anchorMaximum,
        Vector2 anchoredPosition,
        Vector2 size)
    {
        rect.anchorMin = anchorMinimum;
        rect.anchorMax = anchorMaximum;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
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

    private static void ShowResult(string message)
    {
        Debug.Log(message);
        EditorUtility.DisplayDialog("Guild Hall Party Service", message, "OK");
    }
}
