using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class SuncrestDistrictBuilder
{
    private const string TemplateScenePath = "Assets/Scenes/TownScene.unity";
    private const string SceneFolder = "Assets/Scenes/Suncrest";
    private const string SoftGrassTilePath =
        "Assets/Tilemaps/Tiles/Phase1/Phase1_EnvTile_SoftGrassV6.asset";
    private const string DirtTilePathPrefix =
        "Assets/Tilemaps/Tiles/Phase1/Phase1_EnvTile_DirtV";
    private const string CollisionTilePath =
        "Assets/Tilemaps/Tiles/CollisionTile.asset";
    private const string PlaceholderSpritePath =
        "Assets/Tilemaps/Tiles/PlaceholderTileSprite.png";

    private static readonly DistrictDefinition[] Districts =
    {
        new DistrictDefinition(
            "GuildHall",
            "SuncrestGuildHallScene",
            40,
            28,
            new Connection("WhisperMarket", ExitSide.West),
            new Connection("AmberRow", ExitSide.East),
            new Connection("SuncrestWatch", ExitSide.North),
            new Connection("SuncrestInn", ExitSide.South)),
        new DistrictDefinition(
            "WhisperMarket",
            "SuncrestWhisperMarketScene",
            48,
            28,
            new Connection("Ironforge", ExitSide.West),
            new Connection("GuildHall", ExitSide.East),
            new Connection("SuncrestInn", ExitSide.South)),
        new DistrictDefinition(
            "Ironforge",
            "SuncrestIronforgeScene",
            32,
            24,
            new Connection("WhisperMarket", ExitSide.East)),
        new DistrictDefinition(
            "SuncrestInn",
            "SuncrestInnScene",
            36,
            24,
            new Connection("WhisperMarket", ExitSide.West),
            new Connection("SunrootGrove", ExitSide.East),
            new Connection("GuildHall", ExitSide.North)),
        new DistrictDefinition(
            "SunwellShrine",
            "SuncrestShrineScene",
            36,
            28,
            new Connection("SuncrestWatch", ExitSide.West),
            new Connection("AmberRow", ExitSide.South)),
        new DistrictDefinition(
            "AmberRow",
            "SuncrestAmberRowScene",
            44,
            30,
            new Connection("GuildHall", ExitSide.West),
            new Connection("SunrootGrove", ExitSide.East),
            new Connection("SunwellShrine", ExitSide.North)),
        new DistrictDefinition(
            "SunrootGrove",
            "SuncrestGroveScene",
            48,
            32,
            new Connection("AmberRow", ExitSide.West),
            new Connection("SuncrestInn", ExitSide.South)),
        new DistrictDefinition(
            "SuncrestWatch",
            "SuncrestWatchScene",
            40,
            28,
            new Connection("SunwellShrine", ExitSide.East),
            new Connection("GuildHall", ExitSide.South))
    };

    private static readonly string[] PrototypeOnlyObjects =
    {
        "BattleTestEncounter",
        "BattleDefeatTestEncounter",
        "BattleSpeedTestEncounter",
        "TestSign",
        "InteriorTestDoor",
        "TownAfterBattleSpawn",
        "TownFromInteriorSpawn"
    };

    [MenuItem(
        "Tools/AngelBlade RPG/World/Create Missing Suncrest District Scenes")]
    public static void CreateMissingDistrictScenes()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(TemplateScenePath) == null)
        {
            ShowResult("TownScene could not be found, so no district scenes were created.");
            return;
        }

        if (!ValidateRequiredAssets())
        {
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        EnsureAssetFolder(SceneFolder);
        string previousScenePath = EditorSceneManager.GetActiveScene().path;
        int createdCount = 0;
        int skippedCount = 0;

        try
        {
            foreach (DistrictDefinition district in Districts)
            {
                string scenePath = GetScenePath(district);
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null)
                {
                    AddSceneToBuildList(scenePath);
                    skippedCount++;
                    continue;
                }

                if (!AssetDatabase.CopyAsset(TemplateScenePath, scenePath))
                {
                    Debug.LogError($"Could not create {scenePath}.");
                    continue;
                }

                AssetDatabase.Refresh();
                Scene scene = EditorSceneManager.OpenScene(
                    scenePath,
                    OpenSceneMode.Single);
                ConfigureDistrictScene(scene, district);
                EditorSceneManager.SaveScene(scene);
                AddSceneToBuildList(scenePath);
                createdCount++;
            }
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(previousScenePath) &&
                AssetDatabase.LoadAssetAtPath<SceneAsset>(previousScenePath) != null)
            {
                EditorSceneManager.OpenScene(previousScenePath, OpenSceneMode.Single);
            }
            else
            {
                EditorSceneManager.OpenScene(
                    TemplateScenePath,
                    OpenSceneMode.Single);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        ShowResult(
            $"Created {createdCount} district scenes and kept {skippedCount} " +
            "existing scenes unchanged. Open SuncrestGuildHallScene to inspect " +
            "the generated blockout.");
    }

    private static void ConfigureDistrictScene(
        Scene scene,
        DistrictDefinition district)
    {
        RemovePrototypeObjects();

        Tilemap ground = FindTilemap("GroundTilemap");
        Tilemap collision = FindTilemap("CollisionTilemap");
        Grid grid = Object.FindFirstObjectByType<Grid>();
        Transform player = GameObject.Find("Player")?.transform;
        WorldSceneSpawnController2D spawnController =
            Object.FindFirstObjectByType<WorldSceneSpawnController2D>();

        if (ground == null || collision == null || grid == null ||
            player == null || spawnController == null)
        {
            throw new MissingReferenceException(
                $"{district.SceneName} is missing required TownScene template objects.");
        }

        ConfigureTilemapLayers(grid, ground, collision);
        PaintDistrictBlockout(ground, collision, district);

        GameObject oldEntrance = GameObject.Find("TownEntranceSpawn");
        if (oldEntrance != null)
        {
            Object.DestroyImmediate(oldEntrance);
        }

        GameObject transitionRoot = new GameObject("DistrictTransitions");
        PlayerSpawnPoint2D defaultSpawn = CreateSpawnPoint(
            transitionRoot.transform,
            "DefaultSpawn",
            "Default",
            Vector3.zero);

        var arrivalSpawns = new List<PlayerSpawnPoint2D>();
        Sprite placeholderSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            PlaceholderSpritePath);
        foreach (Connection connection in district.Connections)
        {
            DistrictDefinition destination = FindDistrict(connection.DestinationId);
            Vector3 doorPosition = GetDoorPosition(district, connection.Side);
            Vector3 spawnPosition = GetSpawnPosition(district, connection.Side);

            CreateExit(
                transitionRoot.transform,
                destination,
                connection.Side,
                doorPosition,
                placeholderSprite,
                $"From{district.Id}");

            PlayerSpawnPoint2D arrival = CreateSpawnPoint(
                transitionRoot.transform,
                $"From{connection.DestinationId}Spawn",
                $"From{connection.DestinationId}",
                spawnPosition);
            arrivalSpawns.Add(arrival);
        }

        spawnController.Configure(
            player,
            defaultSpawn,
            arrivalSpawns.ToArray());
        player.position = Vector3.zero;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        }

        GameObject marker = new GameObject(
            $"District_{district.Id}_{district.Width}x{district.Height}");
        marker.transform.SetSiblingIndex(0);
        EditorSceneManager.MarkSceneDirty(scene);
    }

    private static void ConfigureTilemapLayers(
        Grid grid,
        Tilemap ground,
        Tilemap collision)
    {
        ConfigureRenderer(ground, -20, true);
        ConfigureRenderer(collision, 20, false);
        GetOrCreateTilemap(grid.transform, "DecorationTilemap", -10, true);
        GetOrCreateTilemap(grid.transform, "ForegroundTilemap", 10, true);
    }

    private static Tilemap GetOrCreateTilemap(
        Transform grid,
        string objectName,
        int sortingOrder,
        bool rendererEnabled)
    {
        Transform existing = grid.Find(objectName);
        GameObject tilemapObject;
        if (existing == null)
        {
            tilemapObject = new GameObject(objectName);
            tilemapObject.transform.SetParent(grid, false);
            tilemapObject.AddComponent<Tilemap>();
            tilemapObject.AddComponent<TilemapRenderer>();
        }
        else
        {
            tilemapObject = existing.gameObject;
        }

        Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();
        ConfigureRenderer(tilemap, sortingOrder, rendererEnabled);
        return tilemap;
    }

    private static void ConfigureRenderer(
        Tilemap tilemap,
        int sortingOrder,
        bool enabled)
    {
        tilemap.color = Color.white;
        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = sortingOrder;
            renderer.enabled = enabled;
        }
    }

    private static void PaintDistrictBlockout(
        Tilemap ground,
        Tilemap collision,
        DistrictDefinition district)
    {
        Tile grass = AssetDatabase.LoadAssetAtPath<Tile>(SoftGrassTilePath);
        Tile collisionTile = AssetDatabase.LoadAssetAtPath<Tile>(
            CollisionTilePath);
        Tile[] dirtTiles = LoadDirtTiles();

        int minimumX = -district.Width / 2;
        int minimumY = -district.Height / 2;
        int maximumX = minimumX + district.Width - 1;
        int maximumY = minimumY + district.Height - 1;

        ground.ClearAllTiles();
        collision.ClearAllTiles();

        const int horizontalCameraMargin = 10;
        const int verticalCameraMargin = 6;
        FillRectangle(
            ground,
            grass,
            minimumX - horizontalCameraMargin,
            minimumY - verticalCameraMargin,
            maximumX + horizontalCameraMargin,
            maximumY + verticalCameraMargin);
        PaintBoundary(
            collision,
            collisionTile,
            minimumX,
            minimumY,
            maximumX,
            maximumY);

        PaintDirtRectangle(ground, dirtTiles, -3, -3, 3, 3);
        foreach (Connection connection in district.Connections)
        {
            switch (connection.Side)
            {
                case ExitSide.West:
                    PaintDirtRectangle(
                        ground,
                        dirtTiles,
                        minimumX,
                        -1,
                        0,
                        1);
                    break;
                case ExitSide.East:
                    PaintDirtRectangle(
                        ground,
                        dirtTiles,
                        0,
                        -1,
                        maximumX,
                        1);
                    break;
                case ExitSide.North:
                    PaintDirtRectangle(
                        ground,
                        dirtTiles,
                        -1,
                        0,
                        1,
                        maximumY);
                    break;
                case ExitSide.South:
                    PaintDirtRectangle(
                        ground,
                        dirtTiles,
                        -1,
                        minimumY,
                        1,
                        0);
                    break;
            }
        }

        ground.CompressBounds();
        collision.CompressBounds();
        EditorUtility.SetDirty(ground);
        EditorUtility.SetDirty(collision);
    }

    private static Tile[] LoadDirtTiles()
    {
        var dirtTiles = new Tile[4];
        for (int index = 0; index < dirtTiles.Length; index++)
        {
            dirtTiles[index] = AssetDatabase.LoadAssetAtPath<Tile>(
                $"{DirtTilePathPrefix}{index + 1}.asset");
        }

        return dirtTiles;
    }

    private static void PaintDirtRectangle(
        Tilemap tilemap,
        Tile[] dirtTiles,
        int minimumX,
        int minimumY,
        int maximumX,
        int maximumY)
    {
        for (int x = minimumX; x <= maximumX; x++)
        {
            for (int y = minimumY; y <= maximumY; y++)
            {
                int hash = x * 397 ^ y * 7919;
                int index = (hash & int.MaxValue) % dirtTiles.Length;
                tilemap.SetTile(new Vector3Int(x, y, 0), dirtTiles[index]);
            }
        }
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

    private static void CreateExit(
        Transform parent,
        DistrictDefinition destination,
        ExitSide side,
        Vector3 position,
        Sprite sprite,
        string destinationSpawnId)
    {
        GameObject exit = new GameObject($"To{destination.Id}");
        exit.transform.SetParent(parent, false);
        exit.transform.position = position;
        exit.transform.localScale = GetExitScale(side);

        SpriteRenderer renderer = exit.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = new Color(0.2f, 0.75f, 0.9f, 1f);
        renderer.sortingOrder = 5;
        exit.AddComponent<BoxCollider2D>();

        SceneDoorInteractable2D door =
            exit.AddComponent<SceneDoorInteractable2D>();
        door.Configure(destination.SceneName, destinationSpawnId);
    }

    private static PlayerSpawnPoint2D CreateSpawnPoint(
        Transform parent,
        string objectName,
        string spawnId,
        Vector3 position)
    {
        GameObject spawnObject = new GameObject(objectName);
        spawnObject.transform.SetParent(parent, false);
        spawnObject.transform.position = position;
        PlayerSpawnPoint2D spawn =
            spawnObject.AddComponent<PlayerSpawnPoint2D>();
        spawn.Configure(spawnId);
        return spawn;
    }

    private static Vector3 GetDoorPosition(
        DistrictDefinition district,
        ExitSide side)
    {
        int minimumX = -district.Width / 2;
        int minimumY = -district.Height / 2;
        int maximumX = minimumX + district.Width - 1;
        int maximumY = minimumY + district.Height - 1;

        switch (side)
        {
            case ExitSide.West:
                return new Vector3(minimumX + 1f, 0f, 0f);
            case ExitSide.East:
                return new Vector3(maximumX - 1f, 0f, 0f);
            case ExitSide.North:
                return new Vector3(0f, maximumY - 1f, 0f);
            default:
                return new Vector3(0f, minimumY + 1f, 0f);
        }
    }

    private static Vector3 GetSpawnPosition(
        DistrictDefinition district,
        ExitSide side)
    {
        Vector3 doorPosition = GetDoorPosition(district, side);
        switch (side)
        {
            case ExitSide.West:
                return doorPosition + Vector3.right * 2f;
            case ExitSide.East:
                return doorPosition + Vector3.left * 2f;
            case ExitSide.North:
                return doorPosition + Vector3.down * 2f;
            default:
                return doorPosition + Vector3.up * 2f;
        }
    }

    private static Vector3 GetExitScale(ExitSide side)
    {
        return side == ExitSide.West || side == ExitSide.East
            ? new Vector3(0.5f, 2f, 1f)
            : new Vector3(2f, 0.5f, 1f);
    }

    private static void RemovePrototypeObjects()
    {
        foreach (string objectName in PrototypeOnlyObjects)
        {
            GameObject target = GameObject.Find(objectName);
            if (target != null)
            {
                Object.DestroyImmediate(target);
            }
        }

        GameObject existingTransitions = GameObject.Find("DistrictTransitions");
        if (existingTransitions != null)
        {
            Object.DestroyImmediate(existingTransitions);
        }
    }

    private static Tilemap FindTilemap(string objectName)
    {
        GameObject tilemapObject = GameObject.Find(objectName);
        return tilemapObject == null
            ? null
            : tilemapObject.GetComponent<Tilemap>();
    }

    private static DistrictDefinition FindDistrict(string districtId)
    {
        foreach (DistrictDefinition district in Districts)
        {
            if (district.Id == districtId)
            {
                return district;
            }
        }

        throw new KeyNotFoundException($"Unknown Suncrest district: {districtId}");
    }

    private static string GetScenePath(DistrictDefinition district)
    {
        return $"{SceneFolder}/{district.SceneName}.unity";
    }

    private static bool ValidateRequiredAssets()
    {
        var missingPaths = new List<string>();
        AddIfMissing<Tile>(SoftGrassTilePath, missingPaths);
        AddIfMissing<Tile>(CollisionTilePath, missingPaths);
        AddIfMissing<Sprite>(PlaceholderSpritePath, missingPaths);
        for (int variant = 1; variant <= 4; variant++)
        {
            AddIfMissing<Tile>(
                $"{DirtTilePathPrefix}{variant}.asset",
                missingPaths);
        }

        if (missingPaths.Count == 0)
        {
            return true;
        }

        ShowResult(
            "Required district assets are missing. Run the Phase 1 environment " +
            "tile importer first.\n\n" + string.Join("\n", missingPaths));
        return false;
    }

    private static void AddIfMissing<T>(
        string assetPath,
        List<string> missingPaths)
        where T : Object
    {
        if (AssetDatabase.LoadAssetAtPath<T>(assetPath) == null)
        {
            missingPaths.Add(assetPath);
        }
    }

    private static void EnsureAssetFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string parent = System.IO.Path.GetDirectoryName(folderPath)
            ?.Replace('\\', '/');
        string folderName = System.IO.Path.GetFileName(folderPath);
        if (!string.IsNullOrEmpty(parent))
        {
            EnsureAssetFolder(parent);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }

    private static void AddSceneToBuildList(string scenePath)
    {
        var scenes = new List<EditorBuildSettingsScene>(
            EditorBuildSettings.scenes);
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

    private static void ShowResult(string message)
    {
        EditorUtility.DisplayDialog("Suncrest District Builder", message, "OK");
    }

    private enum ExitSide
    {
        West,
        East,
        North,
        South
    }

    private sealed class Connection
    {
        public string DestinationId { get; }
        public ExitSide Side { get; }

        public Connection(string destinationId, ExitSide side)
        {
            DestinationId = destinationId;
            Side = side;
        }
    }

    private sealed class DistrictDefinition
    {
        public string Id { get; }
        public string SceneName { get; }
        public int Width { get; }
        public int Height { get; }
        public Connection[] Connections { get; }

        public DistrictDefinition(
            string id,
            string sceneName,
            int width,
            int height,
            params Connection[] connections)
        {
            Id = id;
            SceneName = sceneName;
            Width = width;
            Height = height;
            Connections = connections;
        }
    }
}
