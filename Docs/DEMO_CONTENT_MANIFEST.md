# Demo Content Manifest

This manifest lists the content required for the vertical-slice demo from new
game through the Cherry Blossom ending. It consolidates the authoritative demo
design documents into a production checklist. It does not replace their quest,
combat, dialogue, or system rules.

## Scope Rules

- Typical first-playthrough target: at least 90 minutes.
- Critical-path progress is never gated by side quests, grinding, or elapsed
  playtime.
- Functional placeholder art is acceptable through Milestone 18. Public-demo
  assets and presentation belong to Milestone 19.
- Map areas below are required content boundaries. Whether connected areas use
  one Unity scene, additive scenes, or separate scenes remains a technical
  production choice.
- Exploration shows one controllable party leader. Companions do not form a
  follower train in the demo; they appear in battles and at authored scene
  anchors for recruitment, camp, inn, and story moments.
- All 12 capped demo job trees and all four optional side quests are included.

## Required Scene Systems

- `MainGameScene`: title, new game, and protagonist creation.
- Exploration scene controller: physical movement, collision, interactions,
  encounters, dialogue, quest state, and explicit return positions.
- `BattleScene`: reusable party-versus-enemy battle presentation.
- Ember Camp scene: reusable presentation for `camp_cherry_road` and its two
  demo companion events.
- Job and party-management interfaces may be overlays or dedicated UI scenes,
  but their gameplay state must remain independent from presentation.

Production scene names beyond the existing prototypes should be selected when
the maps are created. Stable location and encounter IDs must not depend on
Unity scene names.

## Maps And Areas

### Suncrest Hollow

The home hub contains eight interconnected districts plus its Grassland exit:

- Whisper Market: Old Marlow, basic goods, and Quest 1 briefing/turn-in.
- Ironforge Smithy: Bren, equipment service, and Quest 2 briefing/turn-in.
- Guild Hall: quest board, job service, and Lysander's pre-recruit position.
- The Suncrest Inn: rest service, party scenes, and `A Taste Of Home`.
- The Sunwell Shrine: recovery service and `The Roadside Chimes`.
- Amber Row: residential space and ordinary town life.
- The Sunroot Grove: town green and subtle, uncommented decay hints.
- Suncrest Watch: Captain Vashti, Quest 3, and the Goblin Boss handoff.
- Grassland exit: Lysander's mandatory recruitment interception after the
  required Quest 3 handoff and three-quest threshold.

Required interior presentation exists for the Guild Hall, inn, smithy, shrine,
and Watch. These may be compact interiors or readable district structures
rather than five large standalone maps.

### Grassland

Required connected field content:

- Caravan attack site and mandatory tutorial battle trigger.
- Four Quest 1 investigation spots: two goblin skirmishes and two evidence
  interactions.
- One hidden chest, scenic overlook, and stranded traveler near the Quest 1
  search space.
- All four investigation spots remain available until turn-in even though only
  two are required for completion.
- Four Quest 2 trail clues and Tallis's collapsed-platform rescue location.
- Three independently approachable Quest 3 scout-rescue locations.
- Three Meadow Sage pickups for `A Taste Of Home`.
- Three roadside chimes for `The Roadside Chimes`.
- Wildflower ridge, old stone bridge, and quiet pond viewpoints for
  `A Painter's View`.
- Eligible open exploration space for step-triggered ambient encounters.
- Goblin Boss approach, intelligence-resolution points, war horn, and arena.
- Locked route toward Cherry Blossom until the Goblin Boss is defeated.

### Cherry Blossom Route And Ember Camp

- Road from Suncrest Hollow toward the nomadic settlement.
- Required designated campsite `camp_cherry_road`.
- First free tutorial rest and `camp_event_demo_first_fire`.
- Optional Lysander event `camp_event_demo_why_this_party`.
- Exact return point preserving the party's expedition position.

### Cherry Blossom Settlement And Grove

- Compact nomadic settlement with limited trade and recovery services.
- Sacred tree/grove as the settlement's central landmark and problem source.
- Out-of-season blossoms, still-air petals, and uneasy NPC presentation without
  visible corruption or a confirmed explanation.
