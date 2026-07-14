# Walkable Town Prototype Setup

This guide converts the pixel-world sandbox into the first physical town scene. Complete it in Unity `6000.5.3f1` after the scripts compile without errors.

## 1. Rename The Scene

1. In the Project window, rename `Assets/Scenes/ExplorationTestScene.unity` to `TownScene.unity`.
2. Open **File > Build Profiles > Scene List**.
3. Confirm `MainGameScene` remains first and `TownScene` remains second.
4. Select the `GameManager` in `MainGameScene` and confirm **Town Scene Name** is `TownScene`.

Rename the scene inside Unity so its `.meta` GUID and Build Profile reference remain intact.

## 2. Add A Defined Town Entrance

1. In `TownScene`, create an empty object named `TownSetup` at world position `0, 0, 0`.
2. Add `WorldSceneSpawnController2D` to `TownSetup`.
3. Create another empty object named `TownEntranceSpawn` at the intended entrance position.
4. Add `PlayerSpawnPoint2D` to it and leave **Spawn Id** as `TownEntrance`.
5. On `TownSetup`, assign the Player root to **Player**.
6. Assign `TownEntranceSpawn` to **Default Spawn Point**.

Entering Play Mode should move the Player root to the entrance while preserving its Z position.

## 3. Add The Exploration HUD

1. Create a Canvas named `ExplorationHUD` using **Screen Space - Overlay**.
2. On **Canvas Scaler**, choose **Scale With Screen Size**.
3. Set **Reference Resolution** to `320 x 180` and **Match** to `0.5`.
4. Add a TextMesh Pro UI object named `StatusText`.
5. Anchor it to the upper-left with a small margin.
6. Use left alignment and a compact readable size; keep **Raycast Target** disabled.
7. Add `ExplorationStatusHUD` to the Canvas.
8. Assign `StatusText` to **Status Text**.

When `TownScene` is opened directly, the HUD can be blank because no character session exists. Test the populated HUD by starting from `MainGameScene` and creating a character.

## 4. Shape The Placeholder Town

Continue using placeholder colors; final environment art is not part of this milestone.

### Automated Setup

1. Open `TownScene` and stay outside Play Mode.
2. Choose **Tools > AngelBlade RPG > Build Placeholder Town**.
3. Save the scene when the command finishes.

The command creates `PathTile`, paints the ground and central path, adds an outer collision boundary, adds two building footprints with door gaps, and moves `TownEntranceSpawn` onto the path. It can be run again to reset the placeholder layout.

### Manual Alternative

1. Expand `GroundTilemap` to make a comfortably walkable area.
2. Duplicate `GroundTile` as `PathTile`, keep **Collider Type** at `None`, and choose a contrasting path color.
3. Add `PathTile` to `ExplorationTestPalette` and paint a path from `TownEntranceSpawn` toward a central square.
4. Use `CollisionTilemap` to create a clear outer boundary.
5. Paint two or three simple building footprints or blocked areas.
6. Leave one-tile gaps where future doors or building entrances will go.
7. Keep the Player and spawn point on walkable ground.

## 5. Full Flow Checklist

- [x] Starting `MainGameScene` still opens the title screen.
- [x] New Game still opens character creation.
- [x] Blank and whitespace-only names are still rejected.
- [x] A valid name loads `TownScene`.
- [x] The Player appears at `TownEntranceSpawn`.
- [x] The HUD shows the chosen name, level, HP, XP, and gold.
- [x] `WASD`, arrow, and diagonal movement still work.
- [x] Tilemap collision and wall sliding still work.
- [x] The camera still follows the Player.
- [x] The temporary direction marker still faces and bobs correctly.
- [x] The Console remains free of errors.

Save all scene changes outside Play Mode.
