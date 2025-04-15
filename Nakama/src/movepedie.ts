interface moveToLearn {
    move_id: number
    levelMin: number
}

enum AttackType {
    None,
    Normal,
    Status,
    Special,
}

enum Target {
    None,
    Opponent,
    Self,
}

interface Move {
    id: number
    type: Type
    attackType: AttackType,
    target: Target,
    cost: number
    power: number
    priority: number;
    effect: MoveEffect; 
}

const Tackle: Move = {
    id: 1,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 40,
    cost: 7,
    priority: 0,
    effect: MoveEffect.None,
};

const Punch: Move = {
    id: 2,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 10,
    priority: 0,
    effect: MoveEffect.None,
};

const Stomp: Move = {
    id: 3,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 65,
    cost: 15,
    priority: 0,
    effect: MoveEffect.None,
};

const Slam: Move = {
    id: 4,
    type: Type.NORMAL,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 80,
    cost: 1,
    priority: 0,
    effect: MoveEffect.HpExplosion,
};

const Growl: Move = {
    id: 5,
    type: Type.NORMAL,
    attackType: AttackType.Status,
    target: Target.Opponent,
    power: 1,
    cost: 5,
    priority: 0,
    effect: MoveEffect.AttackReduce,
};

const Harden: Move = {
    id: 6,
    type: Type.NORMAL,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 2,
    cost: 4,
    priority: 0,
    effect: MoveEffect.DefenseBoost,
};

const Ember: Move = {
    id: 7,
    type: Type.FIRE,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 9,
    priority: 0,
    effect: MoveEffect.Burn,
};

const FirePunch: Move = {
    id: 8,
    type: Type.FIRE,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 70,
    cost: 12,
    priority: 0,
    effect: MoveEffect.None,
};

const Flamethrower: Move = {
    id: 9,
    type: Type.FIRE,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 80,
    cost: 1,
    priority: 0,
    effect: MoveEffect.AttackReduce,
};

const FireBlast: Move = {
    id: 10,
    type: Type.FIRE,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 110,
    cost: 2,
    priority: 0,
    effect: MoveEffect.Burn,
};

const Bubble: Move = {
    id: 11,
    type: Type.WATER,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 6,
    priority: 0,
    effect: MoveEffect.Wet,
};

const BubbleBeam: Move = {
    id: 12,
    type: Type.WATER,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 65,
    cost: 10,
    priority: 0,
    effect: MoveEffect.None,
};

const Waterfall: Move = {
    id: 13,
    type: Type.WATER,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 80,
    cost: 1,
    priority: 0,
    effect: MoveEffect.SpeedBoost,
};

const HydroPump: Move = {
    id: 14,
    type: Type.WATER,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 110,
    cost: 2,
    priority: 0,
    effect: MoveEffect.Wet,
};

const VineWhip: Move = {
    id: 15,
    type: Type.GRASS,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 7,
    priority: 0,
    effect: MoveEffect.Seeded,
};

const RazorLeaf: Move = {
    id: 16,
    type: Type.GRASS,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 75,
    cost: 1,
    priority: 0,
    effect: MoveEffect.DefenseReduce,
};

const SolarBeam: Move = {
    id: 17,
    type: Type.GRASS,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 120,
    cost: 2,
    priority: 0,
    effect: MoveEffect.Seeded,
};

const QuickAttack: Move = {
    id: 18,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 30,
    cost: 6,
    priority: 2,
    effect: MoveEffect.None,
};

const Gust: Move = {
    id: 19,
    type: Type.FLY,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 65,
    cost: 10,
    priority: 1,
    effect: MoveEffect.None,
};

const HyperFang: Move = {
    id: 20,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 80,
    cost: 15,
    priority: 0,
    effect: MoveEffect.None,
};

const ThunderShock: Move = {
    id: 21,
    type: Type.ELECTRIC,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 40,
    cost: 5,
    priority: 0,
    effect: MoveEffect.None,
};

const ElectroBall: Move = {
    id: 22,
    type: Type.ELECTRIC,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 90,
    cost: 1,
    priority: 0,
    effect: MoveEffect.ManaExplosion,
};

const Cleanse: Move = {
    id: 23,
    type: Type.NORMAL,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 0,
    priority: 0,
    effect: MoveEffect.Cleanse,
};


function getMoveById(id: number): Move {
    const move = movePedia.find((move) => move.id === id);
    if (!move) {
        throw new Error(`No Move found with ID: ${id}`);
    }
    return move;
}

function getMovesByIds(ids: number[]): Move[] {
    return ids.map(id => getMoveById(id));
}


const movePedia: Move[] = [
    Tackle,
    Punch,
    Stomp,
    Slam,
    Growl,
    Harden,
    Ember,
    FirePunch,
    Flamethrower,
    FireBlast,
    Bubble,
    BubbleBeam,
    Waterfall,
    HydroPump,
    VineWhip,
    RazorLeaf,
    SolarBeam,
    QuickAttack,
    Cleanse,
];

const rpcLoadMovePedia: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
        return JSON.stringify(movePedia);
    }