- Three grazing-animal interactions and tracks for
  `Where The Herd Won't Graze`.
- Great Stag approach and dedicated regional boss arena.
- Closing scene after the Great Stag is subdued.
- Physical progression barrier beyond the grove marking the demo endpoint.

## Required NPC Set

### Named Or Stable Story Roles

- Player-created protagonist with stable save identity independent of name.
- Iona (`pc_01`), Damari (`pc_02`), Enora (`pc_03`), and Lysander (`pc_04`).
- Old Marlow in Whisper Market.
- Bren at Ironforge Smithy.
- Captain Vashti at Suncrest Watch.
- Tallis, the injured Quest 2 scout.
- Three Quest 3 scouts with separate stable IDs; final names remain open.
- Suncrest Inn innkeeper; final name remains open.
- Sunwell Shrine caretaker; final name remains open.
- Traveling Grassland painter; final name remains open.
- Cherry Blossom herder; final name remains open.
- Cherry Blossom settlement leader or elder; final name remains open.
- Opening caravan survivor or driver; final identity remains open.
- Opening quest issuer; the role is required even though the final NPC or quest
  board assignment remains open.

The Guild Hall quest board is an interactable object and does not require a
dedicated clerk for demo functionality.

### Supporting Population

- One stranded Grassland traveler near the Quest 1 search area.
- Reusable caravan merchants, guards, and townspeople for opening and return
  scenes.
- A small set of Suncrest Hollow background residents distributed across all
  eight districts.
- A small set of Cherry Blossom residents whose dialogue expresses uncertainty
  without knowledge of the world's larger problem.
- Three fixed grazing-animal presentations; no escort or follower AI.

Background NPCs may share animation sets and portraitless dialogue. Final
population counts belong to map composition and performance testing.

## Companion Use

- The protagonist explores as the single visible leader.
- Iona joins during the tutorial damage checkpoint and introduces healing.
- Damari and Enora join during the Hobgoblin sequence.
- The protagonist, Iona, Damari, and Enora become the standing party after the
  tutorial and appear in the Suncrest job introduction.
- Lysander loiters near Guild Hall before recruitment, intercepts the player at
  the Grassland exit after the threshold, and triggers party management.
- All recruited companions appear in battle according to the four active slots.
- Authored party sprites may appear at fixed anchors during camp, inn, quest,
  and boss scenes. They do not pathfind behind the protagonist.
- Benched companions remain available for JP, camp, equipment, and party
  management under the existing roster rules.

## Quest Content

### Required Story Flow

- Delayed caravan opening and unskippable tutorial battle.
- Three guided main quests: `A Worried Merchant`,
  `The Smith And The Missing Scout`, and `The Guard Captain's Warning`.
- Lysander's three-quest reputation threshold and forced recruitment scene.
- Grassland Goblin Boss unlock and fight.
- Suncrest Hollow celebration and Suncrest Inn respite.
- Overdue Cherry Blossom caravan request and required Ember Camp introduction.
- Settlement investigation, Great Stag fight, unresolved closing scene, and
  demo endpoint.

### Required Optional Quests

- `A Taste Of Home`.
- `The Roadside Chimes`.
- `A Painter's View`.
- `Where The Herd Won't Graze`.

All quest objectives, rewards, and completion states use stable IDs and resolve
exactly once across save/load. The four side quests are not deferred content.

## Jobs And Party Progression

- All 12 jobs in `DEMO_JOB_TREES.md` are assignable to every playable
  character after the job tutorial.
- Every tree includes its free core kit, three purchased job abilities or
  passives, permanent stat tracks, Tier 3 mastery, and `10 JP` demo maximum.
- Permanent stat nodes remain active across job switches. Inactive-job traits,
  actions, and passives remain unavailable.
- Job assignment and node purchases occur through the Guild Hall job service.
- Party formation becomes available when Lysander creates a five-member roster.
- Ember Camp permits party formation but not job assignment or node purchases.

## Items, Equipment, And Services

### Required Named Items

