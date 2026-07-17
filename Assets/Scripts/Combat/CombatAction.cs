using System;

public enum CombatActionType
{
    PhysicalAttack,
    Magic,
    Healing,
    Item,
    Defend,
    Ability,
    Escape
}

public interface ICombatant
{
    string CombatantId { get; }
    string DisplayName { get; }
    CombatantStats Stats { get; }
}

public interface ICombatAction
{
    CombatActionType Type { get; }
    CombatActionResult Execute(CombatActionContext context);
}

public class CombatActionContext
{
    public ICombatant Actor { get; }
    public ICombatant Target { get; }
    public ICombatRandom Random { get; }
    public bool TargetIsGuarding { get; }
    public int MinimumTargetHp { get; }

    public CombatActionContext(
        ICombatant actor,
        ICombatant target,
        ICombatRandom random,
        bool targetIsGuarding = false,
        int minimumTargetHp = 0)
    {
        Actor = actor ?? throw new ArgumentNullException(nameof(actor));
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Random = random ?? throw new ArgumentNullException(nameof(random));
        TargetIsGuarding = targetIsGuarding;
        MinimumTargetHp = Math.Min(
            Math.Max(minimumTargetHp, 0),
            Target.Stats.MaxHp);
    }

    public int ApplyDamage(int amount)
    {
        int maximumDamage = Math.Max(
            0,
            Target.Stats.CurrentHp - MinimumTargetHp);
        return Target.Stats.ApplyDamage(Math.Min(
            Math.Max(amount, 0),
            maximumDamage));
    }
}

public class CombatActionResult
{
    public CombatActionType Type { get; }
    public string ActorId { get; }
    public string TargetId { get; }
    public bool Succeeded { get; }
    public int Damage { get; }
    public int Healing { get; }
    public bool WasCritical { get; }
    public bool WasGuarded { get; }
    public int ChancePercent { get; }
    public string Message { get; }

    public CombatActionResult(
        CombatActionType type,
        string actorId,
        string targetId,
        bool succeeded,
        string message,
        int damage = 0,
        int healing = 0,
        bool wasCritical = false,
        bool wasGuarded = false,
        int chancePercent = 100)
    {
        Type = type;
        ActorId = actorId;
        TargetId = targetId;
        Succeeded = succeeded;
        Damage = Math.Max(0, damage);
        Healing = Math.Max(0, healing);
        WasCritical = wasCritical;
        WasGuarded = wasGuarded;
        ChancePercent = Math.Min(Math.Max(chancePercent, 0), 100);
        Message = message ?? string.Empty;
    }
}

public class PhysicalAttackAction : ICombatAction
{
    public CombatActionType Type => CombatActionType.PhysicalAttack;

    public CombatActionResult Execute(CombatActionContext context)
    {
        ICombatant actor = context.Actor;
        ICombatant target = context.Target;
        int hitChance = Clamp(
            actor.Stats.Accuracy - target.Stats.Evasion,
            5,
            100);

        if (!context.Random.RollPercent(hitChance))
        {
            return new CombatActionResult(
                Type,
                actor.CombatantId,
                target.CombatantId,
                false,
                $"{actor.DisplayName} attacks {target.DisplayName} but misses.",
                chancePercent: hitChance);
        }

        bool wasCritical =
            context.Random.RollPercent(actor.Stats.CriticalChance);
        int damage = Math.Max(
            1,
            actor.Stats.Attack - target.Stats.Defense);

        if (wasCritical)
        {
            damage *= 2;
        }

        if (context.TargetIsGuarding)
        {
            damage = Math.Max(1, (damage + 1) / 2);
        }

        int appliedDamage = context.ApplyDamage(damage);
        string message = wasCritical
            ? $"Critical hit! {actor.DisplayName} attacks " +
                $"{target.DisplayName} for {appliedDamage} damage."
            : $"{actor.DisplayName} attacks {target.DisplayName} for " +
                $"{appliedDamage} damage.";

        if (context.TargetIsGuarding)
        {
            message += $" {target.DisplayName} blocks part of the blow.";
        }

        return new CombatActionResult(
            Type,
            actor.CombatantId,
            target.CombatantId,
            true,
            message,
            appliedDamage,
            wasCritical: wasCritical,
            wasGuarded: context.TargetIsGuarding,
            chancePercent: hitChance);
    }

    private static int Clamp(int value, int minimum, int maximum)
    {
        return Math.Min(Math.Max(value, minimum), maximum);
    }
}

public class DefendAction : ICombatAction
{
    public CombatActionType Type => CombatActionType.Defend;

    public CombatActionResult Execute(CombatActionContext context)
    {
        return new CombatActionResult(
            Type,
            context.Actor.CombatantId,
            context.Actor.CombatantId,
            true,
            $"{context.Actor.DisplayName} takes a defensive stance.");
    }
}

public class EscapeAction : ICombatAction
{
    public CombatActionType Type => CombatActionType.Escape;

    public CombatActionResult Execute(CombatActionContext context)
    {
        int escapeChance = Clamp(
            50 +
                ((context.Actor.Stats.Speed - context.Target.Stats.Speed) * 5),
            20,
            95);
        bool escaped = context.Random.RollPercent(escapeChance);

        return new CombatActionResult(
            Type,
            context.Actor.CombatantId,
            context.Target.CombatantId,
            escaped,
            escaped
                ? $"{context.Actor.DisplayName} escaped from battle."
                : $"{context.Actor.DisplayName} could not escape.",
            chancePercent: escapeChance);
    }

    private static int Clamp(int value, int minimum, int maximum)
    {
        return Math.Min(Math.Max(value, minimum), maximum);
    }
}
