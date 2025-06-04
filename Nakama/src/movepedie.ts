
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
    effects: MoveEffectData[];
}

interface MoveEffectData {
    effect: MoveEffect;
    effectModifier: number;
    effectTarget: Target;
}

//#region Normal

const Tackle: Move = {
    id: 1,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 40,
    cost: 7,
    priority: 0,
    effects: [],
};

const Punch: Move = {
    id: 2,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 10,
    priority: 0,
    effects: [],
};

const Stomp: Move = {
    id: 3,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 65,
    cost: 15,
    priority: 0,
    effects: [],

};

const Slam: Move = {
    id: 4,
    type: Type.NORMAL,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 80,
    cost: 1,
    priority: 0,
    effects: [
        { effect: MoveEffect.HpExplosion, effectModifier: 0, effectTarget: Target.Opponent },
    ],
};

const Claw: Move = {
    id: 5,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 30,
    cost: 5,
    priority: 0,
    effects: [],
};

const ClawCombo: Move = {
    id: 6,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 20,
    cost: 7,
    priority: 0,
    effects: [
        { effect: MoveEffect.Combo, effectModifier: 0, effectTarget: Target.Opponent },
    ],
};

const Slash: Move = {
    id: 7,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 60,
    cost: 10,
    priority: 0,
    effects: [],
};

const Cut: Move = {
    id: 8,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 80,
    cost: 13,
    priority: 0,
    effects: [],
};

const QuickAttack: Move = {
    id: 9,
    type: Type.NORMAL,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 30,
    cost: 6,
    priority: 2,
    effects: [],
};


const Growl: Move = {
    id: 10,
    type: Type.NORMAL,
    attackType: AttackType.Status,
    target: Target.Opponent,
    power: 0,
    cost: 5,
    priority: 0,
    effects: [
        { effect: MoveEffect.AttackReduce, effectModifier: 1, effectTarget: Target.Opponent },
    ],
};

const Harden: Move = {
    id: 11,
    type: Type.NORMAL,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 6,
    priority: 0,
    effects: [
        { effect: MoveEffect.DefenseBoost, effectModifier: 2, effectTarget: Target.Self },
    ],
};

const WarmUp: Move = {
    id: 12,
    type: Type.NORMAL,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.SpeedBoost, effectModifier: 2, effectTarget: Target.Self },
    ],
};

const Taunt: Move = {
    id: 13,
    type: Type.NORMAL,
    attackType: AttackType.Status,
    target: Target.Opponent,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.AttackBoost, effectModifier: 1, effectTarget: Target.Opponent },
        { effect: MoveEffect.DefenseReduce, effectModifier: 1, effectTarget: Target.Opponent },
    ],
};

const Cleanse: Move = {
    id: 14,
    type: Type.NORMAL,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 0,
    priority: 0,
    effects: [
        { effect: MoveEffect.Cleanse, effectModifier: 0, effectTarget: Target.Self },
    ],
};

const Focus: Move = {
    id: 15,
    type: Type.NORMAL,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 0,
    priority: 0,
    effects: [
        { effect: MoveEffect.AttackBoost, effectModifier: 2, effectTarget: Target.Self },
    ],
};

//#region Fire

const Ember: Move = {
    id: 101,
    type: Type.FIRE,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 5,
    priority: 0,
    effects: [
        { effect: MoveEffect.Burn, effectModifier: 0, effectTarget: Target.Opponent },
    ],

};

const FirePunch: Move = {
    id: 102,
    type: Type.FIRE,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 70,
    cost: 7,
    priority: 0,
    effects: [],
};

const Flamethrower: Move = {
    id: 103,
    type: Type.FIRE,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 80,
    cost: 1,
    priority: 0,
    effects: [
        { effect: MoveEffect.AttackReduce, effectModifier: 1, effectTarget: Target.Opponent },
    ],

};

const FireBlast: Move = {
    id: 104,
    type: Type.FIRE,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 110,
    cost: 2,
    priority: 0,
    effects: [
        { effect: MoveEffect.Burn, effectModifier: 0, effectTarget: Target.Opponent },
    ],

};

const FireWheel: Move = {
    id: 105,
    type: Type.FIRE,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 7,
    priority: 0,
    effects: [
        { effect: MoveEffect.Burn, effectModifier: 0, effectTarget: Target.Opponent },
    ],

};

