# Demo Main Quest Field Content

This document defines the playable field objectives, encounter structure, and
reward shape for Suncrest Hollow's three Grassland main quests. Quest posting,
NPC, sequencing, and Lysander recruitment rules remain authoritative in
`DEMO_QUEST_FLOW.md`.

The field structures below are locked for demo planning. Dialogue, currency
values, item statistics, enemy statistics, and map coordinates remain content
and balance decisions. Enemy groups and the Goblin Boss action pattern are
defined in `GRASSLAND_COMBAT_DESIGN.md`.

## Grassland Encounter Model

Grassland uses two encounter types at the same time:

1. **Dedicated encounters:** stable, authored goblin fights attached to quest
   locations and rescue states.
2. **Ambient encounters:** low-level generic creature groups triggered after a
   configured number of eligible exploration steps.

Step-triggered ambient encounters are the default approach for later regions
unless a region has an explicit reason to use another policy.

### Encounter Rules

- Dedicated quest encounters use stable IDs and one-time world-state flags.
  Completing one cannot be replaced by an unrelated ambient battle.
- Ambient encounter tables and step thresholds are region data rather than
  hard-coded movement rules.
- Only successful player movement through eligible exploration space advances
  the step counter. Standing still, blocked movement, menus, dialogue, towns,
  cutscenes, and camp do not count.
- The step count and pending ambient encounter state must survive scene changes
  and save/load without immediately duplicating an encounter.
- Ambient encounters use the ordinary escape rules. Authored encounters may
  disable escape only when their definition explicitly says so.
- Encounter selection may use injected randomness, but the step threshold
  should remain inspectable and testable so routes are learnable.

## Quest 1: A Worried Merchant

Old Marlow describes several approximate trouble spots along Grassland's road.
Directions remain intentionally loose, such as a location somewhere past an old
fence line, so the player explores rather than following exact map markers.

### Objective

- Place four stable investigation spots in the region.
- Allow the player to investigate them in any order.
- Require any two spots for quest completion.
- Leave remaining spots available as optional thoroughness objectives before
  turn-in.
- Use two small goblin skirmishes and two environmental-evidence locations.
- Seed one hidden chest, one scenic overlook, and one unrelated stranded
  traveler nearby to reward exploration beyond quest locations.

The exact-marker UI should not reveal every spot precisely. The journal and
dialogue should preserve Marlow's approximate directions while still giving the
player enough information to search intentionally.

### Bren Lead-In

During this quest, the party obtains a **Bren-marked trail knife fragment**. It
belonged to the missing scout, Tallis, and points the player toward Ironforge
Smithy for Quest 2.

- The fragment is discovered as part of the player's second completed
  investigation, regardless of which location that happens to be, so it cannot
  be missed on a valid completion route.
- It is quest evidence, not ordinary sellable or discardable inventory.

### Completion And Reward

- Reporting at least two investigated spots to Old Marlow completes the quest.
- Base rewards cover the required two spots.
- Each additional investigated spot increases the reward through extra gold or
  consumables.
- Investigating all four spots also awards **Marlow's Trade Charm**, a universal
  accessory whose exact statistics will be balanced with the demo item set.
- Reward resolution must use unique spot IDs and grant each bonus exactly once.

## Quest 2: The Smith And The Missing Scout

The trail knife fragment from Quest 1 belongs to Tallis, a scout who carried a
commissioned tool from Bren while tracking goblin movement. Tallis has not
returned.

### Objective

- Bren gives the party the scout's rough last-known heading.
- The field sequence follows a linear trail of authored clues rather than
  returning to Quest 1's scattered search pattern.
- Follow four clues: a discarded pack, disturbed ground, a torn field note, and
  a broken weapon strap.
- The trail leads to one specific rescue location.
- Tallis is found alive but injured, pinned beneath part of a collapsed hunting
  platform.

### Encounter And Payoff

- Three goblins guard the rescue location.
- This encounter is stronger than Quest 1's skirmishes but contains no named or
  unique enemy.
- After rescue, Tallis explains that the Goblin Boss overcommits during its
  heaviest swing and is left off balance afterward.
- The boss intelligence must produce a visible mechanical payoff during the
  eventual boss encounter: after Brutal Swing, the boss enters the one-round
  Off Balance defense penalty defined in `GRASSLAND_COMBAT_DESIGN.md`.
- Dialogue may suggest that regional goblins are organizing around a stronger
  leader, but must not introduce supernatural or later-world foreshadowing.

### Reward

- Grant gold and **Ironforge Fieldguard**, a universal defensive accessory
  crafted by Bren.
- Record the boss-intelligence state independently from inventory so selling or
  replacing the equipment cannot remove learned information.
- Exact statistics and gold value remain balance decisions.

## Quest 3: The Guard Captain's Warning

Captain Vashti's scouts were mapping the approach to the Goblin Boss's
territory. Several are now scattered and being pursued or cornered by goblin
patrols before they can report.

### Objective

- Place three stable scout-rescue locations that may be approached in a
  flexible order.
- Fight an escalated goblin patrol at each attempted rescue.
- Patrols are tougher than Quest 2's guards but contain no named or unique enemy.
- Require any two rescues for quest completion so saving the third scout remains
  a rewarded thoroughness goal.
- Track each scout by stable ID and resolve rescue state exactly once.

No global real-time countdown is currently specified. The pursuit creates
narrative urgency and resolves when the player reaches a rescue location. A
timed failure system must not be added later without an explicit design change.

### Intel And Turn-In

- Each rescued scout contributes one distinct battle advantage: knowledge of
  patrol routes removes one starting guard, an ambush warning grants the party
  opening initiative, and the war horn's location prevents reinforcements.
- Additional rescues improve the accumulated intelligence and final reward
  without becoming mandatory.
- Returning the required scouts or their information to Captain Vashti completes
  Quest 3 and the three-main-quest set.
- Turn-in activates Captain Vashti's Lysander handoff once the three-unique-quest
  reputation threshold is also satisfied.

### Reward

- Grant base gold and a **Suncrest Supply Kit** containing useful recovery
  consumables.
- Scale bonus rewards and accumulated territory intelligence by the number of
  scouts rescued.
- Rescuing all three scouts also awards **Suncrest Watch Insignia**, a universal
  accessory intended to provide a modest defensive and speed benefit.
- Resolve each scout contribution and reward tier exactly once.

## Remaining Content And Balance Decisions

- Tallis's final appearance and dialogue.
- Names, appearances, and dialogue for Quest 3's three scouts.
- Exact investigation, clue, rescue, chest, traveler, and overlook map locations.
- Enemy statistics for every dedicated and ambient fight.
- Exact gold, equipment, consumable, XP, and optional-completion reward values.
