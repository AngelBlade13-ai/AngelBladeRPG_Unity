# Separate Battle Scene Setup

This guide builds and verifies the first dedicated turn-based battle scene in Unity `6000.5.3f1`. The placeholder composition and repeated UI wiring are automated; combat behavior remains in readable runtime classes and is verified through Edit Mode and Play Mode tests.

## How The Battle Loop Works

1. `BattleEncounterInteractable2D` creates a monster and starts combat in `GameSession`.
2. `BattleReturnStore` records the exploration scene and named return spawn.
3. Unity loads `BattleScene`.
4. `BattleSceneController` presents combat and calls the tested `SimpleBattleSystem` and `GameSession` rules.
5. Victory grants rewards once, escape grants none, and defeat returns to the title scene.
6. Victory or escape consumes the recorded destination and returns the Player to `TownAfterBattleSpawn`.

## Build The Prototype

1. Open `TownScene` outside Play Mode.
2. Save any current scene changes.
3. Choose **Tools > AngelBlade RPG > Build Placeholder Battle Scene**.
4. Wait for Unity to create the scene and return to `TownScene`.

The command creates:

- `BattleScene` with a `320 x 180` Canvas, status text, combatant placeholders, battle log, and command buttons.
- `BattleController` with all UI references and button callbacks assigned.
- `BattleTestEncounter`, a red Goblin encounter for victory and escape tests.
- `BattleDefeatTestEncounter`, a purple Ogre encounter that defeats the starting hero.
- `TownAfterBattleSpawn`, used after victory or escape.
- A Scene List entry for `BattleScene`.

The encounter choice is a deterministic prototype. It does not permanently decide whether the finished game uses visible, random, or scripted encounters.

## Escape Check

- [x] Start from `MainGameScene`, create a character, and enter town.
- [x] Interact with the red Goblin encounter using `E`.
- [x] Confirm `BattleScene` shows the hero, Goblin, HP, battle log, Attack, and Escape.
- [x] Select **Escape**, then **Return**.
- [x] Confirm the Player returns at `TownAfterBattleSpawn`.
- [x] Confirm gold and XP did not increase.

## Victory Check

- [x] Interact with the red Goblin encounter again.
- [x] Select **Attack** until the Goblin reaches zero HP.
- [x] Confirm each turn shows both attacks and updates HP.
- [x] Confirm victory reports `15 XP` and `10 gold`.
- [x] Confirm Attack and Escape are replaced by Return after victory.
- [x] Return to town and confirm the HUD shows `XP 15/50` and `Gold 10`.
- [x] Confirm rewards are not granted a second time.

## Defeat Check

- [x] Interact with the purple Ogre encounter.
- [x] Select **Attack** until the hero reaches zero HP.
- [x] Confirm defeat replaces commands with **Return to Title**.
- [x] Confirm continuing loads `MainGameScene` rather than returning to town.
- [x] Start a new game and confirm the new hero has full starting HP.

## General Check

- [x] The battle UI fits at `320 x 180`, `640 x 360`, and `1280 x 720`.
- [x] No text overlaps the command buttons or leaves its panel.
- [x] The town sign and interior doors still work after a battle return.
- [x] The Console remains free of errors throughout the full flow.

Save all scene changes outside Play Mode.
