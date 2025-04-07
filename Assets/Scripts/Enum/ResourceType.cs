using System;

public enum ResourceType
{
    None = 0,

    ______UI______ = 400,

    Coin = 401,
    Gem = 402,
    Trophy = 403,
    Lock = 404,
    Unlock = 405,
    FloatingText = 406,
    BlastDefeated = 407,

    Blast = 408,
    Item = 409,
    BlastPedia = 410,
    ItemPedia = 411,
    MovePedia = 413,

    ErrorMsg = 412,

    Health = 420,
    Mana = 421,
    Attack = 422,
    Defense = 423,
    Speed = 424,
    Type = 425,

    Sun = 440,
    Rain = 441,
    Leaves = 442,

    Burn = 460,
    Seeded = 461,
    Wet = 462,

    ManaExplosion = 463,
    HpExplosion = 464,

    ManaRestore = 465,
    HpRestore = 466,

    AttackBoost = 467,
    DefenseBoost = 468,
    SpeedBoost =  469,
    AttackReduce = 470,
    DefenseReduce = 471,
    SpeedReduce = 472,

    Cleanse = 473,

    AttackDamage = 500,
    AttackStatus = 501,

    PlatformCost = 510,
}

public enum UIType
{
    Coin = ResourceType.Coin,
    Gem = ResourceType.Gem,
    Trophy = ResourceType.Trophy,
    Lock = ResourceType.Lock,
    Unlock = ResourceType.Unlock,
    FloatingText = ResourceType.FloatingText,
    BlastDefeated = ResourceType.BlastDefeated,
    Blast = ResourceType.Blast,
    Item = ResourceType.Item,
    BlastPedia = ResourceType.BlastPedia,
    ItemPedia = ResourceType.ItemPedia,
    ErrorMsg = ResourceType.ErrorMsg,
    MovePedia = ResourceType.MovePedia
}

public enum StatType
{
    None = ResourceType.None,
    Health = ResourceType.Health,
    Mana = ResourceType.Mana,
    Attack = ResourceType.Attack,
    Defense = ResourceType.Defense,
    Speed = ResourceType.Speed,
    Type = ResourceType.Type
}

public enum Meteo
{
    None = ResourceType.None,
    Sun = ResourceType.Sun,
    Rain = ResourceType.Rain,
    Leaves = ResourceType.Leaves,
}