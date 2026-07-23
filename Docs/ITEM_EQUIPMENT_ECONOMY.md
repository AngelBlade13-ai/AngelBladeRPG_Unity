# Item And Equipment Economy

This document defines the Tier 2 demo economy structure. Itemization should
provide satisfying build variety without competing with story pacing or
requiring grinding. Exact item statistics, prices, drop rates, quantities, and
reward tuning remain Milestone 16 data.

## Milestone 16 Implementation Checkpoint

Batch one establishes the code-first economy foundation:

- Fourteen stable demo definitions cover three consumables, all eight weapon
  categories, armor, accessories, and necklaces.
- `GameSession` owns the shared inventory and enforces per-item stack limits.
- Every playable character owns five equipment slots and receives equipment
  bonuses through the same derived-stat recalculation used by jobs.
- Incompatible weapons return to shared inventory when a job switch is
  confirmed through the inventory-aware assignment path.
- Permanent roster removal clears equipped items without returning them to
  shared inventory.

Item use, rest rules, shops, comparisons, reward bundles, and provisional loot
tables are implemented. Exact values and the no-grind pacing proof are recorded
in `DEMO_ECONOMY_BALANCE.md`; final tuning remains subject to end-to-end demo
playtests.

Batch two adds the headless service rules used by later UI:

- Minor Potions restore one living target and are consumed only when healing
  occurs. Camp Rations cannot be spent through ordinary item use.
- The first confirmed tutorial rest is free exactly once. Later successful
  rests consume one ration and fully restore every available active or benched
  character; failed and cancelled attempts consume nothing.
- Whisper Market and Ironforge have stable shop inventories. Buying and selling
  update gold and inventory atomically, and protected equipment cannot be sold.
- Paid town recovery restores the same available roster without charging a
  cancelled, failed, or unnecessary attempt.

Negative-condition removal remains dormant until the temporary-condition model
exists; Field Remedy is defined but cannot be consumed prematurely.

## Currency And Pacing

- Gold is the only currency used for shops and services.
- JP remains character progression, not a spendable shop currency.
- Main quest rewards fund expected upgrades and recovery supplies.
- Optional quests and exploration provide useful breadth and convenience.
- Ambient encounters provide light gold and item drops, but grinding is never
  required to afford critical-path readiness.
- Selling equipment and items returns gold; quest evidence and key items cannot
  be sold or discarded.

## Equipment Slots

Every playable character has five equipment slots:

1. Weapon.
2. Armor.
3. Accessory 1.
4. Accessory 2.
5. Necklace.

Accessories share one item pool across their two slots. The same unique item
instance cannot occupy both slots. Necklaces remain a distinct category for
more identity-defining or utility-focused bonuses.

## Weapon Categories

Weapons are job-compatible rather than fully universal. The initial eight
categories and intended compatibility are:

| Category | Compatible jobs |
| --- | --- |
| Heavy Blade | Knight, Reaver, Mercenary, Paladin |
| Scythe | Blood Mage |
| Bow | Ranger |
| Dagger | Rogue |
| Staff | Mage, White Mage |
| Instrument | Bard |
| Tome Or Charm | Tactician |
| Bonded Totem Or Horn | Summoner |

Heavy Blade includes swords, great blades, and sword-and-shield items. A shield
is part of that Weapon item rather than a separate equipment slot. Enora starts
with a scythe-compatible weapon.

- Switching jobs previews weapon compatibility.
- An incompatible equipped weapon moves safely to shared inventory when the
  switch is confirmed; it is never destroyed.
- The character may switch while unarmed if no compatible weapon is available.
- Armor, accessories, and necklaces are not job-locked in the demo unless a
  later item explicitly states a tested requirement.
- Permanent job-tree stat nodes remain active independently of equipment.

## Rarity

Equipment rarity uses these player-facing tiers:

1. Common.
2. Uncommon.
3. Rare.
4. Epic.
5. Legendary.

Rarity communicates relative scarcity and expected power budget. It does not
guarantee that a higher-rarity item is correct for every build.

Legendary equipment may have a small unique effect and should generally come
from special quests or other authored rewards rather than ordinary random
drops. Individual item names carry narrative flavor; rarity labels stay plain
and functional.

Monster variation uses separate affix language such as Savage, Nimble, and
Elder. Item UI must not use those words as rarity tiers.

## Demo Consumables

- One basic single-target HP recovery potion family.
- At least one item that removes a supported negative status.
- Camp Rations and named quest consumables already required by the content
  manifest.

MP and incapacitation recovery consumables are not required for the demo. Add
them only if encounter testing shows that camp, town services, and job recovery
cannot support fair critical-path pacing.

## Demo Service Roles

- Whisper Market sells consumables and later Camp Rations.
- Ironforge Smithy sells and buys weapons and armor.
- The Suncrest Inn sells full recovery and hosts party scenes.
- The Sunwell Shrine provides recovery and removable-status services.
- Cherry Blossom offers only limited recovery supplies and a modest rest.
- Guild Hall job/party services do not spend gold unless a later explicit
  economy decision adds a fee.

## Balance Requirements

- The player can finish the critical path using guaranteed rewards and ordinary
  encounter income.
- Optional rewards feel worthwhile without becoming required boss keys.
- Shops offer meaningful decisions rather than obvious upgrades for every slot
  after every quest.
- Consumable prices support use instead of encouraging permanent hoarding.
- No random drop is required to demonstrate a job or defeat a demo boss.
- Loot, purchases, sales, and one-time rewards remain deterministic under
  save/load and cannot duplicate.
