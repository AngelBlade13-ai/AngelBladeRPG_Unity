# Running Core Gameplay Tests

The core gameplay tests are Unity Edit Mode tests. They exercise plain gameplay classes without entering Play Mode or opening `MainGameScene`.

## Unity 6.5 Editor

1. Open the project in Unity `6000.5.3f1`.
2. Wait for script compilation to finish.
3. Open `Window > General > Test Runner`.
4. Select the Edit Mode tests.
5. Click `Run All`.

The test assembly is `AngelBladeRPG.EditModeTests` under `Assets/Tests/EditMode`.

Verified on July 14, 2026 with Unity `6000.5.3f1`: 18 passed, 0 failed.

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
