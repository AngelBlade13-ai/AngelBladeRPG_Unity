# Petals in the Dusk Demo Art Checklist

This is the production checklist for art required by the public demo. Work in
small approval batches. Do not create every asset before the scale, palette,
and character proportions have been tested inside Unity.

## Current Technical Targets

These are working targets, not irreversible engine restrictions:

- Game reference resolution: `320 x 180`, 16:9.
- Current world import scale: `16 pixels per unit`.
- Recommended initial tile grid: `16 x 16` source pixels per tile.
- Filtering: Point/Nearest, with no antialiasing or smoothing.
- Exploration uses one visible party leader. Companions appear in battle and at
  authored story, camp, inn, recruitment, and boss-scene anchors.

The project deliberately uses three levels of character presentation:

1. **Overworld sprites:** compact and readable during movement and exploration.
2. **Battle sprites:** larger and more detailed, with room for expressive poses
   and combat animation.
3. **Dialogue portraits:** the highest-detail character presentation, focused
   on face, clothing, and personality.

### Provisional Scale Test

Do not treat these dimensions as final until Phase 1 is approved in Unity:

- Overworld character: test `16 x 32`; compare against `24 x 32` or `32 x 32`
  only if the silhouette needs more width.
- Ordinary battle character: test `32 x 48` and `48 x 64`, then select one
  standard canvas for playable characters.
- Dialogue portrait: test `64 x 64` and `80 x 80` inside the actual dialogue UI.
- Large bosses may exceed the ordinary battle canvas. Frame the Great Stag in
  the battle layout before choosing its final dimensions.

The source canvas can be larger than the character's occupied silhouette, but
all assets in the same category should use consistent canvas, ground line,
pivot, and scale. Avoid non-integer resizing of finished pixel art.

## Phase 1 - Style And Scale Lock

- [ ] Create one finished `16 x 16` environment tile.
- [ ] Create a tiny ground patch using several copies or variations of it.
- [ ] Create one protagonist overworld sprite standing on that patch.
- [ ] Export one version with the sprite and one environment-only version.
- [ ] Create one provisional battle sprite for the same protagonist.
- [ ] Create one provisional dialogue portrait for the same protagonist.
- [ ] Test all three representations at `320 x 180` in Unity.
- [ ] Lock tile size, PPU, overworld canvas, battle canvas, and portrait canvas.
- [ ] Lock palette, outlines, shading depth, lighting direction, and contrast.
- [ ] Confirm silhouettes remain readable against grass, paths, and UI panels.

Do not begin batch production until this phase is approved.

## Phase 2 - Player-Created Protagonist

Create `2-3` complete preset looks. The exact customization categories remain
open, so begin with complete coordinated presets instead of interchangeable
body-part layers.

For each approved preset:

- [ ] Overworld front-facing standing sprite.
- [ ] Overworld back-facing standing sprite.
- [ ] Overworld side-facing standing sprite; mirror only if asymmetry allows.
- [ ] Neutral battle sprite.
- [ ] Neutral dialogue portrait.
- [ ] Confirm hair, clothing, and skin values remain recognizable at all three
  detail levels.

Later animation pass:

- [ ] Four-direction overworld idle/walk coverage.
- [ ] Battle idle, attack, defend, damage, incapacitated, victory, and basic
  magic/item poses.
- [ ] Additional portrait expressions if the dialogue budget allows.

## Phase 3 - Playable Companions

Use stable IDs in filenames because Iona and Lysander remain placeholder names.
Each companion needs all three presentation levels.

- [ ] `pc_01` Iona: warm, steady, easygoing healer/support.
- [ ] `pc_02` Damari: chaotic, crude, aggressive front-line berserker.
- [ ] `pc_03` Enora: bold, dramatic Blood Mage with a signature scythe.
- [ ] `pc_04` Lysander: grizzled, measured, protective Paladin and leader.

For each companion:

- [ ] Front, back, and side overworld standing sprites.
- [ ] Neutral battle sprite with a readable personal silhouette.
- [ ] Neutral dialogue portrait.
- [ ] Optional warm, tense, and hurt/concerned portrait expressions.
- [ ] Fixed-anchor scene sprite suitable for camp, inn, and story staging.

