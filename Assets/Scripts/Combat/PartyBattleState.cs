using System;
using System.Collections.Generic;

public enum BattleTargetType
{
    Self,
    SingleAlly,
    SingleEnemy,
    AllAllies,
    AllEnemies,
    IncapacitatedAlly
}

public sealed class PartyBattleState
{
    private readonly List<ICombatant> partyMembers;
    private readonly List<ICombatant> enemies;
    private readonly Dictionary<string, ICombatant> combatants;

    public IReadOnlyList<ICombatant> PartyMembers => partyMembers.AsReadOnly();
    public IReadOnlyList<ICombatant> Enemies => enemies.AsReadOnly();
    public bool IsPartyDefeated => HasNoLivingCombatants(partyMembers);
    public bool AreEnemiesDefeated => HasNoLivingCombatants(enemies);

    public PartyBattleState(
        IEnumerable<ICombatant> activePartyMembers,
        IEnumerable<ICombatant> enemyCombatants)
    {
        partyMembers = CopyCombatants(
            activePartyMembers,
            nameof(activePartyMembers));
        enemies = CopyCombatants(enemyCombatants, nameof(enemyCombatants));

        if (partyMembers.Count < 1 ||
            partyMembers.Count > PartyRoster.MaximumActiveMembers)
        {
            throw new ArgumentException(
                $"A battle party must contain 1-{PartyRoster.MaximumActiveMembers} members.",
                nameof(activePartyMembers));
        }

        if (enemies.Count < 1)
        {
            throw new ArgumentException(
                "A battle must contain at least one enemy.",
                nameof(enemyCombatants));
        }

        combatants = new Dictionary<string, ICombatant>(StringComparer.Ordinal);
        AddUniqueCombatants(partyMembers);
        AddUniqueCombatants(enemies);
    }

    public static PartyBattleState FromRoster(
        PartyRoster roster,
        IEnumerable<ICombatant> enemyCombatants)
    {
        if (roster == null)
        {
            throw new ArgumentNullException(nameof(roster));
        }

        return new PartyBattleState(
            roster.GetActiveCharacters(),
            enemyCombatants);
    }

    public ICombatant GetCombatant(string combatantId)
    {
        if (string.IsNullOrWhiteSpace(combatantId))
        {
            return null;
        }

        return combatants.TryGetValue(
            combatantId.Trim(),
            out ICombatant combatant)
            ? combatant
            : null;
    }

    public bool TryAddPartyMember(ICombatant combatant)
    {
        if (!IsValidNewCombatant(combatant) ||
            partyMembers.Count >= PartyRoster.MaximumActiveMembers)
        {
            return false;
        }

        partyMembers.Add(combatant);
        combatants.Add(combatant.CombatantId, combatant);
        return true;
    }

    public bool TryReplaceEnemies(IEnumerable<ICombatant> replacements)
    {
        List<ICombatant> nextEnemies;
        try
        {
            nextEnemies = CopyCombatants(replacements, nameof(replacements));
        }
        catch (ArgumentException)
        {
            return false;
        }

        if (nextEnemies.Count < 1)
        {
            return false;
        }

        HashSet<string> nextIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (ICombatant enemy in nextEnemies)
        {
            if (!nextIds.Add(enemy.CombatantId) ||
                combatants.ContainsKey(enemy.CombatantId))
            {
                return false;
            }
        }

        foreach (ICombatant enemy in enemies)
        {
            combatants.Remove(enemy.CombatantId);
        }

        enemies.Clear();
        enemies.AddRange(nextEnemies);
        AddUniqueCombatants(enemies);
        return true;
    }

    public IReadOnlyList<ICombatant> GetValidTargets(
        string actorId,
        BattleTargetType targetType)
    {
        ICombatant actor = GetCombatant(actorId);
        if (actor == null || actor.Stats.CurrentHp <= 0)
        {
            return Array.Empty<ICombatant>();
        }

        bool actorIsPartyMember = partyMembers.Contains(actor);
        List<ICombatant> allies = actorIsPartyMember ? partyMembers : enemies;
        List<ICombatant> opponents = actorIsPartyMember ? enemies : partyMembers;

        switch (targetType)
        {
            case BattleTargetType.Self:
                return new[] { actor };
            case BattleTargetType.SingleAlly:
            case BattleTargetType.AllAllies:
                return FilterByIncapacitation(allies, false);
            case BattleTargetType.SingleEnemy:
            case BattleTargetType.AllEnemies:
                return FilterByIncapacitation(opponents, false);
            case BattleTargetType.IncapacitatedAlly:
                return FilterByIncapacitation(allies, true);
            default:
                throw new ArgumentOutOfRangeException(nameof(targetType));
        }
    }

    public bool TrySelectTarget(
        string actorId,
        BattleTargetType targetType,
        string targetId,
        out ICombatant target)
    {
        target = null;
        if (string.IsNullOrWhiteSpace(targetId) ||
            targetType == BattleTargetType.AllAllies ||
            targetType == BattleTargetType.AllEnemies)
        {
            return false;
        }

        foreach (ICombatant validTarget in GetValidTargets(actorId, targetType))
        {
            if (string.Equals(
                validTarget.CombatantId,
                targetId.Trim(),
                StringComparison.Ordinal))
            {
                target = validTarget;
                return true;
            }
        }

        return false;
    }

    private void AddUniqueCombatants(IEnumerable<ICombatant> entries)
    {
        foreach (ICombatant combatant in entries)
        {
            if (combatants.ContainsKey(combatant.CombatantId))
            {
                throw new ArgumentException(
                    $"Duplicate combatant ID: {combatant.CombatantId}.");
            }

            combatants.Add(combatant.CombatantId, combatant);
        }
    }

    private bool IsValidNewCombatant(ICombatant combatant)
    {
        return combatant != null &&
            !string.IsNullOrWhiteSpace(combatant.CombatantId) &&
            combatant.Stats != null &&
            !combatants.ContainsKey(combatant.CombatantId);
    }

    private static List<ICombatant> CopyCombatants(
        IEnumerable<ICombatant> source,
        string parameterName)
    {
        if (source == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        List<ICombatant> copy = new List<ICombatant>();
        foreach (ICombatant combatant in source)
        {
            if (combatant == null ||
                string.IsNullOrWhiteSpace(combatant.CombatantId) ||
                combatant.Stats == null)
            {
                throw new ArgumentException(
                    "Battle combatants require an ID and stats.",
                    parameterName);
            }

            copy.Add(combatant);
        }

        return copy;
    }

    private static IReadOnlyList<ICombatant> FilterByIncapacitation(
        IEnumerable<ICombatant> source,
        bool incapacitated)
    {
        List<ICombatant> results = new List<ICombatant>();
        foreach (ICombatant combatant in source)
        {
            if ((combatant.Stats.CurrentHp <= 0) == incapacitated)
            {
                results.Add(combatant);
            }
        }

        return results.AsReadOnly();
    }

    private static bool HasNoLivingCombatants(IEnumerable<ICombatant> source)
    {
        foreach (ICombatant combatant in source)
        {
            if (combatant.Stats.CurrentHp > 0)
            {
                return false;
            }
        }

        return true;
    }
}
