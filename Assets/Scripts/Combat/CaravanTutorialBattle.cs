using System;
using System.Collections.Generic;

public enum CaravanTutorialStage
{
    BasicGoblins,
    HobgoblinPressure,
    FullParty,
    Completed
}

public sealed class CaravanTutorialBattle :
    IEnemyBattleCommandSource,
    IBattleDamageRules,
    IBattleCommandObserver
{
    public const string HobgoblinCombatantId =
        "encounter_tutorial_caravan_hobgoblin";

    private readonly GameSession session;

    public CaravanTutorialStage Stage { get; private set; }
    public bool TauntWasDemonstrated { get; private set; }

    public CaravanTutorialBattle(GameSession gameSession)
    {
        session = gameSession ??
            throw new ArgumentNullException(nameof(gameSession));
        Stage = CaravanTutorialStage.BasicGoblins;
    }

    public IReadOnlyList<string> AdvanceAfterRound(
        PartyBattleRoundResult round)
    {
        if (round == null)
        {
            throw new ArgumentNullException(nameof(round));
        }

        switch (Stage)
        {
            case CaravanTutorialStage.BasicGoblins:
                return round.EnemiesWereDefeated
                    ? BeginHobgoblinPressure()
                    : Array.Empty<string>();
            case CaravanTutorialStage.HobgoblinPressure:
                return BeginFullParty();
            case CaravanTutorialStage.FullParty:
                if (round.EnemiesWereDefeated)
                {
                    Stage = CaravanTutorialStage.Completed;
                    return new[]
                    {
                        "The Hobgoblin falls. The caravan is safe."
                    };
                }

                return Array.Empty<string>();
            default:
                return Array.Empty<string>();
        }
    }

    public IReadOnlyList<string> AdvanceAfterAction(
        CombatActionResult action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        switch (Stage)
        {
            case CaravanTutorialStage.BasicGoblins:
                return session.PartyBattle.AreEnemiesDefeated
                    ? BeginHobgoblinPressure()
                    : Array.Empty<string>();
            case CaravanTutorialStage.HobgoblinPressure:
                return action.ActorId == HobgoblinCombatantId
                    ? BeginFullParty()
                    : Array.Empty<string>();
            case CaravanTutorialStage.FullParty:
                if (session.PartyBattle.AreEnemiesDefeated)
                {
                    Stage = CaravanTutorialStage.Completed;
                    return new[]
                    {
                        "The Hobgoblin falls. The caravan is safe."
                    };
                }

                return Array.Empty<string>();
            default:
                return Array.Empty<string>();
        }
    }

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

        string preferredTargetId = Stage == CaravanTutorialStage.HobgoblinPressure
            ? "pc_01"
            : PlayableCharacterData.ProtagonistId;
        ICombatant preferredTarget = battle.GetCombatant(preferredTargetId);
        if (preferredTarget != null && preferredTarget.Stats.CurrentHp > 0)
        {
            return PartyBattleCommand.Attack(
                enemy.CombatantId,
                preferredTarget.CombatantId);
        }

        return new FirstLivingTargetCommandSource().CreateCommand(enemy, battle);
    }

    public int GetMinimumHp(ICombatant target)
    {
        if (target == null)
        {
            return 0;
        }

        if (Stage == CaravanTutorialStage.BasicGoblins &&
            target.CombatantId == PlayableCharacterData.ProtagonistId)
        {
            return 1;
        }

        if (Stage == CaravanTutorialStage.HobgoblinPressure &&
            (target.CombatantId == "pc_01" ||
                target.CombatantId == HobgoblinCombatantId))
        {
            return 1;
        }

        if (Stage == CaravanTutorialStage.FullParty &&
            !TauntWasDemonstrated &&
            target.CombatantId == HobgoblinCombatantId)
        {
            return 1;
        }

        return 0;
    }

    public void OnCommandResolved(
        PartyBattleCommand command,
        CombatActionResult result)
    {
        if (Stage == CaravanTutorialStage.FullParty &&
            command != null && result != null && result.Succeeded &&
            command.ActorId == "pc_02" &&
            command.Type == PartyBattleCommandType.Ability &&
            command.AbilityId == CombatAbilityCatalog.TauntId)
        {
            TauntWasDemonstrated = true;
        }
    }

    private IReadOnlyList<string> BeginHobgoblinPressure()
    {
        LowerToTutorialCheckpoint(session.Player.Stats);
        PlayableCharacterData iona = session.EnsureTutorialCompanion("pc_01");
        MonsterData hobgoblin = MonsterCatalog
            .Get("monster_tutorial_hobgoblin")
            .CreateMonster(HobgoblinCombatantId);

        if (iona == null ||
            !session.PartyBattle.TryAddPartyMember(iona) ||
            !session.ReplaceTutorialEnemies(new[] { hobgoblin }))
        {
            throw new InvalidOperationException(
                "The Iona tutorial reinforcement could not be created.");
        }

        Stage = CaravanTutorialStage.HobgoblinPressure;
        return new[]
        {
            $"The first wave leaves {session.Player.Name} at low HP.",
            "Iona joins the fight. Her Mend ability can restore an ally.",
            "A Hobgoblin charges the caravan!"
        };
    }

    private IReadOnlyList<string> BeginFullParty()
    {
        PlayableCharacterData iona = session.Party.GetCharacter("pc_01");
        LowerToTutorialCheckpoint(iona.Stats);
        PlayableCharacterData damari = session.EnsureTutorialCompanion("pc_02");
        PlayableCharacterData enora = session.EnsureTutorialCompanion("pc_03");

        if (damari == null || enora == null ||
            !session.PartyBattle.TryAddPartyMember(damari) ||
            !session.PartyBattle.TryAddPartyMember(enora))
        {
            throw new InvalidOperationException(
                "The final tutorial reinforcements could not be created.");
        }

        Stage = CaravanTutorialStage.FullParty;
        return new[]
        {
            $"The Hobgoblin's assault leaves Iona at {iona.Stats.CurrentHp} HP.",
            "Damari and Enora join the fight.",
            "Use Damari's Taunt to draw the Hobgoblin's attention, then strike with Enora's Blood Bolt."
        };
    }

    private static void LowerToTutorialCheckpoint(CombatantStats stats)
    {
        stats.CurrentHp = Math.Max(1, stats.MaxHp / 4);
    }
}
