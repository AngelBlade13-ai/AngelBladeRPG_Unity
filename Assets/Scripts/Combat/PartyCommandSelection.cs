using System;
using System.Collections.Generic;

public sealed class PartyCommandSelection
{
    private readonly PartyBattleState battle;
    private readonly List<PartyBattleCommand> commands =
        new List<PartyBattleCommand>();
    private int selectedTargetIndex;
    private CombatAbilityDefinition pendingAbility;
    private ItemDefinition pendingItem;
    private readonly ICombatant fixedActor;
    private readonly Inventory inventory;

    public IReadOnlyList<PartyBattleCommand> Commands => commands.AsReadOnly();
    public bool HasQueuedCommands => commands.Count > 0;
    public bool IsComplete => CurrentActor == null;
    public bool IsChoosingAbility => pendingAbility != null;
    public bool IsChoosingItem => pendingItem != null;
    public CombatAbilityDefinition PendingAbility => pendingAbility;
    public ItemDefinition PendingItem => pendingItem;
    public BattleTargetType CurrentTargetType =>
        pendingItem != null
            ? BattleTargetType.SingleAlly
            : pendingAbility == null
                ? BattleTargetType.SingleEnemy
                : pendingAbility.TargetType;

    public ICombatant CurrentActor
    {
        get
        {
            if (fixedActor != null)
            {
                return commands.Count == 0 && fixedActor.Stats.CurrentHp > 0
                    ? fixedActor
                    : null;
            }

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

    public PartyCommandSelection(
        PartyBattleState battle,
        Inventory inventory = null)
    {
        this.battle = battle ?? throw new ArgumentNullException(nameof(battle));
        this.inventory = inventory;
    }

    public PartyCommandSelection(
        PartyBattleState battle,
        ICombatant readyPartyMember)
        : this(battle, readyPartyMember, null)
    {
    }

    public PartyCommandSelection(
        PartyBattleState battle,
        ICombatant readyPartyMember,
        Inventory inventory)
        : this(battle, inventory)
    {
        if (readyPartyMember == null ||
            readyPartyMember.Stats.CurrentHp <= 0 ||
            !Contains(battle.PartyMembers, readyPartyMember))
        {
            throw new ArgumentException(
                "The ready actor must be a living party member.",
                nameof(readyPartyMember));
        }

        fixedActor = readyPartyMember;
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

    public bool CanChooseBattleItem()
    {
        ICombatant actor = CurrentActor;
        if (actor == null)
        {
            return false;
        }

        foreach (ItemDefinition item in
            BattleItemService.GetUsableItems(inventory))
        {
            if (GetItemTargets(item).Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    public int GetPendingItemQuantity()
    {
        return pendingItem == null || inventory == null
            ? 0
            : inventory.GetQuantity(pendingItem.Id);
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
        ClearPendingChoice();
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
        if (pendingAbility != null)
        {
            return false;
        }

        pendingItem = null;
        if (!CanChooseCurrentCoreAbility())
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

    public bool TryBeginBattleItem()
    {
        if (pendingItem != null)
        {
            return false;
        }

        pendingAbility = null;
        foreach (ItemDefinition item in
            BattleItemService.GetUsableItems(inventory))
        {
            if (GetItemTargets(item).Count < 1)
            {
                continue;
            }

            pendingItem = item;
            selectedTargetIndex = 0;
            return true;
        }

        return false;
    }

    public bool TryQueuePendingItem()
    {
        ICombatant actor = CurrentActor;
        ICombatant target = SelectedTarget;
        if (actor == null || target == null || pendingItem == null ||
            inventory == null ||
            inventory.GetQuantity(pendingItem.Id) < 1 ||
            !BattleItemService.CanTarget(pendingItem, target))
        {
            return false;
        }

        commands.Add(PartyBattleCommand.Item(
            actor.CombatantId,
            pendingItem.Id,
            target.CombatantId));
        ResetForNextActor();
        return true;
    }

    private IReadOnlyList<ICombatant> GetCurrentTargets()
    {
        ICombatant actor = CurrentActor;
        if (actor == null)
        {
            return Array.Empty<ICombatant>();
        }

        return pendingItem == null
            ? battle.GetValidTargets(
                actor.CombatantId,
                CurrentTargetType)
            : GetItemTargets(pendingItem);
    }

    private IReadOnlyList<ICombatant> GetItemTargets(ItemDefinition item)
    {
        ICombatant actor = CurrentActor;
        if (actor == null || item == null)
        {
            return Array.Empty<ICombatant>();
        }

        List<ICombatant> targets = new List<ICombatant>();
        foreach (ICombatant target in battle.GetValidTargets(
            actor.CombatantId,
            BattleTargetType.SingleAlly))
        {
            if (BattleItemService.CanTarget(item, target))
            {
                targets.Add(target);
            }
        }

        return targets.AsReadOnly();
    }

    private void ResetForNextActor()
    {
        ClearPendingChoice();
        selectedTargetIndex = 0;
    }

    private void ClearPendingChoice()
    {
        pendingAbility = null;
        pendingItem = null;
    }

    private static int WrapIndex(int index, int count)
    {
        int wrapped = index % count;
        return wrapped < 0 ? wrapped + count : wrapped;
    }

    private static bool Contains(
        IReadOnlyList<ICombatant> combatants,
        ICombatant candidate)
    {
        foreach (ICombatant combatant in combatants)
        {
            if (combatant == candidate)
            {
                return true;
            }
        }

        return false;
    }
}
