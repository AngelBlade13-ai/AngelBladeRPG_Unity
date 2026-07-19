using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class SuncrestDistrictBlockoutBuilder
{
    private const string SceneFolder = "Assets/Scenes/Suncrest";
    private const string TileFolder = "Assets/Tilemaps/Tiles/Phase1";
    private const string BlockoutTileFolder =
        "Assets/Tilemaps/Tiles/Blockout";
    private const string PlaceholderSpritePath =
        "Assets/Tilemaps/Tiles/PlaceholderTileSprite.png";
    private const string CollisionTilePath =
        "Assets/Tilemaps/Tiles/CollisionTile.asset";
    private const string LayoutRootName = "DistrictLayoutBlockout";

    private const string GuildSceneName = "SuncrestGuildHallScene";
    private const string MarketSceneName = "SuncrestWhisperMarketScene";

    [MenuItem(
        "Tools/AngelBlade RPG/World/Apply Guild Hall And Market Blockouts")]
    public static void ApplyGuildHallAndMarketBlockouts()
    {
        RunBlockoutPass(false);
    }

    [MenuItem(
        "Tools/AngelBlade RPG/World/Rebuild Guild Hall And Market Blockouts")]
    public static void RebuildGuildHallAndMarketBlockouts()
    {
        RunBlockoutPass(true);
    }

    [MenuItem(
        "Tools/AngelBlade RPG/World/Restore Guild Hall And Market Foundations")]
    public static void RestoreGuildHallAndMarketFoundations()
    {
        if (!ValidateScenesAndTiles())
        {
            return;
        }

        bool confirmed = EditorUtility.DisplayDialog(
            "Restore Suncrest Foundations",
            "Remove the generated Guild Hall and Whisper Market visual " +
            "blockouts and return both scenes to clean grass-and-path " +
            "foundations? Shared player, HUD, transitions, and spawn wiring " +
            "will be preserved.",
            "Restore",
            "Cancel");
        if (!confirmed ||
            !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        string previousScenePath = EditorSceneManager.GetActiveScene().path;
        try
        {
            RestoreGuildFoundation();
            RestoreMarketFoundation();
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(previousScenePath) &&
                AssetDatabase.LoadAssetAtPath<SceneAsset>(previousScenePath) != null)
            {
                EditorSceneManager.OpenScene(previousScenePath, OpenSceneMode.Single);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog(
            "Restore Suncrest Foundations",
            "Guild Hall and Whisper Market are back to clean technical " +
            "foundations. Their reciprocal transitions remain wired.",
            "OK");
    }

    private static void RunBlockoutPass(bool rebuildExisting)
    {
        if (!ValidateScenesAndTiles())
        {
            return;
        }

        bool confirmed = EditorUtility.DisplayDialog(
            "Suncrest District Blockouts",
            rebuildExisting
                ? "Rebuild the generated Guild Hall and Whisper Market " +
                  "blockouts? Their Tilemap layout and placeholder content " +
                  "will be replaced, but transitions and shared scene wiring " +
                  "will be preserved."
                : "Apply the first district-specific art and layout pass to " +
                  "Guild Hall and Whisper Market? Existing scenes with a " +
                  $"{LayoutRootName} object will be skipped.",
            rebuildExisting ? "Rebuild" : "Apply",
            "Cancel");
        if (!confirmed ||
            !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        EnsureBlockoutTiles();
        string previousScenePath = EditorSceneManager.GetActiveScene().path;
        int appliedCount = 0;
        int skippedCount = 0;

        try
        {
            ApplySceneBlockout(
                GuildSceneName,
                ApplyGuildHallLayout,
                rebuildExisting,
                ref appliedCount,
                ref skippedCount);
            ApplySceneBlockout(
                MarketSceneName,
                ApplyMarketLayout,
                rebuildExisting,
                ref appliedCount,
                ref skippedCount);
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(previousScenePath) &&
                AssetDatabase.LoadAssetAtPath<SceneAsset>(previousScenePath) != null)
            {
                EditorSceneManager.OpenScene(previousScenePath, OpenSceneMode.Single);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog(
            "Suncrest District Blockouts",
            $"Applied {appliedCount} layouts and kept {skippedCount} existing " +
            "layouts unchanged. Open SuncrestGuildHallScene to inspect the pass.",
            "OK");
    }

    private static void ApplySceneBlockout(
        string sceneName,
        System.Action<Scene> applyLayout,
        bool rebuildExisting,
        ref int appliedCount,
        ref int skippedCount)
    {
        string scenePath = GetScenePath(sceneName);
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        GameObject existingLayout = GameObject.Find(LayoutRootName);
        if (existingLayout != null && !rebuildExisting)
        {
            skippedCount++;
            return;
        }

        if (existingLayout != null)
        {
            Object.DestroyImmediate(existingLayout);
        }

        applyLayout(scene);
        EditorSceneManager.SaveScene(scene);
        appliedCount++;
    }

    private static void ApplyGuildHallLayout(Scene scene)
    {
        Tilemap ground = FindTilemap("GroundTilemap");
        Tilemap decoration = FindTilemap("DecorationTilemap");
        Tilemap foreground = FindTilemap("ForegroundTilemap");
        Tilemap collision = FindTilemap("CollisionTilemap");
        Tile collisionTile = LoadTile(CollisionTilePath);
        Tile[] dirtTiles = LoadVariants("Dirt", 4);
        PathTiles paths = LoadPathTiles();

        ClearAndPaintBase(ground, decoration, foreground, collision, 40, 28);

        PaintHorizontalPath(ground, paths, 0, -20, 19);
        PaintVerticalPath(ground, paths, -8, -14, -3);
        PaintVerticalPath(ground, paths, 9, 3, 13);
        PaintVerticalPath(ground, paths, 0, 3, 7);
        PaintDirtRectangle(ground, dirtTiles, -6, -4, 6, 4);

        GameObject layoutRoot = new GameObject(LayoutRootName);
        CreateBuilding(
            decoration,
            foreground,
            collision,
            collisionTile,
            -6,
            7,
            6,
            10,
            0,
            LoadTile(GetBlockoutTilePath("GuildWall")),
            LoadTile(GetBlockoutTilePath("GuildRoof")),
            LoadTile(GetBlockoutTilePath("GuildAccent")));
        CreateBuildingWing(
            decoration,
            foreground,
            collision,
            collisionTile,
            -9,
            5,
            -6,
            8,
            LoadTile(GetBlockoutTilePath("GuildWall")),
            LoadTile(GetBlockoutTilePath("GuildRoof")));
        CreateBuildingWing(
            decoration,
            foreground,
            collision,
            collisionTile,
            6,
            5,
            9,
            8,
            LoadTile(GetBlockoutTilePath("GuildWall")),
            LoadTile(GetBlockoutTilePath("GuildRoof")));

        CreateMarker(
            layoutRoot.transform,
            "QuestBoardPlaceholder",
            new Vector3(-4f, 3f, 0f),
            new Vector3(1.4f, 1f, 1f),
            new Color(0.45f, 0.27f, 0.12f, 1f),
            true);
        CreateMarker(
            layoutRoot.transform,
            "LysanderPlaceholder",
            new Vector3(4f, 2.5f, 0f),
            new Vector3(0.65f, 1.15f, 1f),
            new Color(0.35f, 0.45f, 0.75f, 1f),
            false);

        PaintGroundDetail(ground, "WhiteFlowerV1", -13, 6);
        PaintGroundDetail(ground, "WhiteFlowerV3", -12, 7);
        PaintGroundDetail(ground, "CornFlowerV2", 14, -6);
        PaintGroundDetail(ground, "CornFlowerV4", 15, -6);
        PaintGroundDetail(ground, "StonesV2", -15, -8);
        PaintGroundDetail(ground, "StonesV6", 15, 8);
        PaintGroundDetail(ground, "WoodV3", -10, 7);

        RepositionTransition("ToSuncrestInn", new Vector3(-8f, -13f, 0f));
        RepositionTransition(
            "FromSuncrestInnSpawn",
            new Vector3(-8f, -10f, 0f));
        RepositionTransition(
            "ToSuncrestWatch",
            new Vector3(9f, 12f, 0f));
        RepositionTransition(
            "FromSuncrestWatchSpawn",
            new Vector3(9f, 9f, 0f));
        RepositionTransition("DefaultSpawn", new Vector3(0f, -3f, 0f));

        FinishScene(scene, ground, decoration, foreground, collision);
    }

    private static void RestoreGuildFoundation()
    {
        Scene scene = EditorSceneManager.OpenScene(
            GetScenePath(GuildSceneName),
            OpenSceneMode.Single);
        RemoveLayoutRoot();

        Tilemap ground = FindTilemap("GroundTilemap");
        Tilemap decoration = FindTilemap("DecorationTilemap");
        Tilemap foreground = FindTilemap("ForegroundTilemap");
        Tilemap collision = FindTilemap("CollisionTilemap");
        PathTiles paths = LoadPathTiles();
        Tile[] dirtTiles = LoadVariants("Dirt", 4);

        ClearAndPaintBase(ground, decoration, foreground, collision, 40, 28);
        PaintHorizontalPath(ground, paths, 0, -20, 19);
        PaintVerticalPath(ground, paths, 0, -14, 13);
        PaintDirtRectangle(ground, dirtTiles, -3, -3, 3, 3);

        RepositionTransition("ToWhisperMarket", new Vector3(-19f, 0f, 0f));
        RepositionTransition(
            "FromWhisperMarketSpawn",
            new Vector3(-16f, 0f, 0f));
        RepositionTransition("ToAmberRow", new Vector3(18f, 0f, 0f));
        RepositionTransition(
            "FromAmberRowSpawn",
            new Vector3(15f, 0f, 0f));
        RepositionTransition("ToSuncrestWatch", new Vector3(0f, 12f, 0f));
        RepositionTransition(
            "FromSuncrestWatchSpawn",
            new Vector3(0f, 9f, 0f));
        RepositionTransition("ToSuncrestInn", new Vector3(0f, -13f, 0f));
        RepositionTransition(
            "FromSuncrestInnSpawn",
            new Vector3(0f, -10f, 0f));
        RepositionTransition("DefaultSpawn", Vector3.zero);

        FinishScene(scene, ground, decoration, foreground, collision);
        EditorSceneManager.SaveScene(scene);
    }

    private static void RestoreMarketFoundation()
    {
        Scene scene = EditorSceneManager.OpenScene(
            GetScenePath(MarketSceneName),
            OpenSceneMode.Single);
        RemoveLayoutRoot();

        Tilemap ground = FindTilemap("GroundTilemap");
        Tilemap decoration = FindTilemap("DecorationTilemap");
        Tilemap foreground = FindTilemap("ForegroundTilemap");
        Tilemap collision = FindTilemap("CollisionTilemap");
        PathTiles paths = LoadPathTiles();
        Tile[] dirtTiles = LoadVariants("Dirt", 4);

        ClearAndPaintBase(ground, decoration, foreground, collision, 48, 28);
        PaintHorizontalPath(ground, paths, 0, -24, 23);
        PaintVerticalPath(ground, paths, 0, -14, 13);
        PaintDirtRectangle(ground, dirtTiles, -3, -3, 3, 3);

        RepositionTransition("ToIronforge", new Vector3(-23f, 0f, 0f));
        RepositionTransition(
            "FromIronforgeSpawn",
            new Vector3(-20f, 0f, 0f));
        RepositionTransition("ToGuildHall", new Vector3(22f, 0f, 0f));
        RepositionTransition(
            "FromGuildHallSpawn",
            new Vector3(19f, 0f, 0f));
        RepositionTransition("ToSuncrestInn", new Vector3(0f, -13f, 0f));
        RepositionTransition(
            "FromSuncrestInnSpawn",
            new Vector3(0f, -10f, 0f));
        RepositionTransition("DefaultSpawn", Vector3.zero);

        FinishScene(scene, ground, decoration, foreground, collision);
        EditorSceneManager.SaveScene(scene);
    }

    private static void RemoveLayoutRoot()
    {
        GameObject layoutRoot = GameObject.Find(LayoutRootName);
        if (layoutRoot != null)
        {
            Object.DestroyImmediate(layoutRoot);
        }
    }

    private static void ApplyMarketLayout(Scene scene)
    {
        Tilemap ground = FindTilemap("GroundTilemap");
        Tilemap decoration = FindTilemap("DecorationTilemap");
        Tilemap foreground = FindTilemap("ForegroundTilemap");
        Tilemap collision = FindTilemap("CollisionTilemap");
        Tile collisionTile = LoadTile(CollisionTilePath);
        Tile[] dirtTiles = LoadVariants("Dirt", 4);
        PathTiles paths = LoadPathTiles();

        ClearAndPaintBase(ground, decoration, foreground, collision, 48, 28);

        PaintHorizontalPath(ground, paths, 0, -24, 23);
        PaintVerticalPath(ground, paths, 8, -14, -5);
        PaintDirtRectangle(ground, dirtTiles, -11, -6, 11, 6);

        GameObject layoutRoot = new GameObject(LayoutRootName);
        Tile stallBase = LoadTile(GetBlockoutTilePath("MarketStall"));
        Tile stallAccent = LoadTile(GetBlockoutTilePath("MarketCanopy"));

        CreateStall(
            decoration,
            foreground,
            collision,
            collisionTile,
            -9,
            3,
            -6,
            5,
            stallBase,
            stallAccent);
        CreateStall(
            decoration,
            foreground,
            collision,
            collisionTile,
            -3,
            3,
            0,
            5,
            stallBase,
            stallAccent);
        CreateStall(
            decoration,
            foreground,
            collision,
            collisionTile,
            4,
            3,
            7,
            5,
            stallBase,
            stallAccent);
        CreateStall(
            decoration,
            foreground,
            collision,
            collisionTile,
            -7,
            -5,
            -4,
            -3,
            stallBase,
            stallAccent);
        CreateStall(
            decoration,
            foreground,
            collision,
            collisionTile,
            1,
            -5,
            4,
            -3,
            stallBase,
            stallAccent);

        CreateSolidFeature(
            decoration,
            collision,
            collisionTile,
            8,
            3,
            9,
            4,
            LoadTile(GetBlockoutTilePath("MarketWell")));
        CreateMarker(
            layoutRoot.transform,
            "OldMarlowPlaceholder",
            new Vector3(-7.5f, 1f, 0f),
            new Vector3(0.65f, 1.1f, 1f),
            new Color(0.25f, 0.7f, 0.68f, 1f),
            false);

        PaintGroundDetail(ground, "PinkFlowerV1", -16, 8);
        PaintGroundDetail(ground, "PinkFlowerV3", -15, 8);
        PaintGroundDetail(ground, "PoppiesV2", 16, 7);
        PaintGroundDetail(ground, "PoppiesV4", 17, 7);
        PaintGroundDetail(ground, "StonesV1", -18, -8);
        PaintGroundDetail(ground, "StonesV7", 18, -7);
        PaintGroundDetail(ground, "WoodV1", -12, 4);
        PaintGroundDetail(ground, "WoodV5", 13, -4);

        RepositionTransition("ToSuncrestInn", new Vector3(8f, -13f, 0f));
        RepositionTransition(
            "FromSuncrestInnSpawn",
            new Vector3(8f, -10f, 0f));
        RepositionTransition("DefaultSpawn", new Vector3(0f, -8f, 0f));

        FinishScene(scene, ground, decoration, foreground, collision);
    }

    private static void ClearAndPaintBase(
        Tilemap ground,
        Tilemap decoration,
        Tilemap foreground,
        Tilemap collision,
        int width,
        int height)
    {
        ground.ClearAllTiles();
        decoration.ClearAllTiles();
        foreground.ClearAllTiles();
        collision.ClearAllTiles();
        ground.color = Color.white;
        decoration.color = Color.white;
        foreground.color = Color.white;
        collision.color = Color.white;

        int minimumX = -width / 2;
        int minimumY = -height / 2;
        int maximumX = minimumX + width - 1;
        int maximumY = minimumY + height - 1;
        Tile grass = LoadTile(
            $"{TileFolder}/Phase1_EnvTile_SoftGrassV6.asset");
        Tile collisionTile = LoadTile(CollisionTilePath);

        FillRectangle(
            ground,
            grass,
            minimumX - 10,
            minimumY - 6,
            maximumX + 10,
            maximumY + 6);
        PaintBoundary(
            collision,
            collisionTile,
            minimumX,
            minimumY,
            maximumX,
            maximumY);
    }

    private static void PaintVerticalPath(
        Tilemap ground,
        PathTiles paths,
        int centerX,
        int minimumY,
        int maximumY)
    {
        for (int y = minimumY; y <= maximumY; y++)
        {
            int variant = PositiveModulo(y, paths.Middle.Length);
            ground.SetTile(
                new Vector3Int(centerX - 1, y, 0),
                paths.VerticalLeft[variant]);
            ground.SetTile(
                new Vector3Int(centerX, y, 0),
                paths.Middle[variant]);
            ground.SetTile(
                new Vector3Int(centerX + 1, y, 0),
                paths.VerticalRight[variant]);
        }
    }

    private static void PaintHorizontalPath(
        Tilemap ground,
        PathTiles paths,
        int centerY,
        int minimumX,
        int maximumX)
    {
        for (int x = minimumX; x <= maximumX; x++)
        {
            int variant = PositiveModulo(x, paths.Middle.Length);
            ground.SetTile(
                new Vector3Int(x, centerY + 1, 0),
                paths.HorizontalTop[variant]);
            ground.SetTile(
                new Vector3Int(x, centerY, 0),
                paths.Middle[variant]);
            ground.SetTile(
                new Vector3Int(x, centerY - 1, 0),
                paths.HorizontalBottom[variant]);
        }
    }

    private static void PaintDirtRectangle(
        Tilemap ground,
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
                int variant = PositiveModulo(x * 397 ^ y * 7919, dirtTiles.Length);
                ground.SetTile(new Vector3Int(x, y, 0), dirtTiles[variant]);
            }
        }
    }

    private static void CreateBuilding(
        Tilemap decoration,
        Tilemap foreground,
        Tilemap collision,
        TileBase collisionTile,
        int minimumX,
        int minimumY,
        int maximumX,
        int maximumY,
        int doorX,
        Tile wall,
        Tile roof,
        Tile accent)
    {
        FillRectangle(
            decoration,
            wall,
            minimumX,
            minimumY,
            maximumX,
            minimumY + 1);
        FillRectangle(
            decoration,
            roof,
            minimumX,
            minimumY + 2,
            maximumX,
            maximumY);
        for (int x = minimumX; x <= maximumX; x += 3)
        {
            foreground.SetTile(new Vector3Int(x, maximumY, 0), accent);
        }

        for (int x = minimumX; x <= maximumX; x++)
        {
            for (int y = minimumY; y <= maximumY; y++)
            {
                if (y == minimumY && x == doorX)
                {
                    decoration.SetTile(new Vector3Int(x, y, 0), accent);
                }

                collision.SetTile(new Vector3Int(x, y, 0), collisionTile);
            }
        }
    }

    private static void CreateStall(
        Tilemap decoration,
        Tilemap foreground,
        Tilemap collision,
        TileBase collisionTile,
        int minimumX,
        int minimumY,
        int maximumX,
        int maximumY,
        Tile baseTile,
        Tile canopyTile)
    {
        FillRectangle(
            decoration,
            baseTile,
            minimumX,
            minimumY,
            maximumX,
            maximumY);
        for (int x = minimumX; x <= maximumX; x++)
        {
            foreground.SetTile(new Vector3Int(x, maximumY, 0), canopyTile);
        }

        FillRectangle(
            collision,
            collisionTile,
            minimumX,
            minimumY,
            maximumX,
            maximumY);
    }

    private static void CreateBuildingWing(
        Tilemap decoration,
        Tilemap foreground,
        Tilemap collision,
        TileBase collisionTile,
        int minimumX,
        int minimumY,
        int maximumX,
        int maximumY,
        Tile wall,
        Tile roof)
    {
        FillRectangle(
            decoration,
            wall,
            minimumX,
            minimumY,
            maximumX,
            minimumY);
        FillRectangle(
            decoration,
            roof,
            minimumX,
            minimumY + 1,
            maximumX,
            maximumY);
        for (int x = minimumX; x <= maximumX; x++)
        {
            foreground.SetTile(new Vector3Int(x, maximumY, 0), roof);
        }

        FillRectangle(
            collision,
            collisionTile,
            minimumX,
            minimumY,
            maximumX,
            maximumY);
    }

    private static void CreateSolidFeature(
        Tilemap decoration,
        Tilemap collision,
        TileBase collisionTile,
        int minimumX,
        int minimumY,
        int maximumX,
        int maximumY,
        Tile featureTile)
    {
        FillRectangle(
            decoration,
            featureTile,
            minimumX,
            minimumY,
            maximumX,
            maximumY);
        FillRectangle(
            collision,
            collisionTile,
            minimumX,
            minimumY,
            maximumX,
            maximumY);
    }

    private static void CreateMarker(
        Transform parent,
        string objectName,
        Vector3 position,
        Vector3 scale,
        Color color,
        bool addCollider)
    {
        GameObject marker = new GameObject(objectName);
        marker.transform.SetParent(parent, false);
        marker.transform.position = position;
        marker.transform.localScale = scale;
        SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
        renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            PlaceholderSpritePath);
        renderer.color = color;
        renderer.sortingOrder = 2;
        if (addCollider)
        {
            marker.AddComponent<BoxCollider2D>();
        }
    }

    private static void PaintGroundDetail(
        Tilemap ground,
        string assetSuffix,
        int x,
        int y)
    {
        ground.SetTile(
            new Vector3Int(x, y, 0),
            LoadTile($"{TileFolder}/Phase1_EnvTile_{assetSuffix}.asset"));
    }

    private static void RepositionTransition(string objectName, Vector3 position)
    {
        GameObject target = GameObject.Find(objectName);
        if (target != null)
        {
            target.transform.position = position;
        }
    }

    private static void FinishScene(
        Scene scene,
        params Tilemap[] tilemaps)
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            tilemap.CompressBounds();
            EditorUtility.SetDirty(tilemap);
        }

        EditorSceneManager.MarkSceneDirty(scene);
    }

    private static PathTiles LoadPathTiles()
    {
        Tile[] verticalLeft = LoadVariants("WideDirtPathRightEdge", 4);
        Tile[] middle = LoadVariants("WideDirtPathMiddle", 4);
        Tile[] verticalRight = LoadVariants("WideDirtPathLeftEdge", 4);
        var horizontalTop = new Tile[4];
        var horizontalBottom = new Tile[4];

        for (int index = 0; index < 4; index++)
        {
            horizontalTop[index] = GetOrCreateRotatedTile(
                verticalLeft[index],
                $"WidePathTopV{index + 1}");
            horizontalBottom[index] = GetOrCreateRotatedTile(
                verticalRight[index],
                $"WidePathBottomV{index + 1}");
        }

        return new PathTiles(
            verticalLeft,
            middle,
            verticalRight,
            horizontalTop,
            horizontalBottom);
    }

    private static Tile GetOrCreateRotatedTile(Tile source, string assetName)
    {
        string path = $"{BlockoutTileFolder}/{assetName}.asset";
        Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
        if (tile == null)
        {
            tile = ScriptableObject.CreateInstance<Tile>();
            AssetDatabase.CreateAsset(tile, path);
        }

        tile.name = assetName;
        tile.sprite = source.sprite;
        tile.color = Color.white;
        tile.transform = Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, -90f));
        tile.colliderType = Tile.ColliderType.None;
        tile.flags = TileFlags.LockAll;
        EditorUtility.SetDirty(tile);
        return tile;
    }

    private static void EnsureBlockoutTiles()
    {
        EnsureAssetFolder(BlockoutTileFolder);
        GetOrCreateColorTile(
            "GuildWall",
            new Color(0.62f, 0.38f, 0.32f, 1f));
        GetOrCreateColorTile(
            "GuildRoof",
            new Color(0.25f, 0.48f, 0.56f, 1f));
        GetOrCreateColorTile(
            "GuildAccent",
            new Color(0.84f, 0.7f, 0.35f, 1f));
        GetOrCreateColorTile(
            "MarketStall",
            new Color(0.56f, 0.32f, 0.2f, 1f));
        GetOrCreateColorTile(
            "MarketCanopy",
            new Color(0.78f, 0.25f, 0.3f, 1f));
        GetOrCreateColorTile(
            "MarketWell",
            new Color(0.38f, 0.52f, 0.58f, 1f));
        AssetDatabase.SaveAssets();
    }

    private static Tile GetOrCreateColorTile(string assetName, Color color)
    {
        string path = GetBlockoutTilePath(assetName);
        Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
        if (tile == null)
        {
            tile = ScriptableObject.CreateInstance<Tile>();
            AssetDatabase.CreateAsset(tile, path);
        }

        tile.name = assetName;
        tile.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            PlaceholderSpritePath);
        tile.color = color;
        tile.transform = Matrix4x4.identity;
        tile.colliderType = Tile.ColliderType.None;
        tile.flags = TileFlags.LockAll;
        EditorUtility.SetDirty(tile);
        return tile;
    }

    private static string GetBlockoutTilePath(string assetName)
    {
        return $"{BlockoutTileFolder}/{assetName}.asset";
    }

    private static Tile[] LoadVariants(string category, int count)
    {
        var tiles = new Tile[count];
        for (int index = 0; index < count; index++)
        {
            tiles[index] = LoadTile(
                $"{TileFolder}/Phase1_EnvTile_{category}V{index + 1}.asset");
        }

        return tiles;
    }

    private static Tile LoadTile(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Tile>(path);
    }

    private static int PositiveModulo(int value, int divisor)
    {
        return (value & int.MaxValue) % divisor;
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

    private static Tilemap FindTilemap(string objectName)
    {
        GameObject tilemapObject = GameObject.Find(objectName);
        return tilemapObject == null
            ? null
            : tilemapObject.GetComponent<Tilemap>();
    }

    private static bool ValidateScenesAndTiles()
    {
        var missing = new List<string>();
        AddIfMissing<SceneAsset>(GetScenePath(GuildSceneName), missing);
        AddIfMissing<SceneAsset>(GetScenePath(MarketSceneName), missing);
        AddIfMissing<Sprite>(PlaceholderSpritePath, missing);
        AddIfMissing<Tile>(CollisionTilePath, missing);
        AddIfMissing<Tile>(
            $"{TileFolder}/Phase1_EnvTile_SoftGrassV6.asset",
            missing);
        for (int variant = 1; variant <= 4; variant++)
        {
            AddIfMissing<Tile>(
                $"{TileFolder}/Phase1_EnvTile_DirtV{variant}.asset",
                missing);
            AddIfMissing<Tile>(
                $"{TileFolder}/Phase1_EnvTile_WideDirtPathRightEdgeV{variant}.asset",
                missing);
            AddIfMissing<Tile>(
                $"{TileFolder}/Phase1_EnvTile_WideDirtPathMiddleV{variant}.asset",
                missing);
            AddIfMissing<Tile>(
                $"{TileFolder}/Phase1_EnvTile_WideDirtPathLeftEdgeV{variant}.asset",
                missing);
        }

        if (missing.Count == 0)
        {
            return true;
        }

        EditorUtility.DisplayDialog(
            "Suncrest District Blockouts",
            "Required generated scenes or Phase 1 tiles are missing:\n\n" +
            string.Join("\n", missing),
            "OK");
        return false;
    }

    private static void AddIfMissing<T>(string path, List<string> missing)
        where T : Object
    {
        if (AssetDatabase.LoadAssetAtPath<T>(path) == null)
        {
            missing.Add(path);
        }
    }

    private static string GetScenePath(string sceneName)
    {
        return $"{SceneFolder}/{sceneName}.unity";
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

    private sealed class PathTiles
    {
        public Tile[] VerticalLeft { get; }
        public Tile[] Middle { get; }
        public Tile[] VerticalRight { get; }
        public Tile[] HorizontalTop { get; }
        public Tile[] HorizontalBottom { get; }

        public PathTiles(
            Tile[] verticalLeft,
            Tile[] middle,
            Tile[] verticalRight,
            Tile[] horizontalTop,
            Tile[] horizontalBottom)
        {
            VerticalLeft = verticalLeft;
            Middle = middle;
            VerticalRight = verticalRight;
            HorizontalTop = horizontalTop;
            HorizontalBottom = horizontalBottom;
        }
    }
}
