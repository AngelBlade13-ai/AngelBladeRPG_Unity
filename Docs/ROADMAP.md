# Petals in the Dusk Product Roadmap

This roadmap guides `AngelBladeRPG_Unity` from its current prototype foundation
to a public vertical-slice demo, then toward a complete commercial PC game. The
long-term goal is a full release on Steam, not a collection of disconnected RPG
systems.

Detailed technical handoff notes live in `PROJECT_HANDOFF.md`. Product and scene
direction lives in `GAMEPLAY_DIRECTION.md`. Public title, brand, and account
boundaries live in `PRODUCT_IDENTITY.md`; the protected player promise lives in
`PLAYER_PROMISE_FEATURE_HIERARCHY.md`. Job and roster constraints live in
`JOB_CLASS_SYSTEM.md`, with demo job availability and progression limits in
`DEMO_JOB_TREE_SCOPE.md` and the playable early trees in
`DEMO_JOB_TREES.md`. Demo narrative boundaries live in
`DEMO_STORY_REFERENCE.md`. The opening quest structure lives in
`DEMO_QUEST_FLOW.md`, with field content in
`DEMO_MAIN_QUEST_CONTENT.md` and Grassland battles in
`GRASSLAND_COMBAT_DESIGN.md`. The Cherry Blossom ending lives in
`DEMO_CHERRY_BLOSSOM_ARC.md`, with the Great Stag battle in
`CHERRY_BLOSSOM_COMBAT_DESIGN.md`. Camp boundaries live in
`CAMP_SYSTEM_DESIGN.md`. Optional demo quests live in `DEMO_SIDE_QUESTS.md`.
The consolidated production inventory lives in `DEMO_CONTENT_MANIFEST.md`.
Economy structure lives in `ITEM_EQUIPMENT_ECONOMY.md`. Contributor and asset
clearance records live in `CONTRIBUTOR_AGREEMENT_TEMPLATE.md` and
`ASSET_LICENSE_TRACKING.md`.

## North Star

Build a polished 2D pixel-fantasy RPG with:

- Top-down exploration through physical towns, interiors, routes, and dungeons.
- Classic turn-based battles in a dedicated battle scene.
- A flexible job system that lets the player shape each party member.
- Character-driven story, party relationships, equipment, progression, and
  meaningful combat decisions.
- Reliable keyboard and controller play on a tested Windows PC build.
- A scope and production pipeline that can support a full commercial game.

The immediate production target is a small but representative public demo. It
must feel like the opening of the real game, not like a mechanics test room.

## Current Position

| Gate | Status |
| --- | --- |
| Prototype and architecture foundation | Complete |
| Walkable world and interactions | Complete |
| Separate battle-scene loop | Complete |
| Job, character, and party data foundation | Complete |
| Core combat expansion | Complete |
| Vertical-slice demo | In progress (Milestone 15) |
| Full-game production | Future |
| Steam release candidate | Future |

Current branch: `feature/milestone-15-party-jobs`

Current verification baseline: 274 Edit Mode tests passed in Unity
`6000.5.3f1`, plus successful Play Mode checks for exploration, interactions,
district transitions, party/job assignment, structured battle outcomes,
guarding, misses, critical hits, speed-based escape, rewards, and speed-based
turn order.

## Release Strategy

Development is divided into four release gates:

1. **Foundation:** Prove the architecture and core mechanics. Milestones 1-13.
2. **Vertical Slice:** Build one polished, representative chapter. Milestones
   14-20.
3. **Public Demo:** Package, test, publish, and learn from the vertical slice.
   Milestone 21.
4. **Full Game:** Scale proven tools and content, then ship and support the
   commercial release. Milestones 22-25.

No release date should be announced until the vertical slice has passed its
content-complete gate and production velocity is measurable.

## Demo Definition Of Done

Milestone 14 locked the following demo target:

- At least 90 minutes for a typical first playthrough, with a shorter critical
  path free from optional-quest or elapsed-time gates.
- A clear beginning, short objective, climax, and demo ending.
- One polished hub or town area.
- Two traveled regions: Grassland and Cherry Blossom.
- One reusable Ember Camp space for recovery and a small set of companion
  moments.
