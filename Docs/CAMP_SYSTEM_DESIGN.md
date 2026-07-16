# Ember Camp System Design

This document defines the intended role and technical boundary of Ember Camp.
The system draws inspiration from party camps in character-driven RPGs, while
remaining appropriate for this game's scope and preserving Suncrest Hollow as the
primary home hub.

## Player Experience

Ember Camp is a temporary party space used between expeditions. It should feel
quieter and more intimate than Suncrest Hollow: a place to recover, check in with
companions, and see the party becoming a found family.

Camp complements the hub rather than replacing it.

| Ember Camp | Suncrest Hollow |
| --- | --- |
| Recover during an expedition | Accept and turn in most quests |
| View camp-specific companion moments | Use market and smithy services |
| Review the current party and equipment | Recruit characters and advance guild or faction content |
| Prepare to continue traveling | Unlock major progression and town story beats |
| Return to the exact expedition position | Provide the broadest set of services and NPCs |

Portable shops, smithing, guild services, and general quest turn-ins do not
belong in camp. Keeping those functions hub-exclusive gives players practical
and narrative reasons to return to Suncrest Hollow.

## Demo Scope

The demo should contain a compact but genuine version of Ember Camp:

- Enter camp from approved safe conditions or designated camp access points.
- Load one reusable camp scene rather than building a separate camp map for
  every region.
- Restore the party through a clearly communicated rest action.
- Present a small authored set of companion conversations or banter unlocked by
  demo progress.
- Allow basic party and equipment review if those screens are available by the
  time camp is implemented.
- Leave camp and return to the exact exploration scene and position.
- Preserve camp conversations, rest state, and return information in save data.

The demo does not need cooking, camp decoration, elaborate schedules, a day and
night simulation, romance systems, or a large cinematic event pipeline.

## Full-Game Expansion

After the demo validates the core loop, Ember Camp may grow to support:

- Region-aware visual variants built from reusable camp layouts and data.
- Larger companion event chains and relationship-dependent group scenes.
- Camp supplies, meals, or preparation bonuses if they create meaningful
  choices without causing unrecoverable resource shortages.
- Party swapping and catch-up support for benched characters.
- Story visitors or special events driven by explicit quest and world-state
  conditions.
- Changes to camp availability during authored story sequences.

These are expansion points, not promises that every feature must ship.

## Hub Protection Rules

- Camp must never become the optimal place to perform every between-battle task.
- Most quest acceptance, turn-ins, purchasing, selling, equipment upgrades,
  recruitment, and faction progression remain in Suncrest Hollow or other
  settlements.
- Camp conversations should deepen party relationships; hub conversations
  should also react to quest and world progress so both spaces remain socially
  valuable.
- The game may suggest returning to Suncrest Hollow when new services or events are
  available, but it should not interrupt exploration solely to force a visit.
- Fast travel, when designed, must not erase the value of routes, camp access,
  or hub return decisions.

## Technical Boundary

Camp rules should remain independent from scene presentation.

- A persistent camp state records rest-related state and consumed event IDs.
- A rest service applies healing and resource recovery through tested gameplay
  rules rather than directly editing UI objects.
- Camp event definitions use stable IDs and explicit unlock conditions based on
  quest, companion, and world state.
- A camp transition records the source scene and exact return position using the
  same explicit return-context approach as battle transitions.
- The camp scene reads party and region context from persistent state and must
  not construct hard-coded companion records.
- Save data records enough state to prevent one-time conversations or rewards
  from replaying incorrectly.

## Decisions Still Open

- Whether camp is entered only at designated campsites or from most safe outdoor
  locations.
- Whether resting is free, consumes supplies, or uses another limitation.
- Which HP, MP, status effects, and temporary bonuses a rest restores or clears.
- Whether resting advances a lightweight time value or respawns encounters.
- Which party, equipment, job, and inventory actions are available in camp.
- The exact number and triggers of companion camp events in the demo.
- Whether the Cherry Blossom settlement uses Ember Camp, its own lodging, or
  both.

These decisions should be resolved during Milestone 14 before content and UI are
budgeted around the camp.