- `Camp Ration` (`item_camp_ration`).
- Bren-marked trail knife fragment as non-sellable Quest 1 evidence.
- Marlow's Trade Charm.
- Ironforge Fieldguard.
- Suncrest Supply Kit reward bundle.
- Suncrest Watch Insignia.
- Suncrest Supper recovery consumable.
- Traveler's Tonic recovery consumable.
- Nomad's Woven Cord.
- Cherry Blossom settlement tea recovery consumable.
- Enora-compatible scythe weapon type and starting scythe.

### Required Item Families

- Single-target HP recovery.
- Single-target MP recovery.
- Removable-status recovery.
- Incapacitated-ally recovery if ordinary battles are balanced around access to
  revival; otherwise revival remains a town/camp service for the demo.
- Basic protagonist and companion starting weapons, armor, and accessories.
- Small weapon and armor upgrades supporting physical, magical, tank, healer,
  and support builds without requiring one item for every job.

Final names, statistics, prices, quantities, weapon restrictions, inventory
capacity, and loot tables remain Milestone 16 decisions.

### Suncrest Hollow Services

- Whisper Market: buy and sell basic consumables and Camp Rations after camp is
  introduced.
- Ironforge Smithy: buy and sell the demo weapon and armor set.
- Guild Hall: quest board, job assignment, job-node purchases, and party help.
- Suncrest Inn: paid full recovery plus authored party scenes.
- Sunwell Shrine: recovery and removable-status service, including the one free
  side-quest reward use.
- Suncrest Watch: story briefing and boss authorization, not a general shop.

### Cherry Blossom Services

- One limited trader for basic recovery supplies and settlement tea.
- One modest recovery option appropriate to the settlement.
- No smithy, job service, quest board, broad equipment shop, or recruitment.

## Enemies And Encounters

### Grassland And Tutorial

- Goblin Skirmisher (`monster_goblin_skirmisher`).
- Goblin Slinger (`monster_goblin_slinger`).
- Goblin Guard (`monster_goblin_guard`).
- Goblin Raider (`monster_goblin_raider`).
- Tutorial Hobgoblin (`monster_tutorial_hobgoblin`).
- Regional Goblin Boss (`boss_grassland_goblin`).
- Slime (`monster_slime`).
- Wild Boar (`monster_wild_boar`).

Role variants require distinct data and readable battle presentation, but may
share a goblin base sprite with equipment, palette, or silhouette changes.

### Cherry Blossom

- Great Stag regional boss with subdued outcome and no death presentation.
- At least two ordinary ambient encounter roles if Cherry Blossom exploration
  testing needs combat pacing before the boss. Their identities remain open and
  must stay grounded; no enemy may explain or embody the later corruption.

Cherry Blossom ambient encounters are the only unresolved enemy-count decision.
The Great Stag itself is already named and mechanically defined.

## UI Inventory

- Title, new-game confirmation, and protagonist creation.
- Exploration HUD and pause menu.
- Dialogue box with speaker presentation and configurable text speed.
- Quest journal supporting posting, briefing, objectives, completion, and
  turn-in states.
- Battle commands, ally/enemy targeting, statuses, telegraphs, rewards, and
  subdued-victory presentation.
- Party formation and bench management.
- Job assignment, tree nodes, JP, affinity guidance, permanent-stat labels, and
  demo-cap presentation.
- Inventory, equipment comparison, item use, buy, and sell.
- Inn, shrine, and camp recovery confirmations.
- Ember Camp actions and companion-talk selection.
- Region transition or route selection presentation.
- Save/load, settings, controller navigation, and demo-ending flow.

## Preliminary Art And Audio Budget

This is a minimum planning budget, not a final outsourcing quote.

