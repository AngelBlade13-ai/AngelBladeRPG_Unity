using System;
using System.Collections.Generic;

public sealed class PartyCommandSelection
{
    private readonly PartyBattleState battle;
    private readonly List<PartyBattleCommand> commands =
        new List<PartyBattleCommand>();
    private int selectedTargetIndex;

    public IReadOnlyList<PartyBattleCommand> Commands => commands.AsReadOnly();
    public bool HasQueuedCommands => commands.Count > 0;
    public bool IsComplete => CurrentActor == null;

    public ICombatant CurrentActor
    {
        get
        {
            int livingIndex = 0;
            foreach (ICombatant partyMember in battle.PartyMembers)
            {
                if (partyMember.Stats.CurrentHp <= 0)
                {
                    continue;
                }

                if (livingIndex == commands.Count)
                {
                    return partyMember;
                }

                livingIndex += 1;
            }

            return null;
        }
    }

    public ICombatant SelectedTarget
    {
        get
        {
            IReadOnlyList<ICombatant> targets = GetCurrentTargets();
            if (targets.Count == 0)
            {
                return null;
            }

            selectedTargetIndex = WrapIndex(
                selectedTargetIndex,
                targets.Count);
            return targets[selectedTargetIndex];
        }
    }

    public PartyCommandSelection(PartyBattleState battle)
    {
        this.battle = battle ?? throw new ArgumentNullException(nameof(battle));
    }

    public bool CycleTarget(int direction)
    {
        IReadOnlyList<ICombatant> targets = GetCurrentTargets();
        if (targets.Count < 1 || direction == 0)
        {
            return false;
        }

        selectedTargetIndex = WrapIndex(
            selectedTargetIndex + Math.Sign(direction),
            targets.Count);
        return true;
    }

    public bool TryQueueAttack()
    {
        ICombatant actor = CurrentActor;
        ICombatant target = SelectedTarget;
        if (actor == null || target == null)
        {
            return false;
        }

        commands.Add(PartyBattleCommand.Attack(
            actor.CombatantId,
            target.CombatantId));
        selectedTargetIndex = 0;
        return true;
    }

    public bool TryQueueDefend()
    {
        ICombatant actor = CurrentActor;
        if (actor == null)
        {
            return false;
        }

        commands.Add(PartyBattleCommand.Defend(actor.CombatantId));
        selectedTargetIndex = 0;
        return true;
    }

    private IReadOnlyList<ICombatant> GetCurrentTargets()
    {
        ICombatant actor = CurrentActor;
        return actor == null
            ? Array.Empty<ICombatant>()
            : battle.GetValidTargets(
                actor.CombatantId,
                BattleTargetType.SingleEnemy);
    }

    private static int WrapIndex(int index, int count)
    {
        int wrapped = index % count;
        return wrapped < 0 ? wrapped + count : wrapped;
    }
}
