# Demo Job Trees

This document defines the first playable progression slice for all 12 demo
jobs. `DEMO_JOB_TREE_SCOPE.md` remains authoritative for access and persistence
rules. Numeric damage, healing, duration, and MP values remain balance data to
be tuned after multi-character combat simulation exists.

## Shared Tree Structure

Every demo job uses the following tree shape:

- **Core:** one trait and one signature action are granted while the job is
  equipped. They cost no Job Points.
- **Tier 1:** two job-skill nodes and two Rank I stat nodes cost `1 JP` each.
- **Tier 2:** the job-skill endpoint costs `2 JP` and requires both earlier
  skill nodes. Two Rank II stat nodes cost `1 JP` and require their matching
  Rank I nodes.
- **Tier 3:** one mastery stat node costs `2 JP` and requires both Rank II stat
  nodes.
- **Demo maximum:** Tier 3. Completing a demo tree costs `10 JP` total.

Each job definition stores its own `DemoMaximumTier` even though the initial
catalog uses Tier 3 for all jobs. This allows later tuning without changing the
progression contract. A full-game tree extends beyond these stable nodes.

## Job Points And Switching

- Job Points are an unspent currency stored per persistent character.
- Every available recruited character receives JP from eligible victories,
  including benched characters. Exact encounter awards are balance data.
- JP may be spent on any job tree regardless of the character's equipped job or
  affinity.
- Nodes may be purchased only through the Suncrest Hollow job service. This
  supports the intended hub-return loop and prevents mid-battle stat changes.
- Purchased nodes remain learned when switching jobs. The demo does not include
  node refunds.
- Changing an equipped job is free but is only available through the Suncrest
  Hollow job service. Ember Camp does not allow job changes.
- Reaching a tree's demo maximum never wastes future JP; unspent JP remains on
  the character for another job.
- Core traits, core actions, purchased job abilities, and purchased job-specific
  passives apply only while their job is equipped.
- Purchased stat nodes from every job remain active after switching jobs and
  combine with ordinary level growth. Switching jobs never acts as a respec.

This model lets players test any job immediately, rewards continued use without
locking them into one path, and avoids requiring twelve separate job XP bars.

## Permanent Stat Nodes

Permanent stat nodes are the cross-job character-building layer. A character
may train as a Knight for durability, then equip Mage and retain that earned HP
and defense while losing access to Knight traits and actions.

- Stat bonuses are flat additions and are granted once per learned stable node
  ID. Affinity does not change their value.
- Effective stats are calculated from base level stats, all learned permanent
  stat nodes, equipment, the equipped job's modifiers, then temporary battle
  effects. Clamp bounded stats only after all permanent additions are summed.
- Derived stat totals are not saved independently. Saves store learned node IDs
  and recalculate bonuses, preventing duplicate application after loading.
- The tree UI labels permanent stat nodes separately from equipped-job skills
  and previews which actions become unavailable before a job switch.
- A future explicit respec may refund nodes and remove their stat bonuses, but
  changing jobs alone never refunds JP or removes permanent training. Respec is
  outside the demo scope.
- When respec is implemented, prefer an atomic whole-job reset: remove that
  job's learned purchased nodes, remove its permanent bonuses, refund its spent
  JP, and clamp current HP or MP to recalculated maxima. Whether the service has
  a gold cost remains a later economy decision.
- Bonuses are intentionally modest because one character may eventually learn
  permanent nodes from many jobs. Demo caps and one-time node purchases prevent
  unlimited stat farming.
- Purchasing a Max HP or Max MP node increases both the maximum and current
  value by the listed amount; it preserves missing resources rather than fully
  restoring the character. Loading recalculates maxima, then clamps saved
  current values without granting another increase.
- Full-game balance must budget against the maximum combined permanent bonuses
  available across every tree, not treat each job's totals in isolation.

### Completed Demo-Tree Stat Totals

