using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class PlaceholderAssetImporter
{
    private const string KenneySheetPath =
        "Assets/ThirdParty/Kenney/RoguelikeRPGPack/" +
        "roguelikeSheet_transparent.png";
    private const string RpgHouseSheetPath =
        "Assets/ThirdParty/knekko/RPGHouseLowRes/" +
        "rpg-house-sprites-lowres.png";
    private const string RpgUrbanSheetPath =
        "Assets/ThirdParty/Kenney/RPGUrban/rpg_urban_packed.png";
    private static readonly string[] UiAndIconFolders =
    {
        "Assets/ThirdParty/Kenney/RPGUIExpansion/PNG",
        "Assets/ThirdParty/Kenney/GameIcons/White1x",
        "Assets/ThirdParty/Kenney/GameIconsExpansion/White1x",
        "Assets/ThirdParty/Kenney/GameIconsExpansion/Colored1x"
    };
    private const int TileSize = 16;
    private const int TileGap = 1;
    private const string KenneyTileFolder =
        "Assets/Tilemaps/Tiles/Placeholders/Kenney";

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Prepare Kenney Placeholder Sheet")]
    public static void PrepareKenneyPlaceholderSheet()
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(KenneySheetPath) as TextureImporter;
        if (importer == null)
        {
            EditorUtility.DisplayDialog(
                "Placeholder Import Failed",
                $"The Kenney sprite sheet was not found at {KenneySheetPath}.",
                "OK");
            return;
        }

        ConfigureForSlicing(importer);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(
            KenneySheetPath);
        if (texture == null)
        {
            ShowFailure("The Kenney sprite sheet could not be loaded.");
            return;
        }

        SpriteDataProviderFactories factories =
            new SpriteDataProviderFactories();
        factories.Init();
        ISpriteEditorDataProvider provider = factories
            .GetSpriteEditorDataProviderFromObject(importer);
        if (provider == null)
        {
            ShowFailure("Unity's Sprite Editor data provider is unavailable.");
            return;
        }

        provider.InitSpriteEditorDataProvider();
        Dictionary<string, GUID> existingIds = provider.GetSpriteRects()
            .GroupBy(rect => rect.name)
            .ToDictionary(group => group.Key, group => group.First().spriteID);
        List<SpriteRect> rects = BuildVisibleTileRects(
            texture,
            existingIds,
            "kenney_roguelike",
            TileGap);
        provider.SetSpriteRects(rects.ToArray());

        ISpriteNameFileIdDataProvider names =
            provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        names?.SetNameFileIdPairs(rects.Select(
            rect => new SpriteNameFileIdPair(rect.name, rect.spriteID)));
        provider.Apply();

        importer.isReadable = false;
        importer.SaveAndReimport();
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog(
            "Kenney Placeholders Ready",
            $"Prepared {rects.Count} non-empty 16x16 placeholder sprites. " +
            "Expand the sprite sheet in the Project window to use them.",
            "OK");
    }

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Prepare RPG House Placeholder Sheet")]
    public static void PrepareRpgHousePlaceholderSheet()
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(RpgHouseSheetPath) as TextureImporter;
        if (importer == null)
        {
            ShowFailure(
                $"The RPG house sprite sheet was not found at " +
                RpgHouseSheetPath + ".");
            return;
        }

        ConfigureForSlicing(importer);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(
            RpgHouseSheetPath);
        ISpriteEditorDataProvider provider = CreateProvider(importer);
        if (texture == null || provider == null)
        {
            ShowFailure("The RPG house sprite sheet could not be prepared.");
            return;
        }

        Dictionary<string, GUID> existingIds = provider.GetSpriteRects()
            .GroupBy(rect => rect.name)
            .ToDictionary(group => group.Key, group => group.First().spriteID);
        List<SpriteRect> rects = BuildConnectedSpriteRects(
            texture,
            existingIds);
        ApplySpriteRects(provider, rects);

        importer.isReadable = false;
        importer.SaveAndReimport();
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog(
            "RPG House Placeholders Ready",
            $"Prepared {rects.Count} complete building sprites. Expand the " +
            "sheet in the Project window and drag a sprite into a scene.",
            "OK");
    }

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Prepare Kenney RPG Urban Sheet")]
    public static void PrepareRpgUrbanSheet()
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(RpgUrbanSheetPath) as TextureImporter;
        if (importer == null)
        {
            ShowFailure($"The RPG Urban sheet was not found at " +
                RpgUrbanSheetPath + ".");
            return;
        }

        ConfigureForSlicing(importer);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(
            RpgUrbanSheetPath);
        ISpriteEditorDataProvider provider = CreateProvider(importer);
        if (texture == null || provider == null)
        {
            ShowFailure("The RPG Urban sprite sheet could not be prepared.");
            return;
        }

        Dictionary<string, GUID> existingIds = provider.GetSpriteRects()
            .GroupBy(rect => rect.name)
            .ToDictionary(group => group.Key, group => group.First().spriteID);
        List<SpriteRect> rects = BuildVisibleTileRects(
            texture,
            existingIds,
            "kenney_urban",
            0);
        ApplySpriteRects(provider, rects);

        importer.isReadable = false;
        importer.SaveAndReimport();
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog(
            "RPG Urban Placeholders Ready",
            $"Prepared {rects.Count} staging sprites. Keep these temporary; " +
            "their visual style is not the final Suncrest direction.",
            "OK");
    }

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Prepare Kenney UI And Icon Sprites")]
    public static void PrepareUiAndIconSprites()
    {
        string[] textureGuids = AssetDatabase.FindAssets(
            "t:Texture2D",
            UiAndIconFolders);
        int preparedCount = 0;
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer =
                AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            ConfigureSingleSprite(importer);
            preparedCount++;
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog(
            "UI And Icons Ready",
            $"Prepared {preparedCount} point-filtered UI and icon sprites.",
            "OK");
    }

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Add Selected Kenney Sprites To Palette")]
    public static void AddSelectedKenneySpritesToPalette()
    {
        Sprite[] sprites = Selection.objects
            .OfType<Sprite>()
            .Where(sprite => AssetDatabase.GetAssetPath(sprite) ==
                KenneySheetPath)
            .ToArray();
        if (sprites.Length == 0)
        {
            ShowFailure(
                "Expand the Kenney sprite sheet and select one or more " +
                "sprite slices first.");
            return;
        }

        EnsureAssetFolder(KenneyTileFolder);
        int preparedCount = 0;
        foreach (Sprite sprite in sprites)
        {
            string tilePath = $"{KenneyTileFolder}/{sprite.name}.asset";
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, tilePath);
            }

            tile.name = sprite.name;
            tile.sprite = sprite;
            tile.color = Color.white;
            tile.transform = Matrix4x4.identity;
            tile.gameObject = null;
            tile.flags = TileFlags.LockAll;
            tile.colliderType = Tile.ColliderType.None;
            EditorUtility.SetDirty(tile);
            if (PixelTilePaletteImporter.AddTileToPalette(tile))
            {
                preparedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog(
            "Kenney Palette Tiles Ready",
            $"Added or updated {preparedCount} selected placeholder tiles.",
            "OK");
    }

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Add Selected Kenney Sprites To Palette",
        true)]
    private static bool CanAddSelectedKenneySpritesToPalette()
    {
        return Selection.objects.OfType<Sprite>().Any(
            sprite => AssetDatabase.GetAssetPath(sprite) == KenneySheetPath);
    }

    private static void ConfigureForSlicing(TextureImporter importer)
    {
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = TileSize;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.isReadable = true;
        importer.SaveAndReimport();
    }

    private static void ConfigureSingleSprite(TextureImporter importer)
    {
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = TileSize;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
    }

    private static List<SpriteRect> BuildVisibleTileRects(
        Texture2D texture,
        IReadOnlyDictionary<string, GUID> existingIds,
        string namePrefix,
        int gap)
    {
        Color32[] pixels = texture.GetPixels32();
        int stride = TileSize + gap;
        int columns = (texture.width + gap) / stride;
        int rows = (texture.height + gap) / stride;
        var rects = new List<SpriteRect>();

        for (int topRow = 0; topRow < rows; topRow++)
        {
            int y = texture.height - TileSize - topRow * stride;
            for (int column = 0; column < columns; column++)
            {
                int x = column * stride;
                if (!ContainsVisiblePixel(
                    pixels,
                    texture.width,
                    x,
                    y))
                {
                    continue;
                }

                string name = $"{namePrefix}_r{topRow:D2}_c{column:D2}";
                GUID id = existingIds.TryGetValue(name, out GUID existingId)
                    ? existingId
                    : GUID.Generate();
                rects.Add(new SpriteRect
                {
                    name = name,
                    rect = new Rect(x, y, TileSize, TileSize),
                    alignment = SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f),
                    spriteID = id
                });
            }
        }

        return rects;
    }

    private static List<SpriteRect> BuildConnectedSpriteRects(
        Texture2D texture,
        IReadOnlyDictionary<string, GUID> existingIds)
    {
        Color32[] pixels = texture.GetPixels32();
        bool[] visited = new bool[pixels.Length];
        var componentRects = new List<Rect>();

        for (int index = 0; index < pixels.Length; index++)
        {
            if (visited[index] || pixels[index].a == 0)
            {
                continue;
            }

            Rect component = FindConnectedComponent(
                pixels,
                visited,
                texture.width,
                texture.height,
                index,
                out int pixelCount);
            if (pixelCount >= 4)
            {
                componentRects.Add(component);
            }
        }

        componentRects.Sort((left, right) =>
        {
            int rowComparison = right.yMax.CompareTo(left.yMax);
            return rowComparison != 0
                ? rowComparison
                : left.xMin.CompareTo(right.xMin);
        });

        var rects = new List<SpriteRect>();
        for (int index = 0; index < componentRects.Count; index++)
        {
            string name = $"rpg_house_chunk_{index:D2}";
            GUID id = existingIds.TryGetValue(name, out GUID existingId)
                ? existingId
                : GUID.Generate();
            rects.Add(new SpriteRect
            {
                name = name,
                rect = componentRects[index],
                alignment = SpriteAlignment.Custom,
                pivot = new Vector2(0.5f, 0f),
                spriteID = id
            });
        }

        return rects;
    }

    private static Rect FindConnectedComponent(
        Color32[] pixels,
        bool[] visited,
        int width,
        int height,
        int startIndex,
        out int pixelCount)
    {
        var queue = new Queue<int>();
        queue.Enqueue(startIndex);
        visited[startIndex] = true;
        int minX = startIndex % width;
        int maxX = minX;
        int minY = startIndex / width;
        int maxY = minY;
        pixelCount = 0;

        while (queue.Count > 0)
        {
            int index = queue.Dequeue();
            int x = index % width;
            int y = index / width;
            minX = Math.Min(minX, x);
            maxX = Math.Max(maxX, x);
            minY = Math.Min(minY, y);
            maxY = Math.Max(maxY, y);
            pixelCount++;

            VisitPixel(pixels, visited, width, height, x - 1, y, queue);
            VisitPixel(pixels, visited, width, height, x + 1, y, queue);
            VisitPixel(pixels, visited, width, height, x, y - 1, queue);
            VisitPixel(pixels, visited, width, height, x, y + 1, queue);
        }

        return new Rect(
            minX,
            minY,
            maxX - minX + 1,
            maxY - minY + 1);
    }

    private static void VisitPixel(
        Color32[] pixels,
        bool[] visited,
        int width,
        int height,
        int x,
        int y,
        Queue<int> queue)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return;
        }

        int index = y * width + x;
        if (visited[index] || pixels[index].a == 0)
        {
            return;
        }

        visited[index] = true;
        queue.Enqueue(index);
    }

    private static bool ContainsVisiblePixel(
        Color32[] pixels,
        int textureWidth,
        int startX,
        int startY)
    {
        for (int y = startY; y < startY + TileSize; y++)
        {
            int rowStart = y * textureWidth;
            for (int x = startX; x < startX + TileSize; x++)
            {
                if (pixels[rowStart + x].a > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static void ShowFailure(string message)
    {
        EditorUtility.DisplayDialog(
            "Placeholder Import Failed",
            message,
            "OK");
    }

    private static ISpriteEditorDataProvider CreateProvider(
        TextureImporter importer)
    {
        SpriteDataProviderFactories factories =
            new SpriteDataProviderFactories();
        factories.Init();
        ISpriteEditorDataProvider provider = factories
            .GetSpriteEditorDataProviderFromObject(importer);
        provider?.InitSpriteEditorDataProvider();
        return provider;
    }

    private static void ApplySpriteRects(
        ISpriteEditorDataProvider provider,
        List<SpriteRect> rects)
    {
        provider.SetSpriteRects(rects.ToArray());
        ISpriteNameFileIdDataProvider names =
            provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        names?.SetNameFileIdPairs(rects.Select(
            rect => new SpriteNameFileIdPair(rect.name, rect.spriteID)));
        provider.Apply();
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
}
