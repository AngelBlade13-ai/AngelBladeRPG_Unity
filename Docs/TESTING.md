# Running Core Gameplay Tests

The core gameplay tests are Unity Edit Mode tests. They exercise plain gameplay classes without entering Play Mode or opening `MainGameScene`.

## Unity 6.5 Editor

1. Open the project in Unity `6000.5.3f1`.
2. Wait for script compilation to finish.
3. Open `Window > General > Test Runner`.
4. Select the Edit Mode tests.
5. Click `Run All`.

The test assembly is `AngelBladeRPG.EditModeTests` under `Assets/Tests/EditMode`.

Verified on July 16, 2026 with Unity `6000.5.3f1`: 177 passed, 0 failed.

The current suite includes 21 core gameplay tests, five pixel-world movement and camera tests, five temporary direction-indicator tests, four walkable-town foundation tests, seven interaction tests, eight door-transition tests, 13 battle-scene tests, 40 job, affinity, progression, playable-character, and party-roster tests, 11 runtime-party targeting tests, 10 authored party-member tests, five speed-based turn-order tests, nine speed-resolved battle-round tests, five bond and roster-history tests, nine shared combat-stat tests, 10 reusable monster-definition tests, and 15 structured combat-action tests.

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
