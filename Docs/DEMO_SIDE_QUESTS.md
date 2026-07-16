# Demo Optional Side Quests

This document defines the demo's four optional side quests. They support the
90-minute-plus first-playthrough target, hub returns, exploration, party
chemistry, and Lysander's reputation threshold without gating the critical
path.

## Shared Rules

- All four quests are optional and use stable quest IDs.
- The first three count as unique completed quests for Lysander's reputation
  threshold. The Cherry Blossom quest occurs after his required join scene.
- No side quest unlocks the Goblin Boss, Cherry Blossom, or the demo ending.
- Side quest state and rewards resolve exactly once across save/load.
- Reuse dialogue, world-interaction, pickup, quest-journal, ordinary battle, and
  reward systems already required elsewhere in the demo.
- Do not add escort AI, real-time failure, crafting minigames, fishing, bespoke
  puzzles, or a separate reputation system for these quests.
- Working NPC names can be selected when the final demo NPC list and culture
  naming guides are authored.

## Side Quest 1: A Taste Of Home

- Stable quest ID: `side_suncrest_taste_of_home`.
- Origin: the Suncrest Inn after the delayed-caravan tutorial and job-system
  introduction.
- Purpose: encourage a Grassland excursion, return the player to the inn, and
  create an early found-family party moment.

### Objective

The innkeeper needs three bundles of Meadow Sage for a familiar local meal.

- Place the three quest pickups along ordinary Grassland exploration routes.
- Picking them up is a quest interaction, not a general-purpose harvesting
  system.
- No dedicated battle is required, though ambient step encounters remain active.
- Return all three bundles to the Suncrest Inn.

### Payoff And Reward

- The innkeeper prepares a meal and the current party shares a short scene.
- Banter should establish comfort and contrast among Iona, Damari, and Enora
  without requiring major backstory.
- Reward several **Suncrest Supper** recovery consumables and one free inn rest.
- Exact healing values and item quantities remain economy-balance decisions.

## Side Quest 2: The Roadside Chimes

- Stable quest ID: `side_suncrest_roadside_chimes`.
- Origin: a caretaker at the Sunwell Shrine after the opening tutorial.
- Purpose: introduce the shrine, reinforce grounded local customs, and reward
  attentive interaction along the Grassland road.

### Objective

Ordinary weather damaged three small roadside chimes maintained by the shrine.

- Locate and reset all three chimes through simple world interactions.
- Each chime has a stable state and a distinct approximate location clue.
- The task contains no supernatural event, sacred prophecy, or hint about the
  later world problem.
- Dedicated combat is not required; ambient encounters remain possible.

### Payoff And Reward

- Return to the Sunwell Shrine after restoring the chimes.
- Reward restorative consumables and one free shrine recovery service.
- The quest must not comment on the subtle Sunroot Grove decay details.

## Side Quest 3: A Painter's View

- Stable quest ID: `side_grassland_painters_view`.
- Origin: a traveling painter encountered in Grassland after the opening
  tutorial.
- Purpose: celebrate Grassland as beautiful, healthy, and free of ominous
  foreshadowing while rewarding off-path exploration.

### Objective

The painter asks the party to locate three views suitable for a landscape
series:

- A wildflower ridge.
- An old stone bridge.
- A quiet pond.

Each viewpoint uses a stable interaction point and a short party observation.
The interactions record discoveries; they do not require a photography,
drawing, or screenshot minigame.

### Payoff And Reward

- Report all three viewpoints to the painter.
- Reward gold and **Traveler's Tonic** recovery consumables.
- The completed paintings may later appear as inexpensive environmental props
  in the Suncrest Inn if the art budget allows, but this is not required for
  quest completion.

## Side Quest 4: Where The Herd Won't Graze

- Stable quest ID: `side_cherry_herd_wont_graze`.
- Origin: a herder in Cherry Blossom's nomadic settlement.
- Purpose: provide a small settlement problem and reinforce unease through
  animal behavior rather than exposition.

### Objective

Three grazing animals have clustered outside their usual range and refuse to
cross a petal-covered part of the settlement outskirts.

- Locate all three animals through tracks and fixed world interactions.
- Do not implement follow or escort AI. Finding the final animal advances them
  safely back to the settlement through quest state and presentation.
- Petals continue falling in still air nearby.
- The animals are frightened but unharmed.
- No battle is required, and neither the herder nor party discovers a cause.

### Payoff And Reward

- The herder admits that the behavior is unusual but offers no explanation.
- Reward restorative settlement tea and **Nomad's Woven Cord**, a universal
  accessory intended to provide a modest speed or evasion benefit.
- Exact item statistics and quantities remain economy-balance decisions.

## Content Budget

These quests require:

- Two Suncrest Hollow quest-giver NPCs: the innkeeper and shrine caretaker.
- One traveling painter in Grassland.
- One Cherry Blossom herder.
- Three Meadow Sage pickups.
- Three roadside chime interactions.
- Three Grassland viewpoints.
- Three fixed grazing-animal interactions and track details.
- Four short turn-in scenes, including one party meal scene.
- Three side-quest consumable definitions and one accessory definition, subject
  to consolidation with the final demo item list.

## Remaining Decisions

- Final NPC names, appearances, and dialogue.
- Exact placement of all pickups and interaction points.
- Final consumable quantities, recovery values, accessory statistics, XP, and
  gold rewards.
- Whether completed landscape paintings appear in the inn within the demo art
  budget.
