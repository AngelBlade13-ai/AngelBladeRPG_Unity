# Cherry Blossom Great Stag Combat Design

This document defines the symbolic, visual, mechanical, and narrative contract
for Cherry Blossom's regional boss. Numeric statistics remain provisional until
Milestone 15 supports the complete active party and multi-target actions.

## Identity

- Stable combatant ID: `boss_cherry_great_stag`.
- Display name: `The Great Stag`.
- Encounter shape: one powerful enemy with no supporting creatures.
- Story role: a traditionally protective territorial animal distressed by the
  sacred tree's broken seasonal rhythm.

The Great Stag is not corrupted, possessed, evil, or responsible for the
region's problem. The demo does not identify the force affecting the tree or
the animal.

## Symbolism And Visual Direction

The stag represents the natural cycle of seasons, renewal, and regrowth. Its
antlers echo tree branches and the natural shedding-and-return cycle that the
sacred tree is no longer following correctly.

- Give it large branch-like antlers carrying cherry buds, open blossoms, and
  falling petals at the same time.
- Keep the animal visibly alive, powerful, exhausted, and distressed.
- Avoid glowing corruption, black residue, wounds caused by an identified
  villain, or other imagery that confirms a supernatural explanation.
- Its movement and sound should communicate fear and territorial panic more
  than cruelty.

## Battle Outcome

Reducing the Great Stag to zero HP subdues it rather than killing it.

- The battle returns a normal victory for progression, XP, and one-time state.
- Presentation uses a collapse, exhausted kneel, or calming animation instead
  of a death animation.
- The creature remains alive for the closing scene and eventually withdraws or
  rests near the sacred tree.
- It does not leave ordinary monster-body loot. Any material reward comes from
  the settlement or demo-ending flow.
- The sacred tree remains unchanged after the battle.

The runtime battle outcome should distinguish `Subdued` presentation from an
ordinary enemy death without creating a second reward path that can duplicate
victory rewards.

## Phase 1: Territorial Control

The opening phase feels deliberate and powerful.

### Antler Strike

- Reliable single-target physical attack.
- Establishes the baseline damage expected from the boss.

### Sweeping Antlers

- Moderate physical damage to the active party.
- Creates healing pressure without immediately threatening a full-party defeat.

### Pawing Earth

- Clearly marks one party member as the target of the next Great Charge.
- The battle UI and log must identify the marked target.
- The Stag's next action becomes Great Charge unless the battle has ended.

### Great Charge

- Heavy physical damage to the marked target.
- A taunt or explicit protection action may redirect the marked target before
  the charge resolves.
- If the final target is Defending, incoming damage is reduced and the collision
  leaves the Great Stag **Staggered** for one round.
- If the target is not Defending, the attack deals its normal heavy damage and
  does not automatically stagger the boss.

### Staggered

- Temporarily reduces the Great Stag's defense and speed.
- Creates a burst opportunity earned by reading and answering the telegraph.
- Ends after one complete round and cannot stack with itself.

This marked-charge sequence tests threat control, Defend, healing preparation,
and burst timing more directly than the Goblin Boss's fixed Brutal Swing tell.

## Phase 2: Panicked

At low HP, the Great Stag enters **Panicked** exactly once.

- Increase attack and speed.
- Reduce defense to represent increasingly reckless movement.
- Retain the marked-charge sequence, but use it less predictably among faster
  attacks.
- Add **Frantic Bound**, two lighter strikes against different active party
  members when at least two valid targets remain.
- Keep Sweeping Antlers available so party-wide recovery still matters.

The phase should feel erratic and sad, not like the Stag has revealed an evil or
magical true form.

## Communication Requirements

- Announce the marked charge target by name and with a non-color-only UI cue.
- Explain when taunt or protection redirects the charge.
- Report the reduced damage and Staggered state when Defend answers the charge.
- Announce the Panicked transition without calling it corruption or possession.
- Use a subdued victory message rather than text saying the party killed the
  Great Stag.

## Test Requirements

- Pawing Earth records one valid target and schedules Great Charge.
- Taunt or protection redirects the pending charge correctly.
- Defending reduces charge damage and applies Staggered exactly once.
- Staggered expires after one round and cannot stack.
- Panicked triggers once when its HP threshold is crossed, including when one
  attack crosses the threshold by a large amount.
- Frantic Bound selects two distinct living targets when possible and degrades
  safely to one target otherwise.
- Reaching zero HP produces one subdued victory and one reward grant.
- Save or checkpoint restoration cannot replay the victory or closing scene.

## Remaining Balance And Production Decisions

- Exact HP threshold for Panicked.
- Action-selection weights and charge frequency.
- Damage, speed, defense, accuracy, XP, and reward values.
- Final sprite proportions, animation list, sound design, and battle background.
- Exact settlement reward and post-battle Stag animation.
