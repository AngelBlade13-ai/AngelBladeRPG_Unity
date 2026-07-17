using System;
using System.Collections.Generic;

public enum PartyBattleCommandType
{
    PhysicalAttack,
    Ability,
    Defend
}

public sealed class PartyBattleCommand
{
    public string ActorId { get; }
    public PartyBattleCommandType Type { get; }
    public string TargetId { get; }
    public string AbilityId { get; }

    private PartyBattleCommand(
        string actorId,
        PartyBattleCommandType type,
        string targetId,
        string abilityId = "")
    {
        if (string.IsNullOrWhiteSpace(actorId))
        {
            throw new ArgumentException(
                "A command actor ID is required.",
                nameof(actorId));
        }

        ActorId = actorId.Trim();
        Type = type;
        TargetId = string.IsNullOrWhiteSpace(targetId)
            ? string.Empty
            : targetId.Trim();
        AbilityId = string.IsNullOrWhiteSpace(abilityId)
            ? string.Empty
            : abilityId.Trim();
    }

    public static PartyBattleCommand Attack(string actorId, string targetId)
    {
        if (string.IsNullOrWhiteSpace(targetId))
        {
            throw new ArgumentException(
                "An attack target ID is required.",
                nameof(targetId));
        }

        return new PartyBattleCommand(
            actorId,
            PartyBattleCommandType.PhysicalAttack,
            targetId);
    }

    public static PartyBattleCommand Defend(string actorId)
    {
        return new PartyBattleCommand(
            actorId,
            PartyBattleCommandType.Defend,
            string.Empty);
    }

    public static PartyBattleCommand Ability(
        string actorId,
        string abilityId,
        string targetId)
    {
        if (string.IsNullOrWhiteSpace(abilityId))
        {
            throw new ArgumentException(
                "An ability ID is required.",
                nameof(abilityId));
        }

        if (string.IsNullOrWhiteSpace(targetId))
        {
            throw new ArgumentException(
                "An ability target ID is required.",
                nameof(targetId));
        }

        return new PartyBattleCommand(
            actorId,
            PartyBattleCommandType.Ability,
            targetId,
            abilityId);
    }
}

public interface IEnemyBattleCommandSource
{
    PartyBattleCommand CreateCommand(
        ICombatant enemy,
        PartyBattleState battle);
}

public sealed class FirstLivingTargetCommandSource : IEnemyBattleCommandSource
{
    public PartyBattleCommand CreateCommand(
        ICombatant enemy,
        PartyBattleState battle)
    {
        if (enemy == null)
        {
            throw new ArgumentNullException(nameof(enemy));
        }

        if (battle == null)
        {
            throw new ArgumentNullException(nameof(battle));
        }

        IReadOnlyList<ICombatant> targets = battle.GetValidTargets(
            enemy.CombatantId,
            BattleTargetType.SingleEnemy);
        return targets.Count > 0
            ? PartyBattleCommand.Attack(
                enemy.CombatantId,
                targets[0].CombatantId)
            : null;
    }
}

public sealed class PartyBattleRoundResult
{
    private readonly List<CombatActionResult> actions;
    private readonly List<string> messages;

    public IReadOnlyList<CombatActionResult> Actions => actions.AsReadOnly();
    public IReadOnlyList<string> Messages => messages.AsReadOnly();
    public bool PartyWasDefeated { get; }
    public bool EnemiesWereDefeated { get; }

    public PartyBattleRoundResult(
        IEnumerable<CombatActionResult> resolvedActions,
        bool partyWasDefeated,
        bool enemiesWereDefeated)
    {
        if (resolvedActions == null)
        {
            throw new ArgumentNullException(nameof(resolvedActions));
        }

        actions = new List<CombatActionResult>(resolvedActions);
        messages = new List<string>();
        foreach (CombatActionResult action in actions)
        {
            if (action == null)
            {
                throw new ArgumentException(
                    "Resolved actions cannot contain null.",
                    nameof(resolvedActions));
            }

            messages.Add(action.Message);
        }

        PartyWasDefeated = partyWasDefeated;
        EnemiesWereDefeated = enemiesWereDefeated;
    }
}

public sealed class PartyBattleRoundResolver
{
    private readonly ITurnOrderRandom tieBreaker;
    private readonly ICombatRandom combatRandom;
    private readonly IEnemyBattleCommandSource enemyCommandSource;
    private readonly PhysicalAttackAction physicalAttack =
        new PhysicalAttackAction();
    private readonly DefendAction defend = new DefendAction();

