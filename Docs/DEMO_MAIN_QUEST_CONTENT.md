# Demo Main Quest Field Content

This document defines the playable field objectives, encounter structure, and
reward shape for Suncrest Hollow's three Grassland main quests. Quest posting,
NPC, sequencing, and Lysander recruitment rules remain authoritative in
`DEMO_QUEST_FLOW.md`.

The field structures below are locked for demo planning. Exact counts within a
stated range, dialogue, currency values, item statistics, and map coordinates
remain content and balance decisions.

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

- Place three or four stable investigation spots in the region.
- Allow the player to investigate them in any order.
- Require any two spots for quest completion.
- Leave remaining spots available as optional thoroughness objectives before
  turn-in.
- Use a mixture of small goblin skirmishes and environmental evidence such as a
  ransacked cart, torn banner, or tracks.
- Seed a few unrelated nearby discoveries, such as a hidden chest, minor NPC,
  or scenic detail, to reward exploration beyond quest locations.

The exact-marker UI should not reveal every spot precisely. The journal and
dialogue should preserve Marlow's approximate directions while still giving the
player enough information to search intentionally.

### Bren Lead-In

During this quest, the party obtains an item bearing Bren's smithing mark. It
belonged to the missing scout and points the player toward Ironforge Smithy for
Quest 2.

- The marked item must be obtained before Quest 1 can be completed, regardless
  of which two investigation spots the player chooses.
- It is quest evidence, not ordinary sellable or discardable inventory.
- Its exact form, such as a tool or weapon fragment, remains undecided.

### Completion And Reward

- Reporting at least two investigated spots to Old Marlow completes the quest.
- Base rewards cover the required two spots.
- Each additional investigated spot increases the reward through extra gold,
  gear, consumables, or another clearly previewed bonus.
- Reward resolution must use unique spot IDs and grant each bonus exactly once.

## Quest 2: The Smith And The Missing Scout

The marked item from Quest 1 belongs to a scout who carried a commissioned tool
or weapon from Bren while tracking goblin movement. The scout has not returned.

### Objective

- Bren gives the party the scout's rough last-known heading.
- The field sequence follows a linear trail of authored clues rather than
  returning to Quest 1's scattered search pattern.
- Clues may include broken equipment, disturbed ground, and a torn note.
- The trail leads to one specific rescue location.
- The scout is found alive but injured or trapped by a non-lethal complication,
  such as capture, a den, or fallen debris.

The scout's name and exact predicament remain undecided.

### Encounter And Payoff

- Two or three goblins guard the rescue location.
- This encounter is stronger than Quest 1's skirmishes but contains no named or
  unique enemy.
- After rescue, the scout describes the Goblin Boss's strength, territory, and
  a useful tell or weakness.
- The boss intelligence must produce a visible mechanical payoff during the
  eventual boss encounter, not exist only as flavor text. Its exact effect is
  chosen when the Goblin Boss is designed.
- Dialogue may suggest that regional goblins are organizing around a stronger
  leader, but must not introduce supernatural or later-world foreshadowing.

### Reward

- Grant gold and one useful equipment item crafted or repaired by Bren.
- Record the boss-intelligence state independently from inventory so selling or
  replacing the equipment cannot remove learned information.
- Exact item slot, statistics, gold value, and intelligence effect remain
  balance decisions.

## Quest 3: The Guard Captain's Warning

Captain Vashti's scouts were mapping the approach to the Goblin Boss's
territory. Several are now scattered and being pursued or cornered by goblin
patrols before they can report.

### Objective

- Place two or three stable scout-rescue locations that may be approached in a
  flexible order.
- Fight an escalated goblin patrol at each attempted rescue.
- Patrols are tougher than Quest 2's guards but contain no named or unique enemy.
- Require fewer than the total available rescues for quest completion so saving
  every scout remains a rewarded thoroughness goal.
- Track each scout by stable ID and resolve rescue state exactly once.

No global real-time countdown is currently specified. The pursuit creates
narrative urgency and resolves when the player reaches a rescue location. A
timed failure system must not be added later without an explicit design change.

### Intel And Turn-In

- Each rescued scout contributes a distinct piece of information about the
  Goblin Boss's territory or approach.
- Additional rescues improve the accumulated intelligence and final reward
  without becoming mandatory.
- Returning the required scouts or their information to Captain Vashti completes
  Quest 3 and the three-main-quest set.
- Turn-in activates Captain Vashti's Lysander handoff once the three-unique-quest
  reputation threshold is also satisfied.

### Reward

- Grant base gold and a useful final gear item or consumable.
- Scale bonus rewards and accumulated territory intelligence by the number of
  scouts rescued.
- Resolve each scout contribution and reward tier exactly once.

## Remaining Content And Balance Decisions

- Whether Quest 1 uses three or four trouble spots and the exact optional POIs.
- The physical form of Bren's marked item.
- The missing scout's name, appearance, and non-lethal predicament.
- Exact clue sequence and rescue-map locations for Quest 2.
- The Goblin Boss tell or weakness and the mechanical benefit of learned intel.
- Whether Quest 3 uses two or three scouts and the exact minimum rescue count.
- Enemy group compositions and statistics for every dedicated and ambient fight.
- Exact gold, equipment, consumable, XP, and optional-completion reward values.
