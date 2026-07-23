using System;
using System.Collections.Generic;

public sealed class PartyBattleActionResolver
{
    private readonly ICombatRandom combatRandom;
    private readonly IEnemyBattleCommandSource enemyCommandSource;
    private readonly IBattleDamageRules damageRules;
    private readonly IBattleCommandObserver commandObserver;
    private readonly BattleItemService battleItems;
    private readonly HashSet<string> guardingIds =
        new HashSet<string>(StringComparer.Ordinal);
    private readonly PhysicalAttackAction physicalAttack =
        new PhysicalAttackAction();
    private readonly DefendAction defend = new DefendAction();
    private readonly EscapeAction escape = new EscapeAction();
    private string tauntingActorId;

    public PartyBattleActionResolver(
        ICombatRandom combatRandom = null,
        IEnemyBattleCommandSource enemyCommandSource = null,
        IBattleDamageRules damageRules = null,
        IBattleCommandObserver commandObserver = null,
        Inventory inventory = null)
    {
        this.combatRandom = combatRandom ?? new SystemCombatRandom();
        this.enemyCommandSource =
            enemyCommandSource ?? new FirstLivingTargetCommandSource();
        this.damageRules = damageRules;
        this.commandObserver = commandObserver;
        battleItems = new BattleItemService(inventory);
    }

    public bool IsGuarding(string combatantId)
    {
        return !string.IsNullOrWhiteSpace(combatantId) &&
            guardingIds.Contains(combatantId.Trim());
    }

    public bool IsTaunting(string combatantId)
    {
        return !string.IsNullOrWhiteSpace(combatantId) &&
            tauntingActorId == combatantId.Trim();
    }

    public CombatActionResult ResolveEnemyAction(
        PartyBattleState battle,
        ICombatant enemy)
    {
        ValidateLivingActor(battle, enemy);
        if (!Contains(battle.Enemies, enemy.CombatantId))
        {
            throw new ArgumentException(
                "The acting combatant is not an enemy.",
                nameof(enemy));
        }

        PartyBattleCommand command = CreateEnemyCommand(enemy, battle);
        if (command == null || command.ActorId != enemy.CombatantId)
        {
            throw new InvalidOperationException(
                $"Enemy command source returned an invalid command for {enemy.CombatantId}.");
        }

        return ResolveCommand(battle, command);
    }

    public CombatActionResult ResolveCommand(
        PartyBattleState battle,
        PartyBattleCommand command)
    {
        if (battle == null)
        {
            throw new ArgumentNullException(nameof(battle));
        }

        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        ICombatant actor = battle.GetCombatant(command.ActorId);
        ValidateLivingActor(battle, actor);
        if (command.Type == PartyBattleCommandType.Item)
        {
            ValidateItemCommand(battle, actor, command);
        }

        guardingIds.Remove(actor.CombatantId);
        if (tauntingActorId == actor.CombatantId)
        {
            tauntingActorId = null;
        }

        CombatActionResult result;
        if (command.Type == PartyBattleCommandType.Defend)
        {
            guardingIds.Add(actor.CombatantId);
            result = defend.Execute(
                new CombatActionContext(actor, actor, combatRandom));
        }
        else if (command.Type == PartyBattleCommandType.Ability)
        {
            result = ResolveAbility(battle, actor, command);
        }
        else if (command.Type == PartyBattleCommandType.Item)
        {
            ICombatant target = battle.GetCombatant(command.TargetId);
            result = battleItems.Use(actor, target, command.ItemId);
        }
        else
        {
            ICombatant target = ResolveTarget(
                battle,
                actor,
                BattleTargetType.SingleEnemy,
                command.TargetId);
            if (target == null)
            {
                throw new ArgumentException(
                    $"No valid target exists for {actor.CombatantId}.",
                    nameof(command));
            }

            result = physicalAttack.Execute(CreateContext(actor, target));
        }

        commandObserver?.OnCommandResolved(command, result);
        return result;
    }

    private void ValidateItemCommand(
        PartyBattleState battle,
        ICombatant actor,
        PartyBattleCommand command)
    {
        if (!Contains(battle.PartyMembers, actor.CombatantId))
        {
            throw new ArgumentException(
                "Only party members can use inventory items.",
                nameof(command));
        }

        ICombatant target = battle.GetCombatant(command.TargetId);
        if (!battleItems.CanUse(command.ItemId, target) ||
            !Contains(battle.PartyMembers, command.TargetId))
        {
            throw new ArgumentException(
                $"{command.ItemId} cannot be used on {command.TargetId}.",
                nameof(command));
        }
    }

