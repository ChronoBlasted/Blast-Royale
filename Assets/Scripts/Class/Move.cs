using System;

[Serializable]
public class Move
{
    public int id;
    public Type type;
    public AttackType attackType;
    public Target target;
    public int power;
    public int cost;
    public int priority;
    public MoveEffect effect;
}


