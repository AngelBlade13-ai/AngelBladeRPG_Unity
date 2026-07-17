# Petals in the Dusk Unity Project Handoff

Last updated: July 16, 2026

This document summarizes the planning, implementation, Git history, Unity Editor work, and next steps discussed while converting the original command-line RPG into a Unity 2D project.

## Repositories

- Unity project: https://github.com/AngelBlade13-ai/AngelBladeRPG_Unity
- Original command-line RPG: https://github.com/AngelBlade13-ai/C-Object-Oriented-Programming-Final

The goal is to reuse the original game's design and gameplay rules while rebuilding its presentation and architecture for Unity as a complete commercial PC RPG. The next production target is a polished vertical-slice demo, followed by full-game production and an eventual Steam release. The original project contains character progression, combat actions, status effects, randomized monsters, loot, inventory, equipment, shops, quests, saving/loading, locations, travel events, bosses, and death recovery.

The public game title is **Petals in the Dusk**. The selected working public
developer/publisher brand is **Rawr! Studios**, with Steamworks onboarding
planned under the sole proprietor's legal name. A preliminary screen found
several similar `Rawr` studio brands, so that working name requires clearance
or replacement before public launch assets or accounts are finalized. Internal
repository and assembly identifiers remain `AngelBladeRPG_Unity` and
`AngelBladeRPG.Runtime`. See `Docs/PRODUCT_IDENTITY.md`.

The player promise, protected feature hierarchy, equipment economy, contributor
agreement draft, and asset-clearance workflow are recorded in
`Docs/PLAYER_PROMISE_FEATURE_HIERARCHY.md`,
`Docs/ITEM_EQUIPMENT_ECONOMY.md`,
`Docs/CONTRIBUTOR_AGREEMENT_TEMPLATE.md`, and
`Docs/ASSET_LICENSE_TRACKING.md`.

The original `Program.cs` is a large console-oriented implementation. Its concepts should be migrated, but its monolithic structure should not be copied directly into Unity.

## Gameplay direction

The intended game is a 2D pixel-fantasy RPG with top-down exploration and movement inspired by `Stardew Valley`. Towns, interiors, paths, and world areas should be physical spaces built with Tilemaps, sprites, collision, and interactable objects.

Combat should use a dedicated turn-based battle scene inspired by older `Final Fantasy` games. Entering an encounter transitions out of exploration; victory, defeat, or escape resolves the encounter and returns the player to the appropriate world location.

The existing full-screen town and battle panels are useful prototypes for gameplay rules and flow. They are not the final presentation architecture. See `Docs/GAMEPLAY_DIRECTION.md` for the current product direction and scene responsibilities.

The demo follows Suncrest Hollow, Grassland, and Cherry Blossom. Suncrest Hollow is the proper in-world name for the home hub previously described as `Town Square`; existing prototype scene names do not need to change yet. Its tone and lore boundaries are recorded in `Docs/DEMO_STORY_REFERENCE.md`. The main character is a custom-created protagonist and is not one of the authored companion IDs `pc_01` through `pc_04`; those records belong to Iona, Damari, Enora, and Lysander.

Demo progression uses a flexible hub-and-expedition quest loop. Players are encouraged to revisit Suncrest Hollow for turn-ins, services, and character moments. The three main quests form a guided story sequence, while travel and side-quest order remain flexible. The Grassland goblin boss is the required gate to Cherry Blossom; optional quest completion and elapsed time are not gates. A typical first run should contain at least 90 minutes of content, while a practiced critical-path run may be substantially shorter.

Ember Camp is now part of the demo plan as a compact recovery and companion-interaction space. It must not duplicate Suncrest Hollow's quest, commerce, smithy, recruitment, or major progression functions. The demo/full-game scope and technical boundaries are recorded in `Docs/CAMP_SYSTEM_DESIGN.md`.

The protagonist begins as a newcomer and investigates a delayed caravan in Grassland. A mandatory staged tutorial battle recruits Iona, then Damari and Enora, before the four return to Suncrest Hollow for the job tutorial. The Guild Hall posts three main quests whose briefings lead to Old Marlow, Bren, and Captain Vashti in different town districts. All three unlock the regional Goblin Boss. Lysander becomes eligible after any three unique quest completions and joins after Captain Vashti's Quest 3 handoff and the forced town-exit scene. The authoritative flow and tutorial safety constraints are in `Docs/DEMO_QUEST_FLOW.md`.

The main quest field content is defined in `Docs/DEMO_MAIN_QUEST_CONTENT.md`: Quest 1 uses minimum-plus-optional investigation spots, Quest 2 follows a linear clue trail to an injured scout, and Quest 3 rescues scattered scouts for scaling intelligence and rewards. Grassland combines these dedicated goblin encounters with step-triggered ambient creature battles. Exact balance values and several content identities remain open.

