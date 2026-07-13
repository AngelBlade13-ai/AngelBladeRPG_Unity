# AngelBlade RPG Unity — Project Handoff

Last updated: July 13, 2026

This document summarizes the planning, implementation, Git history, Unity Editor work, and next steps discussed while converting the original command-line RPG into a Unity 2D project.

## Repositories

- Unity project: https://github.com/AngelBlade13-ai/AngelBladeRPG_Unity
- Original command-line RPG: https://github.com/AngelBlade13-ai/C-Object-Oriented-Programming-Final

The goal is to reuse the original game's design and gameplay rules while rebuilding its presentation and architecture for Unity. The original project contains character progression, combat actions, status effects, randomized monsters, loot, inventory, equipment, shops, quests, saving/loading, locations, travel events, bosses, and death recovery.

The original `Program.cs` is a large console-oriented implementation. Its concepts should be migrated, but its monolithic structure should not be copied directly into Unity.

## Current Git state

At the time this document was generated:

- Active branch: `feature/character-creation`
- Tracking: `origin/feature/character-creation`
- Working tree: clean
- Latest commit: `d35d4c1 commit`
- `main` latest commit: `08e8240 Add shared RPG game state`

Important commits:

| Commit | Description |
| --- | --- |
| `e4963a7` | Initial check-in |
| `325901b` | Initial Unity project setup |
| `5850a20` | Add basic UI navigation and battle prototype |
| `9ee6f72` | Add battle rewards and town status UI |
| `32b7b76` | Add XP-based player leveling |
| `08e8240` | Add shared RPG game state |
| `d35d4c1` | Character-creation code; commit message was simply `commit` |

Historical descriptive branches:

- `feature/ui-navigation-battle-prototype`
- `feature/battle-rewards-town-status`

Completed feature branches were merged into `main` and removed when appropriate. The current character-creation branch has not yet been merged into `main`.

## Unity project baseline

Unity version:

- Unity 6.5 / `6000.5.3f1`
- Universal Render Pipeline
- TextMesh Pro
- Unity Test Framework package is installed

Primary scene:

- `Assets/Scenes/MainGameScene.unity`

Existing panels:

- `TitlePanel`
- `TownPanel`
- `BattlePanel`
- Character creation is being added as `CharacterCreationPanel`

Core scripts:

- `Assets/Scripts/Core/GameManager.cs`
- `Assets/Scripts/Core/GameSession.cs`
- `Assets/Scripts/Characters/PlayerData.cs`
- `Assets/Scripts/Characters/MonsterData.cs`
- `Assets/Scripts/Combat/SimpleBattleSystem.cs`

## Completed gameplay

- Title, town, and battle navigation
- Basic player and monster data
- Fixed Goblin encounter
- Player and monster attacks
- Defense and minimum-damage calculation
- Player HP and monster HP display
- Gold and XP rewards after victory
- Town status display
- XP-based leveling
- Level-up feedback in the battle log
- Shared game-session state
- Duplicate victory reward prevention
- Character-name validation code

## Player leveling behavior

Implemented on `feature/player-leveling`, tested in Play Mode, committed as `32b7b76`, and merged into `main`.

Rules:

- Initial XP requirement: 50
- XP requirement increases by 25 per level
- XP overflow is preserved
- Multiple levels from one XP award are supported
- Max HP increases by 20
- Current HP is restored to Max HP
- Attack increases by 3
- Defense increases by 1
- Town UI displays `XP: current/required`
- Battle log reports the player's new level

Automated checks covered:

- XP below the threshold
- Exact threshold
- Overflow XP
- Stat increases
- Full healing
- Multiple level-ups

Manual Play Mode testing confirmed that leveling, healing, Max HP growth, and the increased XP requirement worked.

## Game-state foundation

Implemented, tested, committed as `08e8240`, and merged into `main`.

`GameSession` now owns:

- Current player
- Current monster
- Whether a battle is over
- Starting a new game
- Starting a battle
- Completing victory rewards
- Completing defeat state

`BattleRewardResult` reports:

- Gold gained
- XP gained
- Whether the player leveled up

`GameManager` is now primarily responsible for:

- Panel navigation
- Button events
- Updating UI text
- Calling gameplay classes

The session prevents victory rewards from being granted more than once. Automated state checks and a complete Play Mode regression test passed.

## Character creation — current work

Branch:

`feature/character-creation`

The code is committed and pushed as `d35d4c1`, although the commit message is only `commit`.

Code behavior:

- New Game opens the character-creation panel instead of immediately creating `Hero`.
- Empty and whitespace-only names are rejected.
- Valid names are trimmed.
- A valid name creates the player and opens town.
- Back returns to the title panel.
- The input field is selected when the panel opens.

New `GameManager` references:

- `characterCreationPanel`
- `playerNameInput`
- `characterCreationErrorText`

New public methods:

- `ConfirmCharacterCreation()`
- `ReturnToTitle()`