- Several purposeful quests and NPC interactions that can be approached in a
  flexible order.
- Multiple normal battles and the Grassland and Cherry Blossom regional bosses.
- A small playable party that demonstrates jobs, party roles, and progression.
- A useful subset of items, equipment, rewards, and town services.
- Working save/load, settings, keyboard controls, and controller navigation.
- Coherent pixel art, animation, UI, sound, and music suitable for public
  screenshots and video.
- A Windows build that launches cleanly outside the Unity Editor.
- No progression blockers, lost saves, duplicate rewards, missing references,
  placeholder instructions, or known critical defects.
- An ending screen that clearly closes the demo and points players toward the
  full game without pretending unfinished content is playable.

The demo does not need every final job, item, location, party member, system, or
story chapter. It does need to accurately represent the quality and identity of
the intended full game.

## Completed Foundation

### Milestones 1-5: Initial RPG Foundation

Status: Complete

- Created the Unity project and main scene structure.
- Built the first title, town, and battle prototypes.
- Added player and monster data, damage, rewards, XP, and leveling.
- Separated shared gameplay state into `GameSession`.

### Milestone 6: Character Creation

Status: Complete

- Added validated player-name entry and title navigation.
- Completed and tested the required Unity UI wiring.

### Milestone 7: Core Gameplay Tests

Status: Complete

- Established Unity Edit Mode coverage for progression, damage, battle state,
  rewards, and duplicate-reward prevention.
- Documented the fast Editor test workflow in `TESTING.md`.

### Milestone 8: Pixel World Foundation

Status: Complete

- Established a `320 x 180` reference resolution and `16` pixels-per-unit
  baseline.
- Added normalized top-down movement, collision, directional state, camera
  follow, Tilemap foundations, and resolution checks.

### Milestone 9: Walkable Town Prototype

Status: Complete

- Replaced the town panel with a physical exploration scene.
- Added scene-safe session state, spawn points, collision, and a compact HUD.

### Milestone 10: World Interaction System

Status: Complete

- Added directional interaction, signs, dialogue presentation, doors, named
  destination spawns, and scene transitions.

### Milestone 11: Separate Turn-Based Battle Scene

Status: Complete

- Replaced the battle panel with a dedicated scene.
- Added encounter transfer, victory, defeat, escape, rewards, and return to the
  correct exploration location.

### Milestone 12: Jobs, Characters, And Party Data

Status: Complete

- Defined 12 jobs and unrestricted character job assignment.
- Added affinity growth, active/reserve party rules, stable character IDs,
  authored profiles, roster history, bond data, and permanent removal state.
- Preserved equipped-item destruction as a future inventory/save invariant.

## Latest Completed Foundation Gate

### Milestone 13: Combat Core Completion

Branch: `feature/core-combat-expansion`

Status: Complete

Goal: finish a deterministic, testable combat foundation that can support party
battles, jobs, equipment, abilities, and demo balancing.

Completed:

- [x] Add shared HP, MP, attack, defense, magic, resistance, and speed stats.
- [x] Preserve existing leveling, rewards, and physical damage behavior.
- [x] Add ID-keyed Goblin, Ogre, Slime, and Wisp definitions.
- [x] Create independent runtime monsters from reusable definitions.
- [x] Migrate exploration encounters to monster IDs.
- [x] Resolve each battle round in speed order without a visible turn queue.
- [x] Prevent a defeated combatant from retaliating.
- [x] Verify player-first and monster-first rounds in Play Mode.
- [x] Run the initial 117-test Edit Mode suite successfully.

Final slices:

- [x] Add explicit combat-result data instead of relying only on log strings.
- [x] Add accuracy and misses with injected, testable randomness.
- [x] Add critical hits as a separate tested rule.
- [x] Add guarding as a separate tested rule and battle command.
- [x] Replace guaranteed escape with a clear, speed-based tested escape rule.
- [x] Establish action contracts for physical attacks, magic, healing, items,
  defend, and future job abilities.
