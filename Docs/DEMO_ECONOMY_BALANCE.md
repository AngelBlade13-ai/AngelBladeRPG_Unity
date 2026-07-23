# Demo Economy Balance

This document records the provisional Milestone 16 prices, authored rewards,
monster loot tables, and critical-path pacing proof. These values are a
playable baseline, not a promise that numerical tuning is finished before the
demo's encounters and quests can be tested end to end.

Runtime IDs and values are defined in `ItemCatalog` and
`DemoEconomyCatalog`. Authored rewards are granted through
`DemoRewardService`, which rejects duplicate claims and partial grants.

## Pacing Goals

- The lowest-income critical route must not require ambient encounters,
  optional objectives, selling, or random drops.
- Consumables should be affordable enough to use rather than hoard.
- Main quests provide the dependable upgrade budget.
- Optional thoroughness and side quests provide convenience or distinctive
  equipment, not required boss keys.
- Ambient loot is a bonus and is excluded from every affordability proof.

## Core Prices

| Purchase | Gold |
| --- | ---: |
| Minor Potion | 30 |
| Field Remedy | 45 |
| Camp Ration | 80 |
| Iron Heavy Blade | 120 |
| Ash Bow | 110 |
| Iron Dagger | 100 |
| Oak Staff | 115 |
| Traveler's Lute | 115 |
| Apprentice Tome | 110 |
| Carved Horn | 120 |
| Padded Armor | 100 |
| Full recovery at the Suncrest Inn or Shrine | 25 |

Quest-only equipment and recovery items are not stocked in ordinary shops.
Their nonzero sell prices preserve player choice without making completion
rewards a required source of spending money.

## Main Story Rewards

| Stable reward ID | Trigger | Gold | Items |
| --- | --- | ---: | --- |
| `reward_main_q1_trail_evidence` | Second Quest 1 investigation | 0 | Bren-marked trail knife fragment |
| `reward_main_q1_base` | Turn in any two Quest 1 spots | 60 | 2 Minor Potions |
| `reward_main_q1_spot_03` | Complete a third Quest 1 spot | 15 | 1 Minor Potion |
| `reward_main_q1_full_clear` | Complete all four Quest 1 spots | 15 | Marlow's Trade Charm |
| `reward_main_q2` | Rescue Tallis and turn in Quest 2 | 90 | Ironforge Fieldguard |
| `reward_main_q3_base` | Rescue and turn in two scouts | 120 | 2 Minor Potions, 1 Field Remedy, 1 Camp Ration |
| `reward_main_q3_full_rescue` | Rescue all three scouts | 30 | Suncrest Watch Insignia |
| `reward_grassland_boss_return` | Return after the Goblin Boss | 120 | 1 Camp Ration |

The knife fragment is a protected key item. It cannot be sold or discarded.

## Side Quest Rewards

| Stable reward ID | Quest | Gold | Items |
| --- | --- | ---: | --- |
| `reward_side_taste_of_home` | A Taste Of Home | 0 | 3 Suncrest Suppers |
| `reward_side_roadside_chimes` | The Roadside Chimes | 0 | 2 Field Remedies |
| `reward_side_painters_view` | A Painter's View | 60 | 2 Traveler's Tonics |
| `reward_side_herd_wont_graze` | Where The Herd Won't Graze | 0 | 2 Settlement Teas, Nomad's Woven Cord |

The free Inn and Shrine recoveries promised by the first two side quests are
service-state rewards rather than inventory items. Their one-time claims will
be implemented with quest state.

## Quest Equipment Baseline

| Item | Provisional bonuses |
| --- | --- |
| Marlow's Trade Charm | Accuracy +2, Evasion +2 |
| Ironforge Fieldguard | Max HP +10, Defense +2 |
| Suncrest Watch Insignia | Defense +1, Speed +1 |
| Nomad's Woven Cord | Speed +2, Evasion +2 |

Suncrest Supper and Traveler's Tonic restore 60 HP. Settlement Tea restores
50 HP. These use the existing single-target recovery rules.

## Optional Monster Loot

| Monster | Drop | Chance |
| --- | --- | ---: |
| Slime | Minor Potion | 20% |
| Wild Boar | Camp Ration | 10% |
| Goblin Skirmisher | Minor Potion | 8% |
| Goblin Slinger | Field Remedy | 10% |
| Goblin Guard | Minor Potion | 12% |
| Goblin Raider | Minor Potion | 15% |

Bosses have no random item table. Their distinctive rewards are authored and
claimed once. Random loot rolling is not yet connected to battle completion:
Milestone 17 must first give encounter roll state a save-safe owner so
reloading cannot reroll or duplicate drops.

## Critical-Path Proof

The minimum-income route counted by the automated pacing test includes:

- the complete caravan tutorial;
- Quest 2's required rescue battle;
- the two lowest-gold valid Quest 3 patrols;
- the Goblin Boss group;
- base rewards for Main Quests 1-3;
- the authored Goblin Boss return reward.

This route guarantees `629 gold`.

The representative readiness basket costs `595 gold`:

- two Iron Heavy Blades: `240`;
- one Oak Staff: `115`;
- one Padded Armor: `100`;
- three Minor Potions: `90`;
- two paid full recoveries: `50`.

The remaining `34 gold` is intentionally modest. The proof does not count
Quest 1 skirmishes, the third scout, optional investigation spots, side quests,
ambient battles, item sales, or random drops. Enora's signature scythe and
authored quest equipment are also outside the purchase basket.

## Future Tuning

Revisit these values after the complete Grassland quest route and Goblin Boss
can be played without test fixtures. Adjust data rather than reward-service
logic. Preserve the no-grind proof and exact-once reward contract when tuning.
