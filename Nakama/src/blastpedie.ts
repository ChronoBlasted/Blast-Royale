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
    status: Status;
    activeMoveset: number[];
}

interface BlastData {
    id: number
    name: string
    desc: string
    type: TYPE
    hp: number
    mana: number
    attack: number
    defense: number
    speed: number
    movepool: MoveToLearn[]
    nextEvolution: nextEvolutionStruct | null
    catchRate: number
    expYield: number
    rarity: Rarity
}

interface nextEvolutionStruct {
    id :number
    levelRequired:number
}

// BlastData

// Définition des Pokémon (monstres) avec leurs mouvements mis à jour
const Florax: BlastData = {
    id: 1,
    name: "Florax",
    desc: "A small plant.",
    type: TYPE.GRASS,
    hp: 65,
    mana: 65,
    attack: 49,
    defense: 49,
    speed: 45,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 4 },
        { move_id: VineWhip.id, levelMin: 10 },
    ],
    nextEvolution: { id: 2, levelRequired: 16 },
    catchRate: 20,
    expYield: 64,
    rarity: Rarity.COMMON,
};

const Florabloom: BlastData = {
    id: 2,
    name: "Florabloom",
    desc: "A plant in full bloom.",
    type: TYPE.GRASS,
    hp: 80,
    mana: 80,
    attack: 62,
    defense: 63,
    speed: 60,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 4 },
        { move_id: VineWhip.id, levelMin: 10 },
        { move_id: RazorLeaf.id, levelMin: 16 },
    ],
    nextEvolution: { id: 3, levelRequired: 32 },
    catchRate: 20,
    expYield: 142,
    rarity: Rarity.COMMON,
};

const Floramajest: BlastData = {
    id: 3,
    name: "Floramajest",
    desc: "A majestic plant.",
    type: TYPE.GRASS,
    hp: 90,
    mana: 90,
    attack: 82,
    defense: 83,
    speed: 80,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 4 },
        { move_id: VineWhip.id, levelMin: 10 },
        { move_id: SolarBeam.id, levelMin: 32 },
    ],
    nextEvolution: null,
    catchRate: 20,
    expYield: 240,
    rarity: Rarity.COMMON,
};

const Pyrex: BlastData = {
    id: 4,
    name: "Pyrex",
    desc: "A small flame.",
    type: TYPE.FIRE,
    hp: 60,
    mana: 60,
    attack: 64,
    defense: 50,
    speed: 65,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Stomp.id, levelMin: 0 },
        { move_id: Slam.id, levelMin: 0 },
        { move_id: Flamethrower.id, levelMin: 0 },
        { move_id: FireBlast.id, levelMin: 0 },
        { move_id: Ember.id, levelMin: 0 },
    ],
    nextEvolution: { id: 5, levelRequired: 16 },
    catchRate: 20,
    expYield: 64,
    rarity: Rarity.COMMON,
};

const Pyroclaw: BlastData = {
    id: 5,
    name: "Pyroclaw",
    desc: "A fiery that is fierce in battle.",
    type: TYPE.FIRE,
    hp: 80,
    mana: 80,
    attack: 80,
    defense: 65,
    speed: 80,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Ember.id, levelMin: 7 },
        { move_id: FirePunch.id, levelMin: 16 },
        { move_id: Flamethrower.id, levelMin: 32 },
    ],
    nextEvolution: { id: 6, levelRequired: 36 },
    catchRate: 20,
    expYield: 142,
    rarity: Rarity.COMMON,
};

const Pyrowyvern: BlastData = {
    id: 6,
    name: "Pyrowyvern",
    desc: "A magnificent fire that can fly.",
    type: TYPE.FIRE,
    hp: 78,
    mana: 78,
    attack: 84,
    defense: 78,
    speed: 100,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Ember.id, levelMin: 7 },
        { move_id: FirePunch.id, levelMin: 16 },
        { move_id: Flamethrower.id, levelMin: 32 },
        { move_id: FireBlast.id, levelMin: 50 },
    ],
    nextEvolution: null,
    catchRate: 20,
    expYield: 240,
    rarity: Rarity.COMMON,
};

