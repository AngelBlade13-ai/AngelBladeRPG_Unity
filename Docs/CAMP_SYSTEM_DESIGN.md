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

- Discover one designated campsite on the road to Cherry Blossom as a required
  story beat after the post-Goblin Suncrest Hollow scenes.
- Load one reusable camp scene rather than building a separate camp map for
  every region.
- Restore the party through the full-rest rules below.
- Present the two authored companion moments below.
- Allow party formation, equipment review, shared-inventory review, item use,
  and companion interaction.
- Leave camp and return to the exact exploration scene and position.
- Preserve camp conversations, rest state, and return information in save data.

The demo does not need cooking, camp decoration, elaborate schedules, a day and
night simulation, romance systems, or a large cinematic event pipeline.

## Demo Access And Return Rules

- Campsite ID: `camp_cherry_road`.
- The first visit is mandatory so every demo player learns the camp loop.
- After discovery, the campsite remains available from its physical road access
  point. The demo does not provide an anywhere-on-the-map camp command.
- Leaving camp returns the party to the exact road scene and access position.
- The route back to Suncrest Hollow remains open; lack of supplies cannot create
  an unrecoverable progression state.
- Entering or leaving camp does not advance quest objectives unless an explicit
  camp-event condition says otherwise.

Designated campsites remain the default full-game access model unless later
testing establishes a strong reason to allow camping from most safe locations.

## Demo Rest Rules

The item `item_camp_ration`, displayed as **Camp Ration**, pays for a full rest.

- The first tutorial full rest is free.
- The party receives two Camp Rations in its Cherry Blossom departure supplies.
- Each later full rest consumes one Camp Ration after confirmation.
- Cancelled, rejected, or failed rest attempts consume nothing.
- Suncrest Hollow's inn or market can sell additional rations once the demo
  economy is implemented. A small number may also be found during exploration.

A full rest:

- Restores HP and MP for every available recruited party member, including
  benched characters.
- Revives incapacitated available party members.
- Clears temporary combat conditions such as poison, burn, buffs, debuffs, and
  guarding state.
- Does not change permanent, equipment, quest, relationship, or story state.
- Does not reset dedicated quest encounters, ambient encounter step progress,
  treasure, pickups, or one-time rewards.
- Does not advance a day, clock, schedule, or timed quest in the demo.

If the party is already fully recovered, the UI should communicate that before
accepting a ration. Camp conversations remain available without spending or
repeating a rest.

## Demo Camp Actions

- **Rest:** perform the confirmed full-rest action.
- **Party:** arrange the four active slots and benched members.
- **Equipment:** inspect and change equipment and use appropriate inventory
  items.
- **Talk:** interact with recruited companions and available camp moments.
- **Leave:** return to the recorded road position.

Job assignment and job-node purchases remain Guild Hall services. A future
respec service, if implemented after the demo, also belongs there. Camp does not
provide shopping, selling, smithing, quest acceptance, quest turn-in,
recruitment, or faction progression.

## Demo Companion Moments

### First Fire

- Stable event ID: `camp_event_demo_first_fire`.
- Triggers once after the free tutorial rest.
- Uses a short group scene centered on Damari and Enora's marriage-like running
  banter while Iona and the protagonist react.
- Lysander remains slightly reserved because he is still new to the group's
  established rhythm.

### Why This Party

- Stable event ID: `camp_event_demo_lysander_stays`.
- Becomes available after First Fire and is started by talking to Lysander.
- Lets him explain a little of why he has chosen to remain with this particular
  party without revealing his full history.
- Remains available on later camp visits if the player leaves without viewing
  it, then resolves exactly once.

Camp displays all recruited, available companions even when they are not in the
four active battle slots. Event availability depends on roster and story state,
not active-party placement or repeated resting.

## Full-Game Expansion

After the demo validates the core loop, Ember Camp may grow to support:

- Region-aware visual variants built from reusable camp layouts and data.
- Larger companion event chains and relationship-dependent group scenes.
- Meals or preparation bonuses if they create meaningful choices without
  causing unrecoverable resource shortages.
- Formation presets and catch-up support for benched characters.
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

## Test Requirements

- The first tutorial rest is free exactly once and does not consume a ration.
- Each later confirmed rest consumes exactly one ration.
- Cancelling a rest or attempting one without supplies consumes nothing.
- Full rest restores and revives every available recruited member, including
  benched characters, without restoring permanently unavailable characters.
- Temporary conditions clear while equipment, quests, relationships, encounter
  steps, pickups, and one-time world state remain unchanged.
- First Fire resolves once after the tutorial rest.
- Why This Party remains available until viewed and then resolves once.
- Changing active formation cannot hide or replay camp events.
- Leaving camp restores the recorded road scene and position.
- Save/load cannot duplicate rations, rests, event rewards, or event completion.

## Decisions Still Open

- Exact Camp Ration price, exploration quantity, icon, and inventory rules.
- Exact campsite road coordinates, return marker, visual layout, and props.
- Final First Fire and Why This Party dialogue.
- Whether full-game camps eventually gain regional layouts, meals, time
  advancement, or broader safe-location access.

The remaining demo values should be resolved with the item economy, map layout,
and dialogue content budgets.
