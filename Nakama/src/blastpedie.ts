const MinIV = 1;
const MaxIV = 31;

interface Blast {
    uuid: string;
    data_id: number;
    exp: number;
    iv: number;
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
    movepool: moveToLearn[]
    nextEvolution: nextEvolutionStruct | null
    catchRate: number
    expYield: number
    rarity: Rarity
}

interface nextEvolutionStruct {
    id: number
    levelRequired: number
}

interface modifierBlastStruct {
    stats: Stats
    amount: number
}

class BlastEntity {
    uuid: string;
    data_id: number;
    exp: number;
    iv: number;

    maxHp:number;
    hp: number;
    maxMana: number;
    mana: number;

    status: Status;
    activeMoveset: number[];
    modifiers: modifierBlastStruct[];
    level: number;

    attack: number;
    defense: number;
    speed: number;

    constructor(uuid: string, data_id: number, exp: number, iv: number, moveset: number[]) {
        this.uuid = uuid;
        this.data_id = data_id;
        this.exp = exp;
        this.iv = iv;
        this.status = Status.None;
        this.activeMoveset = moveset;
        this.modifiers = [];

        this.level = calculateLevelFromExperience(this.exp);

        this.maxHp = calculateBlastHp(getBlastDataById(this.data_id).hp, this.iv, this.level);
        this.hp = this.maxHp;
        this.maxMana = calculateBlastMana(getBlastDataById(this.data_id).mana, this.iv, this.level);
        this.mana = this.maxMana;

        this.attack = calculateBlastStat(getBlastDataById(this.data_id).attack, this.iv, this.level);
        this.defense = calculateBlastStat(getBlastDataById(this.data_id).defense, this.iv, this.level);
        this.speed =calculateBlastStat(getBlastDataById(this.data_id).speed, this.iv, this.level);
    }
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
    ],
    nextEvolution: null,
    catchRate: 30,
    expYield: 64,
    rarity: Rarity.COMMON,
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
        { move_id: Tackle.id, levelMin: 1 },
        { move_id: Growl.id, levelMin: 3 },
        { move_id: VineWhip.id, levelMin: 5 },
        { move_id: Harden.id, levelMin: 7 },
        { move_id: RazorLeaf.id, levelMin: 5 },
        { move_id: SolarBeam.id, levelMin: 5 },
        { move_id: QuickAttack.id, levelMin: 15 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 64,
    rarity: Rarity.RARE,
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
        { move_id: Tackle.id, levelMin: 1 },
        { move_id: Growl.id, levelMin: 3 },
        { move_id: Ember.id, levelMin: 5 },
        { move_id: Harden.id, levelMin: 7 },
        { move_id: FirePunch.id, levelMin: 10 },
        { move_id: Flamethrower.id, levelMin: 13 },
        { move_id: FireBlast.id, levelMin: 5 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 64,
    rarity: Rarity.RARE,
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
        { move_id: Tackle.id, levelMin: 1 },
        { move_id: Stomp.id, levelMin: 3 },
        { move_id: Bubble.id, levelMin: 5 },
        { move_id: Waterfall.id, levelMin: 7 },
        { move_id: HydroPump.id, levelMin: 5 },
        { move_id: Cleanse.id, levelMin: 12 },
    ],
    nextEvolution: null,
    catchRate: 25,
    expYield: 64,
    rarity: Rarity.RARE,
};

const Kitchi: BlastData = { // Kitchi
    id: 4,
    type: Type.NORMAL,
    hp: 40,
    mana: 70,
    attack: 75,
    defense: 65,
    speed: 80,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Ember.id, levelMin: 3 },
        { move_id: FirePunch.id, levelMin: 7 },
    ],
    nextEvolution: { id: 5, levelRequired: 7 },
    catchRate: 30,
    expYield: 32,
    rarity: Rarity.COMMON,
};

const Kenchi: BlastData = { // Kenchi
    id: 5,
    type: Type.NORMAL,
    hp: 50,
    mana: 70,
    attack: 80,
    defense: 70,
    speed: 65,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Ember.id, levelMin: 2 },
        { move_id: FireBlast.id, levelMin: 7 },
    ],
    nextEvolution: null,
    catchRate: 35,
    expYield: 48,
    rarity: Rarity.UNCOMMON,
};

const Mousy: BlastData = { // Mousy
    id: 6,
    type: Type.NORMAL,
    hp: 45,
    mana: 75,
    attack: 65,
    defense: 70,
    speed: 80,
    movepool: [
        { move_id: Tackle.id, levelMin: 0 },
        { move_id: Stomp.id, levelMin: 2 },
        { move_id: Harden.id, levelMin: 7 },
    ],
    nextEvolution: null,
    catchRate: 40,
    expYield: 50,
    rarity: Rarity.COMMON,
};

const Clawball: BlastData = { // Clawball
    id: 7,
    type: Type.GROUND,
    hp: 47,
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
    rarity: Rarity.UNCOMMON,
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
    rarity: Rarity.COMMON,
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
    rarity: Rarity.COMMON,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.UNCOMMON,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.COMMON,
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
    rarity: Rarity.UNCOMMON,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.UNCOMMON,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.EPIC,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.UNCOMMON,
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
    rarity: Rarity.UNCOMMON,
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
    rarity: Rarity.COMMON,
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
    rarity: Rarity.COMMON,
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
    rarity: Rarity.COMMON,
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
    rarity: Rarity.EPIC,
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
    rarity: Rarity.EPIC,
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
    rarity: Rarity.EPIC,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.COMMON,
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
    rarity: Rarity.COMMON,
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
    rarity: Rarity.UNCOMMON,
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
    rarity: Rarity.UNCOMMON,
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
    rarity: Rarity.RARE,
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
    rarity: Rarity.UNCOMMON,
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
    rarity: Rarity.LEGENDARY,
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
    rarity: Rarity.EPIC,
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
    rarity: Rarity.LEGENDARY,
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
    rarity: Rarity.LEGENDARY,
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