# Job And Party System Design

This document records future gameplay constraints for the job, character, party, inventory, and save-data architecture. It intentionally excludes story and world lore.

The vertical-slice demo makes all 12 jobs playable while limiting how far each
job tree can progress. The authoritative demo boundary is recorded in
`DEMO_JOB_TREE_SCOPE.md`.

## Design Goals

- Provide 12 job archetypes across tank, physical damage, magic damage, healer, and support roles.
- Allow every playable character to switch into every job.
- Give each character authored job affinities based on personality.
- Use affinity to influence effectiveness or stat growth, never to block access to a job.
- Give every job a clear role and an explicit mechanical downside.
- Avoid broad all-purpose jobs. Limited overlap is acceptable only when the trade-off remains meaningful.
- Keep job mechanics independent from story-specific systems such as corruption.

## Proposed Jobs

Names are provisional. Role boundaries and trade-offs are the important design constraints.

### Tanks

1. Knight: heavy defense and threat control; slow with low damage.
2. Reaver: a damage sponge that converts danger into heavy damage; less reliable defense and performs best while endangered.

### Physical Damage

3. Mercenary or Swordsman: direct melee damage; little utility.
4. Rogue or Thief: speed, evasion, burst, and status effects; fragile when focused.
5. Ranger or Archer: ranged damage and tactical utility; weaker at close range or without setup.

### Magic Damage

6. Mage or Battlemage: elemental damage; fragile and resource intensive.
7. Blood Mage: spends HP for powerful magic; risks reducing party sustain. The HP cost is mechanical and unrelated to story systems.

### Healers

8. White Mage: strong restoration and protective support; minimal offense.
9. Paladin: front-line damage, protection, limited healing, and possible leadership support; splits resources across roles and is weaker than dedicated specialists.

### Support

10. Bard or Chanter: buffs, debuffs, and minor healing; low personal damage and survivability.
11. Tactician or Strategist: battlefield and turn-order control; minimal direct damage and durability.
12. Beastmaster or Summoner: temporary creature ally; summon is fragile, temporary, or otherwise unreliable. Summoner is part of the demo catalog, using a deliberately small first implementation rather than a general pet system.

## Demo Availability

- All 12 jobs are available when the demo introduces job assignment.
- Every playable character may use every job; affinity guides effectiveness and
  never gates access.
- Each job owns a data-driven demo progression limit appropriate to its tree.
- Learned nodes and job progress belong to a persistent character and survive
  switching jobs or changing active-party slots.
- Permanent stat nodes from learned job trees remain active across job changes.
  Job traits, actions, and job-specific passives require that job to be equipped.
- Exact demo nodes, point rules, and per-job limits must be locked before their
  Milestone 15 gameplay implementation.

## Cross-Job Stat Growth

- Character level growth establishes base stats independently from job trees.
- Learned permanent stat nodes add modest flat bonuses beyond level growth and
  equipment.
- Permanent bonuses from multiple jobs stack, but each stable node may apply
  only once per character.
- Affinity does not alter a permanent node's listed value. It remains guidance
  for current-job effectiveness rather than a penalty on lasting progression.
- Save data stores learned node IDs, not a second serialized permanent-stat
  total. Effective stats are recalculated to prevent duplicated bonuses.
- Switching jobs is not a respec. A future explicit respec would refund nodes
  and remove their bonuses, but node refunds are outside the demo scope.

## Affinity Model

- Store an affinity rating or multiplier for every character-job pairing.
- Author affinities per character rather than deriving them from the currently equipped job.
- Apply affinity to job-related stat growth or performance.
- High affinity should be noticeably beneficial.
- Low affinity should impose a mild but meaningful penalty without making a job unusable.
- Keep base character identity, job definition, and current job assignment as separate data.

The exact formula should be selected during implementation and covered by tests before balancing content is authored around it.

Initial implementation uses `0.9` for low affinity, `1.0` for neutral affinity, and `1.1` for high affinity. These values are intentionally conservative and can be rebalanced without changing job access or character identity.

## Initial Party Member Profiles

Four additional party members currently have stable IDs and natural archetypes. These represent high affinities and combat flavor, not locked classes:

