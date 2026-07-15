using System;
using System.Collections.Generic;
using System.Linq;

public interface ITurnOrderRandom
{
    int NextIndex(int minimumInclusive, int maximumExclusive);
}

public class SystemTurnOrderRandom : ITurnOrderRandom
{
    private readonly Random random;

    public SystemTurnOrderRandom()
    {
        random = new Random();
    }

    public int NextIndex(int minimumInclusive, int maximumExclusive)
    {
        return random.Next(minimumInclusive, maximumExclusive);
    }
}

public static class SpeedTurnOrder
{
    public static IReadOnlyList<BattleTurnParticipant> Build(
        IEnumerable<BattleTurnParticipant> participants,
        ITurnOrderRandom tieBreaker = null)
    {
        if (participants == null)
        {
            throw new ArgumentNullException(nameof(participants));
        }

        List<BattleTurnParticipant> ordered =
            new List<BattleTurnParticipant>();
        HashSet<string> combatantIds = new HashSet<string>();

        foreach (BattleTurnParticipant participant in participants)
        {
            if (participant == null)
            {
                throw new ArgumentException(
                    "Turn-order participants cannot contain null.",
                    nameof(participants));
            }

            if (!combatantIds.Add(participant.CombatantId))
            {
                throw new ArgumentException(
                    $"Duplicate combatant ID: {participant.CombatantId}",
                    nameof(participants));
            }

            ordered.Add(participant);
        }

        ordered = ordered
            .OrderByDescending(participant => participant.Speed)
            .ToList();
        ShuffleEqualSpeedGroups(
            ordered,
            tieBreaker ?? new SystemTurnOrderRandom());
        return ordered;
    }

    private static void ShuffleEqualSpeedGroups(
        List<BattleTurnParticipant> ordered,
        ITurnOrderRandom tieBreaker)
    {
        int groupStart = 0;

        while (groupStart < ordered.Count)
        {
            int groupEnd = groupStart + 1;
            while (groupEnd < ordered.Count &&
                ordered[groupEnd].Speed == ordered[groupStart].Speed)
            {
                groupEnd++;
            }

            for (int index = groupEnd - 1; index > groupStart; index--)
            {
                int swapIndex = tieBreaker.NextIndex(groupStart, index + 1);
                BattleTurnParticipant current = ordered[index];
                ordered[index] = ordered[swapIndex];
                ordered[swapIndex] = current;
            }

            groupStart = groupEnd;
        }
    }
}
