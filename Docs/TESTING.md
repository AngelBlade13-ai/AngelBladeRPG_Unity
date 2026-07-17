# Running Core Gameplay Tests

The core gameplay tests are Unity Edit Mode tests. They exercise plain gameplay classes without entering Play Mode or opening `MainGameScene`.

## Unity 6.5 Editor

1. Open the project in Unity `6000.5.3f1`.
2. Wait for script compilation to finish.
3. Open `Window > General > Test Runner`.
4. Select the Edit Mode tests.
5. Click `Run All`.

The test assembly is `AngelBladeRPG.EditModeTests` under `Assets/Tests/EditMode`.

Verified on July 16, 2026 with Unity `6000.5.3f1`: 210 passed, 0 failed.

The current suite includes 21 core gameplay tests, five pixel-world movement and camera tests, five temporary direction-indicator tests, four walkable-town foundation tests, seven interaction tests, eight door-transition tests, 15 battle-scene tests, seven party-command selection tests, 11 core-ability tests, 40 job, affinity, progression, playable-character, and party-roster tests, 11 runtime-party targeting tests, 13 party-round resolver tests, 10 authored party-member tests, five speed-based turn-order tests, nine legacy speed-resolved battle-round tests, five bond and roster-history tests, nine shared combat-stat tests, 10 reusable monster-definition tests, and 15 structured combat-action tests.

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

### Fastest Development Workflow

Keep the Unity Editor open and use the Test Runner while actively developing. The current Edit Mode suite itself completes in well under one second; most command-line test time comes from launching and initializing a new Unity Editor process.

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