Approved Grassland combat content is recorded in `Docs/GRASSLAND_COMBAT_DESIGN.md`. It defines Skirmisher, Slinger, Guard, and Raider goblins; Slime and Wild Boar ambient groups; the three scout-intelligence advantages; and a non-supernatural Goblin Boss built around Commanding Shout, telegraphed Brutal Swing, Off Balance, reinforcements, and a low-HP Reckless state. Numeric balance remains provisional until party combat is implemented.

After the Goblin Boss, the party receives a warm Suncrest Hollow welcome and a quiet inn scene before investigating another overdue caravan from Cherry Blossom. Ember Camp is introduced on that journey. The nomadic settlement's sacred tree is blooming and shedding out of season, and its traditionally protective Great Stag has become territorial and distressed. Subduing it does not heal the tree; the demo ends on an unresolved scene and an environmental progression block. The authoritative arc is in `Docs/DEMO_CHERRY_BLOSSOM_ARC.md`.

The Cherry Blossom boss is the Great Stag, a living protector whose branch-like flowering antlers symbolize a seasonal cycle falling out of rhythm. Its solo battle marks a target for a heavy charge; taunt can redirect it, and Defend causes a temporary Staggered opening. At low HP it becomes Panicked. Zero HP subdues rather than kills it, while preserving the ordinary one-time victory and reward path. Full rules are in `Docs/CHERRY_BLOSSOM_COMBAT_DESIGN.md`.

Four optional quests are defined in `Docs/DEMO_SIDE_QUESTS.md`: `A Taste Of Home`, `The Roadside Chimes`, `A Painter's View`, and `Where The Herd Won't Graze`. They reuse ordinary quest interactions, support the intended playtime and hub loop, and add no critical-path gates, escort AI, timers, minigames, or new lore explanations.

The demo Ember Camp is locked in `Docs/CAMP_SYSTEM_DESIGN.md`: it is discovered at one designated road campsite, begins with a free tutorial full rest, and uses one Camp Ration per later full rest. Camp restores every available recruited member but does not reset encounters or advance time. It allows party formation, equipment and item use, companion talks, and leaving, while job changes and town services remain in Suncrest Hollow. Its two events are `First Fire` and the optional Lysander conversation `Why This Party`.

All 12 catalog jobs, including Summoner, are playable in the demo for the
protagonist and every companion. Each tree has its own data-driven demo
progression limit; affinity never restricts access, and learned nodes persist
per character when jobs or party slots change. Exact nodes and point pacing are
defined in `Docs/DEMO_JOB_TREES.md`: each job grants a core trait and action,
three purchased job abilities or passives, two two-rank permanent stat tracks,
and one Tier 3 mastery stat node for a `10 JP` complete demo tree. Permanent
stat nodes from all learned jobs remain active after switching; inactive-job
traits, actions, and passives do not. JP is stored per character, may be spent
on any job, and is not lost at a demo cap. See also
`Docs/DEMO_JOB_TREE_SCOPE.md` for the broader boundary.

The required demo maps, NPC roles, single-leader exploration presentation,
quests, services, enemy roster, UI, preliminary asset budget, and explicit cut
list are consolidated in `Docs/DEMO_CONTENT_MANIFEST.md`. It corrects older
planning assumptions: all 12 capped jobs and all four optional side quests are
required, and the Cherry Blossom boss is the already-defined Great Stag.
The artist-facing production order, three-level character presentation,
provisional scale tests, animation backlog, and delivery rules are maintained in
`Docs/DEMO_ART_CHECKLIST.md`.

## Current Git state

Current local state:

- Active branch: `feature/core-combat-expansion`
- The feature branch tracks `origin/feature/core-combat-expansion`.
- Milestone 13 combat core completion is committed and pushed.
- Local `main` latest commit: `92f04da Document job and party system design`
- Remote `origin/main` latest commit: `08e8240 Add shared RPG game state`
- Character creation, walkable exploration, world interactions, the separate battle-scene loop, the first job/roster data slice, reusable monster definitions, and the completed combat core are available on the pushed feature branch.
- The generated `.slnx` file is no longer tracked and can be regenerated by Unity.

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
| `97576e3` | Add project migration handoff |
| `62fb675` | Add character creation UI |
| `92f04da` | Document job and party system design |
| `00c7dd9` | Add world interactions and scene transitions |
| `7c1c767` | Add separate turn-based battle scene |

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
- Optional Unity AI Assistant and AI Inference packages were removed because the project does not use subscription-based Unity AI features.

Setup scene:

- `Assets/Scenes/MainGameScene.unity`
- This scene owns the title and character-creation flow before loading exploration.

Existing panels:

- `TitlePanel`
- `CharacterCreationPanel`

