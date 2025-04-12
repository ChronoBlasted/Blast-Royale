using System;

[Serializable]
public class Move
{
    public int id;
    public Type type;
    public int power;
    public int cost;
    public int priority;
    public int platform_cost;
    public MoveEffect effect;

    public bool IsPlatformAttack() => platform_cost > 0;
    public bool IsStatusAttack() => power > 0;
}

