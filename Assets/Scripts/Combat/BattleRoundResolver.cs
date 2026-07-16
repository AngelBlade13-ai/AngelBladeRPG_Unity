using System.Collections.Generic;

public class BattleRoundResult
{
    public IReadOnlyList<CombatActionResult> Actions { get; }
    public IReadOnlyList<string> Messages { get; }
    public bool PlayerWasDefeated { get; }
    public bool MonsterWasDefeated { get; }
    public bool EscapeSucceeded { get; }

    public BattleRoundResult(
        IReadOnlyList<CombatActionResult> actions,
        bool playerWasDefeated,
        bool monsterWasDefeated,
        bool escapeSucceeded = false)
    {
        Actions = actions;
        List<string> messages = new List<string>();
        foreach (CombatActionResult action in actions)
        {
            messages.Add(action.Message);
        }

        Messages = messages;
        PlayerWasDefeated = playerWasDefeated;
        MonsterWasDefeated = monsterWasDefeated;
        EscapeSucceeded = escapeSucceeded;
    }
}

public class BattleRoundResolver
{
    private const string PlayerId = "player";
    private const string MonsterId = "monster";

    private readonly SimpleBattleSystem battleSystem;
    private readonly ITurnOrderRandom tieBreaker;
    private readonly ICombatRandom combatRandom;
    private readonly DefendAction defendAction;
    private readonly EscapeAction escapeAction;

    public BattleRoundResolver(
        SimpleBattleSystem battleSystem = null,
        ITurnOrderRandom tieBreaker = null,
        ICombatRandom combatRandom = null)
    {
        this.combatRandom = combatRandom ?? new SystemCombatRandom();
        this.battleSystem =
            battleSystem ?? new SimpleBattleSystem(this.combatRandom);
        this.tieBreaker = tieBreaker;
        defendAction = new DefendAction();
        escapeAction = new EscapeAction();
    }

    public BattleRoundResult ResolveAttackRound(
        PlayerData player,
        MonsterData monster)
    {
        List<CombatActionResult> actions =
            new List<CombatActionResult>();
        IReadOnlyList<BattleTurnParticipant> turnOrder =
            SpeedTurnOrder.Build(
                new[]
                {
                    new BattleTurnParticipant(PlayerId, player.Speed),
                    new BattleTurnParticipant(MonsterId, monster.Speed)
                },
                tieBreaker);

        foreach (BattleTurnParticipant participant in turnOrder)
        {
            if (player.CurrentHp <= 0 || monster.CurrentHp <= 0)
            {
                break;
            }

            actions.Add(participant.CombatantId == PlayerId
                ? battleSystem.PlayerAttack(player, monster)
                : battleSystem.MonsterAttack(player, monster));
        }

        return BuildResult(actions, player, monster);
    }

    public BattleRoundResult ResolveDefendRound(
        PlayerData player,
        MonsterData monster)
    {
        List<CombatActionResult> actions =
            new List<CombatActionResult>
            {
                defendAction.Execute(
                    new CombatActionContext(
                        player,
                        monster,
                        combatRandom))
            };

        if (player.CurrentHp > 0 && monster.CurrentHp > 0)
        {
            actions.Add(
                battleSystem.MonsterAttack(
                    player,
                    monster,
                    playerIsGuarding: true));
        }

        return BuildResult(actions, player, monster);
    }

    public BattleRoundResult ResolveEscapeRound(
        PlayerData player,
        MonsterData monster)
    {
        List<CombatActionResult> actions =
            new List<CombatActionResult>();
        CombatActionResult escapeResult = escapeAction.Execute(
            new CombatActionContext(
                player,
                monster,
                combatRandom));
        actions.Add(escapeResult);

        if (!escapeResult.Succeeded &&
            player.CurrentHp > 0 &&
            monster.CurrentHp > 0)
        {
            actions.Add(battleSystem.MonsterAttack(player, monster));
        }

        return BuildResult(
            actions,
            player,
            monster,
            escapeResult.Succeeded);
    }

    private static BattleRoundResult BuildResult(
        IReadOnlyList<CombatActionResult> actions,
        PlayerData player,
        MonsterData monster,
        bool escapeSucceeded = false)
    {
        return new BattleRoundResult(
            actions,
            player.CurrentHp <= 0,
            monster.CurrentHp <= 0,
            escapeSucceeded);
    }
}