- `pc_01`, Iona (placeholder name): warm, steady, and others-first; naturally favors White Mage and keeping allies alive.
- `pc_02`, Damari: chaotic, crude, and aggressive; naturally favors Reaver and trading caution for front-line damage.
- `pc_03`, Enora: bold, dramatic, and drawn to risk; naturally favors Blood Mage, self-damaging magic, and a scythe.
- `pc_04`, Lysander (placeholder name): a grizzled, independent, experienced adventurer who is measured, protective, and leadership-oriented; naturally favors Paladin and balanced protection, damage, and rallying support. He joins after the protagonist earns a local reputation and enters slightly above the current party's level.

Every one of these characters can still equip and switch into every job. Only each clearly stated natural job begins at high affinity; all unspecified affinities remain neutral until deliberately authored. Display-name changes must not change stable IDs or require save-data migration. Two additional roster positions remain open for future characters.

## Player-Created Protagonist

The main character is a player-created protagonist and is separate from the
authored companion records `pc_01` through `pc_04`.

- Store the protagonist under a stable save-data identity that does not depend
  on their display name or selected appearance.
- Do not reuse a companion ID for the protagonist or treat a renamed companion
  as the player character.
- Limited cosmetic customization is planned, but the exact options remain a
  Milestone 14 scope and art-budget decision.
- The protagonist participates in the active battle party and should use the
  same job-access rules unless a later explicit design decision says otherwise.
- Keep protagonist appearance data separate from base combat data, current job,
  equipment, and save progression.

## Party And Turn Order

- Target four active party members in battle.
- Support a flexible authored-companion roster rather than only the four known
  profiles. Earlier planning estimated roughly six or seven playable
  characters, but Milestone 14 must clarify whether that number includes the
  player-created protagonist.
- Do not store character data only in four active-slot records.
- Turn order is based on speed.
- Break equal-speed ties randomly.
- Do not display a turn-order queue; estimating order is part of the intended challenge.
- Build turn order as an internal combat rule from stable combatant IDs and current speed values.
- Use an injectable random source so tie behavior is random in play and deterministic in automated tests.

## Future Roster Rotation

The initial data model should leave room for:

- Story-forced party splits.
- Bonds or affinity between characters who fight together.
- Well-rested or catch-up bonuses for benched characters.
- Usage and bond statistics stored per persistent character, not per party slot.

These systems are lower priority and do not need to ship with the first job implementation.

The foundation now stores active-battle count, benched-battle count, consecutive benched battles, and symmetric bond points on persistent character records. It does not yet assign gameplay bonuses to those values.

## Permanent Roster Removal Constraint

Damari (`pc_02`) is planned to leave the roster permanently at a fixed story point. The reason and other story details are intentionally not recorded here.

- Every item equipped on that character when the event occurs must also be permanently removed.
- Removed equipment must not return to shared inventory or become reassignable.
- Unequipped items already held in shared inventory are unaffected.
- Save data should represent roster availability separately from persistent character identity.
- Save data must record that the removal event occurred so loading cannot restore the character or duplicate the lost equipment.

This is a deliberate gameplay consequence and must be considered during inventory and save-system design even though the story event will be implemented later.

## Implementation Order

1. Finish core gameplay tests.
2. Define persistent character, roster, active-party, and job data contracts.
3. Define job roles, growth values, trade-offs, affinity data, stable tree-node
   IDs, permanent stat bonuses, and per-job demo progression limits.
4. Test assignment, progression limits, affinity effects, learned node
   persistence, cross-job stat aggregation, and party-slot independence.
5. Build data-driven monster and item content against those stable contracts.
6. Add inventory and saving with explicit handling for destroying a removed character's equipped gear exactly once.

## Deferred Story Input

The stable IDs, short personality notes, combat preferences, and initial natural-job affinities needed for the first four additional party members are now recorded. No full lore is needed for the current milestone. Still request only:

- Stable IDs, placeholder names, short personality notes, and combat preferences for the two remaining roster positions.
- Any deliberate high or low affinities beyond the four natural-job affinities already recorded.

Request full histories, relationships, factions, locations, plot events, and dialogue later during quests and world-content planning.
