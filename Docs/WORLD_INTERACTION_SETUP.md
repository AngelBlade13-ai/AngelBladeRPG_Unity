# World Interaction Setup

This guide adds the first manually configured world interaction in Unity `6000.5.3f1`. The first sign is intentionally wired by hand so the reusable interaction pieces are clear before repeated setup is automated.

## How The Pieces Fit Together

- `PlayerInteraction2D` checks a small circle in front of the direction the player last faced.
- `IWorldInteractable` is the shared contract used by signs and future doors and NPCs.
- `DialogueInteractable2D` supplies reusable text for NPCs and world objects.
- `SignInteractable2D` gives signs a clear Inspector name while reusing that dialogue behavior.
- `SimpleDialoguePanel2D` presents and hides one short message.
- The existing **Player/Interact** Input Action uses `E` on keyboard and the north face button on gamepad.

## 1. Verify The Scripts

1. Let Unity finish compiling.
2. Confirm the Console has no red errors.
3. Open **Window > General > Test Runner**.
4. Run all Edit Mode tests.

The interaction selection and dialogue-distance tests should bring the suite to 36 tests.

## 2. Add Player Interaction

1. Open `TownScene` and stay outside Play Mode.
2. Select the root `Player` object.
3. Add the `PlayerInteraction2D` component.
4. Assign **Player/Interact** from `InputSystem_Actions` to **Interact Action**.
5. Leave **Interaction Origin** empty so the Player transform is used.
6. Keep **Interaction Distance** at `0.65` and **Interaction Radius** at `0.35`.
7. Leave **Interaction Layers** at **Everything** for this first test.

The yellow wire circle visible while the Player is selected shows where interaction detection occurs.

## 3. Add The Dialogue Panel

1. Select `ExplorationHUD`.
2. Add `SimpleDialoguePanel2D` to it.
3. Under `ExplorationHUD`, create a UI Panel named `DialoguePanel`.
4. Anchor it across the bottom of the screen with a small margin.
5. Give it a compact height of about `44` at the `320 x 180` reference resolution.
6. Add a TextMesh Pro UI child named `DialogueText`.
7. Stretch the text inside the panel with a small inset and use a readable size around `9` or `10`.
8. Disable **Raycast Target** on `DialogueText`.
9. On `SimpleDialoguePanel2D`, assign `DialoguePanel` and `DialogueText`.

The script hides the panel when the scene starts. Pressing interact on the same sign again closes it, and walking more than `1.5` world units away closes it automatically.

## 4. Add The First Sign

1. Create a simple visible square Sprite named `TestSign` beside the main path.
2. Place it one tile to the side of a walkable space so the Player can face it.
3. Add a `BoxCollider2D`. Leave **Is Trigger** disabled so the sign also blocks movement.
4. Add `SignInteractable2D`.
5. Enter a short placeholder message such as `Town Square - Buildings ahead.`
6. Assign `ExplorationHUD`'s `SimpleDialoguePanel2D` component to **Dialogue**.
7. Save the scene.

## 5. Play Mode Check

- [x] Starting from `MainGameScene` still reaches `TownScene`.
- [x] Movement and collision still work.
- [x] Facing the sign and pressing `E` opens its message.
- [x] Pressing `E` again while facing the sign closes the message.
- [x] Walking away from the sign closes the message automatically.
- [x] Pressing `E` away from the sign does nothing.
- [x] A wall or other collider without `IWorldInteractable` is ignored.
- [x] The Console remains free of errors.

Save all scene changes outside Play Mode.