| Job | Permanent gain after all five stat nodes |
| --- | --- |
| Knight | `30 Max HP`, `3 Defense` |
| Reaver | `30 Max HP`, `3 Attack` |
| Mercenary | `3 Attack`, `6 Critical Chance` |
| Rogue | `3 Speed`, `5 Evasion`, `1 Critical Chance` |
| Ranger | `3 Attack`, `2 Accuracy`, `2 Critical Chance` |
| Mage | `15 Max MP`, `3 Magic Power` |
| Blood Mage | `30 Max HP`, `3 Magic Power` |
| White Mage | `15 Max MP`, `3 Magic Power` |
| Paladin | `20 Max HP`, `2 Magic Power`, `1 Defense`, `1 Magic Defense` |
| Bard | `15 Max MP`, `2 Speed`, `1 Magic Defense` |
| Tactician | `15 Max MP`, `3 Speed` |
| Summoner | `15 Max MP`, `2 Magic Power`, `1 Magic Defense` |

## Shared Combat Terms

The trees deliberately reuse a small status vocabulary:

- **Provoke:** eligible single-target enemy actions must target the provoking
  character while the effect lasts. An enemy retains one Provoke source; a new
  successful application replaces it. Scripted exceptions must be explicit.
- **Protect:** redirects the next eligible single-target physical hit from one
  chosen ally to the protector, then expires. One ally retains one protector;
  a new successful application replaces the previous source.
- **Barrier:** temporary damage absorption displayed separately from HP.
- **Attack Up / Magic Up / Defense Up:** temporary percentage modifiers that do
  not stack with the same named effect; the stronger value replaces the weaker.
- **Attack Down / Defense Down / Slow:** corresponding temporary penalties.
- **Poison:** fixed damage at the end of the affected combatant's action.
- **Bleed:** physical damage at the end of the affected combatant's action.
  Bleed and Poison may coexist.
- **Marked:** enables specified follow-up effects and is consumed only when an
  action says so. Marks retain their source character and effect ID, allowing
  unlike marks to coexist while preventing one source from duplicating itself.
- **Priority:** resolves before ordinary actions. Actions in the same priority
  class use speed, then the combat random source for equal-speed ties.

All durations, replacements, immunities, resisted applications, and expirations
must appear in the battle log and in non-color-only status UI.

## Knight

Stable prefix: `job_knight`

- **Core trait - Bulwark (`job_knight_bulwark`):** Defend reduces physical
  damage more effectively for the Knight than for other jobs.
- **Core action - Provoke (`job_knight_provoke`):** apply Provoke to one enemy
  until the Knight's next turn.
- **Tier 1 - Shield Bash (`job_knight_shield_bash`):** deal light physical
  damage and apply Defense Down for a short duration. Cost: `1 JP`.
- **Tier 1 - Interpose (`job_knight_interpose`):** Protect one ally from the
  next eligible physical hit. Cost: `1 JP`.
- **Tier 2 - Hold The Line (`job_knight_hold_the_line`):** Provoke all eligible
  enemies and enter the normal Defend state in the same action. Cost: `2 JP`.
- **Stat I - Fortitude I (`job_knight_fortitude_1`):** permanently gain
  `10 Max HP`. Cost: `1 JP`.
- **Stat II - Fortitude II (`job_knight_fortitude_2`):** permanently gain
  `10 Max HP`. Requires Fortitude I. Cost: `1 JP`.
- **Stat I - Guard Training I (`job_knight_guard_training_1`):** permanently
  gain `1 Defense`. Cost: `1 JP`.
- **Stat II - Guard Training II (`job_knight_guard_training_2`):** permanently
  gain `1 Defense`. Requires Guard Training I. Cost: `1 JP`.
- **Tier 3 mastery - Knight's Foundation (`job_knight_foundation`):**
  permanently gain `10 Max HP` and `1 Defense`. Requires both Stat II nodes.
  Cost: `2 JP`.

Identity: the safest deliberate protector, with modest personal damage and low
speed.

## Reaver

Stable prefix: `job_reaver`

- **Core trait - Bloodied Fury (`job_reaver_bloodied_fury`):** physical damage
  increases while the Reaver is below half HP. The bonus does not grow further
  at near-zero HP.
- **Core action - Blood Challenge (`job_reaver_blood_challenge`):** Provoke one
  enemy; if that enemy damages the Reaver before the effect ends, the Reaver
  gains Attack Up briefly.
- **Tier 1 - Reckless Strike (`job_reaver_reckless_strike`):** deal heavy
  physical damage and apply Defense Down to the Reaver. Cost: `1 JP`.
- **Tier 1 - Feed The Pain (`job_reaver_feed_the_pain`):** passive; the first
  time each battle the Reaver falls below half HP, gain Attack Up. Cost: `1 JP`.
