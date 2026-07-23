# Running Core Gameplay Tests

The core gameplay tests are Unity Edit Mode tests. They exercise plain gameplay classes without entering Play Mode or opening `MainGameScene`.

## Unity 6.5 Editor

1. Open the project in Unity `6000.5.3f1`.
2. Wait for script compilation to finish.
3. Open `Window > General > Test Runner`.
4. Select the Edit Mode tests.
5. Click `Run All`.

The test assembly is `AngelBladeRPG.EditModeTests` under `Assets/Tests/EditMode`.

Verified on July 23, 2026 with Unity `6000.5.3f1`: 365 passed, 0 failed.

The current suite includes 30 core gameplay tests, five pixel-world movement and camera tests, five temporary direction-indicator tests, four walkable-town foundation tests, seven interaction tests, eight door-transition tests, 19 battle-scene tests, 13 party-command selection tests, 11 core-ability tests, 63 job, affinity, progression, playable-character, party-roster, and party-management tests, 11 runtime-party targeting tests, 13 party-round resolver tests, 10 authored party-member tests, five speed-based turn-order tests, nine legacy speed-resolved battle-round tests, five bond and roster-history tests, nine shared combat-stat tests, 18 reusable monster-definition tests, 14 enemy encounter and layout tests, 15 structured combat-action tests, four item-catalog tests, four inventory tests, eight character-equipment tests, five item-use tests, six camp-rest tests, and nine shop/town-recovery tests.

### Milestone 15 Job Progression Checkpoint

The job-progression checkpoint added 22 Edit Mode cases. The full suite now
reports 160 passing tests in Unity `6000.5.3f1`.

The added coverage checks:

- all 96 stable purchased nodes and the eight-node, 10-JP shape of each job;
- catalog prerequisites, stable-ID uniqueness, and the Tier 3 demo cap;
- atomic JP spending, duplicate prevention, and prerequisite enforcement;
- permanent stat aggregation across job changes;
- equipped-job restrictions for learned abilities and passives;
- active and benched JP awards while permanently removed members are excluded.

### Milestone 15 Runtime Party Checkpoint

The runtime-party checkpoint added 17 Edit Mode cases. The full suite now
reports 177 passing tests in Unity `6000.5.3f1`.

The added coverage checks:

- persistent playable characters implementing the shared combatant contract;
- a stable protagonist roster ID whose combat stats remain synchronized with
  the current prototype `PlayerData`;
- active formation order and exclusion of benched members from battle;
- party and enemy target selection relative to the acting combatant;
- living, incapacitated, self, single-target, and all-target rules;
- duplicate combatant IDs and the four-member active-party limit;
- party defeat occurring only when every active member is incapacitated.

### Milestone 15 Party Round Resolver Checkpoint

The party-round checkpoint added 13 Edit Mode cases. The full suite now reports
190 passing tests in Unity `6000.5.3f1`.

The added coverage checks:

- commands for every living party member before a round can mutate combat;
- shared speed order across multiple party members and enemies;
- incapacitated actors losing queued turns without requiring commands;
- replaceable enemy command selection and invalid-policy rejection;
- automatic ally and enemy retargeting after the original target falls;
- Defend applying only after the defending combatant's turn begins;
- structured party/enemy defeat results and completed-battle no-op behavior.

### Milestone 15 Party Command UI Checkpoint

The command-UI checkpoint adds nine Edit Mode cases. The full suite now reports
199 passing tests in Unity `6000.5.3f1`. The repaired battle scene also passed
its manual command-flow smoke test.

The added coverage checks:

- command selection advancing through living formation order;
- attack and Defend command creation for each active actor;
- enemy-target cycling and wraparound in both directions;
- preserving the selected stable target ID in queued commands;
- completed selection rejecting accidental extra commands;
- party/enemy status text exposing HP, MP, actor, target, and incapacitation;
- command prompts naming the acting party member and selected enemy.

### Milestone 15 Core Ability Foundation Checkpoint

The core-ability checkpoint adds 11 Edit Mode cases. The full suite now reports
210 passing tests in Unity `6000.5.3f1`.

The added coverage checks:

- stable definitions for Power Strike, Ember, Blood Bolt, Mend, and Lay On
  Hands;
