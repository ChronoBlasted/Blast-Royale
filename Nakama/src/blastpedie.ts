const MinIV = 1;
const MaxIV = 31;

interface Blast {
    uuid: string;
    data_id: number;
    exp: number;
    iv: number;
    hp: number;
    maxHp: number;
    mana: number;
    maxMana: number;
    attack: number;
    defense: number;
    speed: number;
    status: STATUS;
    activeMoveset: number[];
}

interface BlastData {
    id: number
    type: Type
    hp: number
    mana: number
    attack: number
    defense: number
    speed: number
    movepool: MoveToLearn[]
    nextEvolution: nextEvolutionStruct | null
    catchRate: number
    expYield: number
    rarity: RARITY
}

interface nextEvolutionStruct {
    id :number
    levelRequired:number
}

// BlastData

const Pantin: BlastData = { // Pantin
    id: 0,
    type: Type.NORMAL,
    hp: 80,
    mana: 75,
    attack: 70,
    defense: 65,
    speed: 60,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: QuickAttack.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 30,
    expYield: 64,
    rarity: RARITY.COMMON,
};

const Lizzy: BlastData = { // Lizzy
    id: 1,
    type: Type.GRASS,
    hp: 70,
    mana: 80,
    attack: 75,
    defense: 70,
    speed: 65,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: VineWhip.id, levelMin: 2 },
        { move_id: RazorLeaf.id, levelMin: 3 },
        { move_id: SolarBeam.id, levelMin: 4 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 142,
    rarity: RARITY.RARE,
};

const Punchball: BlastData = { // Punchball
    id: 2,
    type: Type.GROUND,
    hp: 85,
    mana: 70,
    attack: 80,
    defense: 75,
    speed: 60,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: SolarBeam.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 192,
    rarity: RARITY.RARE,
};

const Jellys: BlastData = { // Jellys
    id: 3,
    type: Type.WATER,
    hp: 75,
    mana: 85,
    attack: 70,
    defense: 65,
    speed: 80,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Stomp.id, levelMin: 5 },
        { move_id: Slam.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 25,
    expYield: 64,
    rarity: RARITY.RARE,
};

const Kitchi: BlastData = { // Kitchi
    id: 4,
    type: Type.NORMAL,
    hp: 70,
    mana: 70,
    attack: 75,
    defense: 65,
    speed: 80,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Ember.id, levelMin: 5 },
        { move_id: FirePunch.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 30,
    expYield: 50,
    rarity: RARITY.COMMON,
};

const Kenchi: BlastData = { // Kenchi
    id: 5,
    type: Type.NORMAL,
    hp: 75,
    mana: 70,
    attack: 80,
    defense: 70,
    speed: 65,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Ember.id, levelMin: 5 },
        { move_id: FireBlast.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 80,
    rarity: RARITY.COMMON,
};

const Mousy: BlastData = { // Mousy
    id: 6,
    type: Type.NORMAL,
    hp: 70,
    mana: 75,
    attack: 65,
    defense: 70,
    speed: 80,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Bubble.id, levelMin: 5 },
        { move_id: BubbleBeam.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 50,
    rarity: RARITY.COMMON,
};

const Clawball: BlastData = { // Clawball
    id: 7,
    type: Type.GROUND,
    hp: 80,
    mana: 70,
    attack: 75,
    defense: 80,
    speed: 65,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Bubble.id, levelMin: 5 },
        { move_id: BubbleBeam.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 45,
    expYield: 90,
    rarity: RARITY.UNCOMMON,
};

const Balt: BlastData = { // Balt
    id: 8,
    type: Type.FLY,
    hp: 70,
    mana: 80,
    attack: 65,
    defense: 70,
    speed: 85,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Bubble.id, levelMin: 5 },
        { move_id: HydroPump.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 50,
    expYield: 60,
    rarity: RARITY.COMMON,
};

