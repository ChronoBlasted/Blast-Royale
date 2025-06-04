
using System;

public enum MoveEffect
{
    None = ResourceType.None,

    Burn = ResourceType.Burn,
    Seeded = ResourceType.Seeded,
    Wet = ResourceType.Wet,

    ManaExplosion = ResourceType.ManaExplosion,
    HpExplosion = ResourceType.HpExplosion,

    ManaRestore = ResourceType.ManaRestore,
    HpRestore = ResourceType.HpRestore,

    AttackBoost = ResourceType.AttackBoost,
    DefenseBoost = ResourceType.DefenseBoost,
    SpeedBoost = ResourceType.SpeedBoost,

    AttackReduce = ResourceType.AttackReduce,
    DefenseReduce = ResourceType.DefenseReduce,
    SpeedReduce = ResourceType.SpeedReduce,

    Cleanse = ResourceType.Cleanse,
    Combo = ResourceType.Combo,
}

public class MoveEffectData
{
    public MoveEffect effect { get; set; }
    public int effectModifier { get; set; }
    public Target effectTarget { get; set; }
}