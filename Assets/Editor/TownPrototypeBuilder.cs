using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TownPrototypeBuilder
{
    private const string GroundTilePath = "Assets/Tilemaps/Tiles/GroundTile.asset";
    private const string CollisionTilePath = "Assets/Tilemaps/Tiles/CollisionTile.asset";
    private const string PathTilePath = "Assets/Tilemaps/Tiles/PathTile.asset";

    [MenuItem("Tools/AngelBlade RPG/Build Placeholder Town")]
    public static void BuildPlaceholderTown()
    {
        Tilemap groundTilemap = FindTilemap("GroundTilemap");
        Tilemap collisionTilemap = FindTilemap("CollisionTilemap");
        GameObject entrance = GameObject.Find("TownEntranceSpawn");

        if (groundTilemap == null || collisionTilemap == null || entrance == null)
        {
            EditorUtility.DisplayDialog(
                "Placeholder Town",
                "Open TownScene and confirm GroundTilemap, CollisionTilemap, and TownEntranceSpawn exist.",
                "OK");
            return;
        }

        Tile groundTile = AssetDatabase.LoadAssetAtPath<Tile>(GroundTilePath);
        Tile collisionTile = AssetDatabase.LoadAssetAtPath<Tile>(CollisionTilePath);

        if (groundTile == null || collisionTile == null)
        {
            EditorUtility.DisplayDialog(
                "Placeholder Town",
                "GroundTile.asset or CollisionTile.asset could not be found.",
                "OK");
            return;
        }

        Tile pathTile = GetOrCreatePathTile(groundTile);

        Undo.RegisterCompleteObjectUndo(groundTilemap, "Build Placeholder Town");
        Undo.RegisterCompleteObjectUndo(collisionTilemap, "Build Placeholder Town");
        Undo.RecordObject(entrance.transform, "Move Town Entrance");

        groundTilemap.ClearAllTiles();
        collisionTilemap.ClearAllTiles();

        FillRectangle(groundTilemap, groundTile, -12, -8, 11, 7);
        FillRectangle(groundTilemap, pathTile, -1, -7, 1, 1);
        FillRectangle(groundTilemap, pathTile, -4, 0, 4, 3);

        PaintBoundary(collisionTilemap, collisionTile, -12, -8, 11, 7);
        PaintBuilding(collisionTilemap, collisionTile, -9, 2, -5, 6, -7);
        PaintBuilding(collisionTilemap, collisionTile, 5, 2, 9, 6, 7);

        entrance.transform.position = new Vector3(0f, -6f, entrance.transform.position.z);

        groundTilemap.CompressBounds();
        collisionTilemap.CompressBounds();
        EditorUtility.SetDirty(groundTilemap);
        EditorUtility.SetDirty(collisionTilemap);
        EditorUtility.SetDirty(entrance.transform);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Selection.activeGameObject = entrance;
        SceneView.lastActiveSceneView?.FrameSelected();
        EditorUtility.DisplayDialog(
            "Placeholder Town",
            "The placeholder town is ready. Save the scene, then test movement and collision in Play Mode.",
            "OK");
    }

    private static Tilemap FindTilemap(string objectName)
    {
        GameObject tilemapObject = GameObject.Find(objectName);
        return tilemapObject == null ? null : tilemapObject.GetComponent<Tilemap>();
    }

    private static Tile GetOrCreatePathTile(Tile groundTile)
    {
        Tile pathTile = AssetDatabase.LoadAssetAtPath<Tile>(PathTilePath);
        if (pathTile != null)
        {
            return pathTile;
        }

        pathTile = ScriptableObject.CreateInstance<Tile>();
        pathTile.name = "PathTile";
        pathTile.sprite = groundTile.sprite;
        pathTile.color = new Color(0.72f, 0.55f, 0.28f, 1f);
        pathTile.colliderType = Tile.ColliderType.None;
        AssetDatabase.CreateAsset(pathTile, PathTilePath);
        AssetDatabase.SaveAssets();
        return pathTile;
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

    private static void PaintBuilding(
        Tilemap tilemap,
        TileBase tile,
        int minimumX,
        int minimumY,
        int maximumX,
        int maximumY,
        int doorX)
    {
        for (int x = minimumX; x <= maximumX; x++)
        {
            tilemap.SetTile(new Vector3Int(x, maximumY, 0), tile);

            if (x != doorX)
            {
                tilemap.SetTile(new Vector3Int(x, minimumY, 0), tile);
            }
        }

        for (int y = minimumY; y <= maximumY; y++)
        {
            tilemap.SetTile(new Vector3Int(minimumX, y, 0), tile);
            tilemap.SetTile(new Vector3Int(maximumX, y, 0), tile);
        }
    }
}