    public PartyBattleRoundResolver(
        ITurnOrderRandom tieBreaker = null,
        ICombatRandom combatRandom = null,
        IEnemyBattleCommandSource enemyCommandSource = null)
    {
        this.tieBreaker = tieBreaker;
        this.combatRandom = combatRandom ?? new SystemCombatRandom();
        this.enemyCommandSource =
            enemyCommandSource ?? new FirstLivingTargetCommandSource();
    }

    public PartyBattleRoundResult ResolveRound(
        PartyBattleState battle,
        IEnumerable<PartyBattleCommand> partyCommands)
    {
        if (battle == null)
        {
            throw new ArgumentNullException(nameof(battle));
        }

        if (partyCommands == null)
        {
            throw new ArgumentNullException(nameof(partyCommands));
        }

        if (battle.IsPartyDefeated || battle.AreEnemiesDefeated)
        {
            return BuildResult(battle, Array.Empty<CombatActionResult>());
        }

        Dictionary<string, PartyBattleCommand> commands =
            ValidatePartyCommands(battle, partyCommands);
        AddEnemyCommands(battle, commands);

        IReadOnlyList<BattleTurnParticipant> turnOrder =
            BuildTurnOrder(battle);
        List<CombatActionResult> actions =
            new List<CombatActionResult>();
        HashSet<string> guardingIds =
            new HashSet<string>(StringComparer.Ordinal);

        foreach (BattleTurnParticipant participant in turnOrder)
        {
            if (battle.IsPartyDefeated || battle.AreEnemiesDefeated)
            {
                break;
            }

            ICombatant actor = battle.GetCombatant(participant.CombatantId);
            if (actor == null || actor.Stats.CurrentHp <= 0)
            {
                continue;
            }

            PartyBattleCommand command = commands[actor.CombatantId];
            if (command.Type == PartyBattleCommandType.Defend)
            {
                actions.Add(defend.Execute(
                    new CombatActionContext(actor, actor, combatRandom)));
                guardingIds.Add(actor.CombatantId);
                continue;
            }

            if (command.Type == PartyBattleCommandType.Ability)
            {
                ResolveAbility(
                    battle,
                    actor,
                    command,
                    guardingIds,
                    actions);
                continue;
            }

            ICombatant target = ResolveAttackTarget(battle, actor, command);
            if (target == null)
            {
                continue;
            }

            actions.Add(physicalAttack.Execute(
                new CombatActionContext(
                    actor,
                    target,
                    combatRandom,
                    guardingIds.Contains(target.CombatantId))));
        }

        return BuildResult(battle, actions);
    }

    private static Dictionary<string, PartyBattleCommand> ValidatePartyCommands(
        PartyBattleState battle,
        IEnumerable<PartyBattleCommand> partyCommands)
    {
        Dictionary<string, PartyBattleCommand> commands =
            new Dictionary<string, PartyBattleCommand>(StringComparer.Ordinal);

        foreach (PartyBattleCommand command in partyCommands)
        {
            if (command == null)
            {
                throw new ArgumentException(
                    "Party commands cannot contain null.",
                    nameof(partyCommands));
            }

            ICombatant actor = FindById(battle.PartyMembers, command.ActorId);
            if (actor == null || actor.Stats.CurrentHp <= 0)
            {
                throw new ArgumentException(
                    $"Command actor {command.ActorId} is not an active combatant.",
                    nameof(partyCommands));
            }

            if (commands.ContainsKey(command.ActorId))
            {
                throw new ArgumentException(
                    $"Duplicate command for {command.ActorId}.",
                    nameof(partyCommands));
            }

            ValidateCommandTarget(battle, command);
            commands.Add(command.ActorId, command);
        }

        foreach (ICombatant partyMember in battle.PartyMembers)
        {
            if (partyMember.Stats.CurrentHp > 0 &&
                !commands.ContainsKey(partyMember.CombatantId))
            {
                throw new ArgumentException(
                    $"Missing command for {partyMember.CombatantId}.",
                    nameof(partyCommands));
            }
        }

        return commands;
    }

    private void AddEnemyCommands(
        PartyBattleState battle,
        IDictionary<string, PartyBattleCommand> commands)
    {
        foreach (ICombatant enemy in battle.Enemies)
        {
            if (enemy.Stats.CurrentHp <= 0)
            {
                continue;
            }

            PartyBattleCommand command =
                enemyCommandSource.CreateCommand(enemy, battle);
            if (command == null || command.ActorId != enemy.CombatantId)
            {
                throw new InvalidOperationException(
                    $"Enemy command source returned an invalid command for {enemy.CombatantId}.");
            }

            ValidateCommandTarget(battle, command);
            commands.Add(enemy.CombatantId, command);
        }
    }