const Stagpan: BlastData = { // Stagpan
    id: 9,
    type: Type.NORMAL,
    hp: 75,
    mana: 70,
    attack: 80,
    defense: 65,
    speed: 70,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: QuickAttack.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 30,
    expYield: 65,
    rarity: RARITY.COMMON,
};

const Botte: BlastData = { // Botte
    id: 10,
    type: Type.GROUND,
    hp: 80,
    mana: 75,
    attack: 70,
    defense: 85,
    speed: 65,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: Gust.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 120,
    rarity: RARITY.RARE,
};

const Booh: BlastData = { // Booh
    id: 11,
    type: Type.NORMAL,
    hp: 70,
    mana: 75,
    attack: 65,
    defense: 70,
    speed: 80,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: QuickAttack.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 80,
    rarity: RARITY.UNCOMMON,
};

const Ghoosto: BlastData = { // Ghoosto
    id: 12,
    type: Type.NORMAL,
    hp: 75,
    mana: 70,
    attack: 80,
    defense: 65,
    speed: 70,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: HyperFang.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 45,
    expYield: 145,
    rarity: RARITY.RARE,
};

const Goblin: BlastData = { // Goblin
    id: 13,
    type: Type.NORMAL,
    hp: 70,
    mana: 75,
    attack: 65,
    defense: 70,
    speed: 80,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 192,
    rarity: RARITY.COMMON,
};

