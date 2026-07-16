public class SimpleBattleSystem
{
    private readonly PhysicalAttackAction physicalAttack;
    private readonly ICombatRandom random;

    public SimpleBattleSystem(ICombatRandom random = null)
    {
        physicalAttack = new PhysicalAttackAction();
        this.random = random ?? new SystemCombatRandom();
    }

    public CombatActionResult PlayerAttack(
        PlayerData player,
        MonsterData monster)
    {
        return ResolvePhysicalAttack(player, monster);
    }

    public CombatActionResult MonsterAttack(
        PlayerData player,
        MonsterData monster,
        bool playerIsGuarding = false)
    {
        return ResolvePhysicalAttack(monster, player, playerIsGuarding);
    }

    private CombatActionResult ResolvePhysicalAttack(
        ICombatant actor,
        ICombatant target,
        bool targetIsGuarding = false)
    {
        return physicalAttack.Execute(
            new CombatActionContext(
                actor,
                target,
                random,
                targetIsGuarding));
    }
}