The obsolete `TownPanel` and `BattlePanel` have been removed. `TownScene` now owns exploration and `BattleScene` owns turn-based combat.

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

## Character creation — completed milestone

Branch:

`feature/character-creation`

The code is committed as `d35d4c1`, and the Unity scene wiring was completed and tested in Play Mode on July 13, 2026.

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
3. `feature/character-creation` — completed and merged into local `main`
4. `test/core-gameplay-foundation` — implemented and verified locally

## Core-foundation test milestone

The Edit Mode suite covers:

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

The original core suite ran successfully in Unity `6000.5.3f1` on July 14, 2026: 18 passed, 0 failed. The current suite contains 210 passing tests after adding pixel-world movement, camera, temporary direction-indicator, walkable-town foundation, directional world interaction, door-transition, battle-scene, job, affinity, progression, party-roster, runtime-party targeting, party-round resolution, party-command selection, core abilities, authored party-member, speed-based turn-order, legacy speed-resolved battle-round, bond, roster-history, shared combat-stat, reusable monster-definition, and structured combat-action coverage. Run instructions are in `Docs/TESTING.md`.

Milestone 15 job progression is currently in progress on
`feature/milestone-15-party-jobs`. The first checkpoint adds stable definitions
for all 96 purchased demo job nodes, character-owned JP and unlock state,
cross-job permanent stat aggregation, equipped-job feature restrictions, and
roster-wide awards for available active and benched characters. Its 22 added
test cases pass as part of the verified 160-test Edit Mode suite.

The second checkpoint is implemented and verified. It makes
each `PlayableCharacterData` a persistent runtime combatant, creates the custom
protagonist under stable ID `pc_protagonist`, preserves active formation order,
and introduces actor-relative ally/enemy targeting with explicit incapacitation
rules. Its 17 added test cases pass as part of the verified 177-test Edit Mode
suite.

The third checkpoint is implemented and verified. The new
`PartyBattleRoundResolver` validates all living-party commands before mutation,
generates enemy commands through a replaceable policy, resolves all living
combatants in speed order, applies Defend from the actor's turn onward, skips
incapacitated turns, and retargets attacks whose original targets fall earlier
in the round. Its 13 added tests pass as part of the verified 190-test Edit Mode
suite.

The fourth checkpoint is implemented and verified. `PartyCommandSelection`
advances through living active members, tracks
the selected enemy, and emits the complete command list. `BattleSceneController`
now renders compact party/enemy HP and MP lists, non-color-only actor/target
markers, target cycling, and party-round submission. `BattlePrototypeBuilder`
repairs the existing scene and serialized references. Its nine added tests pass
as part of the verified 199-test Edit Mode suite, and the repaired scene passed
its manual command-flow smoke test.

The fifth checkpoint is implemented and verified.
`CombatAbilityCatalog` defines the first five core job actions: Power Strike,
Ember, Blood Bolt, Mend, and Lay On Hands. Party commands now validate equipped
jobs, ally/enemy targets, MP costs, and nonlethal Blood Mage HP costs before any
round mutation. The shared round resolver executes their physical, magic, and
healing effects in speed order and fails safely if earlier damage makes an HP
cost unsafe. Its 11 added tests pass as part of the verified 210-test Edit Mode
suite.

There was previously a stale generated `.csproj` reference to the missing file `Assets/Editor/HubForceResolve.cs`. Unity itself successfully compiled the gameplay scripts. Generated Unity project files should remain ignored and can be regenerated rather than manually maintained.

## Historical Migration Roadmap

This phase list records the original command-line migration plan. It is retained
for implementation history, but it no longer controls project order or release
scope. `Docs/ROADMAP.md` is the authoritative product roadmap for the
vertical-slice demo, full-game production, and Steam release.

### Phase 1 — Foundation

- [x] Player leveling
- [x] Shared game session
- [x] Separate rewards/state from UI coordination
- [x] Finish character creation UI and testing
- [x] Add core gameplay tests
- [x] Clean stale generated project references

### Phase 2 — Pixel world foundation

- [x] Choose a pixel-art reference resolution and Pixel Perfect Camera settings
- [x] Add top-down player movement using Unity's Input System
- [x] Add Rigidbody2D collision and normalized diagonal movement
- [x] Add directional idle and walk animation support
- [x] Build a small Tilemap test area
- [x] Add a following camera
- [x] Test movement and framing at supported resolutions

### Phase 3 — Walkable town and interactions

- [x] Replace `TownPanel` as the primary town experience
- [x] Build a small playable town map with collision
- [x] Add doors and map transition points
- [x] Add a reusable interaction system for NPCs and world objects
- [x] Add a compact exploration HUD or pause status view
- [x] Preserve player state and explicit return spawns across scene transitions

### Phase 4 — Separate battle scene

