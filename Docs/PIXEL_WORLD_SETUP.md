# Pixel World Foundation Setup

This guide wires the first walkable exploration test in Unity 6.5. It does not replace `MainGameScene` or remove the working menu prototype.

## Foundation Choices

- Reference resolution: `320 x 180` (16:9)
- Initial sprite pixels per unit: `16`
- Player movement: eight-direction input with normalized diagonal speed
- Initial movement speed: `4` world units per second
- Physics: `Rigidbody2D` using a small feet-level collider
- Input: existing `Player/Move` action in `Assets/Settings/InputSystem_Actions.inputactions`

The movement data supports either four-direction or eight-direction animation. We can make that visual choice after testing a real character sprite.

## 1. Let Unity Compile

1. Open the project in Unity `6000.5.3f1`.
2. Wait for the asset import and script compilation to finish.
3. Open **Window > General > Console** and confirm there are no red compile errors.

Unity should generate `.meta` files for the new `Assets/Scripts/World` folder and scripts.

## 2. Create The Exploration Scene

1. Choose **File > New Scene**.
2. Select the **Lit 2D (URP)** template.
3. Save it as `Assets/Scenes/ExplorationTestScene.unity`.
4. Open **File > Build Profiles > Scene List** and add the open scene.

Keep `MainGameScene` unchanged for now. The new scene is a movement sandbox until it can replace the town prototype's basic responsibilities.

## 3. Configure The Camera

1. Select `Main Camera`.
2. Confirm **Projection** is `Orthographic`.
3. Set its Transform position to `X 0`, `Y 0`, `Z -10`.
4. Add the **Pixel Perfect Camera** component.
5. Set **Assets Pixels Per Unit** to `16`.
6. Set **Reference Resolution** to `320 x 180`.
7. Set **Grid Snapping** to `Upscale Render Texture`.
8. Enable **Crop Frame X** and **Crop Frame Y** if those options are shown separately.
9. Add the `CameraFollow2D` component.

Leave the camera target empty until the Player object exists.

## 4. Create A Temporary Player

1. In the Hierarchy, choose **GameObject > 2D Object > Sprites > Square**.
2. Rename it `Player`.
3. Set its Transform position to `X 0`, `Y 0`, `Z 0`.
4. Set its scale to about `X 0.75`, `Y 1`, `Z 1` so it is easy to recognize as a character placeholder.
5. Pick a visible temporary Sprite Renderer color.
6. Set **Sprite Sort Point** to `Pivot`.
7. Add a `Rigidbody2D` component.
8. Set **Body Type** to `Dynamic`.
9. Set **Gravity Scale** to `0`.
10. Set **Collision Detection** to `Continuous`.
11. Freeze **Rotation Z** under Constraints.
12. Add a `CapsuleCollider2D` or `BoxCollider2D` and resize it around the lower half of the sprite.
13. Add the `PlayerMovement2D` component.

On `PlayerMovement2D`:

1. Set **Move Speed** to `4`.
2. Assign `Player/Move` from `Assets/Settings/InputSystem_Actions.inputactions` to **Move Action**.
3. Leave **Animator** empty while using the placeholder sprite.

On the Main Camera's `CameraFollow2D` component:

1. Drag `Player` into **Target**.
2. Leave **Follow Speed** at `12`.

## 5. Create A Collision Test Area

The full Tilemap comes next, but a few colliders are enough to validate the controller now.

1. Create several square sprites and rename their parent `CollisionTest`.
2. Scale and position them as walls around part of the Player.
3. Add `BoxCollider2D` to each wall.
4. Keep the wall Rigidbody absent so Unity treats the colliders as static.
5. Give the ground and walls visibly different temporary colors.

## 6. Play Mode Checklist

- [ ] `WASD` moves the Player.
- [ ] Arrow keys move the Player.
- [ ] A gamepad left stick moves the Player, if available.
- [ ] Diagonal movement is not faster than horizontal or vertical movement.
- [ ] The Player cannot pass through the collision-test walls.
- [ ] The Player does not rotate or fall.
- [ ] The camera follows without changing its Z position.
- [ ] Pixel rendering remains stable at several Game view sizes.
- [ ] Stopping movement leaves the scene and camera stable.