- physical ability damage, magic defense, healing, and maximum-HP clamping;
- MP costs and Blood Mage HP costs remaining distinct;
- HP costs never incapacitating their caster;
- wrong-job, invalid-target, and insufficient-resource commands rejecting the
  whole round before mutation;
- healing resolving in the shared speed order; and
- a Blood Mage spell failing safely if earlier damage makes its HP cost unsafe.

### Milestone 15 Ability Command UI Checkpoint

The ability-command checkpoint adds eight Edit Mode cases. The full suite now
reports 218 passing tests in Unity `6000.5.3f1`. The repaired battle scene also
passed its manual ability-command smoke test.

The added coverage checks:

- core actions entering their authored ally or enemy targeting mode;
- living-ally target cycling for healing abilities;
- confirmed commands preserving stable ability and target IDs;
- unaffordable MP and unsafe HP costs disabling ability targeting;
- Attack canceling ability mode and returning to enemy targeting;
- the prompt communicating ability name, cost, actor, and target; and
- self-targeted actions displaying both actor and target markers.

### Milestone 15 Equipped-Job Stat Checkpoint

The equipped-job stat checkpoint adds seven Edit Mode cases. The full suite
reports 225 passing tests in Unity `6000.5.3f1`.

The added coverage checks distinct job packages, affinity scaling, permanent
cross-job bonuses, non-stacking recalculation, missing-resource preservation,
external level growth, and incapacitation preservation.

### Milestone 15 Party And Job Service Checkpoint

The party-management checkpoint adds 12 Edit Mode cases. The full suite reports
237 passing tests in Unity `6000.5.3f1`. The generated Guild Hall service passed
its Play Mode check for new-game routing, job preview and assignment, effective
stat updates, modal movement control, and closing back into exploration.

The added coverage checks:

- active members appearing in formation order before stable-ID-sorted reserves;
- adding, benching, and reordering characters without exceeding four members;
- preventing the final active character from being benched;
- invalid formation changes leaving the roster untouched;
- all catalog jobs remaining assignable regardless of affinity; and
- removed or unknown characters being rejected safely.

### Milestone 15 Party Outcome And Reward Checkpoint

This checkpoint adds nine Edit Mode cases. The full suite reports 246 passing
tests in Unity `6000.5.3f1`. It requires no scene rebuilding or Inspector
wiring.

The added coverage checks:

- shared level and XP progression for the protagonist and companions;
- level growth remaining stable across later job changes;
- active participants receiving XP, including incapacitated allies;
- available active and reserve characters receiving the same authored JP;
- reserves receiving no battle XP;
- per-character level-up details in structured battle rewards; and
- victory, defeat, and escape recording participation only once, with defeat
  and escape granting no currency, XP, or JP.

### Milestone 15 Enemy Group And Layout Checkpoint

This checkpoint adds 28 Edit Mode cases. The full suite reports 274 passing
tests in Unity `6000.5.3f1`. It requires no scene rebuilding or Inspector
wiring.

The added coverage checks:

- all eight authored Grassland/tutorial monster roles and their stable IDs;
- all ten current Grassland quest, patrol, ambient, and boss groups;
- duplicate enemy roles receiving unique runtime combatant IDs and labels;
- standard four-enemy and boss five-enemy formation capacity;
- encounter groups fitting their selected layout;
- `GameSession` starting authored groups and retaining legacy single enemies;
- victory waiting for the entire enemy group and aggregating every reward; and
- the Goblin Boss encounter selecting its boss layout and rejecting escape.

### Milestone 15 Caravan Tutorial Checkpoint

This checkpoint adds nine Edit Mode cases. The full suite reports 283 passing
tests in Unity `6000.5.3f1`. No manual Inspector wiring is required.

The added coverage checks:

- the solo Goblin wave, Iona/Hobgoblin pressure stage, Damari and Enora
  reinforcement stage, and final completion occurring in order;
- scripted low-HP checkpoints and combat damage floors remaining nonlethal;
- the Hobgoblin remaining at 1 HP until the reinforcement beat;
- enemy focus moving from Iona to Damari after the taunt demonstration;
- escape being disabled and rewards being unavailable before completion;
- all wave rewards being aggregated exactly once;
- the protagonist, Iona, Damari, and Enora becoming the active party; and
- the completed one-time tutorial refusing replay in the same session.