- [x] Run all 138 Edit Mode tests in Unity `6000.5.3f1` successfully.
- [x] Complete the Play Mode combat regression for guarding, failed and
  successful escape, misses, critical hits, victory, defeat, and rewards.

Exit gate:

- Combat rules are independent from scene UI.
- Random outcomes are controllable in tests.
- Battle logs can explain every outcome to the player.
- The battle scene can accept new actions without another structural rewrite.

## Next Product Gate: Vertical Slice

### Milestone 14: Demo Scope And Narrative Blueprint

Goal: decide exactly what the demo contains before building production content.

Status: Complete

- [x] Establish the demo's location progression as Suncrest Hollow, Grassland, and
  Cherry Blossom.
- [x] Define the demo's tone progression and narrative knowledge boundaries.
- [x] Separate the player-created protagonist from the four authored companion
  identities.
- [x] Record later-story material and systems that must remain outside the demo.
- [x] Define a flexible quest loop with Suncrest Hollow as a recurring hub and the
  Grassland goblin boss as the gate to Cherry Blossom.
- [x] Set a 90-minute-plus typical-playthrough target without artificial
  speedrun barriers.
- [x] Define Ember Camp as a limited expedition recovery and companion space
  that complements rather than replaces Suncrest Hollow.
- [x] Define the delayed-caravan opening, staged tutorial encounter, initial
  four-character party, three-main-quest boss unlock, and Lysander recruitment.
- [x] Choose `Petals in the Dusk` as the public title and preserve
  `AngelBladeRPG_Unity` as the internal repository/assembly identity.
- [x] Select individual/sole-proprietor Steamworks onboarding with
  `Rawr! Studios` as the working public developer/publisher brand.
- [x] Run a preliminary public-name screen, record similar `Rawr` studio brands,
  and carry formal clearance or replacement into the public-identity gate.
- [x] Write the one-paragraph player promise and protected feature hierarchy.
- [x] Define the demo opening and first-region objective through the Goblin Boss.
- [x] Name Suncrest Hollow's districts and define the three main quest postings,
  quest givers, narrative purposes, and Lysander breadcrumb sequence.
- [x] Define the three main quests' field structures, encounter escalation,
  intelligence rewards, and optional thoroughness bonuses.
- [x] Define Grassland enemy roles, dedicated and ambient groups, Quest 3 battle
  advantages, and the regional Goblin Boss action pattern.
- [x] Define the post-Goblin celebration, Cherry Blossom transition, settlement
  problem, Great Stag climax, and unresolved demo ending.
- [x] Define the Great Stag's identity, symbolism, subdued outcome, marked
  charge, Staggered response, and low-HP Panicked phase.
- [x] Define four optional side quests supporting Suncrest Hollow returns,
  Grassland exploration, party warmth, and Cherry Blossom unease.
- [x] Lock the demo's designated Ember Camp access, ration-based full rest,
  available actions, and two one-time companion moments.
- [x] Make all 12 jobs playable in the demo with a data-driven progression
  limit owned by each job tree.
- [x] Define the exact maps, NPC set, and later companion use within the selected
  demo regions.
- [x] Define the exact early nodes, abilities, JP costs, and progression limit
  for each demo job.
- [x] Add permanent cross-job stat nodes while keeping job traits, passives, and
  actions restricted to the currently equipped job.
- [x] Define the required item families and town/settlement service roles.
- [x] Lock the one-currency, five-slot, eight-weapon-category, shared-job
  compatibility, rarity, and no-required-grind economy structure.
- [x] Create the map, scene, quest, and content list for the agreed playtime.
- [x] Use dedicated quest encounters alongside step-triggered ambient random
  encounters as the default regional policy.
- [x] Create a preliminary content budget for maps, sprites, portraits,
  animations, music, sound effects, dialogue, enemies, and UI screens.
- [x] Record contributor-agreement and asset-license tracking requirements.
- [x] Create a cut list of features explicitly deferred until after the demo.

Exit gate:

- A short demo design brief exists and every required asset or scene is listed.
- The story can be completed using the agreed content budget.
- No demo-critical design decision is hidden in chat history.

