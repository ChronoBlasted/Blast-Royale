using System;

[Serializable]
public class BlastData
{
    public int id;
    public string desc;
    public TYPE type;
    public int hp;
    public int mana;
    public int attack;
    public int defense;
    public int speed;
    public MoveToLearn[] movepool;
    public NextEvolution nextEvolution;
    public int catchRate;
    public int expYield;
    public Rarity rarity;
}

public enum Rarity
{
    NONE,
    COMMON,
    UNCOMMON,
    RARE,
    EPIC,
    LEGENDARY,
    ULTIMATE,
    UNIQUE,
}