For the Play Mode smoke test, run
`Tools > AngelBlade RPG > Battle > Add Caravan Tutorial Test Encounter`, then
start from `MainGameScene`. The orange marker in the Guild Hall district
launches the caravan tutorial. Confirm each reinforcement appears, `No Escape`
is disabled, the Hobgoblin survives the two-character pressure round, and the
fight returns to the Guild Hall after the four-character victory.

### Milestone 15 Action-Gauge Checkpoint

This checkpoint adds 21 Edit Mode cases, bringing the expected suite to 304
tests. No new scene or Inspector wiring is required.

The added coverage checks:

- faster combatants filling and acting before slower combatants;
- stable ordering for exact timing ties;
- separately preserving multiple enemy turns that become ready together;
- Wait Mode pausing every gauge while a command menu is open;
- Active Mode continuing every gauge while a command menu is open;
- consuming only the acting combatant's gauge;
- zero-Speed combatants still eventually receiving a turn;
- tutorial enemy replacement and reinforcement gauge synchronization;
- one party action resolving immediately without an automatic enemy reply;
- ready enemies acting without a queued party-command batch;
- Defend lasting until the defender's next action begins;
- immediate ability use, invalid-target fallback, and rejection of living
  targets on the wrong side;
- fixed command selection using only the gauge-ready party member;
- visible `AT` percentage formatting; and
- the tutorial waiting for the Hobgoblin's pressure action before Damari and
  Enora reinforce the party;
- Taunt being a zero-cost active Reaver ability; and
- Taunt redirecting enemies until the Reaver's next action begins.

For the Play Mode smoke test, launch any battle and confirm:

1. `AT` percentages rise at different rates based on Speed.
2. Command buttons appear for only one ready party member.
3. Attack, Ability, and Defend resolve immediately after selection.
4. In default Wait Mode, every `AT` percentage freezes while the command menu
   is open and resumes after the action.
5. Enemies act automatically when their own gauge reaches `100%`.
6. The caravan tutorial still advances through all three authored stages.
7. After Damari joins, the Hobgoblin remains at a minimum of `1 HP` until
   Damari actively uses Taunt; subsequent Hobgoblin attacks target Damari.

### Milestone 15 Keyboard/Gamepad Navigation Checkpoint

This checkpoint adds keyboard and gamepad UI navigation across the title
screen, character creation, the Guild Hall party/job panel, and the battle
command/ability/target menus. It is EventSystem-dependent UI glue rather than
plain gameplay logic, so it adds no new Edit Mode tests and the suite stays at
304 tests.

Required before the Play Mode pass below:

1. In `MainGameScene`, assign `GameManager.titleFirstSelected` to the title
   screen's New Game button and save the scene.
2. Run `Tools > AngelBlade RPG > World > Build Guild Hall Party Service` to
   bake the new Guild Hall navigation graph into `SuncrestGuildHallScene`.
3. Run `Tools > AngelBlade RPG > Battle > Repair Battle Scene Interface` to
   bake the new battle command-bar navigation graph into `BattleScene`.

Manual Play Mode checklist, using only a keyboard (arrow keys and Enter)
and then only a gamepad (d-pad/stick and the South face button):

1. From the title screen, confirm New Game is already highlighted without
   touching the mouse, and that Confirm/Back on the character creation panel
   are reachable.
2. In the Guild Hall panel, confirm every control (character cycle, formation
   move, party toggle, job cycle, apply job, close) is reachable and that
   toggling a control that becomes disabled (for example Move Up at the top
   of formation) moves the highlight to a valid control instead of leaving
   nothing selected.
3. In battle, confirm Attack/Ability/Defend/Escape are reachable each time a
   new party member's turn begins, that entering and leaving Ability
   targeting keeps a valid selection, and that the Continue button is
   automatically selected on victory, defeat, and escape.
4. Confirm the caravan tutorial can be completed end to end using only the
   keyboard, then only a gamepad.

### Milestone 16 Item And Equipment Batch One

This batch adds 16 Edit Mode cases, bringing the expected suite to 320 tests.
The cases cover stable item IDs, all eight weapon categories, five equipment
slots, inventory stack transactions, weapon/job compatibility, equipment stat
recalculation, safe job-switch unequipping, and permanent-removal destruction.