Story checkpoint: this is the point to provide the relevant world premise,
opening plot, demo location, involved characters, factions, and desired tone.
The current authoritative input is recorded in `DEMO_STORY_REFERENCE.md`.
Full-game lore that does not affect the demo remains deferred.

### Milestone 15: Party Battle And Job Gameplay

Goal: make the job and party systems visible and meaningful during play.

Status: In progress

Current implementation checkpoint:

- Stable definitions now cover the 96 purchased demo nodes across all 12 jobs.
- Each persistent character owns unspent JP and learned node IDs.
- Available active and benched roster members can receive JP together.
- Permanent stat bonuses aggregate across learned jobs, while purchased skills
  and passives require their matching job to be equipped.
- The expanded 160-test Edit Mode suite passes in Unity `6000.5.3f1`.
- The next checkpoint gives persistent roster members shared runtime combat
  stats, creates a stable active protagonist record for new sessions, and adds
  actor-relative ally/enemy targeting with incapacitation rules.
- The expanded 177-test Edit Mode suite passes in Unity `6000.5.3f1`.
- The in-progress round-resolver checkpoint accepts one command per living
  party member, generates replaceable enemy commands, resolves every combatant
  by speed, supports speed-sensitive Defend, skips incapacitated turns, and
  retargets attacks when an earlier action defeats the selected target.
- The expanded 190-test Edit Mode suite passes in Unity `6000.5.3f1`.
- The battle-command UI checkpoint displays compact party/enemy
  HP and MP lists, marks the current actor and target without relying on color,
  cycles enemy targets, collects commands in formation order, and submits the
  complete command set to the party round resolver.
- The expanded 199-test Edit Mode suite and repaired-scene smoke test pass in
  Unity `6000.5.3f1`.
- The core-ability checkpoint adds stable definitions for Power
  Strike, Ember, Blood Bolt, Mend, and Lay On Hands; validates job, target, and
  resource requirements before a round mutates; and resolves physical, magic,
  healing, MP-cost, and safe HP-cost effects through the shared speed order.
- The expanded 210-test Edit Mode suite passes in Unity `6000.5.3f1`.
- The ability-command UI checkpoint adds a generated Ability button,
  two-step targeting and confirmation, ally/enemy target cycling, action-cost
  prompts, disabled unaffordable actions, and combined self-target markers.
- The expanded 218-test Edit Mode suite and repaired-scene smoke test pass in
  Unity `6000.5.3f1`.
- The equipped-job stat checkpoint gives all 12 jobs distinct runtime stat
  packages, scales only those packages by character affinity, preserves level
  growth and missing HP/MP during recalculation, and keeps learned permanent
  bonuses active across job changes without stacking derived values.
- The expanded 225-test Edit Mode suite passes in Unity `6000.5.3f1`.
- The party-management checkpoint adds active/reserve validation, four-member
  formation ordering, and unrestricted job assignment with affinity guidance.
  New games now enter `SuncrestGuildHallScene`, where a generated party service
  exposes all 12 jobs and updates effective stats immediately.
- The expanded 237-test Edit Mode suite and Guild Hall Play Mode smoke test pass
  in Unity `6000.5.3f1`.
- The party-outcome checkpoint is implemented and verified.
  Active participants now receive shared XP, every available recruited
  character receives JP, gold remains shared, and structured results report
  each character's level gains. Victory, defeat, and escape record active and
  benched participation exactly once.
- The expanded 246-test Edit Mode suite passes in Unity `6000.5.3f1`.
- The enemy-group checkpoint is implemented and verified.
  It adds the eight authored Grassland/tutorial enemy roles, all ten currently
  defined Grassland quest and ambient groups, stable standard and boss
  formation layouts, unique runtime IDs for duplicate enemies, group targeting
  and victory, aggregated rewards, and runtime placeholder positioning.
- The expanded 274-test Edit Mode suite passes in Unity `6000.5.3f1`.

- [x] Create runtime combatants for multiple active party members.
- [x] Add party targeting for allies and enemies.
- [x] Add MP, physical attacks, magic, healing, defend, and a small ability set.
- [x] Apply job stats and affinities to playable combatants.
- [x] Add stable job-tree nodes, per-character unlock state, and data-driven
  demo progression limits for all 12 playable jobs.
