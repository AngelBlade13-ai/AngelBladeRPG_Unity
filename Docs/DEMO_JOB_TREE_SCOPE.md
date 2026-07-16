# Demo Job Tree Scope

This document defines how the full job catalog appears in the vertical-slice
demo. It supplements `JOB_CLASS_SYSTEM.md`; it does not define final abilities,
balance values, or the full-game job trees.

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
- Changing jobs does not erase learned nodes or job progress.
- Reaching one job's limit does not prevent assigning that job or progressing
  another job.
- The demo limit prevents purchasing deeper nodes. It does not pretend that the
  character has completed the eventual full-game tree.
- Full-game development should extend the same tree and stable IDs instead of
  replacing demo progression with a separate system.

The planning baseline is a compact early slice containing a job's defining
trait, a signature active ability, and at least one additional role-defining
choice or upgrade. Exact shapes and maximum tiers may differ by job when that
produces a clearer or more balanced demo. Those limits must still be authored
as data and covered by tests.

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

## Progression Questions Still Open

- The exact nodes and progression limit for each of the 12 jobs.
- How job points are earned, priced, and banked after a character reaches a
  demo limit.
- Whether respecing learned nodes is free, paid, or unavailable in the demo.
- Whether a character earns progress only for the equipped job or through an
  additional shared source.
- The exact lifetime, control rules, and action economy for the demo summon.

These questions should be resolved together so point pacing is based on the
number and cost of real nodes rather than an arbitrary level cap.

## Required Tests

- Every catalog job can be assigned to the protagonist and every companion.
- Low affinity never blocks assignment or node purchase.
- Each job enforces its authored demo progression limit.
- Switching jobs preserves learned nodes and job-specific progress.
- Progress belongs to persistent character IDs rather than active-party slots.
- Stable job and node IDs survive display-name changes.
- Invalid, duplicate, or cyclic node definitions fail validation.

