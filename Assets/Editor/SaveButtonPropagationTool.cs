using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class SaveButtonPropagationTool
{
    private const string SuncrestSceneFolder = "Assets/Scenes/Suncrest";
    private const string MenuPanelName = "InventoryEquipmentPanel";
    private const string SaveButtonName = "SaveButton";

    [MenuItem(
        "Tools/AngelBlade RPG/UI/Propagate Selected Save Button To Suncrest")]
    public static void PropagateSelectedSaveButton()
    {
        Button sourceButton =
            Selection.activeGameObject?.GetComponent<Button>();
        if (sourceButton == null ||
            !TryFindMenu(
                sourceButton.gameObject.scene,
                out InventoryEquipmentMenu sourceMenu,
                out Transform sourcePanel) ||
            !sourceButton.transform.IsChildOf(sourcePanel))
        {
            EditorUtility.DisplayDialog(
                "Save Button Propagation",
                "Select the authored Save button inside an " +
                "InventoryEquipmentPanel, then run this command again.",
                "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        sourceButton.name = SaveButtonName;
        WireButton(sourceButton, sourceMenu);
        Scene sourceScene = sourceButton.gameObject.scene;
        EditorSceneManager.MarkSceneDirty(sourceScene);
        EditorSceneManager.SaveScene(sourceScene);

        string[] scenePaths = AssetDatabase.FindAssets(
                "t:Scene",
                new[] { SuncrestSceneFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        int updatedCount = 1;
        int skippedCount = 0;

        foreach (string scenePath in scenePaths)
        {
            if (string.Equals(
                scenePath,
                sourceScene.path,
                StringComparison.Ordinal))
            {
                continue;
            }

            Scene targetScene = EditorSceneManager.OpenScene(
                scenePath,
                OpenSceneMode.Additive);
            try
            {
                if (!TryFindMenu(
                        targetScene,
                        out InventoryEquipmentMenu targetMenu,
                        out Transform targetPanel))
                {
                    skippedCount += 1;
                    continue;
                }

                Transform existing = targetPanel.Find(SaveButtonName);
                if (existing != null)
                {
                    Object.DestroyImmediate(existing.gameObject);
                }

                GameObject copy = Object.Instantiate(
                    sourceButton.gameObject,
                    targetPanel);
                copy.name = SaveButtonName;
                WireButton(copy.GetComponent<Button>(), targetMenu);
                EditorSceneManager.MarkSceneDirty(targetScene);
                EditorSceneManager.SaveScene(targetScene);
                updatedCount += 1;
            }
            finally
            {
                EditorSceneManager.CloseScene(targetScene, true);
            }
        }

        AssetDatabase.SaveAssets();
        Selection.activeGameObject = sourceButton.gameObject;
        EditorUtility.DisplayDialog(
            "Save Button Propagation",
            $"Updated {updatedCount} Suncrest exploration menu(s). " +
            $"Skipped {skippedCount} scene(s) without exactly one inventory " +
            "menu.",
            "OK");
    }

    private static bool TryFindMenu(
        Scene scene,
        out InventoryEquipmentMenu menu,
        out Transform panel)
    {
        menu = null;
        panel = null;
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return false;
        }

        InventoryEquipmentMenu[] menus = scene.GetRootGameObjects()
            .SelectMany(root =>
                root.GetComponentsInChildren<InventoryEquipmentMenu>(true))
            .ToArray();
        if (menus.Length != 1)
        {
            return false;
        }

        Transform foundPanel = menus[0].transform
            .GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(candidate =>
                candidate.name == MenuPanelName);
        if (foundPanel == null)
        {
            return false;
        }

        menu = menus[0];
        panel = foundPanel;
        return true;
    }

    private static void WireButton(
        Button button,
        InventoryEquipmentMenu menu)
    {
        while (button.onClick.GetPersistentEventCount() > 0)
        {
            UnityEventTools.RemovePersistentListener(button.onClick, 0);
        }

        button.onClick.RemoveAllListeners();
        UnityEventTools.AddPersistentListener(
            button.onClick,
            menu.SaveGame);
        EditorUtility.SetDirty(button);
    }
}