- [x] Aggregate permanent stat nodes across learned jobs without activating
  inactive-job skills or duplicating bonuses after loading or switching.
- [x] Add a compact party formation and job-assignment flow.
- [x] Handle incapacitation, victory, defeat, XP, and rewards for a party.
- [x] Add enemy groups and battle layouts needed by the demo.
- [ ] Support the tutorial encounter's waves, safe damage checkpoints, disabled
  escape, reinforcements, enemy threat control, and one-time completion.
- [ ] Verify keyboard and controller command navigation.

Exit gate:

- The demo party can complete representative normal and boss battles.
- At least two different party/job strategies are viable.
- Battle UI clearly communicates turn outcomes and valid targets.

### Milestone 16: Items, Equipment, Economy, And Town Services

Goal: complete the smallest progression economy that makes exploration and
combat rewards matter.

- [ ] Add stable item definitions for weapons, armor, accessories, and
  consumables.
- [ ] Add `Camp Ration` and the confirmed full-rest consumption rules.
- [ ] Add inventory quantities and capacity rules, if capacity is retained.
- [ ] Add equipment ownership, stat bonuses, requirements, and comparisons.
- [ ] Implement equipped-item destruction for permanent roster removal exactly
  once without returning those items to shared inventory.
- [ ] Add item use in and out of battle where appropriate.
- [ ] Add one shop or service flow with buying and selling.
- [ ] Add resting or healing in town.
- [ ] Add tested party-rest rules shared by Suncrest Hollow and Ember Camp where
  appropriate.
- [ ] Define demo loot tables, prices, and reward pacing.

Exit gate:

- The player can earn, inspect, equip, use, buy, and sell the demo item set.
- Inventory and equipment rules are covered by Edit Mode tests.

### Milestone 17: Save, Settings, Input, And Build Baseline

Goal: remove platform and persistence risks before producing most demo content.

- [ ] Add versioned save models independent from scene objects.
- [ ] Save player, party, jobs, inventory, equipment, quests, world state, and
  explicit spawn location.
- [ ] Save camp state, consumed camp-event IDs, and exact camp return context.
- [ ] Add manual save, autosave, Continue, new-game confirmation, and corrupt
  save handling.
- [ ] Store saves under Unity's application data path.
- [ ] Add separate music and sound volume settings.
- [ ] Add display mode, resolution, text-speed, and other necessary settings.
- [ ] Support keyboard and common controller navigation across all demo screens.
- [ ] Add input rebinding or document the deliberately supported fixed layout.
- [ ] Establish accessibility basics: readable text, no color-only information,
  configurable text speed, and reduced flashing where applicable.
- [ ] Produce and smoke-test a Windows development build outside the Editor.
- [ ] Add build version display and a repeatable build checklist.

Exit gate:

- A fresh install can start, save, quit, relaunch, continue, and finish the
  current playable loop using keyboard or controller.

### Milestone 18: Demo World, Quest, And Boss Content

Goal: build the complete demo from start to finish using functional placeholder
art where final art is not ready.

- [ ] Build all eight Suncrest Hollow district scenes using the connection
  graph, dimensions, and transition contract in
  `SUNCREST_DISTRICT_LAYOUT.md`.
- [ ] Build the remaining final demo scene list from the Milestone 14 brief.
- [ ] Add NPC schedules or placement only where the demo requires them.
- [ ] Add dialogue data, choices if required, and quest progression.
- [ ] Add the delayed-caravan opening quest; `A Worried Merchant`, `The Smith
  And The Missing Scout`, and `The Guard Captain's Warning`; and the four
  optional quests in `DEMO_SIDE_QUESTS.md` with tested one-time state.
- [ ] Add Lysander's three-quest recruitment threshold, forced exit scene, and
  party-management tutorial without save/load softlocks.
- [ ] Add encounter selection, enemy groups, and progression pacing.
- [ ] Add the Grassland Goblin Boss and Cherry Blossom Great Stag with
  their required battle phases and scripted rules.