- **Tier 2 - Ruinous Blow (`job_reaver_ruinous_blow`):** deal physical damage
  that scales with missing HP, followed by fixed recoil that cannot reduce the
  Reaver below `1 HP`. Cost: `2 JP`.
- **Stat I - Scarred Endurance I (`job_reaver_scarred_endurance_1`):**
  permanently gain `10 Max HP`. Cost: `1 JP`.
- **Stat II - Scarred Endurance II (`job_reaver_scarred_endurance_2`):**
  permanently gain `10 Max HP`. Requires Scarred Endurance I. Cost: `1 JP`.
- **Stat I - Ferocity I (`job_reaver_ferocity_1`):** permanently gain
  `1 Attack`. Cost: `1 JP`.
- **Stat II - Ferocity II (`job_reaver_ferocity_2`):** permanently gain
  `1 Attack`. Requires Ferocity I. Cost: `1 JP`.
- **Tier 3 mastery - Painforged (`job_reaver_painforged`):** permanently gain
  `10 Max HP` and `1 Attack`. Requires both Stat II nodes. Cost: `2 JP`.

Identity: an aggressive damage sponge whose best turns require managed danger,
not repeated healing from zero.

## Mercenary

Stable prefix: `job_mercenary`

- **Core trait - Veteran's Edge (`job_mercenary_veterans_edge`):** basic
  physical attacks receive a small accuracy and critical-chance bonus.
- **Core action - Power Strike (`job_mercenary_power_strike`):** spend MP for a
  reliable heavy physical hit.
- **Tier 1 - Armor Break (`job_mercenary_armor_break`):** deal physical damage
  and apply Defense Down. Cost: `1 JP`.
- **Tier 1 - Second Wind (`job_mercenary_second_wind`):** spend MP to restore a
  modest amount of the user's HP; usable once per battle. Cost: `1 JP`.
- **Tier 2 - Finishing Blow (`job_mercenary_finishing_blow`):** deal heavy
  physical damage, with increased power against a target below one-third HP.
  Cost: `2 JP`.
- **Stat I - Arms Training I (`job_mercenary_arms_training_1`):** permanently
  gain `1 Attack`. Cost: `1 JP`.
- **Stat II - Arms Training II (`job_mercenary_arms_training_2`):** permanently
  gain `1 Attack`. Requires Arms Training I. Cost: `1 JP`.
- **Stat I - Killer Instinct I (`job_mercenary_killer_instinct_1`):**
  permanently gain `2 Critical Chance`. Cost: `1 JP`.
- **Stat II - Killer Instinct II (`job_mercenary_killer_instinct_2`):**
  permanently gain `2 Critical Chance`. Requires Killer Instinct I. Cost:
  `1 JP`.
- **Tier 3 mastery - Seasoned Fighter (`job_mercenary_seasoned_fighter`):**
  permanently gain `1 Attack` and `2 Critical Chance`. Requires both Stat II
  nodes. Cost: `2 JP`.

Identity: straightforward and dependable single-target damage with little
party utility.

## Rogue

Stable prefix: `job_rogue`

- **Core trait - Opportunist (`job_rogue_opportunist`):** deal increased
  physical damage to enemies with a negative status or Marked state.
- **Core action - Quick Strike (`job_rogue_quick_strike`):** a lighter physical
  attack with Priority.
- **Tier 1 - Venom Blade (`job_rogue_venom_blade`):** deal light physical
  damage and attempt to apply Poison. Cost: `1 JP`.
- **Tier 1 - Smoke Veil (`job_rogue_smoke_veil`):** increase the user's evasion
  for a short duration. Cost: `1 JP`.
- **Tier 2 - Exploit Opening (`job_rogue_exploit_opening`):** deal heavy
  physical damage to a debuffed or Marked target; otherwise deal only modest
  damage. Cost: `2 JP`.
- **Stat I - Agility I (`job_rogue_agility_1`):** permanently gain `1 Speed`.
  Cost: `1 JP`.
- **Stat II - Agility II (`job_rogue_agility_2`):** permanently gain `1 Speed`.
  Requires Agility I. Cost: `1 JP`.
- **Stat I - Footwork I (`job_rogue_footwork_1`):** permanently gain
  `2 Evasion`. Cost: `1 JP`.