`GameSession.StartNewGame(string)` was replaced by:

`TryStartNewGame(string playerName)`

Name-validation checks passed for empty, whitespace-only, valid, and padded names.

## Required Unity Editor setup for character creation

This feature requires scene and Inspector work. Do not test it until all three new `GameManager` references are assigned.

Under `Canvas`, create `CharacterCreationPanel` with:

- Title TMP text such as `Name Your Hero`
- TMP Input Field
- Empty error TMP text named `ErrorText`
- Confirm button
- Back button

On the `GameManager` object, assign:

- Character Creation Panel → `CharacterCreationPanel`
- Player Name Input → the TMP Input Field component
- Character Creation Error Text → `ErrorText`

Button callbacks:

- Existing New Game button → `GameManager.StartNewGame`
- Confirm button → `GameManager.ConfirmCharacterCreation`
- Back button → `GameManager.ReturnToTitle`

Recommended input settings:

- Content Type: Standard
- Line Type: Single Line
- Character Limit: 20
- Placeholder: Hero name

Set `CharacterCreationPanel` inactive in the Inspector and save `MainGameScene`.

### Recommended responsive layout

The Unity 6 creation menu does not necessarily show a special “UI Empty Object.” Right-click `CharacterCreationPanel` and use `Create Empty`.

Rename the empty object to `CharacterCreationContent`, then move these children under it in order:

1. Title text
2. Player name input
3. Error text
4. Confirm button
5. Back button

Set `CharacterCreationContent` Rect Transform:

- Anchor: middle-center
- Pivot: `0.5, 0.5`
- Pos X: 0
- Pos Y: 0
- Width: 500
- Height: 400
- Scale: `1, 1, 1`

Add `Vertical Layout Group`:

- Padding: 30 on every side
- Spacing: 15
- Child Alignment: Middle Center
- Control Child Size Width: enabled
- Control Child Size Height: enabled
- Child Force Expand Width: enabled
- Child Force Expand Height: disabled

Add `Content Size Fitter`:

- Horizontal Fit: Unconstrained
- Vertical Fit: Preferred Size

Add a `Layout Element` to each child with these preferred heights:

- Title: 60
- Input field: 55
- Error text: 35
- Confirm button: 55
- Back button: 55

Suggested TMP font sizes:

- Title: 36
- Buttons: 26
- Error: 22
- Input: 26

Disable TMP Auto Size temporarily if text becomes extremely large.

Keep `CharacterCreationPanel` stretched to the Canvas with Left, Right, Top, and Bottom all set to 0.

### Character-creation Play Mode test

- New Game opens character creation.
- Empty input shows `Please enter a hero name.`
- Whitespace-only input is rejected.
- Back returns to title.
- Valid input opens town.
- Leading/trailing spaces are removed.
- The chosen name appears in town.
- The chosen name appears in battle.
- Existing battle, rewards, leveling, and return-to-town behavior still work.

## Phase 1 branch plan

1. `feature/player-leveling` — completed and merged
2. `refactor/game-state-foundation` — completed and merged
3. `feature/character-creation` — current branch
4. `test/core-gameplay-foundation` — next planned branch

After character creation is visually arranged and tested:

1. Ensure `MainGameScene.unity` is saved and tracked if it changed.
2. Give the character-creation commit a descriptive follow-up commit for any scene work, such as `Add character creation UI`.
3. Merge `feature/character-creation` into `main`.
4. Create `test/core-gameplay-foundation`.

## Core-foundation test branch plan

Suggested commits:

1. `Add tests for player leveling`
2. `Add tests for combat damage`
3. `Add tests for game rewards`
4. `Clean up generated Unity project references`
5. `Document the core gameplay architecture`

Tests should cover:

- XP below, at, and above thresholds
- Stat growth and HP restoration
- Multiple level-ups
- Player damage
- Monster damage
- Minimum damage of one
- HP clamping at zero
- Gold and XP granted once
- Level-up reward feedback
- Starting and ending battle state

There was previously a stale generated `.csproj` reference to the missing file `Assets/Editor/HubForceResolve.cs`. Unity itself successfully compiled the gameplay scripts. Generated Unity project files should remain ignored and can be regenerated rather than manually maintained.

## Full migration roadmap

### Phase 1 — Foundation

- [x] Player leveling
- [x] Shared game session
- [x] Separate rewards/state from UI coordination
- [ ] Finish character creation UI and testing
- [ ] Add core gameplay tests
- [ ] Clean stale generated project references

### Phase 2 — Core combat

- [ ] Reusable monster definitions
- [ ] Small monster roster
- [ ] Critical hits
- [ ] Accuracy and misses
- [ ] Block
- [ ] Escape
- [ ] Potions in combat
- [ ] Combat abilities
- [ ] Poison and burn
- [ ] Improved battle-log sequencing
- [ ] Victory and defeat presentation

