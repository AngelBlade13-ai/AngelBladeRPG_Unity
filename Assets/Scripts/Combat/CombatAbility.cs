using System;
using System.Collections.Generic;

public enum CombatAbilityEffect
{
    PhysicalDamage,
    MagicDamage,
    Healing,
    Taunt
}

public enum CombatAbilityCostType
{
    Mp,
    Hp
}

public sealed class CombatAbilityDefinition
{
    public string StableId { get; }
    public string DisplayName { get; }
    public JobId RequiredJob { get; }
    public BattleTargetType TargetType { get; }
    public CombatAbilityEffect Effect { get; }
    public CombatAbilityCostType CostType { get; }
    public int ResourceCost { get; }
    public int Potency { get; }

    public CombatAbilityDefinition(
        string stableId,
        string displayName,
        JobId requiredJob,
        BattleTargetType targetType,
        CombatAbilityEffect effect,
        CombatAbilityCostType costType,
        int resourceCost,
        int potency)
    {
        if (string.IsNullOrWhiteSpace(stableId))
        {
            throw new ArgumentException(
                "A stable ability ID is required.",
                nameof(stableId));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException(
                "An ability name is required.",
                nameof(displayName));
        }

        if (!JobCatalog.Contains(requiredJob))
        {
            throw new ArgumentOutOfRangeException(nameof(requiredJob));
        }

        if (targetType != BattleTargetType.SingleAlly &&
            targetType != BattleTargetType.SingleEnemy &&
            targetType != BattleTargetType.Self)
        {
            throw new ArgumentOutOfRangeException(nameof(targetType));
        }

        if (!Enum.IsDefined(typeof(CombatAbilityEffect), effect))
        {
            throw new ArgumentOutOfRangeException(nameof(effect));
        }

        if (!Enum.IsDefined(typeof(CombatAbilityCostType), costType))
        {
            throw new ArgumentOutOfRangeException(nameof(costType));
        }

        if (resourceCost < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(resourceCost));
        }

        if (potency < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(potency));
        }

        StableId = stableId.Trim();
        DisplayName = displayName.Trim();
        RequiredJob = requiredJob;
        TargetType = targetType;
        Effect = effect;
        CostType = costType;
        ResourceCost = resourceCost;
        Potency = potency;
    }

    public bool CanPayCost(ICombatant actor)
    {
        if (actor == null || actor.Stats.CurrentHp <= 0)
        {
            return false;
        }

        return CostType == CombatAbilityCostType.Mp
            ? actor.Stats.CurrentMp >= ResourceCost
            : actor.Stats.CurrentHp > ResourceCost;
    }
}

public static class CombatAbilityCatalog
{
    public const string PowerStrikeId = "job_mercenary_power_strike";
    public const string TauntId = "job_reaver_taunt";
    public const string EmberId = "job_mage_ember";
    public const string BloodBoltId = "job_blood_mage_blood_bolt";
    public const string MendId = "job_white_mage_mend";
    public const string LayOnHandsId = "job_paladin_lay_on_hands";

    private static readonly Dictionary<string, CombatAbilityDefinition>
        abilities = BuildCatalog();

    public static IReadOnlyCollection<CombatAbilityDefinition> All =>
        abilities.Values;

    public static CombatAbilityDefinition Get(string stableId)
    {
        if (string.IsNullOrWhiteSpace(stableId))
        {
            return null;
        }

        return abilities.TryGetValue(
            stableId.Trim(),
            out CombatAbilityDefinition definition)
            ? definition
            : null;
    }

    public static CombatAbilityDefinition GetCoreForJob(JobId jobId)
    {
        foreach (CombatAbilityDefinition ability in abilities.Values)
        {
            if (ability.RequiredJob == jobId)
            {
                return ability;
            }
        }

        return null;
    }

    private static Dictionary<string, CombatAbilityDefinition> BuildCatalog()
    {
        CombatAbilityDefinition[] definitions =
        {
            Ability(
                TauntId,
                "Taunt",
                JobId.Reaver,
                BattleTargetType.Self,
                CombatAbilityEffect.Taunt,
                CombatAbilityCostType.Mp,
                0,
                1),
            Ability(
                PowerStrikeId,
                "Power Strike",
                JobId.Mercenary,
                BattleTargetType.SingleEnemy,
                CombatAbilityEffect.PhysicalDamage,
                CombatAbilityCostType.Mp,
                4,
                5),
            Ability(
                EmberId,
                "Ember",
                JobId.Mage,
                BattleTargetType.SingleEnemy,
                CombatAbilityEffect.MagicDamage,
                CombatAbilityCostType.Mp,
                4,
                6),
            Ability(
                BloodBoltId,
                "Blood Bolt",
                JobId.BloodMage,
                BattleTargetType.SingleEnemy,
                CombatAbilityEffect.MagicDamage,
                CombatAbilityCostType.Hp,
                8,
                10),
            Ability(
                MendId,
                "Mend",
                JobId.WhiteMage,
                BattleTargetType.SingleAlly,
                CombatAbilityEffect.Healing,
                CombatAbilityCostType.Mp,
                4,
                12),
            Ability(
                LayOnHandsId,
                "Lay On Hands",
                JobId.Paladin,
                BattleTargetType.SingleAlly,
                CombatAbilityEffect.Healing,
                CombatAbilityCostType.Mp,
                5,
                8)
        };

        Dictionary<string, CombatAbilityDefinition> catalog =
            new Dictionary<string, CombatAbilityDefinition>(
                StringComparer.Ordinal);
        foreach (CombatAbilityDefinition definition in definitions)
        {
            catalog.Add(definition.StableId, definition);
        }

        return catalog;
    }