const Nitro: Move = {
    id: 106,
    type: Type.FIRE,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 60,
    cost: 12,
    priority: 1,
    effects: [
        { effect: MoveEffect.SpeedBoost, effectModifier: 1, effectTarget: Target.Self },
    ],

};

const Scald: Move = {
    id: 107,
    type: Type.FIRE,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 80,
    cost: 1,
    priority: 0,
    effects: [
        { effect: MoveEffect.Burn, effectModifier: 0, effectTarget: Target.Opponent },
    ],

};

const FlareBlitz: Move = {
    id: 108,
    type: Type.FIRE,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 110,
    cost: 2,
    priority: 0,
    effects: [
        { effect: MoveEffect.Burn, effectModifier: 0, effectTarget: Target.Opponent },
    ],

};

const FireClaw: Move = {
    id: 109,
    type: Type.FIRE,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 100,
    cost: 15,
    priority: 0,
    effects: [
        { effect: MoveEffect.AttackBoost, effectModifier: 1, effectTarget: Target.Self },
    ],
};

const OverHeat: Move = {
    id: 110,
    type: Type.FIRE,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.AttackBoost, effectModifier: 2, effectTarget: Target.Self },
    ],
};

const Heatwave: Move = {
    id: 111,
    type: Type.FIRE,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.DefenseReduce, effectModifier: 2, effectTarget: Target.Opponent },
    ],
};

const Combustion: Move = {
    id: 112,
    type: Type.FIRE,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.SpeedBoost, effectModifier: 2, effectTarget: Target.Self },
    ],
};



//#region Water

const AquaJet: Move = {
    id: 201,
    type: Type.WATER,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 6,
    priority: 0,
    effects: [
        { effect: MoveEffect.SpeedBoost, effectModifier: 1, effectTarget: Target.Self },
    ],

};

const HydroTail: Move = {
    id: 202,
    type: Type.WATER,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 65,
    cost: 8,
    priority: 0,
    effects: [],

};

const Waterfall: Move = {
    id: 203,
    type: Type.WATER,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 80,
    cost: 1,
    priority: 0,
    effects: [
        { effect: MoveEffect.SpeedBoost, effectModifier: 1, effectTarget: Target.Self },
    ],
};

const HydroBlast: Move = {
    id: 204,
    type: Type.WATER,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 110,
    cost: 2,
    priority: 0,
    effects: [
        { effect: MoveEffect.Wet, effectModifier: 0, effectTarget: Target.Opponent },
    ],
};

const Bubble: Move = {
    id: 205,
    type: Type.WATER,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 6,
    priority: 0,
    effects: [],
};

const BubbleBeam: Move = {
    id: 206,
    type: Type.WATER,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 65,
    cost: 10,
    priority: 0,
    effects: [],
};

const Surf: Move = {
    id: 207,
    type: Type.WATER,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 80,
    cost: 1,
    priority: 0,
    effects: [
        { effect: MoveEffect.SpeedBoost, effectModifier: 1, effectTarget: Target.Self },
    ],
};

const HydroCannon: Move = {
    id: 208,
    type: Type.WATER,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 110,
    cost: 2,
    priority: 0,
    effects: [
        { effect: MoveEffect.Wet, effectModifier: 0, effectTarget: Target.Opponent },
    ],
};

const AquaClaw: Move = {
    id: 209,
    type: Type.WATER,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 100,
    cost: 15,
    priority: 0,
    effects: [
        { effect: MoveEffect.SpeedBoost, effectModifier: 1, effectTarget: Target.Self },
    ],
};

const AquaBoost: Move = {
    id: 210,
    type: Type.WATER,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.AttackBoost, effectModifier: 2, effectTarget: Target.Self },
    ],
};

const AquaWall: Move = {
    id: 211,
    type: Type.WATER,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.DefenseBoost, effectModifier: 2, effectTarget: Target.Self },
    ],
};

const Splash: Move = {
    id: 212,
    type: Type.WATER,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.SpeedReduce, effectModifier: 2, effectTarget: Target.Opponent },
    ],
};

const Aquagym: Move = {
    id: 213,
    type: Type.WATER,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.SpeedBoost, effectModifier: 2, effectTarget: Target.Self },
    ],
};

//#region Grass