### Phase 3 — Data-driven content

- [ ] Monster ScriptableObjects
- [ ] Item ScriptableObjects
- [ ] Weapons, armor, accessories, and potions
- [ ] Rarity tiers
- [ ] Encounter selection
- [ ] Move balance values out of `GameManager`

### Phase 4 — Inventory and equipment

- [ ] Base item model
- [ ] Inventory
- [ ] Weapons
- [ ] Armor
- [ ] Accessory slot
- [ ] Base stats plus equipment bonuses
- [ ] Potions
- [ ] Inventory UI
- [ ] Equipment comparisons

### Phase 5 — Loot and encounters

- [ ] Random monsters
- [ ] Progression scaling
- [ ] Monster affixes
- [ ] Randomized rewards
- [ ] Item drops
- [ ] Rarity-based loot
- [ ] Encounter modifiers
- [ ] Win streaks
- [ ] Victory reward summary

### Phase 6 — Town and shops

- [ ] Expanded town menu
- [ ] Shop panel
- [ ] Buying
- [ ] Selling
- [ ] Camp/rest
- [ ] Gear in town HUD

### Phase 7 — Saving and loading

- [ ] Serializable save models
- [ ] Player and progression save data
- [ ] Inventory and equipment persistence
- [ ] Quest and record persistence
- [ ] Continue button
- [ ] Manual save
- [ ] Autosave
- [ ] Corrupt-save handling
- [ ] Save-format versioning

Use Unity's application data directory rather than writing `savegame.json` beside the executable.

### Phase 8 — Quests and progression

- [ ] Defeat counters
- [ ] Quest definitions and progress
- [ ] Quest journal
- [ ] One-time quest rewards
- [ ] Reputation
- [ ] Win streaks
- [ ] Boss unlock requirements

### Phase 9 — World flavor

- [ ] Locations
- [ ] Backgrounds
- [ ] Ambient descriptions
- [ ] NPC dialogue
- [ ] Travel events
- [ ] Weather/time flavor
- [ ] Current location HUD

Original locations include Town Square, Whisper Market, Ironforge Smithy, Guild Hall, Ember Camp, Ruined Path, Shadow Caverns, and Boss Gate.

### Phase 10 — Bosses and endgame

- [ ] Boss eligibility
- [ ] Multiple boss definitions
- [ ] Boss-specific behavior
- [ ] Multi-phase battles
- [ ] Unique rewards
- [ ] Endgame flow

### Phase 11 — Presentation and polish

- [ ] Final layouts
- [ ] HP and XP bars
- [ ] Sprites and backgrounds
- [ ] Damage feedback
- [ ] Transitions
- [ ] Sound and music
- [ ] Tooltips
- [ ] Settings
- [ ] Help screen
- [ ] Keyboard/controller navigation
- [ ] Resolution testing
- [ ] Playable build

## Suggested later branches

After Phase 1:

1. `feature/expanded-combat-actions`
2. `feature/monster-definitions`
3. `feature/item-data-model`
4. `feature/inventory-equipment`
5. `feature/loot-rewards`
6. `feature/shop-system`
7. `feature/save-load`
8. `feature/quests-progression`
9. `feature/world-locations`
10. `feature/boss-battles`
11. `feature/game-polish`

Each branch should start from the merged result of the previous dependency rather than having all feature branches created from an old `main`.

## Git conventions established

- Use descriptive branch names such as `feature/...`, `refactor/...`, and `test/...`.
- Keep generated Unity directories and IDE files ignored.
- Keep commits focused and independently understandable.
- Use imperative commit messages beginning with words such as Add, Refactor, Prevent, Test, or Document.
- Separate scene/Inspector changes from gameplay-code changes when practical.
- Test a feature in Play Mode before merging.
- Merge verified feature branches into `main`, push `main`, and remove completed feature branches.

## Collaboration convention

When continuing work, explicitly distinguish these categories:

- **Required Unity Editor setup:** creating objects, assigning Inspector references, wiring buttons, saving scenes, regenerating project files, or configuring components.
- **Required gameplay testing:** Play Mode steps that validate behavior.
- **No Editor action required:** code-only work that compiles and can proceed without scene changes.

This distinction was specifically requested so Editor responsibilities are not hidden inside a generic request to test the gameplay loop.

## Resume checklist on another machine

1. Clone or pull `AngelBladeRPG_Unity`.
2. Fetch all remote branches.
3. Check out `feature/character-creation`.
4. Confirm commit `d35d4c1` is present.
5. Open the project using Unity `6000.5.3f1`.
6. Open `Assets/Scenes/MainGameScene.unity`.
7. Check whether `CharacterCreationPanel` and its Inspector wiring were saved in Git.
8. If not, perform the Editor setup documented above.
9. Run the character-creation Play Mode checklist.
10. Commit any saved scene/UI work with a descriptive message.
11. Merge the branch into `main` after verification.

