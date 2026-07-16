# Gameplay Direction

This document defines the intended player experience so future systems support the same game rather than extending the temporary menu-driven prototype.

## Product Identity

`AngelBladeRPG_Unity` is intended to be a 2D pixel-fantasy RPG.

- Exploration uses top-down movement inspired by `Stardew Valley`.
- Towns, interiors, paths, and world areas are physical spaces built from Tilemaps and sprites.
- The player walks, collides with the environment, approaches characters and objects, and presses an interaction control.
- Combat is classic turn-based RPG combat presented in a separate scene, inspired by older `Final Fantasy` games.
- The final game should not feel like a sequence of full-screen point-and-click menus.

The references describe interaction structure and presentation direction. They do not require copying either game's art, characters, rules, or content.

The player controls a custom-created protagonist, not one of the authored
companions. Character creation will eventually offer a deliberately limited set
of pixel-character appearance choices. The protagonist's name and appearance
must remain separate from stable save identity and companion IDs.

## Commercial Direction

This project is intended to become a complete commercial PC game with an
eventual Steam release. The next major product gate is a polished vertical-slice
demo that represents the opening experience of the real game.

- The demo must combine exploration, story, party/job choices, combat,
  progression, saving, settings, and presentation in one complete playable arc.
- Prototype scenes and placeholder assets are development tools, not the final
  public identity.
- Systems should be built deeply enough to support the demo, then scaled only
  after the vertical slice validates the player experience and production
  pipeline.
- Store claims, screenshots, supported platforms, controller support, and
  accessibility claims must always match the build players can actually use.

The authoritative milestone and release gates are recorded in `ROADMAP.md`.

## Demo World Direction

The vertical-slice demo progresses through Suncrest Hollow, Grassland, and Cherry
Blossom. Suncrest Hollow is the lively home hub; Grassland is an ordinary, healthy
first adventure region; Cherry Blossom remains beautiful but introduces quiet
unease and a gate into later full-game content.

The three main quests form a guided story sequence, while travel and optional
quest order remain flexible. Town services, turn-ins, character moments, and
changing NPC dialogue should encourage regular hub visits. Defeating the
Grassland goblin boss is the required gate to Cherry Blossom after the three
main quests make that encounter available; side-quest completion and elapsed
playtime are not progression locks. The expected first playthrough is at least
90 minutes, while a practiced player may complete the critical path
considerably faster.

Story tone, location-specific boundaries, companion guidance, and explicit
demo exclusions are recorded in `DEMO_STORY_REFERENCE.md`.

The opening caravan quest, staged tutorial battle, initial party formation,
three-main-quest unlock, and Lysander recruitment are recorded in
`DEMO_QUEST_FLOW.md`.

Grassland uses a hybrid encounter model: authored quest battles coexist with
ambient generic-creature battles triggered after a configured number of
eligible exploration steps. Field objectives, encounter boundaries, and reward
shapes for the three main quests are recorded in
`DEMO_MAIN_QUEST_CONTENT.md`.

Grassland enemy roles, encounter groups, scout-intelligence advantages, and the
regional Goblin Boss action pattern are recorded in
`GRASSLAND_COMBAT_DESIGN.md`.

The post-Goblin celebration, Cherry Blossom caravan request, Ember Camp
introduction, sacred-tree disturbance, Great Stag boss, and unresolved demo
ending are recorded in `DEMO_CHERRY_BLOSSOM_ARC.md`.

The Great Stag's symbolic direction, marked-charge battle rhythm, Panicked
phase, and nonlethal subdued outcome are recorded in
`CHERRY_BLOSSOM_COMBAT_DESIGN.md`.

The four optional quests supporting hub returns, Grassland exploration, party
chemistry, and Cherry Blossom unease are recorded in `DEMO_SIDE_QUESTS.md`.

Ember Camp is a reusable expedition rest and companion space. It must preserve
Suncrest Hollow's role by excluding general quest turn-ins, commerce, smithing,
recruitment, and major hub progression. Demo and full-game boundaries for this
system are recorded in `CAMP_SYSTEM_DESIGN.md`.

All 12 catalog jobs are playable in the demo by every party member. The demo
offers a meaningful early portion of each tree, with a data-driven progression
limit authored per job instead of hiding or disabling jobs. Affinity remains
guidance rather than an access requirement. The detailed boundary and open
progression questions are recorded in `DEMO_JOB_TREE_SCOPE.md`.

## Core Player Loop

```text
Title
  -> Character creation or Continue
  -> Spawn in an exploration scene
  -> Walk through towns, interiors, and world areas
  -> Interact with NPCs, doors, objects, and encounter triggers
  -> Transition into a separate turn-based battle scene
  -> Resolve victory, defeat, or escape
  -> Return to the correct exploration scene and position
```

## Scene Responsibilities

Names are provisional, but responsibilities should remain separate.

### Title And Setup

- Shows title, New Game, and eventually Continue.
- Handles character creation before entering the world.
- The current title and character-creation panels can continue serving this role while their visual design evolves.

### Exploration Scene

- Owns Tilemaps, environmental collision, doors, NPC placement, and encounter triggers.
- Spawns and moves the visible player character.
- Uses a following pixel-perfect camera.
- Presents status through a compact HUD or pause menu instead of replacing the world with `TownPanel`.
- Records the return scene and player position before combat.

### Battle Scene

- Receives active-party and encounter data from persistent game state.
- Presents party members and enemies in a dedicated combat composition.
- Uses turn-based commands and the tested combat/reward rules as its initial foundation.
- Returns a battle outcome to persistent game state.
- Loads the recorded exploration scene and restores the appropriate return position after victory or escape.

## What Existing Work Still Provides

- `PlayerData` remains the starting player-stat and progression model.
- `MonsterData` remains the starting encounter-combatant model.
- `SimpleBattleSystem` provides tested initial damage rules.
- `GameSession` provides tested player, encounter, reward, victory, and defeat state.
- Character-name validation, leveling, rewards, and duplicate-reward prevention remain valid.
- Edit Mode tests protect these rules while presentation and scenes change.

## Replaced Prototype Elements

- `TownPanel` was replaced by the walkable `TownScene`.
- `BattlePanel` was replaced by the dedicated `BattleScene`.
- `GameManager` now handles only title and character-creation setup flow.
- Visible prototype encounters now supply monster and return data to the battle transition.

The tested gameplay rules survived the presentation change while the obsolete scene objects were removed.

## Near-Term Technical Boundaries

- Movement code should not contain town-specific, story-specific, or battle-specific rules.
- Interactable objects should use a reusable contract rather than direct references to one NPC or door.
- Persistent gameplay state must survive scene changes without depending on scene UI objects.
- Battle scene code should consume encounter data rather than constructing a hard-coded Goblin inside UI code.
- World return information should be explicit data, not inferred from whichever scene happens to be open.
- Pixel-art resolution, sprite pixels-per-unit, camera behavior, and animation dimensions should be chosen consistently before producing large quantities of art.

## Decisions Still Open

- Four-direction versus eight-direction movement animation.
- One exploration scene per location versus larger connected maps.
- Whether battle backgrounds are authored per region or assembled from reusable layers.
- Exact battle camera, party layout, command UI, and transition effect.

These decisions should be made when their milestone begins. They do not block the pixel-world movement foundation.
