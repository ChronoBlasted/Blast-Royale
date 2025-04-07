using System;

[Serializable]
public class Move
{
    public int id;
    public TYPE type;
    public int power;
    public int cost;
    public int priority;
    public int platform_cost;
    public MoveEffect effect;
}

