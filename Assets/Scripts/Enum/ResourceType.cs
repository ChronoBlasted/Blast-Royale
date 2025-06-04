using System;

public enum ResourceType
{
    None = 0,
    BaseImg = 1,

    ______UI______ = 400,

    //UI Resources
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
    ErrorMsg = 412,
    MovePedia = 413,
    BestStage = 414,

    // Stats
    Health = 420,
    Mana = 421,
    Attack = 422,
    Defense = 423,
    Speed = 424,
    Type = 425,

    // Meteo
    Sun = 440,
    Rain = 441,
    Leaves = 442,

    // Status
    Burn = 460,
    Seeded = 461,
    Wet = 462,

    // Move Effects
    ManaExplosion = 463,
    HpExplosion = 464,
    ManaRestore = 465,
    HpRestore = 466,
    AttackBoost = 467,
    DefenseBoost = 468,
    SpeedBoost = 469,
    AttackReduce = 470,
    DefenseReduce = 471,
    SpeedReduce = 472,
    Cleanse = 473,
    Combo = 474,

    Wait = 490,

    AttackDamage = 500,
    AttackStatus = 501,

    // Battle resource
    CatchSuccess = 520,
    CatchFailure = 521,
    BlastFainted = 522,
    BlastExp = 523,

    // Shop
    CoinThree = 600,
    CoinLots = 601,
    CoinMega = 602,
    GemThree = 603,
    GemLots = 604,
    GemMega = 605,
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