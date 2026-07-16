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
