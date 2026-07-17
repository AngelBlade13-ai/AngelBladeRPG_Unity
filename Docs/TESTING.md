# Running Core Gameplay Tests

The core gameplay tests are Unity Edit Mode tests. They exercise plain gameplay classes without entering Play Mode or opening `MainGameScene`.

## Unity 6.5 Editor

1. Open the project in Unity `6000.5.3f1`.
2. Wait for script compilation to finish.
3. Open `Window > General > Test Runner`.
4. Select the Edit Mode tests.
5. Click `Run All`.

The test assembly is `AngelBladeRPG.EditModeTests` under `Assets/Tests/EditMode`.

Verified on July 17, 2026 with Unity `6000.5.3f1`: 274 passed, 0 failed.

The current suite includes 30 core gameplay tests, five pixel-world movement and camera tests, five temporary direction-indicator tests, four walkable-town foundation tests, seven interaction tests, eight door-transition tests, 19 battle-scene tests, 13 party-command selection tests, 11 core-ability tests, 63 job, affinity, progression, playable-character, party-roster, and party-management tests, 11 runtime-party targeting tests, 13 party-round resolver tests, 10 authored party-member tests, five speed-based turn-order tests, nine legacy speed-resolved battle-round tests, five bond and roster-history tests, nine shared combat-stat tests, 18 reusable monster-definition tests, 14 enemy encounter and layout tests, and 15 structured combat-action tests.

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

### Fastest Development Workflow

Keep the Unity Editor open and use the Test Runner while actively developing. The current Edit Mode suite itself completes in well under one second; most command-line test time comes from launching and initializing a new Unity Editor process.

Use project Editor builders for repeated scene creation and Inspector wiring.
After a builder runs, the manual pass should be limited to visual composition,
interaction feel, and a short end-to-end smoke test. Learn a Unity concept by
building it manually once; automate repeats unless deliberate practice is the
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