No Play Mode pass is required for this headless batch. Run the complete Edit
Mode suite once; UI and scene checks begin when the inventory/equipment screens
are introduced.

### Milestone 16 Recovery And Economy Batch Two

This batch adds 20 Edit Mode cases, bringing the expected suite to 340 tests.
It covers successful and rejected item use, no-effect consumption protection,
the one-time free tutorial rest, exact ration consumption, active and benched
party recovery, unavailable-character exclusion, atomic shop transactions,
protected equipment, and paid town recovery.

No Play Mode pass is required for this service-only batch.

### Milestone 16 Exploration Inventory Batch Three

This batch adds five Edit Mode cases, bringing the verified suite to 345 tests.
It covers owned-consumable filtering, deterministic display order,
job-compatible equipment filtering, equipment bonus descriptions, and item-use
feedback.

Required Unity Editor setup:

1. Let Unity finish importing and compiling the new scripts and input action.
2. Run `Tools > AngelBlade RPG > UI > Install Inventory Menu In Exploration Scenes`.
3. Save any scene Unity reopens if it reports unsaved changes.

The installer finds scenes containing both `PlayerMovement2D` and
`ExplorationStatusHUD`, then creates and wires the menu automatically. It is
safe to run again; the generated `ExplorationMenuCanvas` is replaced instead
of duplicated.

Manual Play Mode checklist:

1. Start a game and enter any exploration scene.
2. Press `Tab` or `I`; on a controller, press Start. Confirm the menu opens and
   player movement and interaction pause.
3. Confirm Items and Equipment can be selected with the mouse, keyboard, and
   controller, and that Escape/controller Cancel closes the menu.
4. With an empty inventory, confirm both views clearly report that no usable
   item is available and their action buttons are disabled.
5. After buying or otherwise receiving a potion and equipment, confirm a
   potion heals a damaged available character and is consumed exactly once.
6. Confirm compatible equipment changes stats, appears in the selected slot,
   and returns to inventory when unequipped. Confirm incompatible weapons do
   not appear for the selected character's current job.
7. Close the menu and confirm movement and interaction resume.

### Milestone 16 Town Services Batch Four

This batch adds eight Edit Mode cases, bringing the verified suite to 353 tests.
It covers deterministic shop stock, sellable-inventory filtering, buy and sell
totals, equipment descriptions, rejected-transaction feedback, and recovery
feedback. It also verifies that shop, recovery, inventory, and party-management
menus cannot open on top of one another.

Required Unity Editor setup:

1. Let Unity finish importing and compiling the new scripts.
2. Run `Tools > AngelBlade RPG > World > Town Services > Install Temporary
   Test Fixtures`.
3. The command adds a clearly labeled colored-square fixture and functional
   menu to each service scene, saves those scenes, and reopens the scene that
   was active before it ran.

The temporary fixtures are only for gameplay verification. When permanent
service art and placement are ready, select the intended counter, NPC, or sign
and use the matching command under
`Tools > AngelBlade RPG > World > Town Services`:

   - `Wire Selected as Whisper Market Shop`
   - `Wire Selected as Ironforge Shop`
   - `Wire Selected as Suncrest Inn Recovery`
   - `Wire Selected as Sunwell Shrine Recovery`

The selection-based commands do not move, resize, recolor, or replace the
selected object.

Manual Play Mode checklist:

1. Start from `MainGameScene`, complete a rewarding battle so the player has
   gold, and return to Suncrest Hollow.
   To avoid grinding during verification, close any open service panel and run
   `Tools > AngelBlade RPG > Testing > Grant 1000 Test Gold`. This changes only
   the current Play Mode session and is unavailable in a player build.
2. Enter Whisper Market, interact with the object you wired, and confirm Buy/Sell,
   item cycling, quantity controls, totals, current gold, and Cancel work with
   mouse, keyboard, and controller.
3. Buy a Minor Potion. Open the exploration inventory and confirm it appears
   once with the correct quantity.
4. Enter Ironforge, interact with the object you wired, buy compatible equipment,
   and confirm it can be equipped through the exploration menu. Sell an
   unequipped sellable item and confirm both inventory and gold update once.
