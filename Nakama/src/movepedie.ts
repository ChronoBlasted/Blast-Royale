interface MoveToLearn {
    move_id: number
    levelMin: number
}

interface Move {
    id: number
    type: Type
    power: number
    cost: number
}

const Tackle: Move = {
    id: 1,
    type: Type.NORMAL,
    power: 40,
    cost: 7,
};

const Punch: Move = {
    id: 2,
    type: Type.NORMAL,
    power: 50,
    cost: 15,
};

const Stomp: Move = {
    id: 3,
    type: Type.NORMAL,
    power: 65,
    cost: 25,
};

const Slam: Move = {
    id: 4,
    type: Type.NORMAL,
    power: 80,
    cost: 30,
};

const Growl: Move = {
    id: 5,
    type: Type.NORMAL,
    power: 0,
    cost: 3,
};

const Harden: Move = {
    id: 6,
    type: Type.NORMAL,
    power: 0,
    cost: 4,
};

const Ember: Move = {
    id: 7,
    type: Type.FIRE,
    power: 60,
    cost: 12,
};

const FirePunch: Move = {
    id: 8,
    type: Type.FIRE,
    power: 75,
    cost: 15,
};

const Flamethrower: Move = {
    id: 9,
    type: Type.FIRE,
    power: 90,
    cost: 30,
};

const FireBlast: Move = {
    id: 10,
    type: Type.FIRE,
    power: 110,
    cost: 40,
};

const Bubble: Move = {
    id: 11,
    type: Type.WATER,
    power: 50,
    cost: 5,
};

const BubbleBeam: Move = {
    id: 12,
    type: Type.WATER,
    power: 65,
    cost: 15,
};

const Waterfall: Move = {
    id: 13,
    type: Type.WATER,
    power: 80,
    cost: 25,
};

const HydroPump: Move = {
    id: 14,
    type: Type.WATER,
    power: 110,
    cost: 40,
};

const VineWhip: Move = {
    id: 15,
    type: Type.GRASS,
    power: 50,
    cost: 7,
};

const RazorLeaf: Move = {
    id: 16,
    type: Type.GRASS,
    power: 75,
    cost: 15,
};

const SolarBeam: Move = {
    id: 17,
    type: Type.GRASS,
    power: 120,
    cost: 50,
};

const QuickAttack: Move = {
    id: 18,
    type: Type.NORMAL,
    power: 40,
    cost: 5,
};

const Gust: Move = {
    id: 19,
    type: Type.FLY,
    power: 40,
    cost: 10,
};

const HyperFang: Move = {
    id: 20,
    type: Type.NORMAL,
    power: 80,
    cost: 15,
};

const ThunderShock: Move = {
    id: 21,
    type: Type.ELECTRIC,
    power: 40,
    cost: 5,
};

const ElectroBall: Move = {
    id: 22,
    type: Type.ELECTRIC,
    power: 90,
    cost: 30,
};


function getMoveById(id: number): Move {
    const move = movePedia.find((move) => move.id === id);
    if (!move) {
        throw new Error(`No Move found with ID: ${id}`);
    }
    return move;
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
];

const rpcLoadMovePedia: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
        return JSON.stringify(movePedia);
    }