- [x] Create a dedicated battle scene
- [x] Define encounter data passed from exploration into battle
- [x] Present the player, enemies, commands, combat text, and rewards
- [x] Reuse the tested combat math and reward rules
- [x] Add explicit victory, defeat, and escape outcomes
- [x] Return to the correct exploration scene and position
- [x] Replace the prototype `BattlePanel` flow

### Phase 5 — Jobs, party systems, and character data

- [x] Implement the job and affinity foundation described in `Docs/JOB_CLASS_SYSTEM.md`
- [x] Add party formation and reserve-roster rules
- [x] Support permanent roster-removal state and record equipped-item loss as a mandatory inventory/save rule
- [x] Keep job and party data independent from scene presentation

### Phase 6 — Core combat expansion

- [x] Reusable ID-keyed monster definitions and a small monster roster
- [x] Critical hits, accuracy, misses, guarding, and speed-based escape
- [ ] Potions and combat abilities
- [ ] Poison and burn
- [x] Improved speed-based battle sequencing and structured outcome data

### Phase 7 — Data-driven content

- [ ] Monster ScriptableObjects
- [ ] Item ScriptableObjects
- [ ] Weapons, armor, accessories, and potions
- [ ] Rarity tiers
- [ ] Encounter selection
- [ ] Move balance values out of `GameManager`

### Phase 8 — Inventory and equipment

- [ ] Base item model
- [ ] Inventory
- [ ] Weapons
- [ ] Armor
- [ ] Accessory slot
- [ ] Base stats plus equipment bonuses
- [ ] Potions
- [ ] Destroy a permanently removed character's equipped items exactly once without returning them to shared inventory
- [ ] Inventory UI
- [ ] Equipment comparisons

### Phase 9 — Loot and encounters

- [ ] Random monsters
- [ ] Progression scaling
- [ ] Monster affixes
- [ ] Randomized rewards
- [ ] Item drops
- [ ] Rarity-based loot
- [ ] Encounter modifiers
- [ ] Win streaks
- [ ] Victory reward summary

### Phase 10 — Town services and shops

- [ ] Shopkeeper interaction
- [ ] Shop UI
- [ ] Buying
- [ ] Selling
- [ ] Camp/rest
- [ ] Gear in the exploration HUD or pause menu

### Phase 11 — Saving and loading

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

### Phase 12 — Quests and progression

- [ ] Defeat counters
- [ ] Quest definitions and progress
- [ ] Quest journal
- [ ] One-time quest rewards
- [ ] Reputation
- [ ] Win streaks
- [ ] Boss unlock requirements

### Phase 13 — World content and story

- [ ] Additional locations and interiors
- [ ] Backgrounds
- [ ] Ambient descriptions
- [ ] NPC dialogue
- [ ] Travel events
- [ ] Weather/time flavor
- [ ] Current location HUD

Original locations include Town Square, Whisper Market, Ironforge Smithy, Guild Hall, Ember Camp, Ruined Path, Shadow Caverns, and Boss Gate.

### Phase 14 — Bosses and endgame

- [ ] Boss eligibility
- [ ] Multiple boss definitions
- [ ] Boss-specific behavior
- [ ] Multi-phase battles
- [ ] Unique rewards
- [ ] Endgame flow

### Phase 15 — Presentation and polish

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

1. `feature/pixel-world-foundation`
2. `feature/walkable-town`
3. `feature/world-interactions`
4. `feature/separate-battle-scene`
5. `feature/job-party-data`
6. `feature/expanded-combat-actions`
7. `feature/monster-definitions`
8. `feature/item-data-model`
9. `feature/inventory-equipment`
10. `feature/loot-rewards`
11. `feature/shop-system`
12. `feature/save-load`
13. `feature/quests-progression`
14. `feature/boss-battles`
15. `feature/game-polish`

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

For learning and automation, introduce a new Unity concept manually the first time and explain how its components fit together. Automate repeated scene construction, object creation, and Inspector wiring unless the user asks to practice it manually again. Keep generated Editor tools readable and documented so the project does not depend on unexplained automation.

## Resume checklist on another machine

1. Clone or pull `AngelBladeRPG_Unity`.
2. Fetch all remote branches.
3. Check out `feature/milestone-15-party-jobs`.
4. Confirm the pixel-world, battle-scene, job/party, shared-stat, monster-definition, and structured-action commits are present.
5. Open the project using Unity `6000.5.3f1`.
6. Open `Assets/Scenes/MainGameScene.unity`.
7. Run the Edit Mode suite using `Docs/TESTING.md`.
8. Confirm all 199 tests pass while the party-command UI checkpoint is active.
9. Run the character-creation Play Mode checklist if the Unity version or scene changes.
10. Review the completed Unity Editor checklist in `Docs/PIXEL_WORLD_SETUP.md` when changing the exploration foundation.