5. Confirm unaffordable purchases, full stacks, empty sell lists, and protected
   equipment produce clear feedback without changing inventory or gold.
6. After taking HP or MP damage, use the Inn or Shrine recovery object. Confirm
   the panel requires confirmation, charges exactly 25 gold, and fully restores
   every available active and benched character.
7. Interact again while fully recovered and confirm no additional gold is
   charged.
8. Close each panel and confirm movement and interaction resume.

Verified on July 23, 2026: all four temporary fixtures opened and closed;
Whisper Market buying and selling worked; the Editor-only test-gold command
allowed transaction coverage without grinding; and town recovery behaved as
expected.

### Milestone 16 Battle Items Batch Five

This batch adds 12 Edit Mode cases, bringing the expected suite to 365 tests.
It covers battle-item eligibility, living injured-ally targeting, HP clamping,
one-time inventory consumption, invalid-target rejection, command-selection
state, and explicit rejection by the legacy queued-round resolver.

Required Unity Editor setup:

1. Focus Unity and let script compilation finish.
2. Run `Tools > AngelBlade RPG > Battle > Repair Battle Scene Interface`.
3. The command repairs the existing `BattleScene` in place and adds the
   functional `Item` command without rebuilding exploration scenes.
4. Open `Window > General > Test Runner`, select Edit Mode, and run all tests.
   The expected result is 365 passed and 0 failed.

Manual Play Mode checklist:

1. Enter Play Mode, use
   `Tools > AngelBlade RPG > Testing > Grant 1000 Test Gold`, and buy at least
   two Minor Potions from Whisper Market.
2. Start a battle and allow an available party member to take damage.
3. When a party member's gauge is ready, confirm `Item` is enabled. Select it
   and confirm the prompt reports the Minor Potion quantity.
4. Confirm only living, available, injured allies can be targeted. Cycle among
   multiple valid targets when available.
5. Use the potion. Confirm the target recovers up to 40 HP without exceeding
   maximum HP, the inventory quantity decreases exactly once, and the acting
   character's gauge is consumed.
6. Confirm `Item` is disabled when no Minor Potion is owned or no valid injured
   ally exists. Incapacitated allies must not be valid targets.
7. In Wait Mode, leave the item target prompt open briefly and confirm action
   gauges remain paused until the command is completed or cancelled.

Verified on July 23, 2026: all 365 Edit Mode tests passed and the repaired
`BattleScene` completed its battle-item Play Mode smoke test successfully.

### Milestone 16 Demo Economy Batch Six

This batch adds 15 Edit Mode cases, bringing the verified suite to 380 tests.
It covers stable authored reward bundles, valid reward item quantities,
provisional quest-equipment statistics, protected quest evidence, optional
monster loot tables, exact-once atomic reward grants, inventory-full rollback,
and the no-grind critical-path affordability proof.

The pacing test guarantees `629 gold` on the lowest-income critical route. Its
representative readiness basket costs `595 gold` and includes three relevant
weapon purchases, one armor purchase, three Minor Potions, and two paid full
recoveries. Optional objectives, side quests, random drops, selling, and
grinding are excluded from that income.

No Editor command or Play Mode pass is required for this data-and-service
batch. Let Unity compile, then run all Edit Mode tests. The expected result is
380 passed and 0 failed.

Verified on July 23, 2026: all 380 Edit Mode tests passed.

### Milestone 17 Versioned Save Contract Batch One

This batch adds 10 Edit Mode cases, bringing the expected suite to 390 tests.
It covers initialized version-one save sections, a populated JSON round trip,
required-section validation, unsupported schema rejection, empty and malformed
input handling, and the rule that save records cannot reference Unity scene
objects.

The save contract includes player identity, appearance selection, gold, party
members, active formation, character progression and stats, jobs, affinities,
learned nodes, equipment, roster history, inventory, quest state, world flags,
completed encounters, claimed rewards, camp state, consumed camp events, and
explicit scene/spawn locations. This batch does not write files or load a
runtime `GameSession` yet.

No Editor command or Play Mode pass is required. Let Unity compile, then run
all Edit Mode tests. The expected result is 390 passed and 0 failed.

Verified on July 23, 2026: all 390 Edit Mode tests passed.

