using UnityEngine;

public class PlayerData
{
    public string Name;
    public int Level;
    public int MaxHp;
    public int CurrentHp;
    public int Attack;
    public int Defense;
    public int Gold;
    public int XP;

    public PlayerData(string name)
    {
        Name = name;
        Level = 1;
        MaxHp = 100;
        CurrentHp = 100;
        Attack = 12;
        Defense = 3;
        Gold = 0;
        XP = 0;
    }
}