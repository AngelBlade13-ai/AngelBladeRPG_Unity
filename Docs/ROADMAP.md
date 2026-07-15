# AngelBlade RPG Unity Roadmap

This roadmap tracks the work of converting the original command-line RPG into a beginner-friendly 2D Unity RPG prototype. Keep each milestone small enough to understand, test in the Unity Editor, and commit as its own branch or pull request.

For deeper handoff notes and detailed Unity Editor setup steps, see `PROJECT_HANDOFF.md`. The target player experience and scene architecture are defined in `GAMEPLAY_DIRECTION.md`.

## Target Experience

- A 2D pixel-fantasy RPG with top-down exploration and movement inspired by `Stardew Valley`.
- Towns and world areas are physical Tilemap-based spaces, not menus represented by full-screen panels.
- Combat is turn-based and presented in a separate battle scene, inspired by older `Final Fantasy` games.
- Entering combat transitions from exploration into battle; victory or escape returns the party to the appropriate world location.
- Title and character creation remain in the setup scene. The former `TownPanel` and `BattlePanel` prototypes have been replaced by dedicated exploration and battle scenes.

## Current Progress

- [x] Initial Unity project setup.
- [x] Basic UI navigation between title, town, and battle panels.
- [x] Simple battle prototype with player and monster attacks.
- [x] Battle rewards with gold and XP.
- [x] Town status UI showing hero stats.
- [x] Beginner-friendly XP leveling system.
- [x] Shared game state through `GameSession`.
- [x] Character creation with player name entry.
- [x] Core Edit Mode gameplay test foundation.
- [x] Walkable town, world interactions, and scene doors.
- [x] Separate turn-based battle scene with victory, defeat, and escape.

## Completed Milestones

### 1. Unity Foundation

Branch: `main`

- Created the Unity project.
- Added the main game scene.
- Added starter folders for scripts, scenes, prefabs, sprites, scriptable objects, UI, saving, items, combat, core, and characters.

### 2. UI Navigation And Battle Prototype

Branch: `feature/ui-navigation-battle-prototype`

- Added title, town, and battle panels.
- Added `GameManager` as the main UI flow controller.
- Added `PlayerData`, `MonsterData`, and `SimpleBattleSystem`.
- Added a simple Goblin fight.

### 3. Battle Rewards And Town Status

Branch: `feature/battle-rewards-town-status`

- Added gold and XP rewards to `MonsterData`.
- Granted rewards after victory.
- Updated the town status text to show hero status.

### 4. XP Leveling

Commit: `32b7b76 Add XP-based player leveling`

- Added `XPToNextLevel` to `PlayerData`.
- Added `GainXP(int amount)` to `PlayerData`.
- Increased level, max HP, attack, defense, and next XP requirement on level up.
- Restored HP to full on level up.
- Added battle log messaging when the player levels up.
- Updated town status to show XP progress, such as `XP: 15/50`.

### 5. Shared Game State

Commit: `08e8240 Add shared RPG game state`

- Added `GameSession`.
- Moved player, monster, battle state, victory rewards, and defeat state into one shared game-state class.
- Kept `GameManager` focused mostly on UI flow and UI text updates.

## Completed Milestone 6

### 6. Character Creation

Branch: `feature/character-creation`

Code status:

- `GameManager` now supports a character creation panel.
- `GameManager.StartNewGame()` opens character creation instead of immediately creating `Hero`.
- `GameManager.ConfirmCharacterCreation()` validates the entered name.
- `GameSession.TryStartNewGame(string playerName)` trims names and rejects blank names.
- `GameManager.ReturnToTitle()` supports backing out of character creation.

Unity Editor checklist:

- [x] Add a `CharacterCreationPanel` to the main scene.
- [x] Add a TMP input field for the player name.
- [x] Add an error text object for blank-name validation.
- [x] Add a confirm button wired to `GameManager.ConfirmCharacterCreation()`.
- [x] Add a back button wired to `GameManager.ReturnToTitle()`.
- [x] Assign `characterCreationPanel`, `playerNameInput`, and `characterCreationErrorText` on the `GameManager` component.
- [x] Confirm the panel starts hidden and title, town, and battle panel flow still works.
- [x] Test title, character creation, town, and battle flow in Play Mode.

## Completed Milestone 7

### 7. Core Gameplay Tests

Branch: `test/core-gameplay-foundation`