const MiniDevil: BlastData = { // MiniDevil
    id: 14,
    type: Type.NORMAL,
    hp: 75,
    mana: 70,
    attack: 80,
    defense: 65,
    speed: 70,
    movepool: [
        { move_id: Stomp.id, levelMin: 0 },
        { move_id: Ember.id, levelMin: 5 },
        { move_id: FirePunch.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 112,
    rarity: RARITY.UNCOMMON,
};

const DevilDare: BlastData = { // DevilDare
    id: 15,
    type: Type.NORMAL,
    hp: 80,
    mana: 75,
    attack: 70,
    defense: 85,
    speed: 65,
    movepool: [
        { move_id: Slam.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: FireBlast.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 45,
    expYield: 132,
    rarity: RARITY.RARE,
};

const Masks: BlastData = { // Masks
    id: 16,
    type: Type.NORMAL,
    hp: 70,
    mana: 75,
    attack: 65,
    defense: 70,
    speed: 80,
    movepool: [
        { move_id: QuickAttack.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 50,
    expYield: 156,
    rarity: RARITY.RARE,
};

const Luckun: BlastData = { // Luckun
    id: 17,
    type: Type.NORMAL,
    hp: 75,
    mana: 70,
    attack: 80,
    defense: 65,
    speed: 70,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: ElectroBall.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 30,
    expYield: 255,
    rarity: RARITY.RARE,
};

const MiniHam: BlastData = { // MiniHam
    id: 18,
    type: Type.NORMAL,
    hp: 70,
    mana: 75,
    attack: 65,
    defense: 70,
    speed: 80,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: Slam.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 75,
    rarity: RARITY.UNCOMMON,
};

const SadHam: BlastData = { // SadHam
    id: 19,
    type: Type.NORMAL,
    hp: 75,
    mana: 70,
    attack: 80,
    defense: 65,
    speed: 70,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 100,
    rarity: RARITY.RARE,
};

const MoiHam: BlastData = { // MoiHam
    id: 20,
    type: Type.NORMAL,
    hp: 80,
    mana: 75,
    attack: 70,
    defense: 85,
    speed: 65,
    movepool: [
        { move_id: Stomp.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: FirePunch.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 45,
    expYield: 125,
    rarity: RARITY.EPIC,
};

const Bearos: BlastData = { // Bearos
    id: 21,
    type: Type.NORMAL,
    hp: 70,
    mana: 75,
    attack: 65,
    defense: 70,
    speed: 80,
    movepool: [
        { move_id: Slam.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 50,
    expYield: 160,
    rarity: RARITY.RARE,
};

const Treex: BlastData = { // Treex
    id: 22,
    type: Type.NORMAL,
    hp: 75,
    mana: 70,
    attack: 80,
    defense: 65,
    speed: 70,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: ElectroBall.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 30,
    expYield: 192,
    rarity: RARITY.RARE,
};

const Moutmout: BlastData = { // Moutmout
    id: 23,
    type: Type.NORMAL,
    hp: 70,
    mana: 75,
    attack: 65,
    defense: 70,
    speed: 80,
    movepool: [
        { move_id: QuickAttack.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 128,
    rarity: RARITY.UNCOMMON,
};

const Piggy: BlastData = { // Piggy
    id: 24,
    type: Type.NORMAL,
    hp: 75,
    mana: 70,
    attack: 80,
    defense: 65,
    speed: 70,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: FirePunch.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 160,
    rarity: RARITY.UNCOMMON,
};

const Bleaub: BlastData = { // Bleaub
    id: 25,
    type: Type.NORMAL,
    hp: 70,
    mana: 75,
    attack: 65,
    defense: 70,
    speed: 80,
    movepool: [
        { move_id: Stomp.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 45,
    expYield: 80,
    rarity: RARITY.COMMON,
};

const Shroom: BlastData = { // Shroom
    id: 26,
    type: Type.NORMAL,
    hp: 75,
    mana: 70,
    attack: 80,
    defense: 65,
    speed: 70,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: ElectroBall.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 50,
    expYield: 88,
    rarity: RARITY.COMMON,
};

const Lantern: BlastData = { // Lantern
    id: 27,
    type: Type.WATER,
    hp: 70,
    mana: 75,
    attack: 65,
    defense: 70,
    speed: 80,
    movepool: [
        { move_id: Bubble.id, levelMin: 0 },
        { move_id: Waterfall.id, levelMin: 5 },
        { move_id: HydroPump.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 30,
    expYield: 112,
    rarity: RARITY.COMMON,
};

const Droplet: BlastData = { // Droplet
    id: 28,
    type: Type.WATER,
    hp: 75,
    mana: 70,
    attack: 80,
    defense: 65,
    speed: 70,
    movepool: [
        { move_id: Bubble.id, levelMin: 0 },
        { move_id: BubbleBeam.id, levelMin: 5 },
        { move_id: HydroPump.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 150,
    rarity: RARITY.EPIC,
};
const Fireball: BlastData = { // Fireball
    id: 29,
    type: Type.FIRE,
    hp: 80,
    mana: 60,
    attack: 90,
    defense: 50,
    speed: 70,
    movepool: [
        { move_id: Ember.id, levelMin: 0 },
        { move_id: FirePunch.id, levelMin: 5 },
        { move_id: Flamethrower.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 150,
    rarity: RARITY.EPIC,
};

const Mystical: BlastData = { // Mystical
    id: 30,
    type: Type.LIGHT,
    hp: 75,
    mana: 65,
    attack: 85,
    defense: 55,
    speed: 75,
    movepool: [
        { move_id: QuickAttack.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: ElectroBall.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 45,
    expYield: 150,
    rarity: RARITY.EPIC,
};

const Clover: BlastData = { // Clover
    id: 31,
    type: Type.DARK,
    hp: 70,
    mana: 70,
    attack: 80,
    defense: 60,
    speed: 70,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 50,
    expYield: 112,
    rarity: RARITY.RARE,
};

const Scorlov: BlastData = { // Scorlov
    id: 32,
    type: Type.DARK,
    hp: 85,
    mana: 55,
    attack: 75,
    defense: 65,
    speed: 70,
    movepool: [
        { move_id: Stomp.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: FirePunch.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 30,
    expYield: 128,
    rarity: RARITY.RARE,
};

const Wormie: BlastData = { // Wormie
    id: 33,
    type: Type.GRASS,
    hp: 60,
    mana: 80,
    attack: 70,
    defense: 70,
    speed: 70,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: ElectroBall.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 99,
    rarity: RARITY.COMMON,
};

const Skel: BlastData = { // Skel
    id: 34,
    type: Type.DARK,
    hp: 70,
    mana: 70,
    attack: 70,
    defense: 70,
    speed: 70,
    movepool: [
        { move_id: QuickAttack.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 77,
    rarity: RARITY.COMMON,
};

const Frederic: BlastData = { // Frederic
    id: 35,
    type: Type.LIGHT,
    hp: 75,
    mana: 65,
    attack: 85,
    defense: 55,
    speed: 75,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: FirePunch.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 45,
    expYield: 69,
    rarity: RARITY.UNCOMMON,
};

const Smoky: BlastData = { // Smoky
    id: 36,
    type: Type.WATER,
    hp: 80,
    mana: 60,
    attack: 90,
    defense: 50,
    speed: 70,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: FirePunch.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 45,
    expYield: 130,
    rarity: RARITY.UNCOMMON,
};

const Forty: BlastData = { // Forty
    id: 37,
    type: Type.GROUND,
    hp: 100,
    mana: 55,
    attack: 45,
    defense: 100,
    speed: 45,
    movepool: [
        { move_id: Stomp.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 50,
    expYield: 212,
    rarity: RARITY.RARE,
};

const Bud: BlastData = { // Bud
    id: 38,
    type: Type.DARK,
    hp: 60,
    mana: 80,
    attack: 70,
    defense: 70,
    speed: 70,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: ElectroBall.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 30,
    expYield: 169,
    rarity: RARITY.UNCOMMON,
};

const Hiboo: BlastData = { // Hiboo
    id: 39,
    type: Type.NORMAL,
    hp: 90,
    mana: 100,
    attack: 80,
    defense: 90,
    speed: 100,
    movepool: [
        { move_id: QuickAttack.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 222,
    rarity: RARITY.LEGENDARY,
};

const Eggy: BlastData = { // Eggy
    id: 40,
    type: Type.GROUND,
    hp: 100,
    mana: 40,
    attack: 30,
    defense: 70,
    speed: 20,
    movepool: [
        { move_id: Punch.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: FirePunch.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 118,
    rarity: RARITY.EPIC,
};

const Dracoblast: BlastData = { // Dracoblast
    id: 41,
    type: Type.FLY,
    hp: 90,
    mana: 90,
    attack: 90,
    defense: 90,
    speed: 100,
    movepool: [
        { move_id: Stomp.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 45,
    expYield: 255,
    rarity: RARITY.LEGENDARY,
};

const Cerberus: BlastData = { // Cerberus
    id: 42,
    type: Type.FIRE,
    hp: 100,
    mana: 80,
    attack: 100,
    defense: 80,
    speed: 100,
    movepool: [
        { move_id: Stomp.id, levelMin: 0 },
        { move_id: Harden.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: null,
    catchRate: 45,
    expYield: 255,
    rarity: RARITY.LEGENDARY,
};

const blastPedia: BlastData[] = [
    Pantin,
    Lizzy,
    Punchball,
    Jellys,
    Kitchi,
    Kenchi,
    Mousy,
    Clawball,
    Balt,
    Stagpan,
    Botte,
    Booh,
    Ghoosto,
    Goblin,
    MiniDevil,
    DevilDare,
    Masks,
    Luckun,
    MiniHam,
    SadHam,
    MoiHam,
    Bearos,
    Treex,
    Moutmout,
    Piggy,
    Bleaub,
    Shroom,
    Lantern,
    Droplet,
    Fireball,
    Mystical,
    Clover,
    Scorlov,
    Wormie,
    Skel,
    Frederic,
    Smoky,
    Forty,
    Bud,
    Hiboo,
    Eggy,
    Dracoblast,
    Cerberus,
];

function getBlastDataById(id: number): BlastData {
    const blast = blastPedia.find((blast) => blast.id === id);
    if (!blast) {
        throw new Error(`No Blast found with ID: ${id}`);
    }
    return blast;
}

const rpcLoadBlastPedia: nkruntime.RpcFunction =
    function (ctkx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
        return JSON.stringify(blastPedia);
    }