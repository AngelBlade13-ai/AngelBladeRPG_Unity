# Suncrest Hollow District Layout

Suncrest Hollow is built as eight interconnected outdoor district scenes, not
one oversized town Tilemap. The districts collectively form the home hub and
must feel like parts of one continuous place rather than world-map travel
destinations.

This document owns the district scene boundaries, working blockout sizes, and
connection graph. Exact dimensions may change after traversal playtests.

## Foundation Status

- [x] Imported the 68-tile Phase 1 environment set at `16 PPU` with Point
  filtering.
- [x] Generated all eight district scene foundations.
- [x] Registered all eight district scenes in Build Settings.
- [x] Added shared Ground, Decoration, Collision, and Foreground Tilemaps.
- [x] Added reciprocal district exits and matching arrival spawn IDs.
- [x] Completed a manual traversal smoke test through every district and
  confirmed the interaction transitions return to the expected scenes.
- [ ] Replace the generic grass, dirt routes, and cyan exit markers with
  district-specific authored layouts and landmarks.

The generated Guild Hall and Whisper Market building-placeholder pass was
rejected and reverted. Do not use `SuncrestDistrictBlockoutBuilder`'s
`Apply Guild Hall And Market Blockouts` or `Rebuild Guild Hall And Market
Blockouts` commands as a source of permanent layout, building, road, or
decoration decisions. The user owns those visual and level-design choices.
Automation remains appropriate for neutral technical scaffolding, scene wiring,
colliders, spawn points, transitions, and test fixtures, or when the user
explicitly requests a specific authored placement.

## Scale And Density

- Tile grid: `16 x 16` source pixels at `16 PPU`.
- Reference resolution: `320 x 180`.
- A gameplay camera shows roughly `20 x 11.25` world tiles at once.
- Most districts should cover approximately two camera screens in each
  direction; larger social or natural areas may approach three.
- Exploration comes from routes, landmarks, NPCs, services, and authored
  details. Do not enlarge a district with empty ground solely to increase its
  walking time.
- Keep primary routes at least two walkable tiles wide and make every district
  entrance readable from its arrival spawn.

## District Scenes

| Stable location ID | Working scene name | Blockout size | Primary purpose |
| --- | --- | ---: | --- |
| `suncrest_guild_hall` | `SuncrestGuildHallScene` | `40 x 28` | Central arrival, quest board, jobs, party help, pre-recruit Lysander |
| `suncrest_whisper_market` | `SuncrestWhisperMarketScene` | `48 x 28` | Lively commerce, Old Marlow, consumables, Quest 1 |
| `suncrest_ironforge` | `SuncrestIronforgeScene` | `32 x 24` | Bren, smithy service, equipment, Quest 2 |
| `suncrest_inn` | `SuncrestInnScene` | `36 x 24` | Recovery, party scenes, `A Taste Of Home` |
| `suncrest_sunwell_shrine` | `SuncrestShrineScene` | `36 x 28` | Recovery, caretaker, `The Roadside Chimes` |
| `suncrest_amber_row` | `SuncrestAmberRowScene` | `44 x 30` | Homes, ordinary town life, quieter NPC space |
| `suncrest_sunroot_grove` | `SuncrestGroveScene` | `48 x 32` | Town green and subtle, uncommented decay details |
| `suncrest_watch` | `SuncrestWatchScene` | `40 x 28` | Captain Vashti, Quest 3, boss handoff, Grassland gate |

Stable location IDs belong in save and quest data. Scene names are loading
details and may change without migrating gameplay records.

## Connection Graph

Guild Hall is the central navigation anchor. The surrounding links create
loops so most services can be reached without repeatedly retracing one route.

| District | Direct connections |
| --- | --- |
| Guild Hall | Whisper Market, Amber Row, Suncrest Inn, Suncrest Watch |
| Whisper Market | Guild Hall, Ironforge Smithy, Suncrest Inn |
| Ironforge Smithy | Whisper Market |
| Suncrest Inn | Whisper Market, Guild Hall, Sunroot Grove |
| Amber Row | Guild Hall, Sunwell Shrine, Sunroot Grove |
| Sunroot Grove | Amber Row, Suncrest Inn |
| Sunwell Shrine | Amber Row, Suncrest Watch |
| Suncrest Watch | Guild Hall, Sunwell Shrine, Grassland exit |

The blockout implementation may bend this graph geographically, but it must
preserve these practical rules:

- Guild Hall connects directly to Whisper Market, Amber Row, the inn, and the
  Watch.
- Whisper Market connects to Ironforge Smithy and the inn.
- Amber Row connects to the shrine and Sunroot Grove.
- The inn and Grove provide a southern loop.
- The shrine and Watch provide a northern loop.
- The Grassland gate is in Suncrest Watch so Lysander can perform the required
  interception after the Quest 3 handoff.
- No required town service is more than two district transitions from Guild
  Hall.

## Scene Transition Contract

- District borders use authored street, archway, bridge, or gate transitions,
  not a world-map menu.
- Each connection has a matching `SceneDoorInteractable2D` destination and
  `PlayerSpawnPoint2D` arrival point in the neighboring scene.
- Spawn IDs describe their origin, such as `FromGuildHall` or
  `FromWhisperMarket`, and must be unique within the destination scene.
- Arrival points face the player away from the transition and leave enough
  space to avoid immediately loading the previous scene again.
- Add a short fade when the final presentation pass begins. Loading should not
  resemble long-distance travel.
- Battle return data must preserve the exact district scene and spawn rather
  than returning every encounter to a generic town scene.

## District Composition Rules

Each district blockout needs:

- One first-screen landmark that communicates the district's identity.
- One clear primary route and at least one secondary route, nook, or loop.
- Purposeful space for its required NPCs, service, quest interactions, and
  later story-state changes.
- Collision boundaries that look like buildings, fences, water, vegetation,
  elevation, or town walls rather than invisible boxes.
- An uncluttered arrival area and readable exits.
- Decoration concentrated near landmarks and edges, leaving movement and NPC
  silhouettes clear.

The imported Phase 1 art is suitable for terrain blockouts and visual-density
tests. Soft Grass V6 is the default Suncrest ground; dirt paths establish the
navigation hierarchy. Current flower, stone, and wood tiles have baked grass
backgrounds and should be used sparingly until transparent decoration exports
are available.

## Migration Order

1. Preserve `TownScene` as the working prototype and regression-test reference.
2. Run `Tools > AngelBlade RPG > World > Create Missing Suncrest District
   Scenes` to generate the non-destructive district foundations. Existing
   district scenes are skipped whenever the command is rerun.
3. Treat generated Guild Hall as the reusable district-scene template.
4. Verify the shared player, camera, HUD, Tilemap layers, spawn controller, and scene
   transitions to the template.
5. Test the generated Whisper Market and Guild Hall round trip.
6. Test movement, camera framing, scene return placement, interactions, and
   battle return before authoring the remaining districts.
7. Replace each generated cross-road blockout with authored district content
   one scene at a time using the approved graph.
8. Change the new-game destination from `TownScene` only after the Guild Hall
   scene satisfies the prototype's existing gameplay responsibilities.

The builder copies the proven `TownScene` wiring, removes prototype-only test
encounters and doors from each copy, creates Ground, Decoration, Collision, and
Foreground Tilemaps, paints Soft Grass V6 and dirt navigation routes, adds a
camera-safe terrain margin, creates reciprocal exits and arrival spawns, and
registers all eight scenes in Build Settings. It does not modify `TownScene` or
overwrite an existing district scene.