- [x] Add Edit Mode tests for player leveling and multiple level-ups.
- [x] Add tests for player damage, monster damage, minimum damage, and HP clamping.
- [x] Add tests for battle state and rewards being granted only once.
- [x] Document how to run the core gameplay tests in Unity 6.5.
- [x] Run all tests in Unity `6000.5.3f1`: 18 passed, 0 failed.

## Completed Milestone 8

### 8. Pixel World Foundation

Branch: `feature/pixel-world-foundation`

- [x] Establish a `320 x 180` pixel-art reference resolution and `16` pixels-per-unit baseline.
- [x] Add top-down player movement using Unity's Input System.
- [x] Add Rigidbody2D movement without diagonal speed gain.
- [x] Expose directional idle and walk parameters for a future Animator Controller.
- [x] Create and validate a small Tilemap test area with separate ground and collision layers.
- [x] Add a camera-follow component and document Pixel Perfect Camera settings.
- [x] Keep movement logic independent from town, story, and combat content.
- [x] Test keyboard movement, diagonal movement, Rigidbody2D collision, wall sliding, and camera follow in Play Mode.
- [x] Add and validate a temporary four-direction movement indicator.
- [x] Test pixel-camera framing at `320 x 180`, `640 x 360`, and `1280 x 720`.
- [x] Run all Edit Mode tests in Unity `6000.5.3f1`: 28 passed, 0 failed.

Unity Editor setup and Play Mode checks: `PIXEL_WORLD_SETUP.md`.

## Completed Milestone 9

### 9. Walkable Town Prototype

Branch: `feature/walkable-town`

- [x] Preserve the active `GameSession` across scene loads.
- [x] Route successful character creation to a dedicated town scene.
- [x] Add reusable default-spawn positioning for the Player.
- [x] Add a compact exploration status HUD formatter.
- [x] Add Edit Mode coverage for session replacement, HUD text, and spawn positioning.
- [x] Run all Edit Mode tests in Unity `6000.5.3f1`: 32 passed, 0 failed.
- [x] Replace `TownPanel` as the primary town experience with a physical map.
- [x] Spawn the player at a defined town entrance or return point.
- [x] Add placeholder walls, paths, and building footprints with door gaps using Tilemaps and colliders.
- [x] Preserve the current town status information as an unobtrusive HUD or menu.
- [x] Complete the full title, character creation, town loading, movement, camera, HUD, and collision Play Mode check.

Unity Editor setup and Play Mode checks: `WALKABLE_TOWN_SETUP.md`.

## Completed Milestone 10

### 10. World Interaction System

Branch: `feature/world-interactions`

- [x] Add a reusable interaction contract and facing-based detector in code.
- [x] Add player interaction input using the existing `Player/Interact` action.
- [x] Add a simple dialogue presenter and sign implementation in code.
- [x] Run all Edit Mode tests in Unity `6000.5.3f1`: 44 passed, 0 failed.
- [x] Wire and verify the first sign manually in `TownScene`.
- [x] Add reusable scene doors and one-time named destination spawns in code.
- [x] Generate and verify a paired town/interior door transition.
- [x] Support doors, signs, future NPC dialogue, and location transitions through a shared interface.
- [x] Add a simple dialogue box without introducing full story content yet.
- [x] Preserve player data and explicit return-spawn state across scene transitions.

Unity Editor setup and Play Mode checks: `WORLD_INTERACTION_SETUP.md` and `WORLD_TRANSITION_SETUP.md`.

## Completed Milestone 11

### 11. Separate Turn-Based Battle Scene

Branch: `feature/separate-battle-scene`

- [x] Add explicit in-progress, victory, defeat, and escaped outcomes.
- [x] Add escape handling without granting rewards.
- [x] Add one-time battle return scene and spawn storage.
- [x] Add a reusable exploration encounter interactable in code.
- [x] Add a dedicated battle-scene controller in code.
- [x] Create a dedicated battle scene rather than a battle panel inside the town scene.
- [x] Transfer the active player and encounter data into the battle scene.
- [x] Present classic turn-based commands, combatants, battle text, and rewards.
- [x] Return to the correct exploration scene and position after victory or escape.
- [x] Keep the existing tested combat math and reward rules as the initial battle foundation.
- [x] Use visible interactable encounters for the deterministic prototype while leaving the final encounter policy open.
- [x] Remove the obsolete `TownPanel` and `BattlePanel` scene flow.
- [x] Require interactable targets to be in the player's facing direction.
- [x] Run all Edit Mode tests in Unity `6000.5.3f1`: 60 passed, 0 failed.
- [x] Complete the full title, town, interaction, victory, escape, defeat, and return Play Mode check.