    private static void ValidateCommandTarget(
        PartyBattleState battle,
        PartyBattleCommand command)
    {
        if (command.Type == PartyBattleCommandType.Defend)
        {
            return;
        }

        if (command.Type == PartyBattleCommandType.Ability)
        {
            ValidateAbilityCommand(battle, command);
            return;
        }

        if (!battle.TrySelectTarget(
            command.ActorId,
            BattleTargetType.SingleEnemy,
            command.TargetId,
            out _))
        {
            throw new ArgumentException(
                $"{command.TargetId} is not a valid target for {command.ActorId}.");
        }
    }

    private static void ValidateAbilityCommand(
        PartyBattleState battle,
        PartyBattleCommand command)
    {
        ICombatant actor = battle.GetCombatant(command.ActorId);
        CombatAbilityDefinition ability =
            CombatAbilityCatalog.Get(command.AbilityId);
        if (!(actor is PlayableCharacterData character) ||
            ability == null || character.CurrentJob != ability.RequiredJob)
        {
            throw new ArgumentException(
                $"{command.ActorId} cannot use ability {command.AbilityId}.");
        }

        if (!ability.CanPayCost(actor))
        {
            throw new ArgumentException(
                $"{command.ActorId} cannot pay for ability {command.AbilityId}.");
        }

        if (!battle.TrySelectTarget(
            command.ActorId,
            ability.TargetType,
            command.TargetId,
            out _))
        {
            throw new ArgumentException(
                $"{command.TargetId} is not a valid target for " +
                $"{command.AbilityId}.");
        }
    }

    private IReadOnlyList<BattleTurnParticipant> BuildTurnOrder(
        PartyBattleState battle)
    {
        List<BattleTurnParticipant> participants =
            new List<BattleTurnParticipant>();
        AddLivingParticipants(participants, battle.PartyMembers);
        AddLivingParticipants(participants, battle.Enemies);
        return SpeedTurnOrder.Build(participants, tieBreaker);
    }

    private static void AddLivingParticipants(
        ICollection<BattleTurnParticipant> participants,
        IEnumerable<ICombatant> combatants)
    {
        foreach (ICombatant combatant in combatants)
        {
            if (combatant.Stats.CurrentHp > 0)
            {
                participants.Add(new BattleTurnParticipant(
                    combatant.CombatantId,
                    combatant.Stats.Speed));
            }
        }
    }

    private static ICombatant ResolveAttackTarget(
        PartyBattleState battle,
        ICombatant actor,
        PartyBattleCommand command)
    {
        if (battle.TrySelectTarget(
            actor.CombatantId,
            BattleTargetType.SingleEnemy,
            command.TargetId,
            out ICombatant selectedTarget))
        {
            return selectedTarget;
        }

        IReadOnlyList<ICombatant> livingTargets = battle.GetValidTargets(
            actor.CombatantId,
            BattleTargetType.SingleEnemy);
        return livingTargets.Count > 0 ? livingTargets[0] : null;
    }

    private void ResolveAbility(
        PartyBattleState battle,
        ICombatant actor,
        PartyBattleCommand command,
        ISet<string> guardingIds,
        ICollection<CombatActionResult> actions)
    {
        CombatAbilityDefinition ability =
            CombatAbilityCatalog.Get(command.AbilityId);
        ICombatant target = ResolveAbilityTarget(
            battle,
            actor,
            command,
            ability);
        if (target == null)
        {
            return;
        }

        actions.Add(new CombatAbilityAction(ability).Execute(
            new CombatActionContext(
                actor,
                target,
                combatRandom,
                guardingIds.Contains(target.CombatantId))));
    }

    private static ICombatant ResolveAbilityTarget(
        PartyBattleState battle,
        ICombatant actor,
        PartyBattleCommand command,
        CombatAbilityDefinition ability)
    {
        if (battle.TrySelectTarget(
            actor.CombatantId,
            ability.TargetType,
            command.TargetId,
            out ICombatant selectedTarget))
        {
            return selectedTarget;
        }

        IReadOnlyList<ICombatant> validTargets = battle.GetValidTargets(
            actor.CombatantId,
            ability.TargetType);
        return validTargets.Count > 0 ? validTargets[0] : null;
    }

    private static ICombatant FindById(
        IEnumerable<ICombatant> combatants,
        string combatantId)
    {
        foreach (ICombatant combatant in combatants)
        {
            if (string.Equals(
                combatant.CombatantId,
                combatantId,
                StringComparison.Ordinal))
            {
                return combatant;
            }
        }

        return null;
    }

    private static PartyBattleRoundResult BuildResult(
        PartyBattleState battle,
        IEnumerable<CombatActionResult> actions)
    {
        return new PartyBattleRoundResult(
            actions,
            battle.IsPartyDefeated,
            battle.AreEnemiesDefeated);
    }
}
