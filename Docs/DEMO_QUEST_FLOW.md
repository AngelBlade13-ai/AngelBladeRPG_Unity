# Demo Quest Flow: Opening Through Goblin Boss

This document defines the demo's opening, tutorial encounter, initial party
formation, early quest progression, and Lysander recruitment. It builds on
`DEMO_STORY_REFERENCE.md`, `CAMP_SYSTEM_DESIGN.md`, and
`JOB_CLASS_SYSTEM.md`. Detailed field objectives, encounters, and reward shapes
live in `DEMO_MAIN_QUEST_CONTENT.md`.

## Protagonist Framing

The player-created protagonist is a complete newcomer to Suncrest Hollow, the
hub town previously described by the planning label `Town Square`.

- Early NPCs treat the protagonist as unknown and newly arrived, not as a
  returning resident or established hero.
- The opening should introduce Suncrest Hollow through the protagonist's first
  impressions without requiring a long exposition sequence.
- The protagonist's reason for traveling to Suncrest Hollow may be developed
  later, but dialogue in this slice must not imply an unrecorded local history.

## Opening Quest: The Delayed Caravan

At the beginning of the game, only one quest is available: a caravan carrying
goods for Suncrest Hollow is overdue, and someone is needed to investigate.

1. The protagonist accepts the caravan quest in Suncrest Hollow.
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

After defeating the Hobgoblin, the group returns to Suncrest Hollow.

- The protagonist, Iona, Damari, and Enora become the standing four-character
  party.
- This return introduces job assignment and adjustment for every current party
  member.
- The job tutorial must teach the shared job-access rule without presenting
  each companion's natural affinity as a locked class.

## Main Quest Structure

Three main story quests must be completed before the regional Goblin Boss
encounter becomes available.

- The numbered main quests form a guided story sequence, while travel and side
  quest order remain flexible.
- These required quests form the shortest critical path to the Goblin Boss.
- Approximately three to five side quests should support exploration, hub
  returns, rewards, and character moments.
- Side quests remain optional for story progression and are not required to
  unlock the Goblin Boss.
- Defeating the Goblin Boss unlocks progression from Grassland to Cherry
  Blossom.

### Guild Posting And Briefing Pattern

Each main quest begins as a deliberately vague posting in Guild Hall.

1. The player accepts the posting and receives a district-level lead.
2. The quest enters an `AwaitingBriefing` state rather than becoming fully
   active.
3. The player finds the named quest giver elsewhere in Suncrest Hollow.
4. Speaking with that NPC supplies the details and initializes the active quest
   objective.

This pattern gives the player an organic tour of the hub without presenting a
formal walking tutorial. Main quest state must distinguish posting acceptance,
briefing, objective completion, and final turn-in so save/load cannot skip or
repeat an initialization beat. These quest givers are local flavor characters;
no later-game recurrence is currently planned for them.

### Quest 1: A Worried Merchant

- Posting: Guild Hall, describing goblin trouble affecting the roads.
- Quest giver: Old Marlow in Whisper Market.
- Character: an anxious, talkative shopkeeper whose livelihood depends on safe
  roads.
- Narrative purpose: establishes that the goblin problem is an ordinary but
  continuing threat rather than a single caravan incident.

### Quest 2: The Smith And The Missing Scout

- Posting: Guild Hall, reporting that a scout sent to investigate has not
  returned.
- Quest giver: Bren at Ironforge Smithy.
- Character: gruff but visibly worried because he knows the missing scout.
- Narrative purpose: provides concrete information about the Goblin Boss, such
  as its appearance, territory, or a useful behavioral pattern.
- System opportunity: naturally introduces equipment inspection or upgrades at
  the smithy without making a purchase mandatory.

### Quest 3: The Guard Captain's Warning

- Posting: Guild Hall, representing the town's official response to the growing
  goblin threat.
- Quest giver: Captain Vashti at Suncrest Watch.
- Character: authoritative and treating the situation as a serious escalation.
- Narrative purpose: serves as the final preparation beat before the Goblin
  Boss becomes available.
- Turn-in consequence: completes the required main-quest set and begins the
  Lysander breadcrumb and exit-interception sequence.

## Suncrest Hollow Districts

- **Whisper Market:** commerce; Old Marlow briefs Quest 1 here.
- **Ironforge Smithy:** crafting and equipment; Bren briefs Quest 2 here.
- **Guild Hall:** quest board and initial posting location for all three main
  quests.
- **The Suncrest Inn:** tavern and social hub.
- **The Sunwell Shrine:** temple and healing district.
- **Amber Row:** residential quarter.
- **The Sunroot Grove:** garden and town green; intended location for the
  subtle, uncommented environmental decay hints.
- **Suncrest Watch:** barracks and guard post; Captain Vashti briefs Quest 3,
  and the nearby Grassland exit hosts Lysander's interception.

The shared Suncrest, Amber, and Sun naming is intentional. These districts are
interconnected parts of the hub, not separate world-map travel destinations.

## Lysander Recruitment

Lysander (`pc_04`) is a grizzled, independent, and experienced adventurer who
has grown tired of unreliable parties. He joins after the protagonist has built
a small local reputation rather than joining by default.

### Unlock Rule

- Lysander becomes reputation-eligible after three unique quests have been
  completed.
- Main and side quests both count toward this threshold.
- The three required main quests satisfy the threshold by themselves, so side
  content is never required to recruit him.
- His join sequence becomes pending once he is reputation-eligible and Quest 3
  has been turned in. This preserves Captain Vashti's authored handoff beat.
- Repeatable activity, duplicate rewards, and reloading completed turn-ins must
  not increment the counter again.

### Forced Join Beat

Before recruitment, Lysander is visible loitering near Guild Hall. He is
non-interactive and does not provide dialogue, allowing attentive players to
notice him without explaining his role early.

Once Quest 3 is turned in and the reputation threshold is satisfied:

1. Captain Vashti notes that the paladin was looking for the protagonist.
2. Lysander is removed from his earlier Guild Hall position.
3. He intercepts the party at Suncrest Watch's exit toward Grassland.
4. His dedicated join scene completes before travel can continue.

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

- Remaining field-content and reward tuning listed in
  `DEMO_MAIN_QUEST_CONTENT.md`.
- The exact NPC who authorizes or frames the Goblin Boss quest.
- Specific content for approximately three to five optional side quests.
- What happens immediately after the Goblin Boss falls and the route to Cherry
  Blossom opens.
- The Cherry Blossom regional boss and the demo's final closing beat.
- The later teleport-crystal system. It may support returning to Suncrest Hollow
  in the full game but is not required for the demo.
