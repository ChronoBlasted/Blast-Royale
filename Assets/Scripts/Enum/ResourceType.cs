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
    ArrowUp = 410,
    ArrowDown = 411,
    ArrowLeft = 412,
    ArrowRight = 413,
    BlastPedia = 414,
    ItemPedia = 415,
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
    ArrowUp = ResourceType.ArrowUp,
    ArrowDown = ResourceType.ArrowDown,
    ArrowLeft = ResourceType.ArrowLeft,
    ArrowRight = ResourceType.ArrowRight,
    BlastPedia = ResourceType.BlastPedia,
    ItemPedia = ResourceType.ItemPedia,
}