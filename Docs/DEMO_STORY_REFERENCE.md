# Demo Story And Lore Reference

This document is the authoritative narrative boundary for the public demo. It
covers Suncrest Hollow and the first two traveled regions only. It should guide
dialogue, environment art, encounters, and demo production without requiring
full-game lore.

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
Suncrest Hollow hub -> Grassland -> Cherry Blossom -> demo progression gate
```

This sequence describes narrative progression, not a requirement that every
location be one Unity scene.

## Quest And Progression Structure

The demo uses a hub-and-expedition loop rather than a fixed sequence of quests.

- Suncrest Hollow offers multiple quests that send the player into the available
  world areas and give them reasons to return afterward.
- The three main quests follow the guided sequence in `DEMO_QUEST_FLOW.md`.
  Players may otherwise explore freely and choose optional quest order.
- Optional quests should offer useful rewards, character moments, local world
  detail, or preparation for harder encounters without becoming hidden
  requirements.
- Defeating the Grassland goblin boss is the only required progression gate for
  reaching Cherry Blossom.
- Three main quests unlock that boss encounter. Side-quest completion,
  character level, equipment score, and elapsed playtime do not independently
  block the transition.
- The opening, tutorial party formation, main-quest count, and Lysander
  recruitment flow are defined in `DEMO_QUEST_FLOW.md`.

## Hub Return Incentives

Suncrest Hollow should become a familiar place the player wants to revisit, rather
than a menu they visit only when forced.

- Quest turn-ins and new quest-board work should draw the player back naturally.
- The market, smithy, guild, recovery, party conversations, and future
  progression services may provide practical reasons to return, limited to the
  systems selected for the demo.
- New NPC remarks or companion banter can make the hub feel responsive after
  important quests and boss victories.
- Mandatory returns should be used only when needed for a clear story beat.
  Routine backtracking should remain the player's choice.

Ember Camp provides expedition recovery and quieter companion moments, but it
does not provide the market, smithy, guild, recruitment, or general quest
turn-in functions that make Suncrest Hollow valuable. Its detailed boundaries are
recorded in `CAMP_SYSTEM_DESIGN.md`.

## Demo Pacing

- Target at least 90 minutes for a typical first playthrough that includes
  exploration, several quests, dialogue, preparation, and normal combat.
- Do not enforce that duration with timers, excessive travel, mandatory grinding,
  unskippable repetition, or a required number of optional quests.
- A knowledgeable or highly skilled player may follow a much shorter critical
  path, complete the required objectives, and progress quickly.
- Quest and dialogue state should be deterministic enough that speedrunning
  routes are learnable and repeatable, even when combat still contains clearly
  communicated random outcomes.

## Suncrest Hollow

Suncrest Hollow is the proper name of the player's home-base town, previously
described by the planning label `Town Square`. It should be the liveliest, most
vibrant location in the game.

- Its interconnected districts include Whisper Market, Ironforge Smithy, Guild
  Hall, the Suncrest Inn, the Sunwell Shrine, Amber Row, the Sunroot Grove, and
  Suncrest Watch. Each district is a separate explorable Unity scene, but they
  are adjoining hub districts rather than world-map travel destinations. See
  `SUNCREST_DISTRICT_LAYOUT.md` for the authoritative connection graph.
- NPC activity and environmental detail should make the town feel inhabited and
  welcoming.
- A few optional, easy-to-miss details may suggest that something is slightly
  off, such as a plant wilting unexpectedly or a well running a little low.
- NPCs must not comment on these details, and the details must not affect or
  block gameplay.
- Suncrest Hollow and Ember Camp should feel distinct: the town is lively, social,
  and service-rich, while camp is temporary and intimate.

## Region 1: Grassland

Grassland is the first region reached from Suncrest Hollow. It is vibrant, healthy,
ordinary, and intentionally low stakes.

- The environment contains no corruption or decay.
- The regional boss is a troublesome goblin acting as a bandit-style nuisance.
- The encounter should read as a conventional first boss with no supernatural
  meaning and no connection to a larger threat.
- Defeating this boss unlocks progression to Cherry Blossom. Other Grassland
  side quests remain optional; the three authored main quests are required to
  make the boss encounter available.
- No foreshadowing that belongs to the later story may be moved into this
  region.

## Region 2: Cherry Blossom

Cherry Blossom is the second traveled region. It remains beautiful, vibrant,
and free of visible rot or corruption during the demo, even as the sacred
tree's seasonal behavior becomes subtly wrong.

- A small nomadic settlement lives in the region.
- Settlement residents may express unusual caution, general unease, or the
  sense that something is not right.
- No resident knows or states the cause. Dialogue must not confirm an
  explanation that the player has not earned.
- Services are intentionally more limited than in Suncrest Hollow.
- The regional boss serves both as this region's story beat and as the gate
  preventing travel farther into the full game.
- The boss is the Great Stag, a distressed territorial protector that the party
  subdues rather than kills. Its arc and the demo ending are defined in
  `DEMO_CHERRY_BLOSSOM_ARC.md`, with combat rules in
  `CHERRY_BLOSSOM_COMBAT_DESIGN.md`.
- A later version of this region may change visibly, but that alternate state is
  excluded from the demo.

## Demo Companion Tone

The existing short profiles for Iona (`pc_01`), Damari (`pc_02`), Enora
(`pc_03`), and Lysander (`pc_04`) in `JOB_CLASS_SYSTEM.md` are sufficient for
early dialogue and combat banter.

- Dialogue should establish contrasting personalities and growing
  found-family chemistry without requiring full backstory exposition.
- No permanent companion loss or major late-story turn occurs in the demo.
- The opening recruitment order through Lysander is fixed in
  `DEMO_QUEST_FLOW.md`. Active-party choices and camp-event timing afterward
  remain open planning decisions.

## Explicit Demo Exclusions

- The true cause of the world's decay or any confirmed explanation for it.
- Additional later bosses, regions, factions, or endgame content.
- The planned permanent roster removal and its story circumstances.
- Full-game camp expansions such as cooking, decoration, elaborate schedules,
  and large relationship-event chains.
- Cherry Blossom's future changed state.
- Any content beyond the Cherry Blossom progression gate.
- The later teleport-crystal fast-travel system.

## Remaining Production Decisions

- Final tuning for the three designed main quests and four optional side quests.
- Final Great Stag balance and production assets.
- Final companion dialogue after Lysander joins.
- Exact protagonist customization categories and art budget.
- Final demo item statistics, prices, quantities, and reward tuning under the
  locked economy structure.
- Final Camp Ration economy, camp dialogue, and campsite layout.
- Final dialogue count and asset-production estimates for the 90-minute-plus
  target. The required content inventory is in `DEMO_CONTENT_MANIFEST.md`.

The public title, working developer identity, player promise, feature hierarchy,
equipment structure, and asset-clearance process are recorded in
`MILESTONE_14_DECISIONS.md`. The working `Rawr! Studios` brand still requires
clearance or replacement before public use.

Later lore should remain deferred until a milestone needs it. New story input
should be added here only when it changes the public demo.
