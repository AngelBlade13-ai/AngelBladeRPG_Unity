using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class ContributorAssetDeliveryImporter
{
    private const string TextureFolder =
        "Assets/Sprites/Environments/Phase1";
    private const string TileFolder = "Assets/Tilemaps/Tiles/Phase1";
    private static readonly IReadOnlyDictionary<string, string>
        LegacyTileReplacements = BuildLegacyTileReplacements();

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Sync Phase 1 Contributor Delivery ZIP")]
    public static void SyncPhase1Delivery()
    {
        string zipPath = EditorUtility.OpenFilePanel(
            "Select Phase 1 Contributor Delivery",
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "zip");
        if (string.IsNullOrWhiteSpace(zipPath))
        {
            return;
        }

        try
        {
            Dictionary<string, DeliveryFile> delivery = ReadDelivery(zipPath);
            List<string> existingPaths = Directory
                .GetFiles(TextureFolder, "*.png", SearchOption.TopDirectoryOnly)
                .Select(NormalizeAssetPath)
                .ToList();
            var deliveryPaths = new HashSet<string>(
                delivery.Keys,
                StringComparer.OrdinalIgnoreCase);
            List<string> retiredPaths = existingPaths
                .Where(path => !deliveryPaths.Contains(path))
                .ToList();

            if (retiredPaths.Count > 0 && !EditorUtility.DisplayDialog(
                "Mirror Phase 1 Delivery?",
                $"This delivery contains {delivery.Count} PNGs and omits " +
                $"{retiredPaths.Count} existing PNGs. Omitted contributor " +
                "files and their generated Tile assets will be removed. " +
                "Continue only if the ZIP is the complete authoritative " +
                "delivery.",
                "Mirror Delivery",
                "Cancel"))
            {
                return;
            }

            int migrated = ReplaceLegacyTilesInLoadedScenes();
            PixelTilePaletteImporter.ClearPhase1TilesFromPalette();
            SyncDelivery(delivery, retiredPaths, out int renamed, out int removed);
            PixelTilePaletteImporter.ImportAllPhase1EnvironmentTiles();
            EditorUtility.DisplayDialog(
                "Phase 1 Delivery Synced",
                $"Mirrored {delivery.Count} contributor PNGs. Preserved " +
                $"{renamed} GUIDs through exact-content renames and removed " +
                $"{removed} retired files. Migrated {migrated} painted cells " +
                "in the loaded scene. The project folder now matches the " +
                "delivery ZIP.",
                "OK");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog(
                "Phase 1 Sync Failed",
                exception.Message,
                "OK");
        }
    }

    private static Dictionary<string, DeliveryFile> ReadDelivery(string zipPath)
    {
        var delivery = new Dictionary<string, DeliveryFile>(
            StringComparer.OrdinalIgnoreCase);
        using FileStream stream = File.OpenRead(zipPath);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (!entry.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string assetPath = $"{TextureFolder}/{GetCanonicalName(entry.Name)}";
            if (delivery.ContainsKey(assetPath))
            {
                throw new InvalidDataException(
                    $"The ZIP contains more than one file named {assetPath}.");
            }

            using Stream entryStream = entry.Open();
            using var memory = new MemoryStream();
            entryStream.CopyTo(memory);
            byte[] bytes = memory.ToArray();
            ValidateTile(assetPath, bytes);
            delivery.Add(assetPath, new DeliveryFile(bytes, GetHash(bytes)));
        }

        if (delivery.Count == 0)
        {
            throw new InvalidDataException(
                "The selected ZIP does not contain any PNG files.");
        }

        return delivery;
    }

    private static void SyncDelivery(
        Dictionary<string, DeliveryFile> delivery,
        List<string> retiredPaths,
        out int renamed,
        out int removed)
    {
        renamed = 0;
        removed = 0;
        var availableRetired = new HashSet<string>(
            retiredPaths,
            StringComparer.OrdinalIgnoreCase);

        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (KeyValuePair<string, DeliveryFile> pair in delivery)
            {
                string destination = pair.Key;
                if (!File.Exists(destination))
                {
                    string renameSource = availableRetired.FirstOrDefault(
                        path => GetHash(File.ReadAllBytes(path)) == pair.Value.Hash);
                    if (!string.IsNullOrEmpty(renameSource))
                    {
                        MoveTextureAndTile(renameSource, destination);
                        availableRetired.Remove(renameSource);
                        renamed++;
                    }
                }

                File.WriteAllBytes(destination, pair.Value.Bytes);
            }

            foreach (string retiredPath in availableRetired)
            {
                DeleteTextureAndTile(retiredPath);
                removed++;
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static void MoveTextureAndTile(string source, string destination)
    {
        string moveError = AssetDatabase.MoveAsset(source, destination);
        if (!string.IsNullOrEmpty(moveError))
        {
            throw new IOException(moveError);
        }

        string sourceTile = GetTilePath(source);
        string destinationTile = GetTilePath(destination);
        if (AssetDatabase.LoadMainAssetAtPath(sourceTile) != null)
        {
            moveError = AssetDatabase.MoveAsset(sourceTile, destinationTile);
            if (!string.IsNullOrEmpty(moveError))
            {
                throw new IOException(moveError);
            }
        }
    }

    private static int ReplaceLegacyTilesInLoadedScenes()
    {
        var replacements = new Dictionary<TileBase, TileBase>();
        foreach (KeyValuePair<string, string> pair in LegacyTileReplacements)
        {
            Tile oldTile = AssetDatabase.LoadAssetAtPath<Tile>(
                $"{TileFolder}/Phase1_EnvTile_{pair.Key}.asset");
            Tile newTile = AssetDatabase.LoadAssetAtPath<Tile>(
                $"{TileFolder}/Phase1_EnvTile_{pair.Value}.asset");
            if (oldTile != null && newTile != null)
            {
                replacements.Add(oldTile, newTile);
            }
        }

        int migrated = 0;
        Tilemap[] tilemaps = UnityEngine.Object.FindObjectsByType<Tilemap>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        foreach (Tilemap tilemap in tilemaps)
        {
            var changes = new List<KeyValuePair<Vector3Int, TileBase>>();
            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase existing = tilemap.GetTile(position);
                if (existing != null &&
                    replacements.TryGetValue(existing, out TileBase replacement))
                {
                    changes.Add(
                        new KeyValuePair<Vector3Int, TileBase>(position, replacement));
                }
            }

            if (changes.Count == 0)
            {
                continue;
            }

            Undo.RegisterCompleteObjectUndo(
                tilemap,
                "Migrate retired contributor tiles");
            foreach (KeyValuePair<Vector3Int, TileBase> change in changes)
            {
                tilemap.SetTile(change.Key, change.Value);
            }

            migrated += changes.Count;
            EditorUtility.SetDirty(tilemap);
            if (tilemap.gameObject.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(tilemap.gameObject.scene);
            }
        }

        return migrated;
    }

    private static void DeleteTextureAndTile(string texturePath)
    {
        AssetDatabase.DeleteAsset(GetTilePath(texturePath));
        AssetDatabase.DeleteAsset(texturePath);
    }

    private static string GetTilePath(string texturePath)
    {
        return $"{TileFolder}/{Path.GetFileNameWithoutExtension(texturePath)}.asset";
    }

    private static string GetCanonicalName(string filename)
    {
        string stem = Path.GetFileNameWithoutExtension(filename);
        const string phasePrefix = "Phase1 EnvTile ";
        const string shortPrefix = "EnvTile ";
        if (stem.StartsWith(phasePrefix, StringComparison.OrdinalIgnoreCase))
        {
            stem = stem.Substring(phasePrefix.Length);
        }
        else if (stem.StartsWith(shortPrefix, StringComparison.OrdinalIgnoreCase))
        {
            stem = stem.Substring(shortPrefix.Length);
        }

        string suffix = string.Concat(
            stem.Where(character => !char.IsWhiteSpace(character)));
        return $"Phase1_EnvTile_{suffix}.png";
    }

    private static void ValidateTile(string assetPath, byte[] bytes)
    {
        var texture = new Texture2D(2, 2);
        try
        {
            if (!ImageConversion.LoadImage(texture, bytes) ||
                texture.width != 16 || texture.height != 16)
            {
                throw new InvalidDataException(
                    $"{assetPath} must be an exactly 16x16 PNG.");
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(texture);
        }
    }

    private static string GetHash(byte[] bytes)
    {
        using SHA256 sha = SHA256.Create();
        return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "");
    }

    private static string NormalizeAssetPath(string path)
    {
        return path.Replace('\\', '/');
    }

    private static IReadOnlyDictionary<string, string>
        BuildLegacyTileReplacements()
    {
        var replacements = new Dictionary<string, string>();
        AddVariants(replacements, "CornFlower", "SparseBlueFlower", 4);
        AddVariants(replacements, "Dandelion", "SparseYellowFlower", 2);
        AddVariants(replacements, "PinkFlower", "SparsePinkFlower", 4);
        AddVariants(replacements, "Poppies", "SparseRedFlower", 4);
        AddVariants(replacements, "WhiteFlower", "SparseWhiteFlower", 4);
        replacements.Add(
            "DirtPathCornerV1",
            "DirtPathBottomLeftCornerV1");
        replacements.Add(
            "DirtPathCornerV2",
            "DirtPathTopRightCornerV1");
        return replacements;
    }

    private static void AddVariants(
        IDictionary<string, string> replacements,
        string oldCategory,
        string newCategory,
        int count)
    {
        for (int variant = 1; variant <= count; variant++)
        {
            replacements.Add(
                $"{oldCategory}V{variant}",
                $"{newCategory}V{variant}");
        }
    }

    private sealed class DeliveryFile
    {
        public DeliveryFile(byte[] bytes, string hash)
        {
            Bytes = bytes;
            Hash = hash;
        }

        public byte[] Bytes { get; }
        public string Hash { get; }
    }
}
