enum Type {
    NORMAL,
    FIRE,
    WATER,
    GRASS,
    GROUND,
    FLY,
    ELECTRIC,
    LIGHT,
    DARK
}

interface MoveToLearn {
    move_id: number
    levelMin: number
}

interface Move {
    id: number
    name: string
    desc: string
    type: Type
    power: number
    cost: number
}

const Tackle: Move = {
    id: 1,
    name: "Tackle",
    desc: "A basic physical attack that uses the user's body.",
    type: Type.NORMAL,
    power: 200,
    cost: 7,
};

const Punch: Move = {
    id: 2,
    name: "Punch",
    desc: "A strong punch aimed at the opponent.",
    type: Type.NORMAL,
    power: 50,
    cost: 15,
};

const Stomp: Move = {
    id: 3,
    name: "Stomp",
    desc: "A powerful attack that stomps down on the opponent.",
    type: Type.NORMAL,
    power: 65,
    cost: 25,
};

const Slam: Move = {
    id: 4,
    name: "Slam",
    desc: "A hard slam that causes significant damage.",
    type: Type.NORMAL,
    power: 80,
    cost: 30,
};

const Growl: Move = {
    id: 5,
    name: "Growl",
    desc: "A menacing growl that lowers the target's attack.",
    type: Type.NORMAL,
    power: 0,
    cost: 3,
};

const Harden: Move = {
    id: 6,
    name: "Harden",
    desc: "Increases the user's defense by hardening their body.",
    type: Type.NORMAL,
    power: 0,
    cost: 4,
};

const Ember: Move = {
    id: 7,
    name: "Ember",
    desc: "A small flame attack that may cause a burn.",
    type: Type.FIRE,
    power: 60,
    cost: 12,
};

const FirePunch: Move = {
    id: 8,
    name: "Fire Punch",
    desc: "A punch imbued with fire that burns the target.",
    type: Type.FIRE,
    power: 75,
    cost: 15,
};

const Flamethrower: Move = {
    id: 9,
    name: "Flamethrower",
    desc: "A stream of fire that engulfs the target.",
    type: Type.FIRE,
    power: 90,
    cost: 30,
};

const FireBlast: Move = {
    id: 10,
    name: "Fire Blast",
    desc: "A powerful fire attack that can leave the target burned.",
    type: Type.FIRE,
    power: 110,
    cost: 40,
};

const Bubble: Move = {
    id: 11,
    name: "Bubble",
    desc: "A stream of bubbles that can trap the opponent.",
    type: Type.WATER,
    power: 50,
    cost: 5,
};

const BubbleBeam: Move = {
    id: 12,
    name: "Bubble Beam",
    desc: "A beam of bubbles that strikes the target with pressure.",
    type: Type.WATER,
    power: 65,
    cost: 15,
};

const Waterfall: Move = {
    id: 13,
    name: "Waterfall",
    desc: "A powerful water attack that crashes down on the target.",
    type: Type.WATER,
    power: 80,
    cost: 25,
};

const HydroPump: Move = {
    id: 14,
    name: "Hydro Pump",
    desc: "A massive blast of water that delivers high damage.",
    type: Type.WATER,
    power: 110,
    cost: 40,
};

const VineWhip: Move = {
    id: 15,
    name: "Vine Whip",
    desc: "Attacks the opponent with flexible vines.",
    type: Type.GRASS,
    power: 50,
    cost: 7,
};

const RazorLeaf: Move = {
    id: 16,
    name: "Razor Leaf",
    desc: "Sharp leaves that are fired at the target.",
    type: Type.GRASS,
    power: 75,
    cost: 15,
};

const SolarBeam: Move = {
    id: 17,
    name: "Solar Beam",
    desc: "A powerful beam of solar energy that requires a turn to charge.",
    type: Type.GRASS,
    power: 120,
    cost: 50,
};

const QuickAttack: Move = {
    id: 18,
    name: "Quick Attack",
    desc: "A swift attack that always strikes first.",
    type: Type.NORMAL,
    power: 40,
    cost: 5,
};

const Gust: Move = {
    id: 19,
    name: "Gust",
    desc: "A blast of wind that is effective against bug types.",
    type: Type.FLY,
    power: 40,
    cost: 10,
};

const HyperFang: Move = {
    id: 20,
    name: "Hyper Fang",
    desc: "A sharp bite that deals high damage.",
    type: Type.NORMAL,
    power: 80,
    cost: 15,
};

const ThunderShock: Move = {
    id: 21,
    name: "Thunder Shock",
    desc: "An electric shock that may paralyze the target.",
    type: Type.ELECTRIC,
    power: 40,
    cost: 5,
};

const ElectroBall: Move = {
    id: 22,
    name: "Electro Ball",
    desc: "A ball of electricity that grows stronger with speed.",
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