Unity Editor setup and Play Mode checks: `BATTLE_SCENE_SETUP.md`.

## Latest Completed Milestone

### 12. Jobs, Characters, And Party Data

Complete this after the exploration and battle-scene loop is stable, and before inventory, saving, or substantial character content. Detailed design constraints are recorded in `JOB_CLASS_SYSTEM.md`.

Branch: `feature/job-party-data`

- [x] Define job definitions for 12 focused roles with explicit mechanical trade-offs.
- [x] Support assigning and respecing any available playable character into any job.
- [x] Add per-character job affinities with tested growth multipliers that never restrict access.
- [x] Model a roster of roughly 6-7 characters with 4 active battle slots.
- [x] Keep character records independent from active party slots and future equipment ownership.
- [x] Record Reaver, Blood Mage, White Mage, and Paladin as tentative character affinities rather than locked classes.
- [x] Add ID-keyed authored profiles for Iona, Damari, Enora, and Lysander without embedding full story content.
- [x] Add permanent character-availability state and remove unavailable characters from the active party.
- [x] Run all Edit Mode tests in Unity `6000.5.3f1`: 95 passed, 0 failed.
- [x] Design internal speed-based turn order with random tie-breaking and no visible turn queue.
- [x] Track per-character battle usage, consecutive bench time, and symmetric bond points for future roster systems.
- [x] Preserve permanent-removal state and carry mandatory equipped-item destruction forward as an inventory/save invariant.

## Next Milestone

### 13. Core Combat Expansion

Branch: `feature/core-combat-expansion`

- [x] Define reusable combatant stats needed by jobs and multi-character battles, including speed and magic resources.
- [x] Preserve existing leveling, physical damage, rewards, and battle-scene behavior through compatibility properties.
- [x] Run all Edit Mode tests in Unity `6000.5.3f1`: 102 passed, 0 failed.
- [x] Complete a Play Mode victory and reward regression using the shared stat model.
- [x] Add ID-keyed Goblin, Ogre, Slime, and Wisp definitions that create independent runtime monsters.
- [x] Run all Edit Mode tests after the monster roster slice: 112 passed, 0 failed.
- [x] Migrate the town Goblin and Ogre encounters to catalog IDs and complete a Play Mode regression.
- [x] Integrate speed-based ordering into battle sequencing without showing a turn queue.
- [x] Run all Edit Mode tests after battle-round sequencing: 117 passed, 0 failed.
- [x] Verify player-first and monster-first rounds in Play Mode using the Goblin, Ogre, and Wisp encounters.
- [ ] Add accuracy, misses, critical hits, blocking, and improved escape rules in focused slices.
- [ ] Establish combat-action contracts for physical attacks, magic, healing, and future abilities.
- [ ] Keep each new rule independently testable before expanding the battle UI.

## Story And Lore Timing

- Full story and lore are intentionally deferred while the core tests and gameplay data models are being established.
- When Milestone 12 job-affinity work begins, only a compact playable-roster list and personality traits will be needed.
- Full character histories, world lore, locations, factions, dialogue, and plot structure should be added when the project reaches quests and world-content planning.
- Codex should explicitly ask for the relevant story material when each of those milestones is ready.

## Near-Term Backlog

Keep these as separate milestones unless a later design decision combines them.

- [x] Build the pixel-world movement and camera foundation.
- [ ] Add final 2D player movement sprites and directional animations.
- [ ] Replace the temporary direction indicator with directional pixel sprites and an Animator Controller when suitable character art is available.
- [x] Replace the town panel with a walkable Tilemap town.
- [x] Replace the battle panel with a separate turn-based battle scene.
- [ ] Add multiple enemy types after the basic flow is stable.
- [x] Add the job/class and party-data foundation.
- [ ] Add healing/resting in town.
- [ ] Add inventory, items, and shops.
- [ ] Add saving and loading.
- [ ] Add dialogue or story events from the original command-line RPG.

## Branching And Commit Notes

- Use one feature branch per small milestone.
- Keep commits beginner-readable and focused.
- After code changes, test in the Unity Editor before committing scene or prefab changes.
- Commit scene wiring separately when possible, because Unity scene changes are easier to review when they are not mixed with unrelated code edits.
- Avoid adding inventory, shops, saving, random monsters, or new UI panels until the current milestone is complete.
