using System;
using System.Collections.Generic;

public struct BattleSlotPosition
{
    public float X { get; }
    public float Y { get; }

    public BattleSlotPosition(float x, float y)
    {
        X = x;
        Y = y;
    }
}

public class BattleLayoutDefinition
{
    private readonly List<BattleSlotPosition> partySlots;
    private readonly List<BattleSlotPosition> enemySlots;

    public string Id { get; }
    public IReadOnlyList<BattleSlotPosition> PartySlots =>
        partySlots.AsReadOnly();
    public IReadOnlyList<BattleSlotPosition> EnemySlots =>
        enemySlots.AsReadOnly();

    public BattleLayoutDefinition(
        string id,
        IEnumerable<BattleSlotPosition> partySlotPositions,
        IEnumerable<BattleSlotPosition> enemySlotPositions)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("A battle layout ID is required.", nameof(id));
        }

        partySlots = CopySlots(partySlotPositions, nameof(partySlotPositions));
        enemySlots = CopySlots(enemySlotPositions, nameof(enemySlotPositions));
        if (partySlots.Count < PartyRoster.MaximumActiveMembers)
        {
            throw new ArgumentException(
                "A battle layout needs four party slots.",
                nameof(partySlotPositions));
        }

        if (enemySlots.Count < 1)
        {
            throw new ArgumentException(
                "A battle layout needs at least one enemy slot.",
                nameof(enemySlotPositions));
        }

        Id = id.Trim();
    }

    private static List<BattleSlotPosition> CopySlots(
        IEnumerable<BattleSlotPosition> source,
        string parameterName)
    {
        if (source == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return new List<BattleSlotPosition>(source);
    }
}

public static class BattleLayoutCatalog
{
    public const string StandardId = "layout_standard";
    public const string BossId = "layout_boss";

    private static readonly Dictionary<string, BattleLayoutDefinition> layouts =
        new Dictionary<string, BattleLayoutDefinition>(StringComparer.Ordinal)
        {
            {
                StandardId,
                new BattleLayoutDefinition(
                    StandardId,
                    CreatePartySlots(),
                    new[]
                    {
                        new BattleSlotPosition(0.68f, 0.42f),
                        new BattleSlotPosition(0.78f, 0.52f),
                        new BattleSlotPosition(0.68f, 0.62f),
                        new BattleSlotPosition(0.82f, 0.34f)
                    })
            },
            {
                BossId,
                new BattleLayoutDefinition(
                    BossId,
                    CreatePartySlots(),
                    new[]
                    {
                        new BattleSlotPosition(0.75f, 0.5f),
                        new BattleSlotPosition(0.64f, 0.62f),
                        new BattleSlotPosition(0.64f, 0.38f),
                        new BattleSlotPosition(0.84f, 0.68f),
                        new BattleSlotPosition(0.84f, 0.32f)
                    })
            }
        };

    public static IReadOnlyCollection<BattleLayoutDefinition> All =>
        layouts.Values;

    public static BattleLayoutDefinition Get(string layoutId)
    {
        if (string.IsNullOrWhiteSpace(layoutId))
        {
            return null;
        }

        return layouts.TryGetValue(
            layoutId.Trim(),
            out BattleLayoutDefinition layout)
            ? layout
            : null;
    }

    private static BattleSlotPosition[] CreatePartySlots()
    {
        return new[]
        {
            new BattleSlotPosition(0.28f, 0.42f),
            new BattleSlotPosition(0.18f, 0.52f),
            new BattleSlotPosition(0.28f, 0.62f),
            new BattleSlotPosition(0.14f, 0.34f)
        };
    }
}
