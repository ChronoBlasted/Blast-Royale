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
    ErrorMsg = 412,
    MovePedia = 413,

    Health = 420,
    Mana = 421,
    Attack = 422,
    Defense = 423,
    Speed = 424,
    Type = 425
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
    MovePedia = ResourceType.MovePedia,
}

public enum StatType
{
    None = ResourceType.None,
    Health = ResourceType.Health,
    Mana = ResourceType.Mana,
    Attack = ResourceType.Attack,
    Defense = ResourceType.Defense,
    Speed = ResourceType.Speed,
    Type = ResourceType.Type,
}