const Leafs: Move = {
    id: 301,
    type: Type.GRASS,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 4,
    priority: 0,
    effects: [
        { effect: MoveEffect.Seeded, effectModifier: 0, effectTarget: Target.Opponent },
    ],
};

const RazorLeaf: Move = {
    id: 302,
    type: Type.GRASS,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 75,
    cost: 7,
    priority: 0,
    effects: [
        { effect: MoveEffect.DefenseReduce, effectModifier: 1, effectTarget: Target.Opponent },
    ],
};

const VineWhip: Move = {
    id: 303,
    type: Type.GRASS,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 75,
    cost: 1,
    priority: 0,
    effects: [
        { effect: MoveEffect.Seeded, effectModifier: 0, effectTarget: Target.Opponent },
    ],
};

const GrassKnot: Move = {
    id: 304,
    type: Type.GRASS,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 100,
    cost: 2,
    priority: 0,
    effects: [
        { effect: MoveEffect.SpeedReduce, effectModifier: 2, effectTarget: Target.Opponent },
    ],
};

const FlowerStorm: Move = {
    id: 305,
    type: Type.GRASS,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 150,
    cost: 3,
    priority: 0,
    effects: [
        { effect: MoveEffect.None, effectModifier: 0, effectTarget: Target.None },
    ],
};

const GreenTempest: Move = {
    id: 306,
    type: Type.GRASS,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 50,
    cost: 3,
    priority: 0,
    effects: [
        { effect: MoveEffect.Combo, effectModifier: 0, effectTarget: Target.Opponent },
    ],
};

const MagikLeafs: Move = {
    id: 307,
    type: Type.GRASS,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 50,
    cost: 7,
    priority: 0,
    effects: [
        { effect: MoveEffect.Seeded, effectModifier: 0, effectTarget: Target.Opponent },
    ],
};

const EcoSphere: Move = {
    id: 308,
    type: Type.GRASS,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 80,
    cost: 1,
    priority: 0,
    effects: [
        { effect: MoveEffect.DefenseBoost, effectModifier: 1, effectTarget: Target.Self },
    ],
};

const SolarBeam: Move = {
    id: 309,
    type: Type.GRASS,
    attackType: AttackType.Special,
    target: Target.Opponent,
    power: 120,
    cost: 2,
    priority: 0,
    effects: [
        { effect: MoveEffect.Seeded, effectModifier: 0, effectTarget: Target.Opponent },
    ],
};

const GrassClaw: Move = {
    id: 310,
    type: Type.GRASS,
    attackType: AttackType.Normal,
    target: Target.Opponent,
    power: 100,
    cost: 15,
    priority: 0,
    effects: [
        { effect: MoveEffect.DefenseBoost, effectModifier: 1, effectTarget: Target.Self },
    ],
};

const Growth: Move = {
    id: 311,
    type: Type.GRASS,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.AttackBoost, effectModifier: 2, effectTarget: Target.Self },
    ],
};

const Roots: Move = {
    id: 312,
    type: Type.GRASS,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.DefenseBoost, effectModifier: 2, effectTarget: Target.Self },
    ],
};

const Spore: Move = {
    id: 313,
    type: Type.GRASS,
    attackType: AttackType.Status,
    target: Target.Self,
    power: 0,
    cost: 10,
    priority: 0,
    effects: [
        { effect: MoveEffect.DefenseReduce, effectModifier: 2, effectTarget: Target.Opponent },
    ],
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

    Tackle, Punch, Stomp, Slam,
    Claw, ClawCombo, Slash, Cut,
    QuickAttack,
    Growl, Harden, Focus, WarmUp, Taunt, Cleanse,

    Ember, FirePunch, Flamethrower, FireBlast,
    FireWheel, Nitro, Scald, FlareBlitz,
    FireClaw,
    OverHeat, Heatwave, Combustion,

    AquaJet, HydroTail, Waterfall, HydroBlast,
    Bubble, BubbleBeam, Surf, HydroCannon,
    AquaClaw,
    AquaBoost, AquaWall, Splash, Aquagym,

    Leafs, RazorLeaf, VineWhip, GrassKnot, FlowerStorm, GreenTempest,
    MagikLeafs, EcoSphere, SolarBeam,
    GrassClaw,
    Growth, Roots, Spore,
];

const rpcLoadMovePedia: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
        return JSON.stringify(movePedia);
    }
