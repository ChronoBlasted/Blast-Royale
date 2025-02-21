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
}