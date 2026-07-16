# Milestone 14 Decisions

This document summarizes the final demo-scope decisions and links to their
authoritative detail. It does not replace the linked design documents.

## Public Title

The selected public game title is **Petals in the Dusk**.

`AngelBladeRPG_Unity`, `AngelBladeRPG.Runtime`, and existing editor menu labels
remain internal repository, assembly, and prototype identifiers. They do not
need a risky code-wide rename.

## Developer And Publisher Identity

- Working public developer and publisher brand: **Rawr! Studios**.
- Planned Steamworks legal account type: individual/sole proprietor.
- Steamworks legal onboarding must use the owner's full legal name matching
  bank and tax records, not the public brand.
- The store's developer and publisher fields are intended to show
  **Rawr! Studios** after that brand is cleared for use.

The preliminary conflict screen found existing entertainment, design, and game
uses of very similar `RAWR! Studio`, `Rawr Studio`, and `Rawr Studios` names.
The working brand is therefore selected but not cleared. See
`PRODUCT_IDENTITY.md` before creating public accounts or artwork.

## Player Promise

**Petals in the Dusk is a story-driven fantasy RPG where characters and their
relationships are the main draw, and exploration, jobs, and turn-based combat
deepen the player's connection to the party and their changing world.**

The protected feature tiers are in
`PLAYER_PROMISE_FEATURE_HIERARCHY.md`.

## Item And Equipment Economy

- One currency: gold.
- Five equipment slots: Weapon, Armor, Accessory 1, Accessory 2, and Necklace.
- Eight weapon categories shared across compatible jobs.
- Rarity: Common, Uncommon, Rare, Epic, and Legendary.
- Equipment rarity remains distinct from monster affixes such as Savage,
  Nimble, and Elder.
- Main quests, optional rewards, and light encounter drops support the demo
  economy without mandatory grinding.

Full rules are in `ITEM_EQUIPMENT_ECONOMY.md`.

## Asset Ownership

- `CONTRIBUTOR_AGREEMENT_TEMPLATE.md` provides a review-required starting
  template with flat-fee, revenue-share, deferred, and unpaid options.
- `ASSET_LICENSE_TRACKING.md` defines the asset clearance log and workflow.
- Friend-specific agreements remain pending until each person's role, scope,
  rights, credit, and compensation are confirmed.
- No pending contribution or third-party asset may ship in a public build.

