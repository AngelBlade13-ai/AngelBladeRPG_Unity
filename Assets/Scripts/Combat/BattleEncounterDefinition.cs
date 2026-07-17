using System;
using System.Collections.Generic;

public class BattleEncounterDefinition
{
    private readonly List<string> monsterDefinitionIds;

    public string Id { get; }
    public string DisplayName { get; }
    public string LayoutId { get; }
    public bool EscapeAllowed { get; }
    public bool IsRepeatable { get; }
    public IReadOnlyList<string> MonsterDefinitionIds =>
        monsterDefinitionIds.AsReadOnly();

    public BattleEncounterDefinition(
        string id,
        string displayName,
        string layoutId,
        IEnumerable<string> monsterIds,
        bool escapeAllowed = true,
        bool isRepeatable = true)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("An encounter ID is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException(
                "An encounter display name is required.",
                nameof(displayName));
        }

        BattleLayoutDefinition layout = BattleLayoutCatalog.Get(layoutId);
        if (layout == null)
        {
            throw new ArgumentException("A known battle layout is required.", nameof(layoutId));
        }

        if (monsterIds == null)
        {
            throw new ArgumentNullException(nameof(monsterIds));
        }

        monsterDefinitionIds = new List<string>();
        foreach (string monsterId in monsterIds)
        {
            MonsterDefinition monster = MonsterCatalog.Get(monsterId);
            if (monster == null)
            {
                throw new ArgumentException(
                    $"Unknown monster definition: {monsterId}",
                    nameof(monsterIds));
            }

            monsterDefinitionIds.Add(monster.Id);
        }

        if (monsterDefinitionIds.Count < 1 ||
            monsterDefinitionIds.Count > layout.EnemySlots.Count)
        {
            throw new ArgumentException(
                "The enemy group must fit the selected battle layout.",
                nameof(monsterIds));
        }

        Id = id.Trim();
        DisplayName = displayName.Trim();
        LayoutId = layout.Id;
        EscapeAllowed = escapeAllowed;
        IsRepeatable = isRepeatable;
    }

    public IReadOnlyList<MonsterData> CreateEnemies()
    {
        Dictionary<string, int> totals = new Dictionary<string, int>();
        foreach (string monsterId in monsterDefinitionIds)
        {
            totals[monsterId] = totals.TryGetValue(monsterId, out int count)
                ? count + 1
                : 1;
        }

        Dictionary<string, int> occurrences = new Dictionary<string, int>();
        List<MonsterData> enemies = new List<MonsterData>();
        for (int index = 0; index < monsterDefinitionIds.Count; index++)
        {
            string monsterId = monsterDefinitionIds[index];
            int occurrence = occurrences.TryGetValue(monsterId, out int count)
                ? count + 1
                : 1;
            occurrences[monsterId] = occurrence;

            MonsterData enemy = MonsterCatalog.Get(monsterId).CreateMonster(
                $"{Id}_enemy_{index + 1:00}");
            if (totals[monsterId] > 1)
            {
                enemy.Name = $"{enemy.Name} {occurrence}";
            }

            enemies.Add(enemy);
        }

        return enemies.AsReadOnly();
    }
}