### Milestone 17 Session Mapping Batch Two

This batch adds 10 Edit Mode cases, bringing the expected suite to 400 tests.
It covers capture preconditions, stable job/slot/affinity IDs, a populated JSON
session round trip, shared protagonist/player progression, transient battle
reset, equipped-item ownership, non-stacking stat recalculation, corrupt
catalog-reference rejection, and explicit rejection of future quest content
until its runtime system exists.

The restore path builds a fresh `GameSession` and returns it only after every
current runtime section validates. It does not replace `GameSessionStore.Current`
or write a file yet, so no Play Mode check or Editor command is required.

Let Unity compile, then run all Edit Mode tests. The expected result is
400 passed and 0 failed.

Verified on July 23, 2026: all 400 Edit Mode tests passed.

### Milestone 17 Save File Storage Batch Three

This batch adds 10 Edit Mode cases, bringing the verified suite to 410 tests.
It covers the default application-data save folder, slot path containment,
durable JSON creation, temporary-file cleanup, overwrite backups, missing
saves, corrupt-primary backup recovery, unrecoverable corruption, unsupported
future schemas, and atomic loading into `GameSessionStore`.

Tests use a unique temporary directory and remove it after each case. The
player's real application-data save directory is not touched. Player-facing
manual save, autosave, and Continue controls are not part of this batch.

No Editor command or Play Mode pass is required. Let Unity compile, then run
all Edit Mode tests.

Verified on July 23, 2026: all 410 Edit Mode tests passed.

### Milestone 17 Player Save Flow Batch Four

This batch adds 10 Edit Mode cases, bringing the verified suite to 420 tests.
It covers empty Continue state, started-session and safe-location requirements,
manual slot creation, battle-time save blocking, autosave destinations,
newest-valid-slot selection, corrupt-slot fallback, playtime carry-forward,
and rejection of invalid saves without replacing the current session.

The tests use unique temporary directories and do not touch player saves.
No scene or UI layout is generated by this batch. `GameManager.ContinueGame()`
and `InventoryEquipmentMenu.SaveGame()` are available for existing or
user-authored buttons, while scene and battle boundaries autosave
automatically.

No Editor command or Play Mode pass is required for the code checkpoint. Let
Unity compile, then run all Edit Mode tests.

Verified on July 23, 2026: all 420 Edit Mode tests passed.

### Milestone 17 Save UI And Live Persistence Batch Five

This batch adds no new automated cases; the verified Edit Mode suite remains
at 420. It wires the title Continue control and uses
`SaveButtonPropagationTool` to copy a user-authored Save control across the
eight Suncrest exploration menus without generating its visual design.

Verified manually on July 23, 2026:

1. Started a new game and reached Suncrest Guild Hall.
2. Created a manual save and received the `Game saved.` confirmation.
3. Exited and restarted Play Mode.
4. Continued into the saved scene with recognizable session state restored.
5. Confirmed the Save button remained functional after loading.
6. Crossed a district boundary, restarted Play Mode, and continued at the
   destination scene's safe entrance.

### Fastest Development Workflow

Keep the Unity Editor open and use the Test Runner while actively developing. The current Edit Mode suite itself completes in well under one second; most command-line test time comes from launching and initializing a new Unity Editor process.

Use project Editor tools for repeated technical setup and Inspector wiring.
They may create clearly labeled, replaceable test objects and neutral functional
scaffolding when that speeds up implementation or verification. They must not
invent permanent district layouts, buildings, roads, decoration, landmarks, or
other authored visual composition. Learn a Unity concept by building it
manually once; automate technical repetition unless deliberate practice is the
goal.

Use command-line batch testing for milestone verification, CI, or situations where the Editor is already closed. Avoid adding `-quit` to the test command because the Unity 6.5 test runner controls its own shutdown after writing the result file.

## Windows Command Line

Close the interactive Unity Editor before running the project in batch mode, then run:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.5.3f1\Editor\Unity.exe' `
  -batchmode `
  -projectPath $PWD `
  -runTests `
  -testPlatform EditMode `
  -testResults TestResults\editmode-results.xml `
  -logFile TestResults\editmode.log
```

`TestResults/` is ignored by Git. Open the XML result file for the test count and failures, or inspect the log if Unity exits before producing results.