    public CombatActionResult ResolveEscape(
        PartyBattleState battle,
        ICombatant actor)
    {
        ValidateLivingActor(battle, actor);
        ICombatant target = FirstLiving(battle.Enemies);
        if (target == null)
        {
            throw new InvalidOperationException(
                "Escape requires a living enemy.");
        }

        guardingIds.Remove(actor.CombatantId);
        return escape.Execute(
            new CombatActionContext(actor, target, combatRandom));
    }

    private CombatActionResult ResolveAbility(
        PartyBattleState battle,
        ICombatant actor,
        PartyBattleCommand command)
    {
        CombatAbilityDefinition ability =
            CombatAbilityCatalog.Get(command.AbilityId);
        if (!(actor is PlayableCharacterData character) ||
            ability == null || character.CurrentJob != ability.RequiredJob ||
            !ability.CanPayCost(actor))
        {
            throw new ArgumentException(
                $"{actor.CombatantId} cannot use ability {command.AbilityId}.",
                nameof(command));
        }

        ICombatant target = ResolveTarget(
            battle,
            actor,
            ability.TargetType,
            command.TargetId);
        if (target == null)
        {
            throw new ArgumentException(
                $"No valid target exists for {command.AbilityId}.",
                nameof(command));
        }

        CombatActionResult result = new CombatAbilityAction(ability).Execute(
            CreateContext(actor, target));
        if (result.Succeeded && ability.Effect == CombatAbilityEffect.Taunt)
        {
            tauntingActorId = actor.CombatantId;
        }

        return result;
    }

    private PartyBattleCommand CreateEnemyCommand(
        ICombatant enemy,
        PartyBattleState battle)
    {
        ICombatant taunter = battle.GetCombatant(tauntingActorId);
        if (taunter != null && taunter.Stats.CurrentHp > 0 &&
            Contains(battle.PartyMembers, taunter.CombatantId))
        {
            return PartyBattleCommand.Attack(
                enemy.CombatantId,
                taunter.CombatantId);
        }

        tauntingActorId = null;
        return enemyCommandSource.CreateCommand(enemy, battle);
    }

    private CombatActionContext CreateContext(
        ICombatant actor,
        ICombatant target)
    {
        return new CombatActionContext(
            actor,
            target,
            combatRandom,
            IsGuarding(target.CombatantId),
            damageRules == null ? 0 : damageRules.GetMinimumHp(target));
    }

    private static ICombatant ResolveTarget(
        PartyBattleState battle,
        ICombatant actor,
        BattleTargetType targetType,
        string requestedTargetId)
    {
        if (battle.TrySelectTarget(
            actor.CombatantId,
            targetType,
            requestedTargetId,
            out ICombatant selectedTarget))
        {
            return selectedTarget;
        }

        ICombatant requestedTarget = battle.GetCombatant(requestedTargetId);
        if (requestedTarget != null && requestedTarget.Stats.CurrentHp > 0)
        {
            throw new ArgumentException(
                $"{requestedTargetId} is not a valid target for {actor.CombatantId}.",
                nameof(requestedTargetId));
        }

        IReadOnlyList<ICombatant> targets = battle.GetValidTargets(
            actor.CombatantId,
            targetType);
        return targets.Count > 0 ? targets[0] : null;
    }

    private static void ValidateLivingActor(
        PartyBattleState battle,
        ICombatant actor)
    {
        if (battle == null)
        {
            throw new ArgumentNullException(nameof(battle));
        }

        if (actor == null ||
            battle.GetCombatant(actor.CombatantId) != actor ||
            actor.Stats.CurrentHp <= 0)
        {
            throw new ArgumentException(
                "The action actor must be a living battle combatant.",
                nameof(actor));
        }
    }

    private static bool Contains(
        IReadOnlyList<ICombatant> combatants,
        string combatantId)
    {
        foreach (ICombatant combatant in combatants)
        {
            if (combatant.CombatantId == combatantId)
            {
                return true;
            }
        }

        return false;
    }

    private static ICombatant FirstLiving(
        IReadOnlyList<ICombatant> combatants)
    {
        foreach (ICombatant combatant in combatants)
        {
            if (combatant.Stats.CurrentHp > 0)
            {
                return combatant;
            }
        }

        return null;
    }
}
