# Grassland Combat Content

This document defines Grassland's enemy roles, dedicated encounter groups,
ambient encounter roster, Quest 3 intelligence advantages, and regional Goblin
Boss behavior. Numeric balance is provisional until Milestone 15 supports a
four-character party and enemy groups.

## Stable Combatant IDs

- `monster_goblin_skirmisher`: baseline melee goblin.
- `monster_goblin_slinger`: faster, more evasive ranged goblin.
- `monster_goblin_guard`: defensive goblin that protects allies.
- `monster_goblin_raider`: aggressive, higher-damage patrol goblin.
- `monster_tutorial_hobgoblin`: tutorial mini-boss from the caravan encounter.
- `boss_grassland_goblin`: regional Goblin Boss and Cherry Blossom gate.
- `monster_slime`: ordinary low-threat ambient creature.
- `monster_wild_boar`: durable, physical ambient creature.

The existing prototype `monster_goblin` may remain while systems are being
built, but production encounters should use the role-specific IDs. The
tutorial Hobgoblin and regional Goblin Boss must never share IDs, completion
flags, rewards, or definitions.

## Enemy Roles

### Goblin Skirmisher

- Straightforward melee attacker and baseline for Grassland balance.
- Low defense and no special targeting rule.

### Goblin Slinger

- Faster and more evasive than a Skirmisher.
- Applies ranged pressure to less durable party members.
- Lower durability keeps focused attacks effective.

### Goblin Guard

- Higher defense and lower damage.
- Uses a protection or interception action for allied goblins.
- Gives Damari's threat control and party target selection practical value.

### Goblin Raider

- Stronger physical attacker used in Quest 3 patrols.
- Less defensive than a Guard and dangerous when left unchecked.

### Ambient Creatures

- Slimes appear in small, low-threat groups.
- Wild Boars are sturdier physical opponents that may appear alone or with a
  Slime.
- Wisps and overtly supernatural creatures do not appear in Grassland.

## Dedicated Quest Groups

Quest 1 uses two skirmishes:

- Two Goblin Skirmishers.
- One Goblin Skirmisher and one Goblin Slinger.

Quest 2's rescue encounter uses:

- One Goblin Guard and two Goblin Skirmishers.

Quest 3's three rescue encounters use equally eligible but varied patrols:

- One Goblin Guard, one Goblin Skirmisher, and one Goblin Slinger.
- One Goblin Guard and two Goblin Raiders.
- Two Goblin Raiders and one Goblin Slinger.

Quest 3 locations may be completed in any order, so none of these three groups
may assume that it is always fought first or last.

## Ambient Grassland Groups

- Two Slimes.
- One Wild Boar.
- One Slime and one Wild Boar.

Ambient groups remain easier than dedicated Quest 3 patrols and use the
step-trigger rules in `DEMO_MAIN_QUEST_CONTENT.md`.

## Quest 3 Intelligence Advantages

Each scout rescue records a stable intelligence flag. The scout names are still
open, but their mechanical contributions are fixed:

- **Patrol routes:** remove one Goblin Guard from the boss's starting group.
- **Ambush warning:** all active party members act before enemies during the
  opening round, after which ordinary speed order resumes.
- **War horn location:** the approach scene disables the horn, preventing the
  boss's reinforcement action.

Any two rescued scouts complete Quest 3 and provide their two corresponding
advantages. Saving all three provides all three advantages and the additional
quest reward. The battle UI or approach scene must communicate every applied
advantage.

## Regional Goblin Boss

The Goblin Boss is the strongest regional goblin and an ordinary bandit-style
threat. It has no supernatural origin, corruption, or connection to the later
world mystery.

### Starting Group

- The Goblin Boss begins with two Goblin Guards.
- Patrol-route intelligence reduces the starting group to one Guard.

### Action Pattern

- **Commanding Shout:** strengthens surviving goblin allies for a limited
  duration.
- **Brutal Swing:** announces a clear wind-up, then performs a heavy single-target
  strike on the boss's next action. This encourages Defend, healing preparation,
  or Damari's taunt-style protection.
- **Off Balance:** after Brutal Swing resolves, the boss's defense is reduced for
  one round. Tallis's Quest 2 intelligence explicitly teaches this opening so
  the player can prepare burst damage.
- **Call Reinforcements:** once at half HP, the boss calls two Goblin
  Skirmishers unless the war horn was disabled.
- **Reckless:** at low HP, the boss gains attack and loses defense for the rest
  of the encounter.

Commanding Shout has no benefit when no allied goblins remain. Threshold actions
must trigger exactly once even when a large hit crosses multiple HP thresholds.
Telegraphs, applied intelligence, phase changes, and prevented reinforcements
must all produce clear battle messages.

## Balance Targets

- The fight should test healing, threat control, Defend, target priority, and
  burst timing without requiring one exact job assignment.
- Completing only the required Quest 3 rescues creates a harder but fair fight.
- Saving all three scouts materially reduces risk without trivializing the boss.
- A party using the information well should outperform one that ignores it.
- Exact HP, MP, attack, defense, speed, accuracy, evasion, XP, and gold values
  remain provisional until multi-combatant battle simulation and tests exist.