- **Stat II - Footwork II (`job_rogue_footwork_2`):** permanently gain
  `2 Evasion`. Requires Footwork I. Cost: `1 JP`.
- **Tier 3 mastery - Untouchable (`job_rogue_untouchable`):** permanently gain
  `1 Speed`, `1 Evasion`, and `1 Critical Chance`. Requires both Stat II nodes.
  Cost: `2 JP`.

Identity: fast burst and status pressure that drops sharply when enemies focus
the Rogue.

## Ranger

Stable prefix: `job_ranger`

- **Core trait - Keen Eye (`job_ranger_keen_eye`):** ranged job actions receive
  increased accuracy and ignore enemy counter effects that explicitly require
  a melee hit.
- **Core action - Mark Quarry (`job_ranger_mark_quarry`):** apply Marked to one
  enemy and reveal its current combat statuses. Only one quarry may be marked
  by a Ranger at a time.
- **Tier 1 - Piercing Shot (`job_ranger_piercing_shot`):** deal physical damage
  while ignoring part of the target's defense. Cost: `1 JP`.
- **Tier 1 - Pinning Shot (`job_ranger_pinning_shot`):** deal light physical
  damage and attempt to apply Slow. Cost: `1 JP`.
- **Tier 2 - Predator's Shot (`job_ranger_predators_shot`):** consume the
  Ranger's Marked state on the target for a high-accuracy heavy hit. Cost:
  `2 JP`.
- **Stat I - Draw Strength I (`job_ranger_draw_strength_1`):** permanently gain
  `1 Attack`. Cost: `1 JP`.
- **Stat II - Draw Strength II (`job_ranger_draw_strength_2`):** permanently
  gain `1 Attack`. Requires Draw Strength I. Cost: `1 JP`.
- **Stat I - True Aim I (`job_ranger_true_aim_1`):** permanently gain
  `1 Accuracy`. Cost: `1 JP`.
- **Stat II - True Aim II (`job_ranger_true_aim_2`):** permanently gain
  `1 Accuracy`. Requires True Aim I. Cost: `1 JP`.
- **Tier 3 mastery - Master Hunter (`job_ranger_master_hunter`):** permanently
  gain `1 Attack` and `2 Critical Chance`. Requires both Stat II nodes. Cost:
  `2 JP`.

Identity: accurate setup and payoff without requiring a positional row system.

## Mage

Stable prefix: `job_mage`

- **Core trait - Arcane Focus (`job_mage_arcane_focus`):** offensive spells
  receive a small Magic Up modifier while the Mage is above half MP.
- **Core action - Ember (`job_mage_ember`):** spend MP for single-target
  elemental magic damage.
- **Tier 1 - Frostbind (`job_mage_frostbind`):** spend MP for magic damage and
  attempt to apply Slow. Cost: `1 JP`.
- **Tier 1 - Arc Spark (`job_mage_arc_spark`):** spend MP to damage one target
  and a second living enemy for reduced damage. Cost: `1 JP`.
- **Tier 2 - Arcane Burst (`job_mage_arcane_burst`):** spend substantial MP for
  magic damage to all living enemies. Cost: `2 JP`.
- **Stat I - Arcane Reserves I (`job_mage_arcane_reserves_1`):** permanently
  gain `5 Max MP`. Cost: `1 JP`.
- **Stat II - Arcane Reserves II (`job_mage_arcane_reserves_2`):** permanently
  gain `5 Max MP`. Requires Arcane Reserves I. Cost: `1 JP`.
- **Stat I - Spellcraft I (`job_mage_spellcraft_1`):** permanently gain
  `1 Magic Power`. Cost: `1 JP`.
- **Stat II - Spellcraft II (`job_mage_spellcraft_2`):** permanently gain
  `1 Magic Power`. Requires Spellcraft I. Cost: `1 JP`.
- **Tier 3 mastery - Arcane Scholar (`job_mage_arcane_scholar`):** permanently
  gain `5 Max MP` and `1 Magic Power`. Requires both Stat II nodes. Cost:
  `2 JP`.

Identity: flexible magical damage and early multi-target pressure constrained
by MP and low durability.

## Blood Mage

Stable prefix: `job_blood_mage`

- **Core trait - Crimson Casting (`job_blood_mage_crimson_casting`):** Blood
  Mage job spells spend HP instead of MP and cannot be selected when their HP
  cost would incapacitate the caster.
