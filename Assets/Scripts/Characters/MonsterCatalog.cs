using System;
using System.Collections.Generic;

public static class MonsterCatalog
{
    private static readonly Dictionary<string, MonsterDefinition> monsters =
        new Dictionary<string, MonsterDefinition>(StringComparer.Ordinal)
        {
            {
                "monster_goblin",
                new MonsterDefinition(
                    "monster_goblin",
                    "Goblin",
                    35,
                    8,
                    1,
                    8,
                    0,
                    0,
                    0,
                    10,
                    15,
                    accuracy: 90,
                    evasion: 5,
                    criticalChance: 5)
            },
            {
                "monster_ogre",
                new MonsterDefinition(
                    "monster_ogre",
                    "Ogre",
                    100,
                    30,
                    20,
                    5,
                    0,
                    0,
                    4,
                    20,
                    25,
                    accuracy: 85,
                    evasion: 0,
                    criticalChance: 10)
            },
            {
                "monster_slime",
                new MonsterDefinition(
                    "monster_slime",
                    "Slime",
                    20,
                    5,
                    0,
                    4,
                    0,
                    0,
                    0,
                    4,
                    6,
                    accuracy: 85,
                    evasion: 0,
                    criticalChance: 0)
            },
            {
                "monster_wisp",
                new MonsterDefinition(
                    "monster_wisp",
                    "Wisp",
                    30,
                    4,
                    2,
                    14,
                    12,
                    10,
                    9,
                    8,
                    12,
                    accuracy: 95,
                    evasion: 15,
                    criticalChance: 15)
            },
            {
                "monster_goblin_skirmisher",
                new MonsterDefinition(
                    "monster_goblin_skirmisher",
                    "Goblin Skirmisher",
                    35, 8, 1, 8, 0, 0, 0, 8, 15,
                    accuracy: 90,
                    evasion: 5,
                    criticalChance: 5)
            },
            {
                "monster_goblin_slinger",
                new MonsterDefinition(
                    "monster_goblin_slinger",
                    "Goblin Slinger",
                    28, 7, 0, 12, 0, 0, 0, 9, 16,
                    accuracy: 92,
                    evasion: 12,
                    criticalChance: 7)
            },
            {
                "monster_goblin_guard",
                new MonsterDefinition(
                    "monster_goblin_guard",
                    "Goblin Guard",
                    55, 6, 6, 5, 0, 0, 2, 12, 20,
                    accuracy: 88,
                    evasion: 2,
                    criticalChance: 3)
            },
            {
                "monster_goblin_raider",
                new MonsterDefinition(
                    "monster_goblin_raider",
                    "Goblin Raider",
                    42, 12, 2, 10, 0, 0, 1, 14, 24,
                    accuracy: 90,
                    evasion: 7,
                    criticalChance: 10)
            },
            {
                "monster_tutorial_hobgoblin",
                new MonsterDefinition(
                    "monster_tutorial_hobgoblin",
                    "Hobgoblin",
                    100, 14, 5, 7, 0, 0, 3, 25, 45,
                    accuracy: 92,
                    evasion: 4,
                    criticalChance: 8,
                    jobPointReward: 2)
            },
            {
                "boss_grassland_goblin",
                new MonsterDefinition(
                    "boss_grassland_goblin",
                    "Goblin Boss",
                    240, 18, 8, 8, 0, 0, 5, 80, 100,
                    accuracy: 93,
                    evasion: 5,
                    criticalChance: 12,
                    jobPointReward: 4)
            },
            {
                "monster_wild_boar",
                new MonsterDefinition(
                    "monster_wild_boar",
                    "Wild Boar",
                    50, 11, 3, 9, 0, 0, 1, 10, 18,
                    accuracy: 90,
                    evasion: 4,
                    criticalChance: 7)
            }
        };

    public static IReadOnlyCollection<MonsterDefinition> All =>
        monsters.Values;

    public static MonsterDefinition Get(string monsterId)
    {
        if (string.IsNullOrWhiteSpace(monsterId))
        {
            return null;
        }

        return monsters.TryGetValue(
            monsterId.Trim(),
            out MonsterDefinition definition)
            ? definition
            : null;
    }
}
