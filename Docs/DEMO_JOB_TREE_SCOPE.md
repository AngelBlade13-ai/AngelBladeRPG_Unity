# Demo Job Tree Scope

This document defines how the full job catalog appears in the vertical-slice
demo. It supplements `JOB_CLASS_SYSTEM.md`; it does not define final abilities,
balance values, or the full-game job trees.

The approved early nodes, shared JP structure, and per-job demo endpoints are
defined in `DEMO_JOB_TREES.md`.

## Demo Promise

- All 12 catalog jobs are available as playable jobs in the demo.
- The player-created protagonist and every recruited companion may use any job.
- Character-job affinity affects effectiveness but never unlocks, hides, or
  forbids a job.
- Each job exposes a meaningful early section of its progression tree and then
  stops at a clearly communicated demo progression limit.
- Jobs must be usable in representative demo battles, not merely visible in a
  menu or labeled as coming later.

The demo catalog is Knight, Reaver, Mercenary, Rogue, Ranger, Mage, Blood Mage,
White Mage, Paladin, Bard, Tactician, and Summoner.

## Progression Limit

The progression limit belongs to the job definition. It must not be a single
hardcoded check that assumes every tree has the same shape.

- Give every job a stable ID and an explicit demo maximum tier or equivalent
  node boundary.
- Store unlockable nodes under stable IDs so later display-name changes do not
  invalidate saves.
- Track learned job nodes per persistent character and per job, not on the
  current party slot.
- Changing jobs does not erase learned nodes, job progress, or permanent stat
  bonuses purchased in other job trees.
- Reaching one job's limit does not prevent assigning that job or progressing
  another job.
- The demo limit prevents purchasing deeper nodes. It does not pretend that the
  character has completed the eventual full-game tree.
- Full-game development should extend the same tree and stable IDs instead of
  replacing demo progression with a separate system.

The approved demo slice contains a job's defining trait and signature action,
three purchased job abilities or passives, two two-rank permanent stat tracks,
and one permanent mastery stat node. Exact full-game shapes may differ by job,
but demo limits remain authored as data and covered by tests.

## Presentation

- The job-assignment screen should show every available job from the start of
  the job tutorial.
- The UI must distinguish a demo progression limit from an unmet gameplay
  prerequisite.
- Do not display a large grid of nonfunctional full-game nodes merely to imply
  future depth. Show the playable demo tree and its endpoint clearly.
- Affinity may be shown as guidance, but the UI must not present low affinity
  as an access restriction.

## Production Boundaries

Making all 12 jobs playable is a meaningful content commitment. Before
Milestone 15 implementation expands beyond contracts, define for every job:

- Its granted trait or baseline rule.
- Its demo active abilities and passive nodes.
- Node prerequisites, costs, and demo maximum.
- Required resources, targets, status effects, and battle UI behavior.
- At least one intended strength and one observable trade-off.

Summoner is part of the demo catalog. Its first slice should use the smallest
reliable summon behavior that demonstrates the job without requiring a general
pet AI or creature-collection system.

## Resolved Progression Rules

- Each tree grants a free core trait and action, contains three purchased job
  abilities or passives, two two-rank permanent stat tracks, and ends at a
  Tier 3 permanent mastery node. A complete demo tree costs `10 JP`.
- JP is an unspent per-character currency that may be spent on any job and is
  retained when a job reaches its demo limit.
- Eligible victories grant JP to every available recruited character,
  including benched characters; exact encounter awards remain balance data.
- Learned nodes remain purchased when switching. Stat-node bonuses from every
  learned job remain active, while inactive-job traits, actions, and passives
  become unavailable. Job switching is free at the Suncrest Hollow job service;
  node refunds are not included in the demo.
- The Ember Wisp is a temporary auxiliary combatant with fixed behavior rather
  than a persistent pet or general-purpose AI character.

Exact combat formulas, MP and HP costs, effect durations, and encounter JP
awards remain balance work after multi-party simulation is available.

## Required Tests

- Every catalog job can be assigned to the protagonist and every companion.
- Low affinity never blocks assignment or node purchase.
- Each job enforces its authored demo progression limit.
- Switching jobs preserves learned nodes and job-specific progress.
- Switching jobs retains all learned permanent stat bonuses while disabling
  traits, actions, and passives belonging to inactive jobs.
- Permanent stat totals are derived once from unique learned node IDs after
  loading and cannot be duplicated by repeated job switches.
- Inactive-job skills and job-specific passives cannot be selected or applied
  even when their nodes remain learned.
- Buying Max HP or Max MP nodes increases the corresponding current value once;
  loading and switching jobs cannot repeat that resource gain.
- Node purchases reject missing prerequisites or insufficient JP without
  partially spending currency or applying stats.
- Progress belongs to persistent character IDs rather than active-party slots.
- Stable job and node IDs survive display-name changes.
- Invalid, duplicate, or cyclic node definitions fail validation.
