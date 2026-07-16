# Item And Equipment Economy

This document defines the demo economy structure. Exact item statistics,
prices, drop rates, quantities, and reward tuning remain Milestone 16 data.

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
| Sword | Knight, Mercenary, Paladin, Tactician |
| Greatblade | Knight, Reaver, Mercenary |
| Dagger | Rogue, Ranger, Tactician |
| Bow | Ranger, Rogue |
| Staff | Mage, White Mage, Summoner |
| Scythe | Reaver, Blood Mage |
| Tome | Mage, Blood Mage, White Mage, Tactician, Summoner |
| Instrument | Bard |

This matrix is the initial production interpretation of the eight-category
decision and should be balance-tested before item content is authored in bulk.
Enora starts with a scythe-compatible weapon.

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

Monster variation uses separate affix language such as Savage, Nimble, and
Elder. Item UI must not use those words as rarity tiers.

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