- [ ] Add checkpoints, recovery behavior, and a demo ending flow.
- [ ] Add the reusable Ember Camp scene, `First Fire`, and `Why This Party`.
- [ ] Complete an internal start-to-finish playthrough with no debug shortcuts.

Exit gate:

- The entire demo is content-complete and playable with placeholders.
- No required story beat, map, battle, or progression step is missing.

### Milestone 19: Demo Art, Audio, UI, And Store-Ready Identity

Goal: replace prototype presentation with a cohesive public-facing vertical
slice.

- [ ] Lock the pixel-art palette, sprite scale, animation dimensions, and UI
  style guide.
- [ ] Clear or replace the working `Rawr! Studios` brand before commissioning
  public logo, capsule, domain, or storefront identity assets.
- [ ] Complete signed contributor agreements and asset-by-asset clearance
  records for every production asset used in external builds.
- [ ] Replace the movement indicator and colored encounter squares.
- [ ] Add final demo character, enemy, environment, portrait, item, and battle
  assets.
- [ ] Add readable exploration, dialogue, menu, inventory, job, and battle UI.
- [ ] Give Ember Camp a cohesive final environment and readable companion-event
  presentation without duplicating Suncrest Hollow's services.
- [ ] Add battle feedback, transitions, damage/healing feedback, and status
  icons.
- [ ] Add demo music, ambience, and sound effects with verified usage rights.
- [ ] Add credits and third-party license notices.
- [ ] Capture honest gameplay screenshots and video from the current build.
- [ ] Prepare title treatment, capsule art, descriptions, and feature claims
  that match implemented content.

Exit gate:

- The demo has no visible test-room presentation in the intended player path.
- Screenshots and footage accurately represent the playable build.

### Milestone 20: Demo Integration, Balance, And QA

Goal: turn the content-complete vertical slice into a dependable public build.

- [ ] Freeze demo features and triage all known defects.
- [ ] Balance jobs, encounters, boss difficulty, rewards, shops, and healing.
- [ ] Test new game, save migration, continue, defeat, escape, and demo ending.
- [ ] Test supported resolutions, aspect ratios, keyboard, and controllers.
- [ ] Test clean-machine installation and first launch.
- [ ] Profile load times, memory, scene transitions, and common frame spikes.
- [ ] Add logging suitable for diagnosing tester reports without collecting
  personal data by default.
- [ ] Run external playtests and record completion time, confusion points,
  defects, and balance feedback.
- [ ] Fix all critical and high-severity demo issues.
- [ ] Produce a versioned demo release candidate and archive its source commit.

Exit gate:

- Multiple external players can finish without developer assistance.
- Save data survives normal use and upgrades from the previous demo test build.
- There are no known critical defects or progression blockers.

### Milestone 21: Steam Demo Release

Goal: publish the vertical slice as a correctly configured Steam demo and use it
to validate player interest and production assumptions.

- [ ] Complete Steamworks onboarding and the Steam Direct app setup when the
  title, ownership, tax, banking, and public identity are ready.
- [ ] Configure the base game store presence with truthful feature claims.
- [ ] Create and link the separate demo App ID.
- [ ] Configure depots, launch options, supported operating systems, controller
  support, accessibility fields, and content survey accurately.
- [ ] Upload and test the demo through the Steam client.
- [ ] Complete required store and build checklists.
- [ ] Submit store presence and builds with time reserved for review feedback.
- [ ] Publish a Coming Soon page when the visual identity and feature set are
  stable enough for public wishlists.
- [ ] Prepare demo-specific screenshots, description, capsule/library assets,
  trailer, support contact, and privacy disclosures where applicable.
- [ ] Create a launch and feedback plan for announcements, bug reports, and
  updates.
- [ ] Tag and archive the released demo build.

Steam Cloud and achievements are valuable candidates, but neither should block
the first demo unless the demo design or save-transfer plan genuinely needs
them.

Exit gate:

- The demo installs, launches, saves, and completes through the public Steam
  client configuration.
- Store claims, screenshots, supported features, and the shipped build agree.
- Feedback is collected into actionable production decisions.

## Full-Game Milestones

### Milestone 22: Demo Feedback And Production Lock

