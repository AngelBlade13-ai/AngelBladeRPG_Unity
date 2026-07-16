# Demo Quest Flow: Opening Through Goblin Boss

This document defines the demo's opening, tutorial encounter, initial party
formation, early quest progression, and Lysander recruitment. It builds on
`DEMO_STORY_REFERENCE.md`, `CAMP_SYSTEM_DESIGN.md`, and
`JOB_CLASS_SYSTEM.md`.

## Protagonist Framing

The player-created protagonist is a complete newcomer to Town Square.

- Early NPCs treat the protagonist as unknown and newly arrived, not as a
  returning resident or established hero.
- The opening should introduce Town Square through the protagonist's first
  impressions without requiring a long exposition sequence.
- The protagonist's reason for traveling to Town Square may be developed later,
  but dialogue in this slice must not imply an unrecorded local history.

## Opening Quest: The Delayed Caravan

At the beginning of the game, only one quest is available: a caravan carrying
goods for Town Square is overdue, and someone is needed to investigate.

1. The protagonist accepts the caravan quest in Town Square.
2. The player travels into Grassland.
3. The caravan is found under attack by goblins.
4. The goblins notice the protagonist and initiate the tutorial encounter.
5. This encounter is scripted, mandatory, and cannot be escaped.

Additional main and side quests become available after this opening sequence.

## Tutorial Encounter

The tutorial encounter introduces mechanics through authored battle states and
escalating stakes rather than presenting every command at once.

### Stage 1: Basic Goblins

- The protagonist fights the first wave alone.
- The available command presentation teaches the basic solo combat loop.
- A controlled damage checkpoint leaves the protagonist at low HP without
  allowing the scripted tutorial damage to defeat them.

### Stage 2: Iona Joins

- Iona (`pc_01`) enters when healing has an immediate purpose.
- Her arrival introduces healing and magic mechanics.
- The protagonist and Iona then face the tutorial Hobgoblin.

### Stage 3: Hobgoblin Pressure

- The Hobgoblin is a tutorial mini-boss, not the regional Goblin Boss.
- It focuses pressure on Iona and brings her to low HP without using scripted
  damage that can accidentally defeat her.
- The encounter is authored so the protagonist and Iona cannot complete this
  stage before the reinforcement beat. This should use an explicit encounter
  checkpoint or HP floor rather than misleading displayed damage or impossible
  hidden statistics.

### Stage 4: Damari And Enora Join

- Damari (`pc_02`) and Enora (`pc_03`) enter as an already-established close
  pair.
- Damari introduces taunt-style threat control and front-line protection.
- Enora demonstrates high-impact burst damage.
- The four-character party defeats the Hobgoblin and resolves the caravan
  attack.

The tutorial encounter must use its own stable encounter and combatant IDs.
`Hobgoblin` is the tutorial mini-boss; `Goblin Boss` is the later regional boss
and progression gate. Names, definitions, rewards, completion flags, and quest
conditions must never conflate the two.

## Tutorial Safety And Replay Rules

- Escape is disabled for this encounter and the UI must communicate that the
  command is unavailable.
- Scripted damage checkpoints must clamp to a safe HP floor and cannot defeat a
  combatant.
- Reinforcement stages and tutorial prompts must advance from explicit battle
  state, not from frame timing or battle-log text.
- Saving is not required during the encounter. The most recent safe checkpoint
  must reload without duplicating recruits, rewards, or quest progress.
- Tutorial explanations may be advanced quickly, but the required encounter
  itself is not skippable.
- Automated tests should cover stage order, HP floors, disabled escape,
  reinforcement triggers, and one-time completion.

## Post-Tutorial Party And Job Introduction

After defeating the Hobgoblin, the group returns to Town Square.

- The protagonist, Iona, Damari, and Enora become the standing four-character
  party.
- This return introduces job assignment and adjustment for every current party
  member.
- The job tutorial must teach the shared job-access rule without presenting
  each companion's natural affinity as a locked class.

## Main Quest Structure

Three main story quests must be completed before the regional Goblin Boss
encounter becomes available.

- The three main quests may be completed in a flexible order unless an
  individual quest has a clearly authored dependency.
- These required quests form the shortest critical path to the Goblin Boss.
- Approximately three to five side quests should support exploration, hub
  returns, rewards, and character moments.
- Side quests remain optional for story progression and are not required to
  unlock the Goblin Boss.
- Defeating the Goblin Boss unlocks progression from Grassland to Cherry
  Blossom.

The identities, quest givers, locations, and objectives of the three main
quests remain to be authored.

## Lysander Recruitment

Lysander (`pc_04`) is a grizzled, independent, and experienced adventurer who
has grown tired of unreliable parties. He joins after the protagonist has built
a small local reputation rather than joining by default.

### Unlock Rule

- Lysander unlocks after three unique quests have been completed.
- Main and side quests both count toward this threshold.
- The three required main quests satisfy the threshold by themselves, so side
  content is never required to recruit him.
- Repeatable activity, duplicate rewards, and reloading completed turn-ins must
  not increment the counter again.

### Forced Join Beat

Once the threshold is reached and Lysander has not joined, he blocks the Town
Square exit toward Grassland.

- Attempting to leave triggers his dedicated join scene before travel can
  continue.
- The join scene is mandatory and guarantees his recruitment before the Goblin
  Boss encounter.
- The exit block and join scene must be driven by persistent quest and roster
  state so save/load cannot bypass the scene or leave the exit permanently
  blocked.
- Completing the join scene clears the block exactly once.

### Mechanical Introduction

- Lysander joins at a slightly higher level than the current party so his
  established experience has immediate mechanical weight.
- The exact level offset or scaling formula must be selected during balancing;
  it should be derived from current progression rather than a fragile fixed
  level.
- His recruitment immediately introduces party management because the roster
  now contains five characters while the active battle party remains capped at
  four.
- The tutorial teaches arranging active members and benching a companion without
  implying that Lysander must be selected.

## Open Narrative Items

- Identities and locations of the three main quest givers.
- Objectives and story content of the three main quests.
- The exact NPC who authorizes or frames the Goblin Boss quest.
- What happens immediately after the Goblin Boss falls and the route to Cherry
  Blossom opens.
- The Cherry Blossom regional boss and the demo's final closing beat.
