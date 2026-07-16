# Demo Story And Lore Reference

This document is the authoritative narrative boundary for the public demo. It
covers Town Square and the first two traveled regions only. It should guide
Milestone 14 planning, dialogue, environment art, encounters, and demo content
without requiring full-game lore.

## Narrative Guardrails

- The broader game leans toward melancholy dark fantasy, but the demo begins as
  a grounded fantasy adventure.
- Found-family warmth should already be visible in companion dialogue and
  banter, even before deeper relationships and histories are explored.
- Grassland must contain no corruption, supernatural warning, ominous
  foreshadowing, or suggestion that its goblin trouble has larger implications.
- Unease begins only in Cherry Blossom and remains subtle. Atmosphere and NPC
  caution may suggest that something feels wrong, but nobody knows why.
- The true cause of the world's decay, later bosses, endgame material, and the
  planned mid-story character loss are outside the demo and must not be exposed
  or built into demo-critical content.

## Player-Created Protagonist

The main character is created by the player and is not Iona, Damari, Enora, or
Lysander. Those four are authored companions with their own stable IDs,
personalities, and job affinities.

- Character creation should eventually provide a limited set of readable pixel
  character options inspired by the scope of games such as `Stardew Valley` and
  `Terraria`, without copying their assets or exact interfaces.
- The protagonist needs a stable save-data identity separate from their chosen
  display name and appearance.
- Cosmetic choices and exact customization categories are still open Milestone
  14 decisions. No final art or menu should be produced around an assumed list
  yet.
- The protagonist participates in the active party. Whether they count toward
  the previously estimated total roster size is still an explicit design
  decision.

## Demo Location Sequence

The demo's world scope follows this order:

```text
Town Square hub -> Grassland -> Cherry Blossom -> demo progression gate
```

The exact opening objective, return trips, quest structure, and final demo scene
remain to be defined. This sequence describes narrative progression, not a
requirement that every location be one Unity scene.

## Town Square

Town Square is the player's home base and should be the liveliest, most vibrant
location in the game.

- The hub contains a market, smithy, and guild or quest-board hall across a few
  interconnected areas. These are parts of the hub, not separate travel
  destinations.
- NPC activity and environmental detail should make the town feel inhabited and
  welcoming.
- A few optional, easy-to-miss details may suggest that something is slightly
  off, such as a plant wilting unexpectedly or a well running a little low.
- NPCs must not comment on these details, and the details must not affect or
  block gameplay.
- Ember Camp is a later mobile rest-point concept and is excluded from the demo.

## Region 1: Grassland

Grassland is the first region reached from Town Square. It is vibrant, healthy,
ordinary, and intentionally low stakes.

- The environment contains no corruption or decay.
- The regional boss is a troublesome goblin acting as a bandit-style nuisance.
- The encounter should read as a conventional first boss with no supernatural
  meaning and no connection to a larger threat.
- No foreshadowing that belongs to the later story may be moved into this
  region.

## Region 2: Cherry Blossom

Cherry Blossom is the second traveled region. It remains beautiful, vibrant,
and physically pristine during the demo.

- A small nomadic settlement lives in the region.
- Settlement residents may express unusual caution, general unease, or the
  sense that something is not right.
- No resident knows or states the cause. Dialogue must not confirm an
  explanation that the player has not earned.
- Services are intentionally more limited than in Town Square.
- The regional boss serves both as this region's story beat and as the gate
  preventing travel farther into the full game.
- The boss's identity, the exact reason it blocks progress, and everything
  beyond the gate remain undefined for this milestone.
- A later version of this region may change visibly, but that alternate state is
  excluded from the demo.

## Demo Companion Tone

The existing short profiles for Iona (`pc_01`), Damari (`pc_02`), Enora
(`pc_03`), and Lysander (`pc_04`) in `JOB_CLASS_SYSTEM.md` are sufficient for
early dialogue and combat banter.

- Dialogue should establish contrasting personalities and growing
  found-family chemistry without requiring full backstory exposition.
- No permanent companion loss or major late-story turn occurs in the demo.
- The exact companions present at each demo beat and when they join remain open
  planning decisions.

## Explicit Demo Exclusions

- The true cause of the world's decay or any confirmed explanation for it.
- Additional later bosses, regions, factions, or endgame content.
- The planned permanent roster removal and its story circumstances.
- Ember Camp.
- Cherry Blossom's future changed state.
- Any content beyond the Cherry Blossom progression gate.

## Milestone 14 Decisions Still Needed

- Public game title and developer or publisher identity.
- One-paragraph player promise and feature hierarchy.
- The demo opening quest, immediate objective, climax, and closing beat.
- Cherry Blossom boss identity and its demo-safe motivation.
- Which companions join the protagonist, and at what points.
- Exact protagonist customization categories and art budget.
- Demo jobs, abilities, items, services, and encounter policy.
- Target playtime, map and scene list, dialogue budget, and full asset list.

Later lore should remain deferred until a milestone needs it. New story input
should be added here only when it changes the public demo.
