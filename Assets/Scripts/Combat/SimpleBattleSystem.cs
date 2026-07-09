using UnityEngine;

public class SimpleBattleSystem
{
    public string PlayerAttack(PlayerData player, MonsterData monster)
    {
        int damage = Mathf.Max(1, player.Attack - monster.Defense);

        monster.CurrentHp -= damage;

        if (monster.CurrentHp < 0)
        {
            monster.CurrentHp = 0;
        }

        return $"{player.Name} attacks {monster.Name} for {damage} damage.";
    }

    public string MonsterAttack(PlayerData player, MonsterData monster)
    {
        int damage = Mathf.Max(1, monster.Attack - player.Defense);

        player.CurrentHp -= damage;

        if (player.CurrentHp < 0)
        {
            player.CurrentHp = 0;
        }

        return $"{monster.Name} attacks {player.Name} for {damage} damage.";
    }
}