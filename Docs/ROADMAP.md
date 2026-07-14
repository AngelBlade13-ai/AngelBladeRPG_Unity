# AngelBlade RPG Unity Roadmap

This roadmap tracks the work of converting the original command-line RPG into a beginner-friendly 2D Unity RPG prototype. Keep each milestone small enough to understand, test in the Unity Editor, and commit as its own branch or pull request.

For deeper handoff notes, original-project migration ideas, and detailed Unity Editor setup steps, see `PROJECT_HANDOFF.md`.

## Current Progress

- [x] Initial Unity project setup.
- [x] Basic UI navigation between title, town, and battle panels.
- [x] Simple battle prototype with player and monster attacks.
- [x] Battle rewards with gold and XP.
- [x] Town status UI showing hero stats.
- [x] Beginner-friendly XP leveling system.
- [x] Shared game state through `GameSession`.
- [x] Character creation with player name entry.

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

## Latest Completed Milestone

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

## Next Milestone

### 7. Core Gameplay Tests

Planned branch: `test/core-gameplay-foundation`

- [ ] Add Edit Mode tests for player leveling and multiple level-ups.
- [ ] Add tests for player damage, monster damage, minimum damage, and HP clamping.
- [ ] Add tests for battle state and rewards being granted only once.
- [ ] Document how to run the core gameplay tests in Unity 6.5.

## Planned Architecture Milestone

### 8. Jobs, Characters, And Party Data

Complete this after the core gameplay tests and before inventory, saving, or substantial data-driven content. Detailed design constraints are recorded in `JOB_CLASS_SYSTEM.md`.

- [ ] Define data-driven job definitions for 12 focused roles with explicit mechanical trade-offs.
- [ ] Support assigning and respecing any playable character into any job.
- [ ] Add per-character job affinities that influence growth without restricting access.
- [ ] Model a roster of roughly 6-7 characters with 4 active battle slots.
- [ ] Keep character records independent from active party slots and equipment ownership.
- [ ] Design speed-based turn order with random tie-breaking and no visible turn queue.
- [ ] Leave extension points for bonds, roster rotation, and benched-character bonuses.
- [ ] Ensure a permanently removed character's equipped gear is also permanently removed rather than returned to shared inventory.

## Story And Lore Timing

- Full story and lore are intentionally deferred while the core tests and gameplay data models are being established.
- When the job-affinity work begins, only a compact playable-roster list and personality traits will be needed.
- Full character histories, world lore, locations, factions, dialogue, and plot structure should be added when the project reaches quests and world-content planning.
- Codex should explicitly ask for the relevant story material when each of those milestones is ready.

## Near-Term Backlog

Keep these as separate milestones unless a later design decision combines them.

- [ ] Improve battle UI with clearer action buttons and return-to-town behavior.
- [ ] Add simple 2D player and enemy sprites.
- [ ] Add a small town map screen with movement or clickable locations.
- [ ] Add multiple enemy types after the basic flow is stable.
- [ ] Add the job/class and party-data foundation.
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
