using UnityEngine;

public class MonsterData
{
    public string Name;
    public int MaxHp;
    public int CurrentHp;
    public int Attack;
    public int Defense;

    public MonsterData(string name, int hp, int attack, int defense)
    {
        Name = name;
        MaxHp = hp;
        CurrentHp = hp;
        Attack = attack;
        Defense = defense;
    }
}