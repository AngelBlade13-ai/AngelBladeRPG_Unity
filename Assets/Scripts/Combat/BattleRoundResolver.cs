using System.Collections.Generic;

public class BattleRoundResult
{
    public IReadOnlyList<string> Messages { get; }
    public bool PlayerWasDefeated { get; }
    public bool MonsterWasDefeated { get; }

    public BattleRoundResult(
        IReadOnlyList<string> messages,
        bool playerWasDefeated,
        bool monsterWasDefeated)
    {
        Messages = messages;
        PlayerWasDefeated = playerWasDefeated;
        MonsterWasDefeated = monsterWasDefeated;
    }
}

public class BattleRoundResolver
{
    private const string PlayerId = "player";
    private const string MonsterId = "monster";

    private readonly SimpleBattleSystem battleSystem;
    private readonly ITurnOrderRandom tieBreaker;

    public BattleRoundResolver(
        SimpleBattleSystem battleSystem = null,
        ITurnOrderRandom tieBreaker = null)
    {
        this.battleSystem = battleSystem ?? new SimpleBattleSystem();
        this.tieBreaker = tieBreaker;
    }

    public BattleRoundResult ResolveAttackRound(
        PlayerData player,
        MonsterData monster)
    {
        List<string> messages = new List<string>();
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

            messages.Add(participant.CombatantId == PlayerId
                ? battleSystem.PlayerAttack(player, monster)
                : battleSystem.MonsterAttack(player, monster));
        }

        return new BattleRoundResult(
            messages,
            player.CurrentHp <= 0,
            monster.CurrentHp <= 0);
    }
}
