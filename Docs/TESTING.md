# Running Core Gameplay Tests

The core gameplay tests are Unity Edit Mode tests. They exercise plain gameplay classes without entering Play Mode or opening `MainGameScene`.

## Unity 6.5 Editor

1. Open the project in Unity `6000.5.3f1`.
2. Wait for script compilation to finish.
3. Open `Window > General > Test Runner`.
4. Select the Edit Mode tests.
5. Click `Run All`.

The test assembly is `AngelBladeRPG.EditModeTests` under `Assets/Tests/EditMode`.

Verified on July 16, 2026 with Unity `6000.5.3f1`: 138 passed, 0 failed.

The current suite includes 18 core gameplay tests, five pixel-world movement and camera tests, five temporary direction-indicator tests, four walkable-town foundation tests, seven interaction tests, eight door-transition tests, 13 battle-scene tests, 15 job, affinity, and party-roster tests, 10 authored party-member tests, five speed-based turn-order tests, nine speed-resolved battle-round tests, five bond and roster-history tests, nine shared combat-stat tests, 10 reusable monster-definition tests, and 15 structured combat-action tests.

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