const Aquaflare: BlastData = {
    id: 7,
    name: "Aquaflare",
    desc: "A small water.",
    type: TYPE.WATER,
    hp: 60,
    mana: 60,
    attack: 48,
    defense: 65,
    speed: 43,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Bubble.id, levelMin: 5 },
    ],
    nextEvolution: { id: 8, levelRequired: 16 },
    catchRate: 20,
    expYield: 64,
    rarity: Rarity.COMMON,
};

const Aquablast: BlastData = {
    id: 8,
    name: "Aquablast",
    desc: "A water that has a strong shell.",
    type: TYPE.WATER,
    hp: 80,
    mana: 80,
    attack: 63,
    defense: 80,
    speed: 58,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Bubble.id, levelMin: 5 },
        { move_id: BubbleBeam.id, levelMin: 10 },
    ],
    nextEvolution: { id: 9, levelRequired: 36 },
    catchRate: 20,
    expYield: 142,
    rarity: Rarity.COMMON,
};

const Aqualith: BlastData = {
    id: 9,
    name: "Aqualith",
    desc: "A powerful water with cannons.",
    type: TYPE.WATER,
    hp: 79,
    mana: 79,
    attack: 83,
    defense: 100,
    speed: 78,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Bubble.id, levelMin: 5 },
        { move_id: BubbleBeam.id, levelMin: 10 },
        { move_id: HydroPump.id, levelMin: 36 },
    ],
    nextEvolution: null,
    catchRate: 20,
    expYield: 239,
    rarity: Rarity.COMMON,
};

const Zephyrex: BlastData = {
    id: 10,
    name: "Zephyrex", 
    desc: "A small bird that can fly.",
    type: TYPE.NORMAL,
    hp: 40,
    mana: 40,
    attack: 45,
    defense: 40,
    speed: 55,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: QuickAttack.id, levelMin: 10 },
    ],
    nextEvolution: { id: 11, levelRequired: 18 },
    catchRate: 20,
    expYield: 50,
    rarity: Rarity.COMMON,
};

const Zephyrwing: BlastData = {
    id: 11,
    name: "Zephyrwing",
    desc: "A powerful bird known for its sharp beak.",
    type: TYPE.NORMAL,
    hp: 63,
    mana: 63,
    attack: 60,
    defense: 55,
    speed: 71,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: QuickAttack.id, levelMin: 10 },
        { move_id: Gust.id, levelMin: 20 },
    ],
    nextEvolution: null,
    catchRate: 20,
    expYield: 100,
    rarity: Rarity.UNCOMMON,
};

const Gnawbit: BlastData = {
    id: 12,
    name: "Gnawbit",
    desc: "A small, purple rodent.",
    type: TYPE.NORMAL,
    hp: 30,
    mana: 30,
    attack: 56,
    defense: 35,
    speed: 72,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: QuickAttack.id, levelMin: 10 },
    ],
    nextEvolution: { id: 13, levelRequired: 20 },
    catchRate: 25,
    expYield: 51,
    rarity: Rarity.COMMON,
};

const Gnawfang: BlastData = {
    id: 13,
    name: "Gnawfang",
    desc: "A strong and aggressive rodent.",
    type: TYPE.NORMAL,
    hp: 55,
    mana: 55,
    attack: 81,
    defense: 60,
    speed: 97,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: QuickAttack.id, levelMin: 10 },
        { move_id: HyperFang.id, levelMin: 20 },
    ],
    nextEvolution: null,
    catchRate: 20,
    expYield: 145,
    rarity: Rarity.UNCOMMON,
};

const Electrix: BlastData = {
    id: 14,
    name: "Electrix", // Electrix
    desc: "A small, electric known for its cute appearance.",
    type: TYPE.ELECTRIC,
    hp: 35,
    mana: 35,
    attack: 55,
    defense: 40,
    speed: 90,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Growl.id, levelMin: 5 },
        { move_id: ThunderShock.id, levelMin: 10 },
    ],
    nextEvolution: { id: 15, levelRequired: 20 },
    catchRate: 25,
    expYield: 112,
    rarity: Rarity.COMMON,
};

// Tableau de tous les Pokémon
const blastPedia: BlastData[] = [
    Florax,
    Florabloom,
    Floramajest,
    Pyrex,
    Pyroclaw,
    Pyrowyvern,
    Aquaflare,
    Aquablast,
    Aqualith,
    Zephyrex,
    Zephyrwing,
    Gnawbit,
    Gnawfang,
    Electrix,
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