    private static CombatAbilityDefinition Ability(
        string stableId,
        string displayName,
        JobId requiredJob,
        BattleTargetType targetType,
        CombatAbilityEffect effect,
        CombatAbilityCostType costType,
        int resourceCost,
        int potency)
    {
        return new CombatAbilityDefinition(
            stableId,
            displayName,
            requiredJob,
            targetType,
            effect,
            costType,
            resourceCost,
            potency);
    }
}

public sealed class CombatAbilityAction : ICombatAction
{
    private readonly CombatAbilityDefinition ability;

    public CombatActionType Type
    {
        get
        {
            switch (ability.Effect)
            {
                case CombatAbilityEffect.MagicDamage:
                    return CombatActionType.Magic;
                case CombatAbilityEffect.Healing:
                    return CombatActionType.Healing;
                default:
                    return CombatActionType.Ability;
            }
        }
    }

    public CombatAbilityAction(CombatAbilityDefinition definition)
    {
        ability = definition ??
            throw new ArgumentNullException(nameof(definition));
    }

    public CombatActionResult Execute(CombatActionContext context)
    {
        if (!ability.CanPayCost(context.Actor))
        {
            return new CombatActionResult(
                Type,
                context.Actor.CombatantId,
                context.Target.CombatantId,
                false,
                $"{context.Actor.DisplayName} cannot use " +
                    $"{ability.DisplayName}.");
        }

        PayCost(context.Actor);

        switch (ability.Effect)
        {
            case CombatAbilityEffect.PhysicalDamage:
                return ResolvePhysicalDamage(context);
            case CombatAbilityEffect.MagicDamage:
                return ResolveMagicDamage(context);
            case CombatAbilityEffect.Healing:
                return ResolveHealing(context);
            case CombatAbilityEffect.Taunt:
                return ResolveTaunt(context);
            default:
                throw new InvalidOperationException(
                    $"Unsupported ability effect: {ability.Effect}.");
        }
    }

    private void PayCost(ICombatant actor)
    {
        if (ability.CostType == CombatAbilityCostType.Mp)
        {
            actor.Stats.TrySpendMp(ability.ResourceCost);
            return;
        }

        actor.Stats.ApplyDamage(ability.ResourceCost);
    }

    private CombatActionResult ResolvePhysicalDamage(
        CombatActionContext context)
    {
        int damage = Math.Max(
            1,
            context.Actor.Stats.Attack + ability.Potency -
                context.Target.Stats.Defense);
        if (context.TargetIsGuarding)
        {
            damage = Math.Max(1, (damage + 1) / 2);
        }

        int appliedDamage = context.ApplyDamage(damage);
        string message =
            $"{context.Actor.DisplayName} uses {ability.DisplayName} on " +
            $"{context.Target.DisplayName} for {appliedDamage} damage.";
        if (context.TargetIsGuarding)
        {
            message +=
                $" {context.Target.DisplayName} blocks part of the blow.";
        }

        return new CombatActionResult(
            Type,
            context.Actor.CombatantId,
            context.Target.CombatantId,
            true,
            message,
            appliedDamage,
            wasGuarded: context.TargetIsGuarding);
    }

    private CombatActionResult ResolveMagicDamage(
        CombatActionContext context)
    {
        int damage = Math.Max(
            1,
            context.Actor.Stats.MagicPower + ability.Potency -
                context.Target.Stats.MagicDefense);
        int appliedDamage = context.ApplyDamage(damage);

        return new CombatActionResult(
            Type,
            context.Actor.CombatantId,
            context.Target.CombatantId,
            true,
            $"{context.Actor.DisplayName} casts {ability.DisplayName} on " +
                $"{context.Target.DisplayName} for {appliedDamage} damage.",
            appliedDamage);
    }

    private CombatActionResult ResolveHealing(CombatActionContext context)
    {
        int healing = ability.Potency + context.Actor.Stats.MagicPower;
        int restoredHp = context.Target.Stats.RestoreHp(healing);

        return new CombatActionResult(
            Type,
            context.Actor.CombatantId,
            context.Target.CombatantId,
            true,
            $"{context.Actor.DisplayName} uses {ability.DisplayName} on " +
                $"{context.Target.DisplayName}, restoring {restoredHp} HP.",
            healing: restoredHp);
    }

    private CombatActionResult ResolveTaunt(CombatActionContext context)
    {
        return new CombatActionResult(
            Type,
            context.Actor.CombatantId,
            context.Actor.CombatantId,
            true,
            $"{context.Actor.DisplayName} taunts the enemy and draws its attention.");
    }
}