- **Core action - Blood Bolt (`job_blood_mage_blood_bolt`):** spend HP for
  strong single-target magic damage.
- **Tier 1 - Siphon (`job_blood_mage_siphon`):** spend HP for modest magic
  damage, then restore HP based on damage actually dealt. Cost: `1 JP`.
- **Tier 1 - Hemorrhage (`job_blood_mage_hemorrhage`):** spend HP for magic
  damage and attempt to apply Bleed. Cost: `1 JP`.
- **Tier 2 - Red Deluge (`job_blood_mage_red_deluge`):** spend a large amount
  of HP for magic damage to all living enemies. Cost: `2 JP`.
- **Stat I - Vital Reservoir I (`job_blood_mage_vital_reservoir_1`):**
  permanently gain `10 Max HP`. Cost: `1 JP`.
- **Stat II - Vital Reservoir II (`job_blood_mage_vital_reservoir_2`):**
  permanently gain `10 Max HP`. Requires Vital Reservoir I. Cost: `1 JP`.
- **Stat I - Bloodcraft I (`job_blood_mage_bloodcraft_1`):** permanently gain
  `1 Magic Power`. Cost: `1 JP`.
- **Stat II - Bloodcraft II (`job_blood_mage_bloodcraft_2`):** permanently gain
  `1 Magic Power`. Requires Bloodcraft I. Cost: `1 JP`.
- **Tier 3 mastery - Crimson Vessel (`job_blood_mage_crimson_vessel`):**
  permanently gain `10 Max HP` and `1 Magic Power`. Requires both Stat II
  nodes. Cost: `2 JP`.

Identity: high-impact magic with visible self-inflicted risk. HP costs occur
before the spell resolves and cannot critically hit the caster.

## White Mage

Stable prefix: `job_white_mage`

- **Core trait - Healing Grace (`job_white_mage_healing_grace`):** outgoing
  healing is increased slightly.
- **Core action - Mend (`job_white_mage_mend`):** spend MP to restore one ally's
  HP.
- **Tier 1 - Purify (`job_white_mage_purify`):** spend MP to remove one
  removable negative status from an ally. Cost: `1 JP`.
- **Tier 1 - Ward (`job_white_mage_ward`):** spend MP to grant one ally a
  Barrier. Cost: `1 JP`.
- **Tier 2 - Healing Light (`job_white_mage_healing_light`):** spend substantial
  MP to restore a moderate amount of HP to all living allies. Cost: `2 JP`.
- **Stat I - Sacred Reserves I (`job_white_mage_sacred_reserves_1`):**
  permanently gain `5 Max MP`. Cost: `1 JP`.
- **Stat II - Sacred Reserves II (`job_white_mage_sacred_reserves_2`):**
  permanently gain `5 Max MP`. Requires Sacred Reserves I. Cost: `1 JP`.
- **Stat I - Healing Study I (`job_white_mage_healing_study_1`):** permanently
  gain `1 Magic Power`. Cost: `1 JP`.
- **Stat II - Healing Study II (`job_white_mage_healing_study_2`):** permanently
  gain `1 Magic Power`. Requires Healing Study I. Cost: `1 JP`.
- **Tier 3 mastery - Blessed Spirit (`job_white_mage_blessed_spirit`):**
  permanently gain `5 Max MP` and `1 Magic Power`. Requires both Stat II nodes.
  Cost: `2 JP`.

Identity: the strongest dedicated recovery job, with intentionally weak
personal offense.

## Paladin

Stable prefix: `job_paladin`

- **Core trait - Steadfast Presence (`job_paladin_steadfast_presence`):** while
  the Paladin is conscious, active allies receive a small defense bonus that
  does not stack with another Paladin's copy of this trait.
- **Core action - Lay On Hands (`job_paladin_lay_on_hands`):** spend MP for a
  modest single-target heal.
- **Tier 1 - Smite (`job_paladin_smite`):** spend MP for a physical strike with
  bonus magic-based damage. Cost: `1 JP`.
- **Tier 1 - Guardian Oath (`job_paladin_guardian_oath`):** Protect one ally and
  gain Defense Up until the Paladin's next turn. Cost: `1 JP`.
- **Tier 2 - Rallying Light (`job_paladin_rallying_light`):** spend substantial
  MP to apply Defense Up and a small heal to all living allies. Cost: `2 JP`.