Save the scene outside Play Mode after the checks pass.

## 7. Create The First Tilemap

### Create The Layers

1. Exit Play Mode.
2. In the Hierarchy, choose **GameObject > 2D Object > Tilemap > Rectangular**.
3. Rename the created `Tilemap` child to `GroundTilemap`.
4. Select its **Tilemap Renderer** and set **Order in Layer** to `-10`.
5. Duplicate `GroundTilemap` and rename the copy `CollisionTilemap`.
6. Set `CollisionTilemap`'s **Order in Layer** to `0`.
7. Add a **Tilemap Collider 2D** component to `CollisionTilemap` only.

The resulting hierarchy should contain one `Grid` parent with `GroundTilemap` and `CollisionTilemap` children. Ground tiles provide visuals; collision tiles define walls and blocked terrain.

### Create The Palette And Placeholder Tiles

1. Create `Assets/Tilemaps`, `Assets/Tilemaps/Palettes`, and `Assets/Tilemaps/Tiles` folders in the Project window.
2. Open **Window > 2D > Tile Palette**.
3. Create a rectangular palette named `ExplorationTestPalette` and save it in `Assets/Tilemaps/Palettes`.
4. In `Assets/Tilemaps/Tiles`, choose **Create > 2D > Sprites > Square**.
5. Name the new sprite `PlaceholderTileSprite`.
6. Drag `PlaceholderTileSprite` into the empty `ExplorationTestPalette` area.
7. When Unity asks where to create the Tile asset, choose `Assets/Tilemaps/Tiles`.
8. Rename the generated Tile asset `GroundTile`, choose a temporary ground color, and set **Collider Type** to `None`.
9. Duplicate `GroundTile` as `CollisionTile`, choose a contrasting wall color, and set **Collider Type** to `Grid`.
10. Drag `CollisionTile` into an empty cell in `ExplorationTestPalette`.

### Paint And Test

1. Set the Tile Palette's active Tilemap to `GroundTilemap` and paint a small rectangular floor.
2. Set the active Tilemap to `CollisionTilemap` and paint a few walls around the floor, leaving room to move.
3. Disable the temporary `WallRight` and `WallTop` objects.
4. Enter Play Mode and confirm the Player can cross the ground but cannot pass through collision tiles.
5. Confirm diagonal movement slides along Tilemap walls and the camera continues following.
6. Exit Play Mode and save the scene.

## Future Animation Parameters

`PlayerMovement2D` updates these optional Animator parameters:

- Float: `MoveX`
- Float: `MoveY`
- Float: `LastMoveX`
- Float: `LastMoveY`
- Bool: `IsMoving`

`LastMoveX` and `LastMoveY` preserve the direction the player was facing when movement stops, allowing directional idle animations later.

## Temporary Direction Indicator

Until suitable directional character sprites are available, use a triangle child to verify four-direction facing without changing the Player's physics rotation.

1. Create `Assets/Sprites/Characters/Placeholders` folders as needed.
2. In the final folder, choose **Create > 2D > Sprites > Triangle** and name it `PlayerDirectionPlaceholder`.
3. Create an empty child under `Player` named `PlayerVisual`.
4. Set the root Player scale to `1, 1, 1` so child visuals do not inherit non-uniform stretching.
5. Set the root **Capsule Collider 2D** size to approximately `X 0.75`, `Y 0.75`, preserving the smaller collision footprint that the old root scale provided.
6. Reset `PlayerVisual`'s local Transform, then add a **Sprite Renderer** using `PlayerDirectionPlaceholder`.
7. Use a uniform `PlayerVisual` scale, such as `0.75, 0.75, 1`, and choose a color that is easy to read over the test ground.
8. Disable the root Player's square **Sprite Renderer**, but keep its Rigidbody2D, collider, and movement component enabled.
9. Add `PlaceholderDirectionVisual2D` to the Player root.
10. Assign the Player's `PlayerMovement2D` component to **Movement**.
11. Assign the `PlayerVisual` child to **Visual**.
12. Leave **Bob Height** at `0.05` and **Bob Speed** at `8`.

In Play Mode, the triangle should point in the last cardinal movement direction and bob only while moving. Diagonal movement uses its dominant axis for the temporary four-direction display.
