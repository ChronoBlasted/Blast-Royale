using System;

[Flags]
public enum NotifFlags
{
    None = 0,
    Quest = 1 << 0,
    Friends = 1 << 1,
    BlastPediaReward = 1 << 2,
    ItemPediaReward = 1 << 3,
    MovePediaReward = 1 << 4,
    DailyReward = 1 << 5,
}