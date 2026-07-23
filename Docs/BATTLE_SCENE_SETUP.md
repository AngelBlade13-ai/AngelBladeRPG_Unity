# Separate Battle Scene Setup

This guide builds and verifies the first dedicated turn-based battle scene in Unity `6000.5.3f1`. The placeholder composition and repeated UI wiring are automated; combat behavior remains in readable runtime classes and is verified through Edit Mode and Play Mode tests.

## How The Battle Loop Works

1. `BattleEncounterInteractable2D` creates a monster and starts combat in `GameSession`.
2. `BattleReturnStore` records the exploration scene and named return spawn.
3. Unity loads `BattleScene`.
4. `BattleSceneController` collects commands in active formation order and
   submits them to the tested party round resolver.
5. Victory grants rewards once, escape grants none, and defeat returns to the title scene.
6. Victory or escape consumes the recorded destination and returns the Player to `TownAfterBattleSpawn`.

## Build Or Repair The Battle Scene

1. Leave Play Mode and save any current scene changes.
2. Choose **Tools > AngelBlade RPG > Battle > Repair Battle Scene Interface**.
3. Wait for Unity to create or repair `BattleScene`, then return to the scene
   that was open before the command ran.

The command creates:

- `BattleScene` with a `320 x 180` Canvas, compact party/enemy HP and MP lists,
  actor/target markers, combatant placeholders, expanded battle log, target
  controls, and command buttons.
- `BattleController` with all UI references and button callbacks assigned.
- A Scene List entry for `BattleScene`.

The command does not create encounters, return points, layouts, or decoration
in any exploration scene.

## Party Command UI Repair

After pulling the party-command checkpoint:

1. Leave Play Mode and save the currently open scene.
2. Choose **Tools > AngelBlade RPG > Battle > Repair Battle Scene Interface**.
3. Because `BattleScene` already exists, confirm the tool reports that it
   repaired the interface without overwriting the scene.
4. Confirm the previously open scene was not modified.

The repair adds and wires `CommandPromptText`, `PreviousTargetButton`,
`NextTargetButton`, and `AbilityButton`, expands the battle log, and resizes the
party/enemy status areas. No manual Inspector wiring is required.

### Party Command Smoke Test

- [x] Start from `MainGameScene`, create a protagonist, and enter a battle.
- [x] Confirm the party status begins with `>` on the acting protagonist.
- [x] Confirm the selected enemy begins with `*` and appears in the command
  prompt.
- [x] Confirm target arrows are disabled when only one enemy is alive.
- [x] Confirm Attack and Defend resolve through the new party round flow.
- [x] Confirm Ability changes to Confirm and the prompt shows Power Strike's
  `4 MP` cost and current target.
- [x] Confirm ally/enemy target cycling remains covered by Edit Mode tests.
- [x] Confirm Confirm spends MP, resolves Power Strike, and returns the button
  label to Ability for the next round.
- [x] Confirm HP/MP and the battle log update after each resolved round.
- [x] Confirm victory, defeat, escape, and return behavior still work.
- [x] Confirm text and controls do not overlap at `320 x 180`, `640 x 360`, and
  `1280 x 720`.

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
