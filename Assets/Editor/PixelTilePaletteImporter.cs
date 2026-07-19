using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class PixelTilePaletteImporter
{
    private const string PalettePath =
        "Assets/Tilemaps/Palettes/ExplorationTestPalette.prefab";
    private const string TileFolder = "Assets/Tilemaps/Tiles";
    private const string Phase1TileFolder = "Assets/Tilemaps/Tiles/Phase1";
    private const string Phase1TextureFolder =
        "Assets/Sprites/Environments/Phase1";
    private const string SoftGrassTexturePath =
        "Assets/Sprites/Enviroments/Phase1_EnvTile_SoftGrassV1.png";

    private static readonly string[] Phase1CategoryOrder =
    {
        "SoftGrass",
        "HardGrass",
        "Dirt",
        "DirtMiddle",
        "WideDirtPathRight",
        "WideDirtPathMiddle",
        "WideDirtPathLeft",
        "ThinDirtPath",
        "DirtPathTopLeftCorner",
        "DirtPathTopRightCorner",
        "DirtPathBottomLeftCorner",
        "DirtPathBottomRightCorner",
        "DirtPathCorner",
        "Farmland",
        "Cobblestone",
        "Wood",
        "WoodPlank",
        "Stones",
        "BigStones",
        "Pebble",
        "SmallStone",
        "SparseBlueFlower",
        "SparsePinkFlower",
        "SparsePurpleFlower",
        "SparseRedFlower",
        "SparseWhiteFlower",
        "SparseYellowFlower",
        "DenseBlueFlower",
        "DensePinkFlower",
        "DensePurpleFlower",
        "DenseRedFlower",
        "DenseWhiteFlower",
        "DenseYellowFlower",
        "CornFlower",
        "Dandelion",
        "PinkFlower",
        "Poppies",
        "WhiteFlower"
    };

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Repair Soft Grass Test Tile")]
    public static void RepairSoftGrassTestTile()
    {
        ImportTextureAsTile(SoftGrassTexturePath, true);
    }

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Import All Phase 1 Environment Tiles")]
    public static void ImportAllPhase1EnvironmentTiles()
    {
        EnsureAssetFolder(Phase1TileFolder);

        string[] textureGuids = AssetDatabase.FindAssets(
            "t:Texture2D",
            new[] { Phase1TextureFolder });
        var texturePaths = new List<string>();
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                texturePaths.Add(path);
            }
        }

        texturePaths.Sort(ComparePhase1Paths);
        var tiles = new List<Tile>();
        foreach (string texturePath in texturePaths)
        {
            Tile tile = CreateOrUpdateTile(texturePath, Phase1TileFolder);
            if (tile != null)
            {
                tiles.Add(tile);
            }
        }

        if (tiles.Count == 0)
        {
            EditorUtility.DisplayDialog(
                "Phase 1 Import Failed",
                $"No 16x16 PNG files were found under {Phase1TextureFolder}.",
                "OK");
            return;
        }

        if (!ArrangePhase1TilesInPalette(tiles))
        {
            EditorUtility.DisplayDialog(
                "Phase 1 Import Failed",
                "The exploration Tile Palette could not be loaded.",
                "OK");
            return;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog(
            "Phase 1 Tiles Ready",
            $"Imported {tiles.Count} tiles and arranged them by category " +
            "in the exploration palette.",
            "OK");
    }

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Move Mispainted Soft Grass To Ground")]
    public static void MoveMispaintedSoftGrassToGround()
    {
        GameObject groundObject = GameObject.Find("GroundTilemap");
        GameObject foregroundObject = GameObject.Find("ForegroundTilemap");
        Tilemap ground = groundObject == null
            ? null
            : groundObject.GetComponent<Tilemap>();
        Tilemap foreground = foregroundObject == null
            ? null
            : foregroundObject.GetComponent<Tilemap>();

        if (ground == null || foreground == null)
        {
            EditorUtility.DisplayDialog(
                "Soft Grass Repair",
                "The active scene needs GroundTilemap and ForegroundTilemap objects.",
                "OK");
            return;
        }

        Undo.RegisterCompleteObjectUndo(ground, "Move Soft Grass To Ground");
        Undo.RegisterCompleteObjectUndo(
            foreground,
            "Move Soft Grass To Ground");

        int movedCount = 0;
        foreach (Vector3Int position in foreground.cellBounds.allPositionsWithin)
        {
            TileBase tile = foreground.GetTile(position);
            if (tile == null || !tile.name.StartsWith(
                "Phase1_EnvTile_SoftGrass",
                StringComparison.Ordinal))
            {
                continue;
            }

            ground.SetTile(position, tile);
            foreground.SetTile(position, null);
            movedCount++;
        }

        ground.CompressBounds();
        foreground.CompressBounds();
        EditorUtility.SetDirty(ground);
        EditorUtility.SetDirty(foreground);
        EditorSceneManager.MarkSceneDirty(
            EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog(
            "Soft Grass Repair",
            movedCount == 0
                ? "No misplaced SoftGrass tiles were found in the foreground."
                : $"Moved {movedCount} SoftGrass tiles to GroundTilemap. " +
                    "Save the scene after inspecting the result.",
            "OK");
    }

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Add Selected 16x16 Tile To Palette")]
    public static void AddSelectedTileToPalette()
    {
        string texturePath = AssetDatabase.GetAssetPath(Selection.activeObject);
        ImportTextureAsTile(texturePath, true);
    }

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Add Selected 16x16 Tile To Palette",
        true)]
    private static bool CanAddSelectedTileToPalette()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return !string.IsNullOrWhiteSpace(path) &&
            AssetImporter.GetAtPath(path) is TextureImporter;
    }

    private static void ImportTextureAsTile(
        string texturePath,
        bool showResult)
    {
        Tile tile = CreateOrUpdateTile(texturePath, TileFolder);
        if (tile == null)
        {
            ShowError(showResult, "Select an exactly 16x16 PNG texture.");
            return;
        }

        if (!AddTileToPalette(tile))
        {
            ShowError(showResult, "The exploration Tile Palette could not be loaded.");
            return;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        if (showResult)
        {
            EditorUtility.DisplayDialog(
                "Pixel Tile Ready",
                $"{tile.name} is configured and available in the exploration palette.",
                "OK");
        }
    }

    private static Tile CreateOrUpdateTile(
        string texturePath,
        string tileFolder)
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer == null)
        {
            return null;
        }

        ConfigureImporter(importer);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(
            texturePath);
        if (texture == null || texture.width != 16 || texture.height != 16)
        {
            return null;
        }

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
        if (sprite == null)
        {
            return null;
        }

        EnsureAssetFolder(tileFolder);
        string tilePath = $"{tileFolder}/" +
            $"{Path.GetFileNameWithoutExtension(texturePath)}.asset";
        Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
        if (tile == null)
        {
            tile = ScriptableObject.CreateInstance<Tile>();
            AssetDatabase.CreateAsset(tile, tilePath);
        }

        tile.name = Path.GetFileNameWithoutExtension(texturePath);
        tile.sprite = sprite;
        tile.color = Color.white;
        tile.transform = Matrix4x4.identity;
        tile.gameObject = null;
        tile.flags = TileFlags.LockAll;
        tile.colliderType = Tile.ColliderType.None;
        EditorUtility.SetDirty(tile);
        return tile;
    }

    private static void ConfigureImporter(TextureImporter importer)
    {
        bool needsReimport =
            importer.textureType != TextureImporterType.Sprite ||
            importer.spriteImportMode != SpriteImportMode.Single ||
            !Mathf.Approximately(importer.spritePixelsPerUnit, 16f) ||
            importer.filterMode != FilterMode.Point ||
            importer.textureCompression !=
                TextureImporterCompression.Uncompressed ||
            importer.mipmapEnabled ||
            !importer.alphaIsTransparency;
        if (!needsReimport)
        {
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 16f;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
    }

    private static bool ArrangePhase1TilesInPalette(List<Tile> tiles)
    {
        GameObject paletteRoot = PrefabUtility.LoadPrefabContents(PalettePath);
        if (paletteRoot == null)
        {
            return false;
        }

        try
        {
            Tilemap palette = paletteRoot.GetComponentInChildren<Tilemap>(true);
            if (palette == null)
            {
                return false;
            }

            var phase1Tiles = new HashSet<Tile>(tiles);
            foreach (Vector3Int position in palette.cellBounds.allPositionsWithin)
            {
                if (palette.GetTile(position) is Tile existing &&
                    phase1Tiles.Contains(existing))
                {
                    palette.SetTile(position, null);
                }
            }

            palette.CompressBounds();
            BoundsInt bounds = palette.cellBounds;
            int startX = bounds.size.x == 0 ? 0 : bounds.xMin;
            int rowY = bounds.size.y == 0 ? 0 : bounds.yMin - 2;

            string currentCategory = null;
            int column = 0;
            foreach (Tile tile in tiles)
            {
                string category = GetPhase1Category(tile.name);
                if (currentCategory != category)
                {
                    if (currentCategory != null)
                    {
                        rowY--;
                    }

                    currentCategory = category;
                    column = 0;
                }

                palette.SetTile(
                    new Vector3Int(startX + column, rowY, 0),
                    tile);
                column++;
            }

            palette.CompressBounds();
            EditorUtility.SetDirty(palette);
            PrefabUtility.SaveAsPrefabAsset(paletteRoot, PalettePath);
            return true;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(paletteRoot);
        }
    }

    private static int ComparePhase1Paths(string left, string right)
    {
        string leftName = Path.GetFileNameWithoutExtension(left);
        string rightName = Path.GetFileNameWithoutExtension(right);
        int categoryComparison = GetCategoryIndex(leftName).CompareTo(
            GetCategoryIndex(rightName));
        if (categoryComparison != 0)
        {
            return categoryComparison;
        }

        return GetVariantNumber(leftName).CompareTo(
            GetVariantNumber(rightName));
    }

    private static int GetCategoryIndex(string assetName)
    {
        string category = GetPhase1Category(assetName);
        int index = Array.IndexOf(Phase1CategoryOrder, category);
        return index < 0 ? Phase1CategoryOrder.Length : index;
    }

    private static string GetPhase1Category(string assetName)
    {
        const string prefix = "Phase1_EnvTile_";
        string categoryAndVariant = assetName.StartsWith(prefix)
            ? assetName.Substring(prefix.Length)
            : assetName;
        int variantMarker = categoryAndVariant.LastIndexOf('V');
        return variantMarker > 0
            ? categoryAndVariant.Substring(0, variantMarker)
            : categoryAndVariant;
    }

    private static int GetVariantNumber(string assetName)
    {
        int variantMarker = assetName.LastIndexOf('V');
        if (variantMarker >= 0 &&
            int.TryParse(assetName.Substring(variantMarker + 1), out int number))
        {
            return number;
        }

        return 0;
    }

    private static void EnsureAssetFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
        string folderName = Path.GetFileName(folderPath);
        if (!string.IsNullOrEmpty(parent))
        {
            EnsureAssetFolder(parent);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }

    internal static bool AddTileToPalette(Tile tile)
    {
        GameObject paletteRoot = PrefabUtility.LoadPrefabContents(PalettePath);
        if (paletteRoot == null)
        {
            return false;
        }

        try
        {
            Tilemap palette = paletteRoot.GetComponentInChildren<Tilemap>(true);
            if (palette == null)
            {
                return false;
            }

            foreach (Vector3Int position in palette.cellBounds.allPositionsWithin)
            {
                if (palette.GetTile(position) == tile)
                {
                    return true;
                }
            }

            BoundsInt bounds = palette.cellBounds;
            Vector3Int newPosition = bounds.size.x == 0
                ? Vector3Int.zero
                : new Vector3Int(bounds.xMax + 1, bounds.yMin, 0);
            palette.SetTile(newPosition, tile);
            palette.CompressBounds();
            EditorUtility.SetDirty(palette);
            PrefabUtility.SaveAsPrefabAsset(paletteRoot, PalettePath);
            return true;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(paletteRoot);
        }
    }

    private static void ShowError(bool showResult, string message)
    {
        if (showResult)
        {
            EditorUtility.DisplayDialog("Pixel Tile Import Failed", message, "OK");
        }
    }
}