- [ ] Analyze completion, balance, usability, technical, and audience feedback.
- [ ] Decide what to keep, change, cut, or prototype further.
- [ ] Lock the full-game feature set, content budget, target platforms, and
  production schedule.
- [ ] Decide whether Early Access fits the actual production and community plan;
  do not use it only as a substitute for finishing the game.
- [ ] Establish release, pricing, localization, marketing, and support plans.

### Milestone 23: Full-Game Production

- [ ] Expand the story, world, quests, party roster, jobs, enemies, bosses,
  items, equipment, shops, music, and art using proven demo pipelines.
- [ ] Add full-game save migration and content validation tools.
- [ ] Add localization-ready text storage before large dialogue production.
- [ ] Maintain playable chapter gates instead of integrating everything only at
  the end.
- [ ] Run recurring automated, build, controller, and external playtests.

### Milestone 24: Release Candidate And Steam Launch Readiness

- [ ] Reach content complete, then feature freeze.
- [ ] Complete localization, accessibility review, credits, legal, and licenses.
- [ ] Complete performance, compatibility, save migration, and clean-install QA.
- [ ] Finalize store assets, pricing, release date, support process, and launch
  communications.
- [ ] Submit a near-final build and store presence for Valve review with at least
  the recommended lead time.
- [ ] Maintain the required Coming Soon period before release.
- [ ] Archive and sign off the release candidate before pressing Release.

### Milestone 25: Launch And Post-Launch Support

- [ ] Release deliberately through Steamworks and verify the live package.
- [ ] Monitor crash reports, support requests, save issues, and severe defects.
- [ ] Ship focused hotfixes without breaking existing saves.
- [ ] Publish clear patch notes and maintain a tested rollback plan.
- [ ] Review player feedback without allowing every request to redefine scope.
- [ ] Decide on post-launch updates only after the base game is stable.

## Quality Gates For Every Milestone

- Define the player-visible outcome before implementation begins.
- Keep gameplay rules independent from scene presentation where practical.
- Add automated coverage proportional to behavioral risk.
- Complete required Play Mode and build checks before marking the gate done.
- Record manual Unity wiring and automate repeated setup.
- Update `ROADMAP.md`, `TESTING.md`, and `PROJECT_HANDOFF.md` at each gate.
- Commit focused work and push milestone branches so another machine can resume.
- Do not mark a milestone complete while required work remains hidden in chat.

## Branching Strategy

- Finish the current `feature/core-combat-expansion` branch, push it, and merge
  verified work before starting the vertical slice.
- Use one branch per milestone or tightly scoped slice.
- Start dependent branches from the latest integrated branch, not an old `main`.
- Keep code, tests, generated scene changes, and documentation reviewable.
- Tag public demo and full-game release candidates.
- Keep generated Unity and IDE files ignored.

## Scope Control

- A feature enters the demo only if it supports the demo player promise.
- Prefer a small complete content set over many incomplete systems.
- Do not build full-game quantities of art or dialogue before the vertical slice
  validates the pipeline and presentation.
- New ideas go into a post-demo backlog unless they replace an agreed demo item.
- Placeholder assets are acceptable during implementation, but not in the
  intended public demo path unless clearly deliberate and presentation-ready.

## Steam Planning References

Current platform details must be rechecked against official Steamworks
documentation when the relevant milestone begins:

- Steam Direct fee and onboarding:
  `https://partner.steamgames.com/doc/gettingstarted/appfee`
- Store and build review:
  `https://partner.steamgames.com/doc/store/review_process`
- Coming Soon page:
  `https://partner.steamgames.com/doc/store/coming_soon`
- Demo applications:
  `https://partner.steamgames.com/doc/store/application/demos`
- Release process:
  `https://partner.steamgames.com/doc/store/releasing`

As of July 2026, Valve documents a $100 USD Steam Direct fee for each new product
submitted through Steam Direct, typically 3-5 business days for store/build
review with at least 7 business days recommended, and a minimum two-week Coming
Soon period before a full release. A Steam demo uses a separate App ID linked to
the base game and has its own configuration and release checklist.
