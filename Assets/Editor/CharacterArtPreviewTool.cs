using System;
using UnityEditor;
using UnityEngine;

public sealed class CharacterArtTestPostprocessor : AssetPostprocessor
{
    private const string CandidateFolder =
        "Assets/Sprites/Characters/Candidates/";

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(CandidateFolder, StringComparison.Ordinal))
        {
            return;
        }

        Configure((TextureImporter)assetImporter);
    }

    public static void Configure(TextureImporter importer)
    {
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 16f;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Clamp;
        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        settings.spriteAlignment = (int)SpriteAlignment.Custom;

        // This candidate's feet sit seven pixels above its 48px canvas edge.
        settings.spritePivot = new Vector2(0.5f, 7f / 48f);
        importer.SetTextureSettings(settings);
    }
}

public static class CharacterArtPreviewTool
{
    private const string IonaCandidatePath =
        "Assets/Sprites/Characters/Candidates/pc_01_Iona/" +
        "pc_01_Iona_OverworldFront_Candidate01.png";
    private const string PreviewName =
        "[ART TEST] pc_01 Iona Overworld Candidate 01";

    [MenuItem(
        "Tools/AngelBlade RPG/Art/Place Iona Overworld Scale Test")]
    public static void PlaceIonaOverworldScaleTest()
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(IonaCandidatePath) as TextureImporter;
        if (importer == null)
        {
            EditorUtility.DisplayDialog(
                "Iona Scale Test",
                "The Iona candidate texture could not be found.",
                "OK");
            return;
        }

        CharacterArtTestPostprocessor.Configure(importer);
        importer.SaveAndReimport();

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(IonaCandidatePath);
        if (sprite == null)
        {
            EditorUtility.DisplayDialog(
                "Iona Scale Test",
                "The Iona candidate has not finished importing. Let Unity " +
                "refresh, then try again.",
                "OK");
            return;
        }

        GameObject existing = GameObject.Find(PreviewName);
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing);
        }

        GameObject preview = new GameObject(PreviewName);
        Undo.RegisterCreatedObjectUndo(preview, "Place Iona scale test");

        SpriteRenderer renderer = preview.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 100;

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            preview.transform.position = player.transform.position +
                new Vector3(2f, 0f, 0f);

            SpriteRenderer playerRenderer =
                player.GetComponentInChildren<SpriteRenderer>();
            if (playerRenderer != null)
            {
                renderer.sortingLayerID = playerRenderer.sortingLayerID;
                renderer.sortingOrder = playerRenderer.sortingOrder + 1;
            }
        }
        else
        {
            Vector3 scenePivot = SceneView.lastActiveSceneView == null
                ? Vector3.zero
                : SceneView.lastActiveSceneView.pivot;
            preview.transform.position = new Vector3(
                Mathf.Round(scenePivot.x),
                Mathf.Round(scenePivot.y),
                0f);
        }

        Selection.activeGameObject = preview;
        SceneView.lastActiveSceneView?.FrameSelected();
        EditorUtility.DisplayDialog(
            "Iona Scale Test",
            "Iona was placed beside the Player at 16 PPU. This is a " +
            "temporary scene object: use Undo or delete it when the scale " +
            "review is finished.",
            "OK");
    }
}