| Category | Minimum demo content |
| --- | --- |
| Exploration tilesets | Suncrest Hollow, Grassland, Cherry Blossom, plus compact interiors/camp variants |
| Major environment set pieces | Sunroot Grove detail, caravan site, collapsed platform, goblin arena, sacred tree, Great Stag grove, demo barrier |
| Playable character sets | Protagonist plus Iona, Damari, Enora, and Lysander |
| Playable portraits | Protagonist presentation plus 4 companion portraits |
| Named/support NPC field sets | Approximately 15 roles, with shared animation and background sets allowed |
| Named NPC portraits | Old Marlow, Bren, Captain Vashti, settlement leader; additional portraits budget permitting |
| Enemy data roles | 8 Grassland/tutorial roles, Great Stag, and up to 2 Cherry ambient roles |
| Enemy visual bases | Base goblin plus variants, Hobgoblin, Goblin Boss, Slime, Wild Boar, Great Stag, and optional Cherry ambient bases |
| Required music loops | Title/menu, Suncrest Hollow, Grassland, normal battle, boss, Cherry Blossom, Ember Camp, ending/quiet scene |
| Core SFX groups | UI, footsteps/interactions, weapons, physical impacts, magic/healing, statuses, enemies, recovery/camp, transitions |
| Required UI surfaces | Approximately 12 groups listed in the UI inventory |
| Authored quest sequences | Opening, 3 main quests, 4 side quests, Lysander recruitment, 2 regional boss arcs, camp introduction, ending |

Protagonist preset count, animation frame counts, dialogue line count, final
portrait coverage, and unique versus shared boss music remain budget decisions.

## Explicit Demo Cut List

- Teleport crystals and full-game fast travel.
- Follower-train companion pathfinding in exploration.
- Job-tree nodes beyond each demo cap and any full-game cross-job skill system.
- Node refunds/respec service.
- Full Ember Camp regional variation, meals, crafting, supply simulation, and
  deeper relationship systems.
- Regions beyond Cherry Blossom and content beyond the closing barrier.
- Mid-story permanent companion removal and its scene implementation.
- Later characters and entities including Elenya, Nyra, Kharos, the Guardian,
  and Valtar.
- Visible world-corruption states, the true cause of decay, and later reveals.
- Cherry Blossom's future changed-state map.
- Escort AI, real-time quest failure, fishing, crafting minigames, and bespoke
  side-quest minigames.
- Large freeform protagonist customization beyond the final limited presets.
- Steam achievements, trading cards, Workshop support, and post-demo content.

All 12 demo jobs and the four authored optional side quests are required and
must never be placed on the cut list without an explicit scope change.

## Remaining Decisions

- Clearance or replacement of the working `Rawr! Studios` public brand.
- Exact Unity scene segmentation and production scene names.
- Final protagonist customization categories and preset count.
- Final names, appearances, portraits, and dialogue for unnamed NPC roles.
- Whether Old Marlow or Captain Vashti gives the Cherry Blossom caravan request
  and final Goblin Boss authorization wording.
- Final item statistics, prices, quantities, drop rates, rewards, and
  JP/XP/gold tuning under the locked economy structure.
- Whether Cherry Blossom needs two ambient enemy roles after route playtesting.
- Shared versus unique Hobgoblin, Goblin Boss, and Great Stag music.
- Final map coordinates, population density, animation lists, dialogue count,
  and protagonist/companion art scope.
- Completed contributor agreements and asset-by-asset clearance entries as
  production assets are selected or created.

## Source Documents

- `DEMO_STORY_REFERENCE.md`
- `DEMO_QUEST_FLOW.md`
- `DEMO_MAIN_QUEST_CONTENT.md`
- `GRASSLAND_COMBAT_DESIGN.md`
- `DEMO_CHERRY_BLOSSOM_ARC.md`
- `CHERRY_BLOSSOM_COMBAT_DESIGN.md`
- `DEMO_SIDE_QUESTS.md`
- `CAMP_SYSTEM_DESIGN.md`
- `DEMO_JOB_TREE_SCOPE.md`
- `DEMO_JOB_TREES.md`
- `JOB_CLASS_SYSTEM.md`
- `MILESTONE_14_DECISIONS.md`
- `PRODUCT_IDENTITY.md`
- `PLAYER_PROMISE_FEATURE_HIERARCHY.md`
- `ITEM_EQUIPMENT_ECONOMY.md`
- `CONTRIBUTOR_AGREEMENT_TEMPLATE.md`
- `ASSET_LICENSE_TRACKING.md`