All characters may equip all 12 jobs. Do not create twelve complete costumes
per character for the demo. Preserve each character's identity and use weapon,
pose, icon, or effect changes to communicate jobs unless a smaller costume
system is explicitly approved later.

## Phase 4 - Named And Quest NPCs

### Required Portraits And Field Sprites

- [ ] Old Marlow: anxious, talkative Whisper Market merchant.
- [ ] Bren: gruff but caring Ironforge Smithy blacksmith.
- [ ] Captain Vashti: authoritative Suncrest Watch leader.
- [ ] Cherry Blossom settlement leader or elder: uneasy but unable to explain
  what is wrong.

### Required Field Sprites, Portrait Optional

- [ ] Tallis, the injured Quest 2 scout.
- [ ] Three visibly distinguishable Quest 3 rescue scouts.
- [ ] Suncrest Inn innkeeper.
- [ ] Sunwell Shrine caretaker.
- [ ] Traveling Grassland painter.
- [ ] Cherry Blossom herder.
- [ ] Opening caravan survivor or driver.
- [ ] Opening quest issuer, unless the final flow uses only the quest board.
- [ ] Stranded Grassland traveler.
- [ ] Lysander near Guild Hall before recruitment; reuse `pc_04` art.

### Reusable Supporting Sets

- [ ] Caravan merchant variation.
- [ ] Suncrest guard variation.
- [ ] Several reusable Suncrest resident variations.
- [ ] Several reusable Cherry Blossom resident variations.
- [ ] Three grazing-animal presentations for `Where The Herd Won't Graze`.

Supporting population can share body bases, animation sets, and portraitless
dialogue. Named roles should remain identifiable through silhouette, palette,
hair, clothing, or props.

## Phase 5 - Grassland And Tutorial Enemies

Goblin roles may share a strong base design but must remain readable in battle.

- [ ] Goblin Skirmisher: basic close-range role.
- [ ] Goblin Slinger: readable ranged weapon and silhouette.
- [ ] Goblin Guard: defensive equipment or heavier silhouette.
- [ ] Goblin Raider: more aggressive elite field silhouette.
- [ ] Tutorial Hobgoblin: clearly larger/stronger than ordinary goblins.
- [ ] Regional Goblin Boss: distinct from the Hobgoblin and clearly the
  strongest regional goblin.
- [ ] Slime.
- [ ] Wild Boar.

For each required enemy visual base:

- [ ] Neutral battle sprite.
- [ ] Attack pose.
- [ ] Damage pose.
- [ ] Defeated pose.
- [ ] Telegraph pose or effect where its mechanics require one.

## Phase 6 - Cherry Blossom Enemies

- [ ] Great Stag regional boss with flowering, branch-like antlers.
- [ ] Neutral/guarded stance.
- [ ] Marking or target-selection tell.
- [ ] Heavy charge telegraph and attack presentation.
- [ ] Staggered presentation.
- [ ] Low-HP Panicked presentation.
- [ ] Subdued pose; it is not killed and should not use a normal death image.
- [ ] Up to two grounded ambient enemy bases only if route playtesting confirms
  they are needed. Their identities are still open.

## Phase 7 - Exploration Environments

### Suncrest Hollow

- [ ] Shared town terrain, path, wall, roof, water, and vegetation tiles.
- [ ] Whisper Market dressing and stalls.
- [ ] Ironforge Smithy exterior and compact interior.
- [ ] Guild Hall exterior and compact interior.
- [ ] Suncrest Inn exterior and compact interior.
- [ ] Sunwell Shrine exterior and compact interior.
- [ ] Suncrest Watch exterior and compact interior.
- [ ] Amber Row residential dressing.
- [ ] Sunroot Grove vegetation and subtle, uncommented unhealthy details.
- [ ] Grassland town exit and Lysander interception space.

### Grassland