- **Stat I - Sacred Vigor I (`job_paladin_sacred_vigor_1`):** permanently gain
  `10 Max HP`. Cost: `1 JP`.
- **Stat II - Sacred Vigor II (`job_paladin_sacred_vigor_2`):** permanently gain
  `10 Max HP`. Requires Sacred Vigor I. Cost: `1 JP`.
- **Stat I - Conviction I (`job_paladin_conviction_1`):** permanently gain
  `1 Magic Power`. Cost: `1 JP`.
- **Stat II - Conviction II (`job_paladin_conviction_2`):** permanently gain
  `1 Magic Power`. Requires Conviction I. Cost: `1 JP`.
- **Tier 3 mastery - Oathbound (`job_paladin_oathbound`):** permanently gain
  `1 Defense` and `1 Magic Defense`. Requires both Stat II nodes. Cost: `2 JP`.

Identity: a leadership-oriented hybrid who can stabilize several problems but
cannot outperform a dedicated tank, attacker, or healer at its specialty.

## Bard

Stable prefix: `job_bard`

- **Core trait - Lingering Refrain (`job_bard_lingering_refrain`):** positive
  statuses applied by Bard job actions last one additional round, up to each
  status's authored maximum.
- **Core action - Verse Of Valor (`job_bard_verse_of_valor`):** spend MP to
  grant one ally Attack Up and Magic Up for a short duration.
- **Tier 1 - Soothing Melody (`job_bard_soothing_melody`):** spend MP for minor
  healing to all living allies. Cost: `1 JP`.
- **Tier 1 - Cutting Verse (`job_bard_cutting_verse`):** spend MP to attempt
  Attack Down on one enemy. Cost: `1 JP`.
- **Tier 2 - Chorus Of Resolve (`job_bard_chorus_of_resolve`):** spend
  substantial MP to grant all living allies Attack Up and Magic Up. Cost:
  `2 JP`.
- **Stat I - Breath Control I (`job_bard_breath_control_1`):** permanently gain
  `5 Max MP`. Cost: `1 JP`.
- **Stat II - Breath Control II (`job_bard_breath_control_2`):** permanently
  gain `5 Max MP`. Requires Breath Control I. Cost: `1 JP`.
- **Stat I - Tempo I (`job_bard_tempo_1`):** permanently gain `1 Speed`. Cost:
  `1 JP`.
- **Stat II - Tempo II (`job_bard_tempo_2`):** permanently gain `1 Speed`.
  Requires Tempo I. Cost: `1 JP`.
- **Tier 3 mastery - Virtuoso (`job_bard_virtuoso`):** permanently gain
  `5 Max MP` and `1 Magic Defense`. Requires both Stat II nodes. Cost: `2 JP`.

Identity: sustained party-wide momentum through buffs, debuffs, and light
recovery, balanced by low damage and low personal durability.

## Tactician

Stable prefix: `job_tactician`

- **Core trait - Forethought (`job_tactician_forethought`):** the first support
  action used by the Tactician each battle has Priority.
- **Core action - Analyze (`job_tactician_analyze`):** apply Defense Down and
  reveal the target's current combat statuses; Analyze itself deals no damage.
- **Tier 1 - Hasten (`job_tactician_hasten`):** spend MP to increase one ally's
  speed for a short duration. Cost: `1 JP`.
- **Tier 1 - Disrupt (`job_tactician_disrupt`):** spend MP to attempt Slow on
  one enemy. Cost: `1 JP`.
- **Tier 2 - Coordinated Assault (`job_tactician_coordinated_assault`):** mark
  one enemy so the next two successful allied damaging actions against it deal
  increased damage, then remove the mark. Cost: `2 JP`.
- **Stat I - Preparation I (`job_tactician_preparation_1`):** permanently gain
  `5 Max MP`. Cost: `1 JP`.
- **Stat II - Preparation II (`job_tactician_preparation_2`):** permanently gain
  `5 Max MP`. Requires Preparation I. Cost: `1 JP`.
- **Stat I - Initiative I (`job_tactician_initiative_1`):** permanently gain
  `1 Speed`. Cost: `1 JP`.
- **Stat II - Initiative II (`job_tactician_initiative_2`):** permanently gain
  `1 Speed`. Requires Initiative I. Cost: `1 JP`.
