using System;
using System.Collections.Generic;

public static class BattleEncounterCatalog
{
    public const string CaravanTutorialId = "encounter_tutorial_caravan";
    public const string Quest1SkirmishAId = "encounter_grassland_q1_a";
    public const string Quest1SkirmishBId = "encounter_grassland_q1_b";
    public const string Quest2RescueId = "encounter_grassland_q2_rescue";
    public const string Quest3PatrolAId = "encounter_grassland_q3_a";
    public const string Quest3PatrolBId = "encounter_grassland_q3_b";
    public const string Quest3PatrolCId = "encounter_grassland_q3_c";
    public const string AmbientSlimesId = "encounter_grassland_ambient_slimes";
    public const string AmbientBoarId = "encounter_grassland_ambient_boar";
    public const string AmbientMixedId = "encounter_grassland_ambient_mixed";
    public const string GoblinBossId = "encounter_grassland_boss";

    private static readonly Dictionary<string, BattleEncounterDefinition>
        encounters = new Dictionary<string, BattleEncounterDefinition>(
            StringComparer.Ordinal)
        {
            {
                CaravanTutorialId,
                new BattleEncounterDefinition(
                    CaravanTutorialId,
                    "The Delayed Caravan",
                    BattleLayoutCatalog.BossId,
                    new[]
                    {
                        "monster_goblin_skirmisher",
                        "monster_goblin_skirmisher"
                    },
                    escapeAllowed: false,
                    isRepeatable: false)
            },
            {
                Quest1SkirmishAId,
                Encounter(
                    Quest1SkirmishAId,
                    "Roadside Skirmish",
                    "monster_goblin_skirmisher",
                    "monster_goblin_skirmisher")
            },
            {
                Quest1SkirmishBId,
                Encounter(
                    Quest1SkirmishBId,
                    "Goblin Crossfire",
                    "monster_goblin_skirmisher",
                    "monster_goblin_slinger")
            },
            {
                Quest2RescueId,
                Encounter(
                    Quest2RescueId,
                    "Tallis Rescue",
                    "monster_goblin_guard",
                    "monster_goblin_skirmisher",
                    "monster_goblin_skirmisher")
            },
            {
                Quest3PatrolAId,
                Encounter(
                    Quest3PatrolAId,
                    "Scout Patrol: Crossroads",
                    "monster_goblin_guard",
                    "monster_goblin_skirmisher",
                    "monster_goblin_slinger")
            },
            {
                Quest3PatrolBId,
                Encounter(
                    Quest3PatrolBId,
                    "Scout Patrol: Ridge",
                    "monster_goblin_guard",
                    "monster_goblin_raider",
                    "monster_goblin_raider")
            },
            {
                Quest3PatrolCId,
                Encounter(
                    Quest3PatrolCId,
                    "Scout Patrol: Bridge",
                    "monster_goblin_raider",
                    "monster_goblin_raider",
                    "monster_goblin_slinger")
            },
            {
                AmbientSlimesId,
                Encounter(
                    AmbientSlimesId,
                    "Slime Pair",
                    "monster_slime",
                    "monster_slime")
            },
            {
                AmbientBoarId,
                Encounter(
                    AmbientBoarId,
                    "Wild Boar",
                    "monster_wild_boar")
            },
            {
                AmbientMixedId,
                Encounter(
                    AmbientMixedId,
                    "Grassland Creatures",
                    "monster_slime",
                    "monster_wild_boar")
            },
            {
                GoblinBossId,
                new BattleEncounterDefinition(
                    GoblinBossId,
                    "Goblin Boss",
                    BattleLayoutCatalog.BossId,
                    new[]
                    {
                        "boss_grassland_goblin",
                        "monster_goblin_guard",
                        "monster_goblin_guard"
                    },
                    escapeAllowed: false,
                    isRepeatable: false)
            }
        };

    public static IReadOnlyCollection<BattleEncounterDefinition> All =>
        encounters.Values;

    public static BattleEncounterDefinition Get(string encounterId)
    {
        if (string.IsNullOrWhiteSpace(encounterId))
        {
            return null;
        }

        return encounters.TryGetValue(
            encounterId.Trim(),
            out BattleEncounterDefinition encounter)
            ? encounter
            : null;
    }

    private static BattleEncounterDefinition Encounter(
        string id,
        string displayName,
        params string[] monsterIds)
    {
        return new BattleEncounterDefinition(
            id,
            displayName,
            BattleLayoutCatalog.StandardId,
            monsterIds);
    }
}
