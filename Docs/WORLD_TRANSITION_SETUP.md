# World Transition Prototype Setup

This guide verifies reusable doors and explicit return positions in Unity `6000.5.3f1`. The scene duplication and repeated wiring are automated after the first interaction was configured manually.

## How Door Transitions Work

1. `SceneDoorInteractable2D` records a destination spawn ID.
2. Unity loads the destination scene by name.
3. `WorldSceneSpawnController2D` consumes that spawn ID once.
4. The Player is placed at the matching `PlayerSpawnPoint2D`.
5. If no matching request exists, the scene uses its default spawn.

`GameSessionStore` remains in memory during the scene load, so the active player data survives the trip.

## Build The Test Interior

1. Open `TownScene` outside Play Mode.
2. Save any current changes.
3. Choose **Tools > AngelBlade RPG > Build Interaction Test Interior**.
4. Wait for Unity to create and reopen `TownScene`.

The command creates:

- `InteriorTestDoor` in `TownScene`, targeting `InteractionTestInterior`.
- `TownFromInteriorSpawn` in `TownScene`, used when returning.
- `InteractionTestInterior` as a compact generated exploration scene.
- `InteriorEntranceSpawn` as the interior's default and requested entrance.
- `ReturnToTownDoor` in the interior, targeting `TownScene`.
- A Scene List entry for `InteractionTestInterior`.

The generated interior deliberately reuses the current Player, camera, HUD, movement, interaction, and Tilemap setup. It is a technical transition test, not final environment content.

## Full Flow Check

- [x] Start from `MainGameScene` and create a character.
- [x] Enter `TownScene` with the chosen name still shown in the HUD.
- [x] Use `E` on `InteriorTestDoor` to enter `InteractionTestInterior`.
- [x] Confirm the Player appears at `InteriorEntranceSpawn`.
- [x] Confirm movement, collision, camera follow, and HUD still work.
- [x] Use `E` on `ReturnToTownDoor` to return to `TownScene`.
- [x] Confirm the Player appears at `TownFromInteriorSpawn` rather than the original town entrance.
- [x] Confirm the player name and stats survive both scene transitions.
- [x] Confirm the sign still works after returning to town.
- [x] Confirm the Console remains free of errors.

Save scene changes outside Play Mode.
