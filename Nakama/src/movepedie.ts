interface moveToLearn {
    move_id: number
    levelMin: number
}

interface Move {
    id: number
    type: Type
    power: number
    cost: number
    priority: number;
    platform_cost: number
    effect: MoveEffect; 
}

const Tackle: Move = {
    id: 1,
    type: Type.NORMAL,
    power: 40,
    cost: 7,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.None,
};

const Punch: Move = {
    id: 2,
    type: Type.NORMAL,
    power: 50,
    cost: 15,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.None,
};

const Stomp: Move = {
    id: 3,
    type: Type.NORMAL,
    power: 65,
    cost: 25,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.None,
};

const Slam: Move = {
    id: 4,
    type: Type.NORMAL,
    power: 80,
    cost: 30,
    priority: 0,
    platform_cost: 1,
    effect: MoveEffect.HpExplosion,
};

const Growl: Move = {
    id: 5,
    type: Type.NORMAL,
    power: 0,
    cost: 3,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.AttackReduce,
};

const Harden: Move = {
    id: 6,
    type: Type.NORMAL,
    power: 0,
    cost: 4,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.DefenseBoost,
};

const Ember: Move = {
    id: 7,
    type: Type.FIRE,
    power: 50,
    cost: 12,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.Burn,
};

const FirePunch: Move = {
    id: 8,
    type: Type.FIRE,
    power: 75,
    cost: 15,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.None,
};

const Flamethrower: Move = {
    id: 9,
    type: Type.FIRE,
    power: 90,
    cost: 30,
    priority: 0,
    platform_cost: 1,
    effect: MoveEffect.AttackBoost,
};

const FireBlast: Move = {
    id: 10,
    type: Type.FIRE,
    power: 110,
    cost: 40,
    priority: 0,
    platform_cost: 2,
    effect: MoveEffect.Burn,
};

const Bubble: Move = {
    id: 11,
    type: Type.WATER,
    power: 50,
    cost: 5,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.Wet,
};

const BubbleBeam: Move = {
    id: 12,
    type: Type.WATER,
    power: 65,
    cost: 15,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.None,
};

const Waterfall: Move = {
    id: 13,
    type: Type.WATER,
    power: 80,
    cost: 25,
    priority: 0,
    platform_cost: 1,
    effect: MoveEffect.SpeedBoost,
};

const HydroPump: Move = {
    id: 14,
    type: Type.WATER,
    power: 110,
    cost: 40,
    priority: 0,
    platform_cost: 2,
    effect: MoveEffect.Wet,
};

const VineWhip: Move = {
    id: 15,
    type: Type.GRASS,
    power: 50,
    cost: 7,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.Seeded,
};

const RazorLeaf: Move = {
    id: 16,
    type: Type.GRASS,
    power: 75,
    cost: 15,
    priority: 0,
    platform_cost: 1,
    effect: MoveEffect.DefenseBoost,
};

const SolarBeam: Move = {
    id: 17,
    type: Type.GRASS,
    power: 120,
    cost: 0,
    priority: 0,
    platform_cost: 2,
    effect: MoveEffect.Seeded,
};

const QuickAttack: Move = {
    id: 18,
    type: Type.NORMAL,
    power: 40,
    cost: 5,
    priority: 1,
    platform_cost: 0,
    effect: MoveEffect.None,
};

const Gust: Move = {
    id: 19,
    type: Type.FLY,
    power: 40,
    cost: 10,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.None,
};

const HyperFang: Move = {
    id: 20,
    type: Type.NORMAL,
    power: 80,
    cost: 15,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.None,
};

const ThunderShock: Move = {
    id: 21,
    type: Type.ELECTRIC,
    power: 40,
    cost: 5,
    priority: 0,
    platform_cost: 0,
    effect: MoveEffect.None,
};

const ElectroBall: Move = {
    id: 22,
    type: Type.ELECTRIC,
    power: 90,
    cost: 30,
    priority: 0,
    platform_cost: 1,
    effect: MoveEffect.ManaExplosion,
};

const Cleanse: Move = {
    id: 23,
    type: Type.NORMAL,
    power: 0,
    cost: 0,
    priority: 0,
    platform_cost: 3,
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
    Cleanse,
];

const rpcLoadMovePedia: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
        return JSON.stringify(movePedia);
    }
