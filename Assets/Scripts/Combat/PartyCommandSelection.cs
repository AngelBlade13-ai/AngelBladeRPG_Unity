using System;
using System.Collections.Generic;

public sealed class PartyCommandSelection
{
    private readonly PartyBattleState battle;
    private readonly List<PartyBattleCommand> commands =
        new List<PartyBattleCommand>();
    private int selectedTargetIndex;
    private CombatAbilityDefinition pendingAbility;

    public IReadOnlyList<PartyBattleCommand> Commands => commands.AsReadOnly();
    public bool HasQueuedCommands => commands.Count > 0;
    public bool IsComplete => CurrentActor == null;
    public bool IsChoosingAbility => pendingAbility != null;
    public CombatAbilityDefinition PendingAbility => pendingAbility;
    public BattleTargetType CurrentTargetType => pendingAbility == null
        ? BattleTargetType.SingleEnemy
        : pendingAbility.TargetType;

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

    public CombatAbilityDefinition GetCurrentCoreAbility()
    {
        return CurrentActor is PlayableCharacterData character
            ? CombatAbilityCatalog.GetCoreForJob(character.CurrentJob)
            : null;
    }

    public bool CanChooseCurrentCoreAbility()
    {
        ICombatant actor = CurrentActor;
        CombatAbilityDefinition ability = GetCurrentCoreAbility();
        return actor != null && ability != null &&
            ability.CanPayCost(actor) &&
            battle.GetValidTargets(
                actor.CombatantId,
                ability.TargetType).Count > 0;
    }

    public int GetCurrentTargetCount()
    {
        return GetCurrentTargets().Count;
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
        pendingAbility = null;
        ICombatant actor = CurrentActor;
        ICombatant target = SelectedTarget;
        if (actor == null || target == null)
        {
            return false;
        }

        commands.Add(PartyBattleCommand.Attack(
            actor.CombatantId,
            target.CombatantId));
        ResetForNextActor();
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
        ResetForNextActor();
        return true;
    }

    public bool TryBeginCoreAbility()
    {
        if (pendingAbility != null || !CanChooseCurrentCoreAbility())
        {
            return false;
        }

        pendingAbility = GetCurrentCoreAbility();
        selectedTargetIndex = 0;
        return true;
    }

    public bool TryQueuePendingAbility()
    {
        ICombatant actor = CurrentActor;
        ICombatant target = SelectedTarget;
        if (actor == null || target == null || pendingAbility == null ||
            !pendingAbility.CanPayCost(actor))
        {
            return false;
        }

        commands.Add(PartyBattleCommand.Ability(
            actor.CombatantId,
            pendingAbility.StableId,
            target.CombatantId));
        ResetForNextActor();
        return true;
    }

    private IReadOnlyList<ICombatant> GetCurrentTargets()
    {
        ICombatant actor = CurrentActor;
        return actor == null
            ? Array.Empty<ICombatant>()
            : battle.GetValidTargets(
                actor.CombatantId,
                CurrentTargetType);
    }

    private void ResetForNextActor()
    {
        pendingAbility = null;
        selectedTargetIndex = 0;
    }

    private static int WrapIndex(int index, int count)
    {
        int wrapped = index % count;
        return wrapped < 0 ? wrapped + count : wrapped;
    }
}
