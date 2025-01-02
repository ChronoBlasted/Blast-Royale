"use strict";
let InitModule = function (ctx, logger, nk, initializer) {
    // Set up hooks.
    initializer.registerAfterAuthenticateDevice(afterAuthenticate);
    initializer.registerAfterAuthenticateEmail(afterAuthenticate);
    initializer.registerRpc('deleteAccount', rpcDeleteAccount);
    logger.info('XXXXXXXXXXXXXXXXXXXX - Blast Royale TypeScript loaded - XXXXXXXXXXXXXXXXXXXX');
};
function afterAuthenticate(ctx, logger, nk, data) {
    if (!data.created) {
        logger.info('User with id: %s account data already existing', ctx.userId);
        return;
    }
    let user_id = ctx.userId;
    let username = "Player_" + ctx.username;
    let metadata = {
        battle_pass: false,
        area: 0,
        win: 0,
        loose: 0,
        blast_captured: 0,
        blast_defeated: 0,
    };
    let displayName = "NewPlayer";
    let timezone = null;
    let location = null;
    let langTag = "EN";
    let avatarUrl = null;
    try {
        nk.accountUpdateId(user_id, username, displayName, timezone, location, langTag, avatarUrl, metadata);
    }
    catch (error) {
        logger.error('Error init update account : %s', error);
    }
    storeUserWallet(nk, user_id, DefaultWallet, logger);
    const writeBlasts = {
        collection: DeckCollectionName,
        key: DeckCollectionKey,
        permissionRead: DeckPermissionRead,
        permissionWrite: DeckPermissionWrite,
        value: defaultBlastCollection(nk, logger, ctx.userId),
        userId: ctx.userId,
    };
    try {
        nk.storageWrite([writeBlasts]);
    }
    catch (error) {
        logger.error('storageWrite error: %q', error);
        throw error;
    }
    logger.debug('new user id: %s account data initialised', ctx.userId);
}
function rpcDeleteAccount(ctx, logger, nk) {
    nk.accountDeleteId(ctx.userId);
}
const healManaPerRound = 20;
const healManaPerWait = 50;
function calculateBlastStat(baseStat, iv, level) {
    return Math.floor(((2 * baseStat + iv) * level) / 100 + 5);
}
function calculateBlastHp(baseHp, iv, level) {
    return Math.floor(((2 * baseHp + iv) * level) / 100 + level + 10);
}
function calculateBlastMana(baseMana, iv, level) {
    return Math.floor(((baseMana + iv) * (level / 100) + level / 2) + 10);
}
function calculateLevelFromExperience(experience) {
    if (experience < 0) {
        throw new Error("L'expérience totale ne peut pas être négative.");
    }
    let niveau = 1;
    let experienceNiveau = 0;
    for (let i = 1; i <= 100; i++) {
        experienceNiveau = Math.floor((i ** 3) * 100 / 2);
        if (experience < experienceNiveau) {
            break;
        }
        niveau = i;
    }
    return niveau;
}
function calculateExperienceFromLevel(level) {
    if (level < 1 || level > 100) {
        throw new Error("Le niveau doit être compris entre 1 et 100.");
    }
    let experienceNiveau = 0;
    for (let i = 1; i <= level; i++) {
        experienceNiveau = Math.floor((i ** 3) * 100 / 2);
    }
    return experienceNiveau;
}
function getRandomActiveMoveset(blastData, exp) {
    const availableMoves = blastData.movepool
        .filter(m => calculateLevelFromExperience(exp) >= m.levelMin)
        .map(m => m.move_id);
    const shuffledMoves = shuffleArray(availableMoves);
    const randomMoveset = shuffledMoves.slice(0, 4);
    return randomMoveset;
}
function clamp(value, min, max) {
    return Math.min(Math.max(value, min), max);
}
function getRandomNumber(min, max) {
    const minCeiled = Math.ceil(min);
    const maxFloored = Math.floor(max);
    return Math.floor(Math.random() * (maxFloored - minCeiled + 1)) + minCeiled;
}
function shuffleArray(array) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
}
var Currency;
(function (Currency) {
    Currency["Coins"] = "coins";
    Currency["Gems"] = "gems";
    Currency["Trophies"] = "trophies";
    Currency["Hard"] = "hard";
})(Currency || (Currency = {}));
;
let DefaultWallet = {
    [Currency.Coins]: 1000,
    [Currency.Gems]: 100,
    [Currency.Trophies]: 0,
};
function storeUserWallet(nk, user_id, changeset, logger) {
    try {
        nk.walletUpdate(user_id, changeset);
    }
    catch (error) {
        logger.error('Error storing wallet of player : %s', user_id);
    }
}
var Type;
(function (Type) {
    Type[Type["NORMAL"] = 0] = "NORMAL";
    Type[Type["FIRE"] = 1] = "FIRE";
    Type[Type["WATER"] = 2] = "WATER";
    Type[Type["GRASS"] = 3] = "GRASS";
    Type[Type["GROUND"] = 4] = "GROUND";
    Type[Type["FLY"] = 5] = "FLY";
    Type[Type["ELECTRIC"] = 6] = "ELECTRIC";
    Type[Type["LIGHT"] = 7] = "LIGHT";
    Type[Type["DARK"] = 8] = "DARK";
})(Type || (Type = {}));
const Tackle = {
    id: 1,
    name: "Tackle",
    desc: "A basic physical attack that uses the user's body.",
    type: Type.NORMAL,
    power: 200,
    cost: 7,
};
const Punch = {
    id: 2,
    name: "Punch",
    desc: "A strong punch aimed at the opponent.",
    type: Type.NORMAL,
    power: 50,
    cost: 15,
};
const Stomp = {
    id: 3,
    name: "Stomp",
    desc: "A powerful attack that stomps down on the opponent.",
    type: Type.NORMAL,
    power: 65,
    cost: 25,
};
const Slam = {
    id: 4,
    name: "Slam",
    desc: "A hard slam that causes significant damage.",
    type: Type.NORMAL,
    power: 80,
    cost: 30,
};
const Growl = {
    id: 5,
    name: "Growl",
    desc: "A menacing growl that lowers the target's attack.",
    type: Type.NORMAL,
    power: 0,
    cost: 3,
};
const Harden = {
    id: 6,
    name: "Harden",
    desc: "Increases the user's defense by hardening their body.",
    type: Type.NORMAL,
    power: 0,
    cost: 4,
};
const Ember = {
    id: 7,
    name: "Ember",
    desc: "A small flame attack that may cause a burn.",
    type: Type.FIRE,
    power: 60,
    cost: 12,
};
const FirePunch = {
    id: 8,
    name: "Fire Punch",
    desc: "A punch imbued with fire that burns the target.",
    type: Type.FIRE,
    power: 75,
    cost: 15,
};
const Flamethrower = {
    id: 9,
    name: "Flamethrower",
    desc: "A stream of fire that engulfs the target.",
    type: Type.FIRE,
    power: 90,
    cost: 30,
};
const FireBlast = {
    id: 10,
    name: "Fire Blast",
    desc: "A powerful fire attack that can leave the target burned.",
    type: Type.FIRE,
    power: 110,
    cost: 40,
};
const Bubble = {
    id: 11,
    name: "Bubble",
    desc: "A stream of bubbles that can trap the opponent.",
    type: Type.WATER,
    power: 50,
    cost: 5,
};
const BubbleBeam = {
    id: 12,
    name: "Bubble Beam",
    desc: "A beam of bubbles that strikes the target with pressure.",
    type: Type.WATER,
    power: 65,
    cost: 15,
};
const Waterfall = {
    id: 13,
    name: "Waterfall",
    desc: "A powerful water attack that crashes down on the target.",
    type: Type.WATER,
    power: 80,
    cost: 25,
};
const HydroPump = {
    id: 14,
    name: "Hydro Pump",
    desc: "A massive blast of water that delivers high damage.",
    type: Type.WATER,
    power: 110,
    cost: 40,
};
const VineWhip = {
    id: 15,
    name: "Vine Whip",
    desc: "Attacks the opponent with flexible vines.",
    type: Type.GRASS,
    power: 50,
    cost: 7,
};
const RazorLeaf = {
    id: 16,
    name: "Razor Leaf",
    desc: "Sharp leaves that are fired at the target.",
    type: Type.GRASS,
    power: 75,
    cost: 15,
};
const SolarBeam = {
    id: 17,
    name: "Solar Beam",
    desc: "A powerful beam of solar energy that requires a turn to charge.",
    type: Type.GRASS,
    power: 120,
    cost: 50,
};
const QuickAttack = {
    id: 18,
    name: "Quick Attack",
    desc: "A swift attack that always strikes first.",
    type: Type.NORMAL,
    power: 40,
    cost: 5,
};
const Gust = {
    id: 19,
    name: "Gust",
    desc: "A blast of wind that is effective against bug types.",
    type: Type.FLY,
    power: 40,
    cost: 10,
};
const HyperFang = {
    id: 20,
    name: "Hyper Fang",
    desc: "A sharp bite that deals high damage.",
    type: Type.NORMAL,
    power: 80,
    cost: 15,
};
const ThunderShock = {
    id: 21,
    name: "Thunder Shock",
    desc: "An electric shock that may paralyze the target.",
    type: Type.ELECTRIC,
    power: 40,
    cost: 5,
};
const ElectroBall = {
    id: 22,
    name: "Electro Ball",
    desc: "A ball of electricity that grows stronger with speed.",
    type: Type.ELECTRIC,
    power: 90,
    cost: 30,
};
function getMoveById(id) {
    const move = movePedia.find((move) => move.id === id);
    if (!move) {
        throw new Error(`No Move found with ID: ${id}`);
    }
    return move;
}
const movePedia = [
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
const rpcLoadMovePedia = function (ctx, logger, nk) {
    return JSON.stringify(movePedia);
};
var ItemBehaviour;
(function (ItemBehaviour) {
    ItemBehaviour[ItemBehaviour["NONE"] = 0] = "NONE";
    ItemBehaviour[ItemBehaviour["HEAL"] = 1] = "HEAL";
    ItemBehaviour[ItemBehaviour["MANA"] = 2] = "MANA";
    ItemBehaviour[ItemBehaviour["STATUS"] = 3] = "STATUS";
    ItemBehaviour[ItemBehaviour["CATCH"] = 4] = "CATCH";
})(ItemBehaviour || (ItemBehaviour = {}));
;
var Status;
(function (Status) {
    Status[Status["NONE"] = 0] = "NONE";
    Status[Status["SLEEP"] = 1] = "SLEEP";
    Status[Status["BURN"] = 2] = "BURN";
    Status[Status["POISONOUS"] = 3] = "POISONOUS";
    Status[Status["WET"] = 4] = "WET";
})(Status || (Status = {}));
;
var Rarity;
(function (Rarity) {
    Rarity[Rarity["NONE"] = 0] = "NONE";
    Rarity[Rarity["COMMON"] = 1] = "COMMON";
    Rarity[Rarity["UNCOMMON"] = 2] = "UNCOMMON";
    Rarity[Rarity["RARE"] = 3] = "RARE";
    Rarity[Rarity["EPIC"] = 4] = "EPIC";
    Rarity[Rarity["LEGENDARY"] = 5] = "LEGENDARY";
    Rarity[Rarity["ULTIMATE"] = 6] = "ULTIMATE";
    Rarity[Rarity["UNIQUE"] = 7] = "UNIQUE";
})(Rarity || (Rarity = {}));
const healthPotionData = {
    id: 1,
    name: "Potion",
    desc: "Give 20 HP",
    behaviour: ItemBehaviour.HEAL,
    gain_amount: 20,
    status: Status.NONE,
    catchRate: 0,
    rarity: Rarity.COMMON,
};
const superHealthPotionData = {
    id: 2,
    name: "Super Potion",
    desc: "Give 50 HP",
    behaviour: ItemBehaviour.HEAL,
    gain_amount: 50,
    status: Status.NONE,
    catchRate: 0,
    rarity: Rarity.COMMON,
};
const hyperHealthPotionData = {
    id: 3,
    name: "Hyper Potion",
    desc: "Give 200 HP",
    behaviour: ItemBehaviour.HEAL,
    gain_amount: 200,
    status: Status.NONE,
    catchRate: 0,
    rarity: Rarity.COMMON,
};
const manaPotionData = {
    id: 4,
    name: "Elixir",
    desc: "Give 10 Mana",
    behaviour: ItemBehaviour.MANA,
    gain_amount: 10,
    status: Status.NONE,
    catchRate: 0,
    rarity: Rarity.COMMON,
};
const superManaPotionData = {
    id: 5,
    name: "Super Elixir",
    desc: "Give 25 Mana",
    behaviour: ItemBehaviour.MANA,
    gain_amount: 25,
    status: Status.NONE,
    catchRate: 0,
    rarity: Rarity.COMMON,
};
const hyperManaPotionData = {
    id: 6,
    name: "Hyper Elixir",
    desc: "Give 100 Mana",
    behaviour: ItemBehaviour.MANA,
    gain_amount: 100,
    status: Status.NONE,
    catchRate: 0,
    rarity: Rarity.COMMON,
};
const blastTrapData = {
    id: 7,
    name: "BlastTrap",
    desc: "Catch with bonus 1",
    behaviour: ItemBehaviour.CATCH,
    gain_amount: 0,
    status: Status.NONE,
    catchRate: 1,
    rarity: Rarity.COMMON,
};
const superBlastTrapData = {
    id: 8,
    name: "Super BlastTrap",
    desc: "Catch with bonus 1.5",
    behaviour: ItemBehaviour.CATCH,
    gain_amount: 0,
    status: Status.NONE,
    catchRate: 1.5,
    rarity: Rarity.COMMON,
};
const hyperBlastTrapData = {
    id: 9,
    name: "Hyper BlastTrap",
    desc: "Catch with bonus 2",
    behaviour: ItemBehaviour.CATCH,
    gain_amount: 0,
    status: Status.NONE,
    catchRate: 2,
    rarity: Rarity.COMMON,
};
const itemPedia = [
    healthPotionData,
    superHealthPotionData,
    hyperHealthPotionData,
    manaPotionData,
    superManaPotionData,
    hyperManaPotionData,
    blastTrapData,
    superBlastTrapData,
    hyperBlastTrapData,
];
function getItemDataById(id) {
    const item = itemPedia.find((item) => item.id === id);
    if (!item) {
        throw new Error(`No Item found with ID: ${id}`);
    }
    return item;
}
const rpcLoadItemPedia = function (ctx, logger, nk) {
    return JSON.stringify(itemPedia);
};
function getRandomItem(amount) {
    const randomIndex = Math.floor(Math.random() * itemPedia.length);
    let newItem = {
        data_id: itemPedia[randomIndex].id,
        amount: amount,
    };
    return newItem;
}
const MinIV = 1;
const MaxIV = 31;
// BlastData
// Définition des Pokémon (monstres) avec leurs mouvements mis à jour
const Florax = {
    id: 1,
    name: "Florax",
    desc: "A small plant.",
    type: Type.GRASS,
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
const Florabloom = {
    id: 2,
    name: "Florabloom",
    desc: "A plant in full bloom.",
    type: Type.GRASS,
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
const Floramajest = {
    id: 3,
    name: "Floramajest",
    desc: "A majestic plant.",
    type: Type.GRASS,
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
const Pyrex = {
    id: 4,
    name: "Pyrex",
    desc: "A small flame.",
    type: Type.FIRE,
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
const Pyroclaw = {
    id: 5,
    name: "Pyroclaw",
    desc: "A fiery that is fierce in battle.",
    type: Type.FIRE,
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
const Pyrowyvern = {
    id: 6,
    name: "Pyrowyvern",
    desc: "A magnificent fire that can fly.",
    type: Type.FIRE,
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
const Aquaflare = {
    id: 7,
    name: "Aquaflare",
    desc: "A small water.",
    type: Type.WATER,
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
const Aquablast = {
    id: 8,
    name: "Aquablast",
    desc: "A water that has a strong shell.",
    type: Type.WATER,
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
const Aqualith = {
    id: 9,
    name: "Aqualith",
    desc: "A powerful water with cannons.",
    type: Type.WATER,
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
const Zephyrex = {
    id: 10,
    name: "Zephyrex",
    desc: "A small bird that can fly.",
    type: Type.NORMAL,
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
const Zephyrwing = {
    id: 11,
    name: "Zephyrwing",
    desc: "A powerful bird known for its sharp beak.",
    type: Type.NORMAL,
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
const Gnawbit = {
    id: 12,
    name: "Gnawbit",
    desc: "A small, purple rodent.",
    type: Type.NORMAL,
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
const Gnawfang = {
    id: 13,
    name: "Gnawfang",
    desc: "A strong and aggressive rodent.",
    type: Type.NORMAL,
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
const Electrix = {
    id: 14,
    name: "Electrix",
    desc: "A small, electric known for its cute appearance.",
    type: Type.ELECTRIC,
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
const blastPedia = [
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
function getBlastDataById(id) {
    const blast = blastPedia.find((blast) => blast.id === id);
    if (!blast) {
        throw new Error(`No Blast found with ID: ${id}`);
    }
    return blast;
}
const rpcLoadBlastPedia = function (ctkx, logger, nk) {
    return JSON.stringify(blastPedia);
};
const DeckPermissionRead = 2;
const DeckPermissionWrite = 0;
const DeckCollectionName = 'blasts_collection';
const DeckCollectionKey = 'user_blasts';
function getRandomIV(min = MinIV, max = MaxIV) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}
function generateUUID() {
    let uuid = '', i, random;
    for (i = 0; i < 36; i++) {
        if (i === 8 || i === 13 || i === 18 || i === 23) {
            uuid += '-';
        }
        else if (i === 14) {
            uuid += '4'; // Le 14e caractère est toujours "4" pour un UUID v4
        }
        else {
            random = Math.random() * 16 | 0;
            if (i === 19) {
                uuid += (random & 0x3 | 0x8).toString(16); // Le 19e caractère est limité à 8, 9, A, ou B
            }
            else {
                uuid += random.toString(16);
            }
        }
    }
    return uuid;
}
const DefaultDeckBlasts = [
    (() => {
        const iv = getRandomIV(10, MaxIV);
        return {
            uuid: generateUUID(),
            data_id: Florax.id,
            exp: calculateExperienceFromLevel(5),
            iv: iv,
            hp: calculateBlastHp(Florax.hp, iv, 5),
            maxHp: calculateBlastHp(Florax.hp, iv, 5),
            mana: calculateBlastMana(Florax.mana, iv, 5),
            maxMana: calculateBlastMana(Florax.mana, iv, 5),
            attack: calculateBlastStat(Florax.attack, iv, 5),
            defense: calculateBlastStat(Florax.defense, iv, 5),
            speed: calculateBlastStat(Florax.speed, iv, 5),
            status: Status.NONE,
            activeMoveset: getRandomActiveMoveset(Florax, calculateExperienceFromLevel(5))
        };
    })(),
    (() => {
        const iv = getRandomIV(10, MaxIV);
        return {
            uuid: generateUUID(),
            data_id: Pyrex.id,
            exp: calculateExperienceFromLevel(5),
            iv: iv,
            hp: calculateBlastHp(Pyrex.hp, iv, 5),
            maxHp: calculateBlastHp(Pyrex.hp, iv, 5),
            mana: calculateBlastMana(Pyrex.mana, iv, 5),
            maxMana: calculateBlastMana(Pyrex.mana, iv, 5),
            attack: calculateBlastStat(Pyrex.attack, iv, 5),
            defense: calculateBlastStat(Pyrex.defense, iv, 5),
            speed: calculateBlastStat(Pyrex.speed, iv, 5),
            status: Status.NONE,
            activeMoveset: getRandomActiveMoveset(Pyrex, calculateExperienceFromLevel(5))
        };
    })(),
    (() => {
        const iv = getRandomIV(10, MaxIV);
        return {
            uuid: generateUUID(),
            data_id: Aquaflare.id,
            exp: calculateExperienceFromLevel(5),
            iv: iv,
            hp: calculateBlastHp(Aquaflare.hp, iv, 5),
            maxHp: calculateBlastHp(Aquaflare.hp, iv, 5),
            mana: calculateBlastMana(Aquaflare.mana, iv, 5),
            maxMana: calculateBlastMana(Aquaflare.mana, iv, 5),
            attack: calculateBlastStat(Aquaflare.attack, iv, 5),
            defense: calculateBlastStat(Aquaflare.defense, iv, 5),
            speed: calculateBlastStat(Aquaflare.speed, iv, 5),
            status: Status.NONE,
            activeMoveset: getRandomActiveMoveset(Aquaflare, calculateExperienceFromLevel(5))
        };
    })(),
];
const rpcLoadUserBlast = function (ctx, logger, nk, payload) {
    return JSON.stringify(loadUserBlast(nk, logger, ctx.userId));
};
function loadUserBlast(nk, logger, userId) {
    let storageReadReq = {
        key: DeckCollectionKey,
        collection: DeckCollectionName,
        userId: userId,
    };
    let objects;
    try {
        objects = nk.storageRead([storageReadReq]);
    }
    catch (error) {
        logger.error('storageRead error: %s', error);
        throw error;
    }
    if (objects.length === 0) {
        throw Error('user cards storage object not found');
    }
    let BlastCollection = objects[0].value;
    return BlastCollection;
}
function storeUserBlasts(nk, logger, userId, cards) {
    try {
        nk.storageWrite([
            {
                key: DeckCollectionKey,
                collection: DeckCollectionName,
                userId: userId,
                value: cards,
                permissionRead: DeckPermissionRead,
                permissionWrite: DeckPermissionWrite,
            }
        ]);
    }
    catch (error) {
        logger.error('storageWrite error: %s', error);
        throw error;
    }
}
function defaultBlastCollection(nk, logger, userId) {
    let cards = {
        deckBlasts: DefaultDeckBlasts,
        storedBlasts: DefaultDeckBlasts,
    };
    storeUserBlasts(nk, logger, userId, cards);
    return {
        deckBlasts: DefaultDeckBlasts,
        storedBlasts: DefaultDeckBlasts,
    };
}