- [ ] Vibrant grass, dirt, paths, water, rocks, trees, flowers, and boundaries.
- [ ] Caravan attack site and damaged caravan pieces.
- [ ] Quest investigation evidence props.
- [ ] Hidden chest and scenic overlook dressing.
- [ ] Tallis's collapsed platform.
- [ ] Meadow Sage pickups and roadside chimes.
- [ ] Wildflower ridge, old stone bridge, and quiet pond viewpoints.
- [ ] Goblin approach, war horn, arena, and locked Cherry Blossom route.

### Cherry Blossom And Ember Camp

- [ ] Road from Suncrest Hollow to the settlement.
- [ ] Reusable Ember Camp environment and campfire.
- [ ] Nomadic settlement structures and limited-service stalls.
- [ ] Healthy cherry trees, petals, grass, paths, water, and grazing space.
- [ ] Sacred tree and grove set piece.
- [ ] Great Stag approach and dedicated grove arena.
- [ ] Physical demo-ending barrier beyond the grove.
- [ ] Quiet closing-scene composition.

## Phase 8 - Props, Items, And Equipment

- [ ] Quest board and readable posting states.
- [ ] Shop, smithy, inn, shrine, camp, and recovery props.
- [ ] Camp Ration icon.
- [ ] Required quest-item and reward icons from the demo manifest.
- [ ] Basic consumable icons.
- [ ] Weapon, armor, accessory, and necklace icon families.
- [ ] Eight readable weapon-category silhouettes.
- [ ] Enora's starting scythe.
- [ ] Chest, pickup, interaction, and objective markers.

Decide whether equipped weapons visibly change battle sprites before battle
animation begins. Item icons are required even if equipped models are not.

## Phase 9 - UI And Combat Effects

- [ ] Title treatment for `Petals in the Dusk`.
- [ ] Dialogue box and portrait frame.
- [ ] Exploration HUD and pause menu art.
- [ ] Quest journal surfaces and objective markers.
- [ ] Battle commands and ally/enemy target markers.
- [ ] HP, MP, status, turn feedback, rewards, and subdued-victory presentation.
- [ ] Party formation and bench-management surfaces.
- [ ] Job assignment, job tree, JP, affinity, and demo-cap presentation.
- [ ] Inventory, equipment comparison, item-use, shop, and recovery surfaces.
- [ ] Ember Camp action and companion-talk surfaces.
- [ ] Save/load, settings, route transition, and demo-ending surfaces.
- [ ] Physical hit, weapon, magic, healing, status, guard, and boss-telegraph
  effects.

UI must remain readable at `320 x 180`, support controller focus, and never rely
on color alone to communicate status or valid targets.

## Phase 10 - Animation And Public Demo Polish

- [ ] Complete approved overworld movement animations.
- [ ] Complete playable battle animation sets.
- [ ] Complete enemy and boss battle animation sets.
- [ ] Add portrait expression swaps selected by the dialogue plan.
- [ ] Add ambient environment animation such as water, fire, forge, foliage,
  petals, and flags where it meaningfully improves the scene.
- [ ] Prepare final title/key art and Steam capsule assets after public brand
  clearance.
- [ ] Verify every public screenshot uses final or clearly representative art.

## Asset Delivery Rules

- [ ] Supply transparent PNG exports with no smoothing or antialiasing.
- [ ] Keep source files and exported sprites together under stable asset names.
- [ ] Record canvas size, frame size, PPU, pivot/ground line, palette, and frame
  order in the handoff notes.
- [ ] Use stable IDs rather than display names where a name may change.
- [ ] Keep overworld, battle, portrait, UI, and source files in separate folders.
- [ ] Record every commissioned, purchased, or third-party asset in
  `ASSET_LICENSE_TRACKING.md`.
- [ ] Complete the contributor agreement and compensation terms before final
  production assets are accepted.

## Explicitly Outside Demo Art Scope

- Regions beyond Cherry Blossom.
- Later corruption-state maps and the future changed Cherry Blossom region.
- Mid-story permanent character removal.
- Later characters and endgame entities.
- Follower-train companion movement.
- Full-game camp variations, crafting, fishing, and additional job-tree tiers.
- Twelve unique job costumes for every playable character.