- **Tier 3 mastery - Mastermind (`job_tactician_mastermind`):** permanently gain
  `5 Max MP` and `1 Speed`. Requires both Stat II nodes. Cost: `2 JP`.

Identity: action timing and team focus with almost no direct damage or
durability.

## Summoner

Stable prefix: `job_summoner`

- **Core trait - Spirit Bond (`job_summoner_spirit_bond`):** only one spirit
  summoned by a character may be active. It disappears if its summoner is
  incapacitated or battle ends.
- **Core action - Call Ember Wisp (`job_summoner_call_ember_wisp`):** spend MP
  to create an auxiliary Wisp with modest HP for three rounds and designate one
  enemy. After its summoner completes an action, the Wisp makes one fixed magic
  attack against that enemy. Selecting another enemy with a Summoner action
  changes the designated target. If it becomes invalid, use the first living
  enemy in stable battle order so tests and replays do not depend on untracked
  AI choices.
- **Tier 1 - Wisp's Aid (`job_summoner_wisps_aid`):** command the active Wisp to
  replace its next attack with a modest heal on one selected ally. Cost: `1 JP`.
- **Tier 1 - Reinforce Bond (`job_summoner_reinforce_bond`):** spend MP to grant
  the Wisp a Barrier and extend its remaining duration by one round, up to its
  authored maximum. Cost: `1 JP`.
- **Tier 2 - Spirit Flare (`job_summoner_spirit_flare`):** dismiss the active
  Wisp to deal magic damage to all living enemies. Cost: `2 JP`.
- **Stat I - Bond Reserves I (`job_summoner_bond_reserves_1`):** permanently
  gain `5 Max MP`. Cost: `1 JP`.
- **Stat II - Bond Reserves II (`job_summoner_bond_reserves_2`):** permanently
  gain `5 Max MP`. Requires Bond Reserves I. Cost: `1 JP`.
- **Stat I - Invocation I (`job_summoner_invocation_1`):** permanently gain
  `1 Magic Power`. Cost: `1 JP`.
- **Stat II - Invocation II (`job_summoner_invocation_2`):** permanently gain
  `1 Magic Power`. Requires Invocation I. Cost: `1 JP`.
- **Tier 3 mastery - Spirit Keeper (`job_summoner_spirit_keeper`):** permanently
  gain `5 Max MP` and `1 Magic Defense`. Requires both Stat II nodes. Cost:
  `2 JP`.

Identity: flexible delayed value that can be attacked or disrupted. The Wisp
has no equipment, inventory, XP, independent AI selection, or persistent world
state, and it never occupies one of the four normal active-party slots.

## Shared Implementation Requirements

Milestone 15 needs reusable systems for:

- Stable ability and node definitions with prerequisites and JP costs.
- Per-character JP balances and learned-node sets grouped by job ID.
- Permanent stat aggregation derived from all learned stat-node IDs without
  enabling inactive-job traits, passives, or actions.
- MP and HP action costs with disabled-command reasons.
- Status application, replacement, duration, immunity, and expiration.
- Single, all, and limited multi-target actions.
- Priority and temporary speed modifiers without exposing a turn queue.
- Provoke, Protect, Barrier, Marked, and delayed telegraphed actions.
- Auxiliary Summoner combatants with a fixed behavior contract.
- Passive hooks for battle start, action selection, damage calculation, HP
  thresholds, action completion, incapacitation, and battle end.

Prefer data-driven action composition for ordinary damage, healing, costs, and
statuses. Use focused custom rules only for behavior such as Protect redirects,
missing-HP scaling, and the Wisp lifecycle.

## Balance And Validation Targets

- Every job can contribute immediately with only its free core kit.
- Early skill and stat purchases should create a noticeable tactical
  improvement before the Grassland Goblin Boss.
- A normal critical-path player should be able to complete at least one tree
  per regularly used character during the demo without mandatory grinding.
- Optional battles may broaden job experimentation but must not be required to
  make the main path fair.
- No composition or affinity pairing is required to answer the Goblin Boss or
  Great Stag mechanics.
- Reaver and Blood Mage risk mechanics must never make an accidental menu
  selection immediately incapacitate their user.
- Summoner turns, expirations, dismissals, and rewards must remain deterministic
  under injected test randomness and must never duplicate battle rewards.

Exact combat formulas and encounter JP awards will be set through simulation
after the multi-party runtime is implemented.
