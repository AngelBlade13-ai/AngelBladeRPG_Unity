using UnityEngine;

public class SimpleBattleSystem
{
    public string PlayerAttack(PlayerData player, MonsterData monster)
    {
        int damage = Mathf.Max(1, player.Attack - monster.Defense);

        monster.Stats.ApplyDamage(damage);

        return $"{player.Name} attacks {monster.Name} for {damage} damage.";
    }

    public string MonsterAttack(PlayerData player, MonsterData monster)
    {
        int damage = Mathf.Max(1, monster.Attack - player.Defense);

        player.Stats.ApplyDamage(damage);

        return $"{monster.Name} attacks {player.Name} for {damage} damage.";
    }
}
