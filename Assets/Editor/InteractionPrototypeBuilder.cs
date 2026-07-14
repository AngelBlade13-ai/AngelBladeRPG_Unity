using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class InteractionPrototypeBuilder
{
    private const string TownScenePath = "Assets/Scenes/TownScene.unity";
    private const string InteriorScenePath =
        "Assets/Scenes/InteractionTestInterior.unity";
    private const string InteriorSceneName = "InteractionTestInterior";
    private const string GroundTilePath =
        "Assets/Tilemaps/Tiles/GroundTile.asset";
    private const string CollisionTilePath =
        "Assets/Tilemaps/Tiles/CollisionTile.asset";

    [MenuItem("Tools/AngelBlade RPG/Build Interaction Test Interior")]
    public static void BuildInteractionTestInterior()
    {
        if (EditorSceneManager.GetActiveScene().path != TownScenePath)
        {
            EditorUtility.DisplayDialog(
                "Interaction Test Interior",
                "Open TownScene before building the interaction test interior.",
                "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        Sprite doorSprite = GetDoorSprite();
        EnsureTownTransition(doorSprite);
        Scene townScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(townScene);
        EditorSceneManager.SaveScene(townScene);

        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(InteriorScenePath) != null)
        {
            AddSceneToBuildList(InteriorScenePath);
            EditorUtility.DisplayDialog(
                "Interaction Test Interior",
                "The existing interior was kept and the town-side door and return spawn were repaired.",
                "OK");
            return;
        }

        if (!AssetDatabase.CopyAsset(TownScenePath, InteriorScenePath))
        {
            EditorUtility.DisplayDialog(
                "Interaction Test Interior",
                "Unity could not create InteractionTestInterior.",
                "OK");
            return;
        }

        AssetDatabase.Refresh();
        Scene interiorScene = EditorSceneManager.OpenScene(
            InteriorScenePath,
            OpenSceneMode.Single);
        ConfigureInterior(interiorScene, doorSprite);
        EditorSceneManager.SaveScene(interiorScene);
        AddSceneToBuildList(InteriorScenePath);
        EditorSceneManager.OpenScene(TownScenePath, OpenSceneMode.Single);

        EditorUtility.DisplayDialog(
            "Interaction Test Interior",
            "The paired town and interior doors are ready. Save TownScene, then test the round trip from MainGameScene.",
            "OK");
    }

    private static void EnsureTownTransition(Sprite doorSprite)
    {
        GameObject spawnObject = GameObject.Find("TownFromInteriorSpawn");
        PlayerSpawnPoint2D townReturnSpawn;

        if (spawnObject == null)
        {
            townReturnSpawn = CreateSpawnPoint(
                "TownFromInteriorSpawn",
                "TownFromInterior",
                new Vector3(-1f, -3f, 0f));
        }
        else
        {
            townReturnSpawn =
                spawnObject.GetComponent<PlayerSpawnPoint2D>();
            townReturnSpawn.Configure("TownFromInterior");
            spawnObject.transform.position = new Vector3(-1f, -3f, 0f);
        }

        GameObject door = GameObject.Find("InteriorTestDoor");
        if (door == null)
        {
            CreateDoor(
                "InteriorTestDoor",
                new Vector3(-2f, -3f, 0f),
                doorSprite,
                InteriorSceneName,
                "InteriorEntrance");
        }
        else
        {
            SceneDoorInteractable2D sceneDoor =
                door.GetComponent<SceneDoorInteractable2D>();
            sceneDoor.Configure(InteriorSceneName, "InteriorEntrance");
        }

        AddSpawnToController(townReturnSpawn);
    }

    private static void ConfigureInterior(Scene scene, Sprite doorSprite)
    {
        DestroyIfPresent("TestSign");
        DestroyIfPresent("InteriorTestDoor");
        DestroyIfPresent("TownFromInteriorSpawn");

        GameObject entranceObject = GameObject.Find("TownEntranceSpawn");
        entranceObject.name = "InteriorEntranceSpawn";
        entranceObject.transform.position = new Vector3(0f, -1f, 0f);
        PlayerSpawnPoint2D entrance =
            entranceObject.GetComponent<PlayerSpawnPoint2D>();
        entrance.Configure("InteriorEntrance");

        CreateDoor(
            "ReturnToTownDoor",
            new Vector3(0f, 1f, 0f),
            doorSprite,
            "TownScene",
            "TownFromInterior");

        WorldSceneSpawnController2D controller =
            Object.FindFirstObjectByType<WorldSceneSpawnController2D>();
        SetAdditionalSpawns(controller, new PlayerSpawnPoint2D[0]);
        PaintInterior();
        EditorSceneManager.MarkSceneDirty(scene);
    }

    private static Sprite GetDoorSprite()
    {
        GameObject sign = GameObject.Find("TestSign");
        SpriteRenderer signRenderer = sign == null
            ? null
            : sign.GetComponent<SpriteRenderer>();

        if (signRenderer != null && signRenderer.sprite != null)
        {
            return signRenderer.sprite;
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Tilemaps/Tiles/PlaceholderTileSprite.png");
    }

    private static GameObject CreateDoor(
        string objectName,
        Vector3 position,
        Sprite sprite,
        string destinationScene,
        string destinationSpawn)
    {
        GameObject door = new GameObject(objectName);
        door.transform.position = position;
        door.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        SpriteRenderer renderer = door.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = new Color(0.2f, 0.75f, 0.9f, 1f);
        door.AddComponent<BoxCollider2D>();

        SceneDoorInteractable2D sceneDoor =
            door.AddComponent<SceneDoorInteractable2D>();
        sceneDoor.Configure(destinationScene, destinationSpawn);
        return door;
    }

    private static PlayerSpawnPoint2D CreateSpawnPoint(
        string objectName,
        string spawnId,
        Vector3 position)
    {
        GameObject spawnObject = new GameObject(objectName);
        spawnObject.transform.position = position;
        PlayerSpawnPoint2D spawn =
            spawnObject.AddComponent<PlayerSpawnPoint2D>();
        spawn.Configure(spawnId);
        return spawn;
    }

    private static void AddSpawnToController(PlayerSpawnPoint2D spawnPoint)
    {
        WorldSceneSpawnController2D controller =
            Object.FindFirstObjectByType<WorldSceneSpawnController2D>();
        SetAdditionalSpawns(controller, new[] { spawnPoint });
    }

    private static void SetAdditionalSpawns(
        WorldSceneSpawnController2D controller,
        PlayerSpawnPoint2D[] spawnPoints)
    {
        SerializedObject serializedController = new SerializedObject(controller);
        SerializedProperty spawns =
            serializedController.FindProperty("additionalSpawnPoints");
        spawns.arraySize = spawnPoints.Length;

        for (int index = 0; index < spawnPoints.Length; index++)
        {
            spawns.GetArrayElementAtIndex(index).objectReferenceValue =
                spawnPoints[index];
        }

        serializedController.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void PaintInterior()
    {
        Tilemap ground = GameObject.Find("GroundTilemap").GetComponent<Tilemap>();
        Tilemap collision =
            GameObject.Find("CollisionTilemap").GetComponent<Tilemap>();
        Tile groundTile = AssetDatabase.LoadAssetAtPath<Tile>(GroundTilePath);
        Tile collisionTile =
            AssetDatabase.LoadAssetAtPath<Tile>(CollisionTilePath);

        ground.ClearAllTiles();
        collision.ClearAllTiles();
        FillRectangle(ground, groundTile, -5, -4, 5, 4);
        PaintBoundary(collision, collisionTile, -5, -4, 5, 4);
        ground.CompressBounds();
        collision.CompressBounds();
    }

    private static void FillRectangle(
        Tilemap tilemap,
        TileBase tile,
        int minimumX,
        int minimumY,
        int maximumX,
        int maximumY)
    {
        for (int x = minimumX; x <= maximumX; x++)
        {
            for (int y = minimumY; y <= maximumY; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    private static void PaintBoundary(
        Tilemap tilemap,
        TileBase tile,
        int minimumX,
        int minimumY,
        int maximumX,
        int maximumY)
    {
        for (int x = minimumX; x <= maximumX; x++)
        {
            tilemap.SetTile(new Vector3Int(x, minimumY, 0), tile);
            tilemap.SetTile(new Vector3Int(x, maximumY, 0), tile);
        }

        for (int y = minimumY; y <= maximumY; y++)
        {
            tilemap.SetTile(new Vector3Int(minimumX, y, 0), tile);
            tilemap.SetTile(new Vector3Int(maximumX, y, 0), tile);
        }
    }

    private static void DestroyIfPresent(string objectName)
    {
        GameObject target = GameObject.Find(objectName);
        if (target != null)
        {
            Object.DestroyImmediate(target);
        }
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
