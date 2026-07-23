using System;
using System.Collections.Generic;

public class CharacterRosterHistory
{
    private readonly Dictionary<string, int> bondPoints =
        new Dictionary<string, int>(StringComparer.Ordinal);

    public string CharacterId { get; }
    public int BattlesActive { get; private set; }
    public int BattlesBenched { get; private set; }
    public int ConsecutiveBenchedBattles { get; private set; }

    public CharacterRosterHistory(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
        {
            throw new ArgumentException(
                "A character ID is required.",
                nameof(characterId));
        }

        CharacterId = characterId.Trim();
    }

    internal void RecordBattleParticipation(bool wasActive)
    {
        if (wasActive)
        {
            BattlesActive++;
            ConsecutiveBenchedBattles = 0;
            return;
        }

        BattlesBenched++;
        ConsecutiveBenchedBattles++;
    }

    public int GetBondPoints(string otherCharacterId)
    {
        if (string.IsNullOrWhiteSpace(otherCharacterId))
        {
            return 0;
        }

        return bondPoints.TryGetValue(
            otherCharacterId.Trim(),
            out int points)
            ? points
            : 0;
    }

    internal bool TryAddBondPoints(string otherCharacterId, int points)
    {
        if (string.IsNullOrWhiteSpace(otherCharacterId) || points <= 0)
        {
            return false;
        }

        string otherId = otherCharacterId.Trim();
        if (string.Equals(otherId, CharacterId, StringComparison.Ordinal))
        {
            return false;
        }

        bondPoints[otherId] = GetBondPoints(otherId) + points;
        return true;
    }

    internal bool TryRestore(
        int battlesActive,
        int battlesBenched,
        int consecutiveBenchedBattles,
        IEnumerable<KeyValuePair<string, int>> bonds)
    {
        if (battlesActive < 0 || battlesBenched < 0 ||
            consecutiveBenchedBattles < 0 ||
            consecutiveBenchedBattles > battlesBenched || bonds == null)
        {
            return false;
        }

        Dictionary<string, int> restored =
            new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (KeyValuePair<string, int> bond in bonds)
        {
            if (string.IsNullOrWhiteSpace(bond.Key) || bond.Value <= 0)
            {
                return false;
            }

            string otherId = bond.Key.Trim();
            if (string.Equals(otherId, CharacterId, StringComparison.Ordinal) ||
                restored.ContainsKey(otherId))
            {
                return false;
            }

            restored.Add(otherId, bond.Value);
        }

        BattlesActive = battlesActive;
        BattlesBenched = battlesBenched;
        ConsecutiveBenchedBattles = consecutiveBenchedBattles;
        bondPoints.Clear();
        foreach (KeyValuePair<string, int> bond in restored)
        {
            bondPoints.Add(bond.Key, bond.Value);
        }

        return true;
    }
}
