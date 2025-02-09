"use strict";
let InitModule = function (ctx, logger, nk, initializer) {
    // Set up hooks.
    initializer.registerAfterAuthenticateDevice(afterAuthenticate);
    initializer.registerAfterAuthenticateEmail(afterAuthenticate);
    // Blast
    initializer.registerRpc('loadUserBlast', rpcLoadUserBlast);
    initializer.registerRpc('swapDeckBlast', rpcSwapDeckBlast);
    initializer.registerRpc('evolveBlast', rpcUpgradeBlast);
    initializer.registerRpc('swapMove', rpcSwapBlastMove);
    // Bag
    initializer.registerRpc('loadUserItem', rpcLoadUserItems);
    initializer.registerRpc('swapDeckItem', rpcSwapDeckItem);
    // Leaderboard
    initializer.registerRpc('getAroundLeaderboard', rpcGetAroundTrophyLeaderboard);
    // Store
    initializer.registerRpc('loadBlastTrapOffer', rpcLoadBlastTrapOffer);
    initializer.registerRpc('buyTrapOffer', rpcBuyTrapOffer);
    initializer.registerRpc('loadGemOffer', rpcLoadGemsOffer);
    initializer.registerRpc('buyGemOffer', rpcBuyGemOffer);
    initializer.registerRpc('loadCoinOffer', rpcLoadCoinsOffer);
    initializer.registerRpc('buyCoinOffer', rpcBuyCoinOffer);
    initializer.registerRpc('canClaimDailyShop', rpcCanClaimDailyShop);
    initializer.registerRpc('claimDailyShop', rpcGetDailyShopOffer);
    initializer.registerRpc('buyDailyShopOffer', rpcBuyDailyShopOffer);
    // Others
    initializer.registerRpc('loadBlastPedia', rpcLoadBlastPedia);
    initializer.registerRpc('loadItemPedia', rpcLoadItemPedia);
    initializer.registerRpc('loadMovePedia', rpcLoadMovePedia);
    initializer.registerRpc('loadAllArea', rpcLoadAllArea);
    // Wild Battle
    initializer.registerRpc('findWildBattle', rpcFindOrCreateWildBattle);
    initializer.registerMatch('wildBattle', {
        matchInit,
        matchJoinAttempt,
        matchJoin,
        matchLeave,
        matchLoop,
        matchSignal,
        matchTerminate
    });
    initializer.registerRpc('deleteAccount', rpcDeleteAccount);
    createTrophyLeaderboard(nk, logger, ctx);
    createBlastDefeatedLeaderboard(nk, logger, ctx);
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
    const writeItems = {
        collection: BagCollectionName,
        key: BagCollectionKey,
        permissionRead: BagPermissionRead,
        permissionWrite: BagPermissionWrite,
        value: defaultItemsCollection(nk, logger, ctx.userId),
        userId: ctx.userId,
    };
    try {
        nk.storageWrite([writeItems]);
    }
    catch (error) {
        logger.error('storageWrite error: %q', error);
        throw error;
    }
    writeRecordTrophyLeaderboard(nk, logger, ctx);
    writeRecordBlastDefeatedLeaderboard(nk, logger, ctx, 0);
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
function calculateExperienceGain(expYield, enemyLevel, yourLevel) {
    const experience = Math.floor(((expYield * enemyLevel / 7) * ((2 * enemyLevel + 10) / (enemyLevel + yourLevel + 10)) + 1));
    return experience;
}
function getRandomActiveMoveset(blastData, exp) {
    const availableMoves = blastData.movepool
        .filter(m => calculateLevelFromExperience(exp) >= m.levelMin)
        .map(m => m.move_id);
    const shuffledMoves = shuffleArray(availableMoves);
    const randomMoveset = shuffledMoves.slice(0, 4);
    return randomMoveset;
}
//#region Battle
function calculateDamage(attackerLevel, attackerAttack, defenderDefense, attackerType, defenderType, movePower) {
    const damage = ((2 * attackerLevel / 5 + 2) * movePower * getTypeMultiplier(attackerType, defenderType) * (attackerAttack / defenderDefense) / 50) + 1;
    return Math.floor(damage);
}
function getTypeMultiplier(moveType, defenderType) {
    switch (moveType) {
        case TYPE.FIRE:
            switch (defenderType) {
                case TYPE.GRASS:
                    return 2;
                case TYPE.WATER:
                    return 0.5;
                default:
                    return 1;
            }
        case Type.WATER:
            switch (defenderType) {
                case Type.FIRE:
                    return 2;
                case Type.GRASS:
                    return 0.5;
                default:
                    return 1;
            }
        case TYPE.GRASS:
            switch (defenderType) {
                case TYPE.WATER:
                    return 2;
                case TYPE.FIRE:
                    return 0.5;
                default:
                    return 1;
            }
        case TYPE.NORMAL:
            switch (defenderType) {
                case TYPE.LIGHT:
                    return 0.5;
                case TYPE.DARK:
                    return 0.5;
                default:
                    return 1;
            }
        case TYPE.GROUND:
            switch (defenderType) {
                case TYPE.ELECTRIC:
                    return 2;
                case TYPE.FLY:
                    return 0;
                default:
                    return 1;
            }
        case TYPE.FLY:
            switch (defenderType) {
                case TYPE.ELECTRIC:
                    return 0;
                case TYPE.GROUND:
                    return 2;
                default:
                    return 1;
            }
        case TYPE.ELECTRIC:
            switch (defenderType) {
                case TYPE.GROUND:
                    return 0;
                case TYPE.FLY:
                    return 2;
                default:
                    return 1;
            }
        case TYPE.LIGHT:
            switch (defenderType) {
                case TYPE.DARK:
                    return 2;
                case TYPE.NORMAL:
                    return 2;
                case TYPE.LIGHT:
                    return 0.5;
                default:
                    return 1;
            }
        case TYPE.DARK:
            switch (defenderType) {
                case TYPE.LIGHT:
                    return 2;
                case TYPE.NORMAL:
                    return 2;
                case TYPE.DARK:
                    return 0.5;
                default:
                    return 1;
            }
        default:
            return 1;
    }
}
function calculateStaminaRecovery(maxStamina, currentStamina, useWait = false) {
    const normalRecovery = maxStamina * 0.2;
    const waitRecovery = maxStamina * 0.5;
    let recoveredStamina = currentStamina + (useWait ? waitRecovery : normalRecovery);
    if (recoveredStamina > maxStamina) {
        recoveredStamina = maxStamina;
    }
    return Math.floor(recoveredStamina);
}
function getFasterBlast(blast1, blast2) {
    if (blast1.speed > blast2.speed) {
        return true;
    }
    else {
        return false;
    }
}
function isAllBlastDead(allPlayerBlasts) {
    return allPlayerBlasts.every((blast) => blast.hp === 0);
}
function isBlastAlive(blast) {
    return blast.hp > 0;
}
function addExpOnBlastInGame(nk, logger, playerId, currentPlayerBlast, enemyBlast) {
    let expToAdd = calculateExperienceGain(getBlastDataById(currentPlayerBlast.data_id).expYield, calculateLevelFromExperience(enemyBlast.exp), calculateLevelFromExperience(currentPlayerBlast.exp));
    addExpOnBlast(nk, logger, playerId, currentPlayerBlast.uuid, expToAdd);
}
function healHealthBlast(blast, amount) {
    blast.hp += amount;
    if (blast.hp > blast.maxHp)
        blast.hp = blast.maxHp;
    return blast;
}
function healManaBlast(blast, amount) {
    blast.mana += amount;
    if (blast.mana > blast.maxMana)
        blast.mana = blast.maxMana;
    return blast;
}
function calculateCaptureProbability(currentHP, maxHP, catchRate, temCardBonus, statusBonus) {
    const hpFactor = (3 * maxHP - 2 * currentHP) / (3 * maxHP);
    const baseProbability = catchRate * hpFactor * temCardBonus * statusBonus;
    const captureProbability = Math.min(Math.max(baseProbability, 0), 1);
    return captureProbability;
}
function isBlastCaptured(currentHP, maxHP, catchRate, temCardBonus, statusBonus) {
    const captureProbability = calculateCaptureProbability(currentHP, maxHP, catchRate, temCardBonus, statusBonus) * 100;
    const randomValue = Math.random() * 100;
    return randomValue <= captureProbability;
}
//#endregion
function clamp(value, min, max) {
    return Math.min(Math.max(value, min), max);
}
function getRandomNumber(min, max) {
    const minCeiled = Math.ceil(min);
    const maxFloored = Math.floor(max);
    return Math.floor(Math.random() * (maxFloored - minCeiled + 1)) + minCeiled;
}
function randomElement(array) {
    const randomIndex = Math.floor(Math.random() * array.length);
    return array[randomIndex];
}
function shuffleArray(array) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
}
function msecToSec(n) {
    return Math.floor(n / 1000);
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
function updateWalletWithCurrency(nk, userId, currencyKeyName, amount) {
    const changeset = {
        [currencyKeyName]: amount,
    };
    let result = nk.walletUpdate(userId, changeset);
    return result;
}
function getCurrencyInWallet(nk, userId, currencyKeyName) {
    var amountToReturn = 0;
    try {
        let results = nk.walletLedgerList(userId);
        switch (currencyKeyName) {
            case Currency.Coins:
                amountToReturn = results.items[0].changeset[Currency.Coins];
                break;
            case Currency.Gems:
                amountToReturn = results.items[0].changeset[Currency.Gems];
                break;
            case Currency.Trophies:
                amountToReturn = results.items[0].changeset[Currency.Trophies];
                break;
        }
        return amountToReturn;
    }
    catch (error) {
        // Handle error
    }
    return amountToReturn;
}
var TYPE;
(function (TYPE) {
    TYPE[TYPE["NORMAL"] = 0] = "NORMAL";
    TYPE[TYPE["FIRE"] = 1] = "FIRE";
    TYPE[TYPE["WATER"] = 2] = "WATER";
    TYPE[TYPE["GRASS"] = 3] = "GRASS";
    TYPE[TYPE["GROUND"] = 4] = "GROUND";
    TYPE[TYPE["FLY"] = 5] = "FLY";
    TYPE[TYPE["ELECTRIC"] = 6] = "ELECTRIC";
    TYPE[TYPE["LIGHT"] = 7] = "LIGHT";
    TYPE[TYPE["DARK"] = 8] = "DARK";
})(TYPE || (TYPE = {}));
const Tackle = {
    id: 1,
    name: "Tackle",
    desc: "A basic physical attack that uses the user's body.",
    type: TYPE.NORMAL,
    power: 200,
    cost: 7,
};
const Punch = {
    id: 2,
    name: "Punch",
    desc: "A strong punch aimed at the opponent.",
    type: TYPE.NORMAL,
    power: 50,
    cost: 15,
};
const Stomp = {
    id: 3,
    name: "Stomp",
    desc: "A powerful attack that stomps down on the opponent.",
    type: TYPE.NORMAL,
    power: 65,
    cost: 25,
};
const Slam = {
    id: 4,
    name: "Slam",
    desc: "A hard slam that causes significant damage.",
    type: TYPE.NORMAL,
    power: 80,
    cost: 30,
};
const Growl = {
    id: 5,
    name: "Growl",
    desc: "A menacing growl that lowers the target's attack.",
    type: TYPE.NORMAL,
    power: 0,
    cost: 3,
};
const Harden = {
    id: 6,
    name: "Harden",
    desc: "Increases the user's defense by hardening their body.",
    type: TYPE.NORMAL,
    power: 0,
    cost: 4,
};
const Ember = {
    id: 7,
    name: "Ember",
    desc: "A small flame attack that may cause a burn.",
    type: TYPE.FIRE,
    power: 60,
    cost: 12,
};
const FirePunch = {
    id: 8,
    name: "Fire Punch",
    desc: "A punch imbued with fire that burns the target.",
    type: TYPE.FIRE,
    power: 75,
    cost: 15,
};
const Flamethrower = {
    id: 9,
    name: "Flamethrower",
    desc: "A stream of fire that engulfs the target.",
    type: TYPE.FIRE,
    power: 90,
    cost: 30,
};
const FireBlast = {
    id: 10,
    name: "Fire Blast",
    desc: "A powerful fire attack that can leave the target burned.",
    type: TYPE.FIRE,
    power: 110,
    cost: 40,
};
const Bubble = {
    id: 11,
    name: "Bubble",
    desc: "A stream of bubbles that can trap the opponent.",
    type: TYPE.WATER,
    power: 50,
    cost: 5,
};
const BubbleBeam = {
    id: 12,
    name: "Bubble Beam",
    desc: "A beam of bubbles that strikes the target with pressure.",
    type: TYPE.WATER,
    power: 65,
    cost: 15,
};
const Waterfall = {
    id: 13,
    name: "Waterfall",
    desc: "A powerful water attack that crashes down on the target.",
    type: TYPE.WATER,
    power: 80,
    cost: 25,
};
const HydroPump = {
    id: 14,
    name: "Hydro Pump",
    desc: "A massive blast of water that delivers high damage.",
    type: TYPE.WATER,
    power: 110,
    cost: 40,
};
const VineWhip = {
    id: 15,
    name: "Vine Whip",
    desc: "Attacks the opponent with flexible vines.",
    type: TYPE.GRASS,
    power: 50,
    cost: 7,
};
const RazorLeaf = {
    id: 16,
    name: "Razor Leaf",
    desc: "Sharp leaves that are fired at the target.",
    type: TYPE.GRASS,
    power: 75,
    cost: 15,
};
const SolarBeam = {
    id: 17,
    name: "Solar Beam",
    desc: "A powerful beam of solar energy that requires a turn to charge.",
    type: TYPE.GRASS,
    power: 120,
    cost: 50,
};
const QuickAttack = {
    id: 18,
    name: "Quick Attack",
    desc: "A swift attack that always strikes first.",
    type: TYPE.NORMAL,
    power: 40,
    cost: 5,
};
const Gust = {
    id: 19,
    name: "Gust",
    desc: "A blast of wind that is effective against bug types.",
    type: TYPE.FLY,
    power: 40,
    cost: 10,
};
const HyperFang = {
    id: 20,
    name: "Hyper Fang",
    desc: "A sharp bite that deals high damage.",
    type: TYPE.NORMAL,
    power: 80,
    cost: 15,
};
const ThunderShock = {
    id: 21,
    name: "Thunder Shock",
    desc: "An electric shock that may paralyze the target.",
    type: TYPE.ELECTRIC,
    power: 40,
    cost: 5,
};
const ElectroBall = {
    id: 22,
    name: "Electro Ball",
    desc: "A ball of electricity that grows stronger with speed.",
    type: TYPE.ELECTRIC,
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
function getDeckItem(nk, logger, userId) {
    let userCards;
    userCards = loadUserItems(nk, logger, userId);
    return userCards.deckItems;
}
const MinIV = 1;
const MaxIV = 31;
// BlastData
// Définition des Pokémon (monstres) avec leurs mouvements mis à jour
const Florax = {
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
const Florabloom = {
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
const Floramajest = {
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
const Pyrex = {
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
const Pyroclaw = {
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
const Pyrowyvern = {
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
const Aquaflare = {
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
const Aquablast = {
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
const Aqualith = {
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
const Zephyrex = {
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
const Zephyrwing = {
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
const Gnawbit = {
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
const Gnawfang = {
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
const Electrix = {
    id: 14,
    name: "Electrix",
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
const thePlains = {
    id: 0,
    name: "The Plains",
    trophyRequired: 0,
    blastIds: [Zephyrex.id, Gnawbit.id],
    blastLevels: [2, 5]
};
const theWildForests = {
    id: 1,
    name: "The Wild Forests",
    trophyRequired: 300,
    blastIds: [Gnawbit.id, Zephyrwing.id, Electrix.id],
    blastLevels: [6, 10]
};
const theDarkCaves = {
    id: 2,
    name: "The Dark Caves",
    trophyRequired: 600,
    blastIds: [Florax.id, Pyrex.id, Aquaflare.id],
    blastLevels: [11, 20]
};
const allArea = [
    theDarkCaves,
    theWildForests,
    thePlains,
];
const rpcLoadAllArea = function () {
    return JSON.stringify(allArea);
};
function getRandomBlastInPlayerArea(userId, logger, nk) {
    let metadata = nk.accountGetId(userId).user.metadata;
    let randomBlastId = getRandomBlastIdInPlayerAreaIdInArea(metadata.area);
    let blastData = getBlastDataById(randomBlastId);
    let randomblastLevels = calculateExperienceFromLevel(getRandomLevelInArea(metadata.area));
    let randomIv = getRandomNumber(MinIV, MaxIV);
    let newBlast = {
        uuid: nk.uuidv4(),
        data_id: randomBlastId,
        exp: randomblastLevels,
        iv: randomIv,
        hp: calculateBlastHp(blastData.hp, randomIv, calculateLevelFromExperience(randomblastLevels)),
        maxHp: calculateBlastHp(blastData.hp, randomIv, calculateLevelFromExperience(randomblastLevels)),
        mana: calculateBlastMana(blastData.mana, randomIv, calculateLevelFromExperience(randomblastLevels)),
        maxMana: calculateBlastMana(blastData.mana, randomIv, calculateLevelFromExperience(randomblastLevels)),
        attack: calculateBlastStat(blastData.attack, randomIv, calculateLevelFromExperience(randomblastLevels)),
        defense: calculateBlastStat(blastData.defense, randomIv, calculateLevelFromExperience(randomblastLevels)),
        speed: calculateBlastStat(blastData.speed, randomIv, calculateLevelFromExperience(randomblastLevels)),
        status: Status.NONE,
        activeMoveset: getRandomActiveMoveset(blastData, randomblastLevels)
    };
    logger.debug('user %s successfully get a random blast', userId);
    return newBlast;
}
function getRandomBlastInAllPlayerArea(userId, nk) {
    let metadata = nk.accountGetId(userId).user.metadata;
    let randomBlastId = getRandomBlastIdInPlayerAreaWithTrophy(getCurrencyInWallet(nk, userId, Currency.Trophies));
    let randomData = getBlastDataById(randomBlastId);
    let randomblastLevels = getRandomLevelInArea(metadata.area);
    let randomIv = getRandomNumber(MinIV, MaxIV);
    let newBlast = {
        uuid: nk.uuidv4(),
        data_id: randomBlastId,
        exp: calculateExperienceFromLevel(getRandomLevelInArea(metadata.area)),
        iv: randomIv,
        hp: calculateBlastHp(randomData.hp, randomIv, randomblastLevels),
        maxHp: calculateBlastHp(randomData.hp, randomIv, randomblastLevels),
        mana: calculateBlastMana(randomData.mana, randomIv, randomblastLevels),
        maxMana: calculateBlastMana(randomData.mana, randomIv, randomblastLevels),
        attack: calculateBlastStat(randomData.attack, randomIv, randomblastLevels),
        defense: calculateBlastStat(randomData.defense, randomIv, randomblastLevels),
        speed: calculateBlastStat(randomData.speed, randomIv, randomblastLevels),
        status: Status.NONE,
        activeMoveset: getRandomActiveMoveset(randomData, calculateExperienceFromLevel(getRandomLevelInArea(metadata.area)))
    };
    return newBlast;
}
function getRandomBlastIdInPlayerAreaIdInArea(id) {
    const area = allArea.find((area) => area.id === id);
    if (area && area.blastIds.length > 0) {
        const randomIndex = Math.floor(Math.random() * area.blastIds.length);
        return area.blastIds[randomIndex];
    }
    return 0;
}
function getRandomBlastIdInPlayerAreaWithTrophy(amountOfTrophy) {
    const allAreaUnderTrophy = getAllAreaUnderTrophy(amountOfTrophy);
    const randomAreaIndex = Math.floor(Math.random() * (allAreaUnderTrophy.length - 1));
    const randomBlastId = getRandomBlastIdInPlayerAreaIdInArea(allAreaUnderTrophy[randomAreaIndex].id);
    return randomBlastId;
}
function getAllAreaUnderTrophy(amountOfTrophy) {
    const areaUnderTrophy = [];
    for (const aire of allArea) {
        if (aire.trophyRequired <= amountOfTrophy) {
            areaUnderTrophy.push(aire);
        }
    }
    return areaUnderTrophy;
}
function getRandomLevelInArea(id) {
    const area = allArea.find((area) => area.id === id);
    if (area) {
        const [minLevel, maxLevel] = area.blastLevels;
        const randomLevel = getRandomNumber(minLevel, maxLevel);
        return randomLevel;
    }
    return 0;
}
var OfferType;
(function (OfferType) {
    OfferType[OfferType["COINS"] = 0] = "COINS";
    OfferType[OfferType["GEMS"] = 1] = "GEMS";
    OfferType[OfferType["BLAST"] = 2] = "BLAST";
    OfferType[OfferType["ITEM"] = 3] = "ITEM";
})(OfferType || (OfferType = {}));
//#region BlastTrap Offer
const blastTrap1 = {
    data_id: blastTrapData.id,
    amount: 1,
};
const blastTrapOffer1 = {
    name: "BlastTrap",
    desc: "X1",
    type: OfferType.ITEM,
    currency: Currency.Coins,
    coinsAmount: 0,
    gemsAmount: 0,
    blast: null,
    item: blastTrap1,
    price: 100,
    isAlreadyBuyed: false,
};
const blastTrap5 = {
    data_id: blastTrapData.id,
    amount: 5,
};
const blastTrapOffer2 = {
    name: "BlastTrap",
    desc: "X5",
    type: OfferType.ITEM,
    currency: Currency.Coins,
    coinsAmount: 0,
    gemsAmount: 0,
    blast: null,
    item: blastTrap5,
    price: 450,
    isAlreadyBuyed: false,
};
const blastTrap20 = {
    data_id: blastTrapData.id,
    amount: 5,
};
const blastTrapOffer3 = {
    name: "BlastTrap",
    desc: "X5",
    type: OfferType.ITEM,
    currency: Currency.Coins,
    coinsAmount: 0,
    gemsAmount: 0,
    blast: null,
    item: blastTrap20,
    price: 1900,
    isAlreadyBuyed: false,
};
const blastTrapOffer = [
    blastTrapOffer1,
    blastTrapOffer2,
    blastTrapOffer3,
];
const rpcLoadBlastTrapOffer = function (ctx, logger, nk) {
    return JSON.stringify(blastTrapOffer);
};
const rpcBuyTrapOffer = function (ctx, logger, nk, payload) {
    var indexOffer = JSON.parse(payload);
    var storeOffer = blastTrapOffer[indexOffer];
    try {
        nk.walletUpdate(ctx.userId, { [storeOffer.currency]: -storeOffer.price });
    }
    catch (error) {
        logger.error('error buying blast trap: %s', error);
        throw error;
    }
    addItem(nk, logger, ctx, storeOffer.item);
    // return playerWallet and Wallets
};
//#endregion
//#region Coins Offer
const coinsOffer1 = {
    name: "Coins",
    desc: "20000",
    type: OfferType.ITEM,
    currency: Currency.Gems,
    coinsAmount: 20000,
    gemsAmount: 0,
    blast: null,
    item: null,
    price: 100,
    isAlreadyBuyed: false,
};
const coinsOffer2 = {
    name: "Coins",
    desc: "60000",
    type: OfferType.ITEM,
    currency: Currency.Gems,
    coinsAmount: 65000,
    gemsAmount: 0,
    blast: null,
    item: null,
    price: 300,
    isAlreadyBuyed: false,
};
const coinsOffer3 = {
    name: "Coins",
    desc: "140000",
    type: OfferType.ITEM,
    currency: Currency.Gems,
    coinsAmount: 140000,
    gemsAmount: 0,
    blast: null,
    item: null,
    price: 600,
    isAlreadyBuyed: false,
};
const coinsOffer = [
    coinsOffer1,
    coinsOffer2,
    coinsOffer3,
];
const rpcLoadCoinsOffer = function (ctx, logger, nk) {
    return JSON.stringify(coinsOffer);
};
const rpcBuyCoinOffer = function (ctx, logger, nk, payload) {
    var indexOffer = JSON.parse(payload);
    var storeOffer = coinsOffer[indexOffer];
    try {
        nk.walletUpdate(ctx.userId, { [storeOffer.currency]: -storeOffer.price });
        nk.walletUpdate(ctx.userId, { [Currency.Coins]: storeOffer.coinsAmount });
    }
    catch (error) {
        logger.error('error buying blast trap: %s', error);
        throw error;
    }
};
//#endregion
//#region Coins Offer
const gemsOffer1 = {
    name: "Gems",
    desc: "100",
    type: OfferType.GEMS,
    currency: Currency.Hard,
    coinsAmount: 0,
    gemsAmount: 100,
    blast: null,
    item: null,
    price: 0,
    isAlreadyBuyed: false,
};
const gemsOffer2 = {
    name: "Gems",
    desc: "200",
    type: OfferType.GEMS,
    currency: Currency.Hard,
    coinsAmount: 0,
    gemsAmount: 200,
    blast: null,
    item: null,
    price: 0,
    isAlreadyBuyed: false,
};
const gemsOffer3 = {
    name: "Gems",
    desc: "300",
    type: OfferType.GEMS,
    currency: Currency.Hard,
    coinsAmount: 0,
    gemsAmount: 300,
    blast: null,
    item: null,
    price: 0,
    isAlreadyBuyed: false,
};
const gemsOffer4 = {
    name: "Gems",
    desc: "400",
    type: OfferType.GEMS,
    currency: Currency.Hard,
    coinsAmount: 0,
    gemsAmount: 400,
    blast: null,
    item: null,
    price: 0,
    isAlreadyBuyed: false,
};
const gemsOffer5 = {
    name: "Gems",
    desc: "500",
    type: OfferType.GEMS,
    currency: Currency.Hard,
    coinsAmount: 0,
    gemsAmount: 500,
    blast: null,
    item: null,
    price: 0,
    isAlreadyBuyed: false,
};
const gemsOffer6 = {
    name: "Gems",
    desc: "600",
    type: OfferType.GEMS,
    currency: Currency.Hard,
    coinsAmount: 0,
    gemsAmount: 600,
    blast: null,
    item: null,
    price: 0,
    isAlreadyBuyed: false,
};
const gemsOffer = [
    gemsOffer1,
    gemsOffer2,
    gemsOffer3,
    gemsOffer4,
    gemsOffer5,
    gemsOffer6,
];
const rpcLoadGemsOffer = function (ctx, logger, nk) {
    return JSON.stringify(gemsOffer);
};
const rpcBuyGemOffer = function (ctx, logger, nk, payload) {
    var indexOffer = JSON.parse(payload);
    var storeOffer = gemsOffer[indexOffer];
    try {
        // Verif
        // Achat in app
        nk.walletUpdate(ctx.userId, { [Currency.Gems]: storeOffer.gemsAmount });
    }
    catch (error) {
        logger.error('error buying blast trap: %s', error);
        throw error;
    }
};
//#endregion
const DailyShopPermissionRead = 2;
const DailyShopPermissionWrite = 0;
const DailyShopCollectionName = 'shop';
const DailyShopCollectionKey = 'daily';
function getLastDailyShopObject(context, logger, nk) {
    if (!context.userId) {
        throw Error('No user ID in context');
    }
    var objectId = {
        collection: DailyShopCollectionName,
        key: DailyShopCollectionKey,
        userId: context.userId,
    };
    var objects;
    try {
        objects = nk.storageRead([objectId]);
    }
    catch (error) {
        logger.error('storageRead error: %s', error);
        throw error;
    }
    var dailyShop = {
        lastClaimUnix: 0,
        lastDailyShop: generateRandomDailyShop(nk, context.userId),
    };
    objects.forEach(function (object) {
        if (object.key == DailyShopCollectionKey) {
            dailyShop = object.value;
        }
    });
    return dailyShop;
}
function canUserClaimDailyShop(dailyShop) {
    if (!dailyShop.lastClaimUnix) {
        dailyShop.lastClaimUnix = 0;
    }
    var d = new Date();
    d.setHours(0, 0, 0, 0);
    return dailyShop.lastClaimUnix < msecToSec(d.getTime());
}
function rpcCanClaimDailyShop(context, logger, nk, payload) {
    var dailyShop = getLastDailyShopObject(context, logger, nk);
    var response = {
        canClaimDailyShop: canUserClaimDailyShop(dailyShop),
        lastDailyShop: dailyShop.lastDailyShop,
    };
    var result = JSON.stringify(response);
    return result;
}
function rpcGetDailyShopOffer(context, logger, nk, payload) {
    var dailyShop = getLastDailyShopObject(context, logger, nk);
    if (canUserClaimDailyShop(dailyShop)) {
        var newShop = generateRandomDailyShop(nk, context.userId);
        dailyShop.lastClaimUnix = msecToSec(Date.now());
        dailyShop.lastDailyShop = newShop;
        var write = {
            collection: DailyShopCollectionName,
            key: DailyShopCollectionKey,
            permissionRead: DailyShopPermissionRead,
            permissionWrite: DailyShopPermissionWrite,
            value: dailyShop,
            userId: context.userId,
        };
        if (dailyShop.version) {
            // Use OCC to prevent concurrent writes.
            write.version = dailyShop.version;
        }
        // Update daily reward storage object for user.
        try {
            nk.storageWrite([write]);
        }
        catch (error) {
            logger.error('storageWrite error: %q', error);
            throw error;
        }
    }
    var result = JSON.stringify(dailyShop);
    return result;
}
function rpcBuyDailyShopOffer(context, logger, nk, payload) {
    var dailyShop = getLastDailyShopObject(context, logger, nk);
    var indexOffer = JSON.parse(payload);
    if (dailyShop.lastDailyShop[indexOffer].isAlreadyBuyed == true) {
        throw Error('Daily shop offer already buyed');
    }
    try {
        nk.walletUpdate(context.userId, { [dailyShop.lastDailyShop[indexOffer].currency]: -dailyShop.lastDailyShop[indexOffer].price });
    }
    catch (error) {
        logger.error('error buying blast trap: %s', error);
        throw error;
    }
    dailyShop.lastDailyShop[indexOffer].isAlreadyBuyed = true;
    var write = {
        collection: DailyShopCollectionName,
        key: DailyShopCollectionKey,
        permissionRead: DailyShopPermissionRead,
        permissionWrite: DailyShopPermissionWrite,
        value: dailyShop,
        userId: context.userId,
    };
    // Update daily reward storage object for user.
    try {
        nk.storageWrite([write]);
    }
    catch (error) {
        logger.error('storageWrite error: %q', error);
        throw error;
    }
    if (dailyShop.lastDailyShop[indexOffer].blast != null) {
        addBlast(nk, logger, context.userId, dailyShop.lastDailyShop[indexOffer].blast);
    }
    if (dailyShop.lastDailyShop[indexOffer].item != null) {
        addItem(nk, logger, context, dailyShop.lastDailyShop[indexOffer].item);
    }
    var result = JSON.stringify(dailyShop);
    logger.debug('Succefuly buy daily shop offer response: %q', result);
    return result;
}
function generateRandomDailyShop(nk, userId) {
    var dailyShop;
    dailyShop = [
        getRandomStoreOffer(nk, userId),
        getRandomStoreOffer(nk, userId),
        getRandomStoreOffer(nk, userId),
        getRandomStoreOffer(nk, userId),
        getRandomStoreOffer(nk, userId),
        getRandomStoreOffer(nk, userId),
    ];
    return dailyShop;
}
function getRandomOfferType() {
    const offerTypeValues = Object.values(OfferType);
    const randomIndex = Math.floor(Math.random() * offerTypeValues.length);
    return offerTypeValues[randomIndex];
}
function getRandomStoreOffer(nk, userId) {
    let offer = {
        name: "",
        desc: "",
        type: getRandomOfferType(),
        coinsAmount: 0,
        gemsAmount: 0,
        blast: null,
        item: null,
        price: 0,
        currency: Currency.Coins,
        isAlreadyBuyed: false,
    };
    if (Math.random() < 0.5) {
        offer.blast = getRandomBlastInAllPlayerArea(userId, nk);
        offer.price = getBlastPrice(offer.blast);
        offer.currency = Currency.Coins;
        offer.desc = "LVL." + calculateLevelFromExperience(offer.blast.exp).toString();
        offer.name = getBlastDataById(offer.blast.data_id).name;
    }
    else {
        offer.item = getRandomItem(5);
        var itemData = getItemDataById(offer.item.data_id);
        offer.price = getItemPrice(offer.item);
        offer.currency = Currency.Coins;
        offer.desc = offer.item.amount.toString();
        offer.name = itemData.name;
    }
    return offer;
}
function getBlastPrice(blast) {
    var coeffRarity = 1;
    switch (getBlastDataById(blast.data_id).rarity) {
        case Rarity.COMMON:
            coeffRarity = 1;
            break;
        case Rarity.UNCOMMON:
            coeffRarity = 1.5;
            break;
        case Rarity.RARE:
            coeffRarity = 2;
            break;
        case Rarity.EPIC:
            coeffRarity = 3;
            break;
        case Rarity.LEGENDARY:
            coeffRarity = 10;
            break;
        case Rarity.UNIQUE:
            coeffRarity = 5;
            break;
    }
    return Math.round(200 * coeffRarity * calculateLevelFromExperience(blast.exp));
}
function getItemPrice(item) {
    var coeffRarity = 1;
    var itemData = getItemDataById(item.data_id);
    switch (itemData.rarity) {
        case Rarity.COMMON:
            coeffRarity = 1;
            break;
        case Rarity.UNCOMMON:
            coeffRarity = 1.5;
            break;
        case Rarity.RARE:
            coeffRarity = 2;
            break;
        case Rarity.EPIC:
            coeffRarity = 3;
            break;
        case Rarity.LEGENDARY:
            coeffRarity = 10;
            break;
        case Rarity.UNIQUE:
            coeffRarity = 5;
            break;
    }
    return Math.round(100 * coeffRarity * item.amount);
}
const DailyRewardPermissionRead = 2;
const DailyRewardPermissionWrite = 0;
const DailyRewardCollectionName = 'reward';
const DailyRewardCollectionKey = 'daily';
function getLastDailyRewardObject(context, logger, nk, payload) {
    if (!context.userId) {
        throw Error('No user ID in context');
    }
    if (payload) {
        throw Error('No input allowed');
    }
    var objectId = {
        collection: DailyRewardCollectionName,
        key: DailyRewardCollectionKey,
        userId: context.userId,
    };
    var objects;
    try {
        objects = nk.storageRead([objectId]);
    }
    catch (error) {
        logger.error('storageRead error: %s', error);
        throw error;
    }
    var dailyReward = {
        lastClaimUnix: 0,
        totalDay: 0,
    };
    objects.forEach(function (object) {
        if (object.key == DailyRewardCollectionKey) {
            dailyReward = object.value;
        }
    });
    return dailyReward;
}
function canUserClaimDailyReward(dailyReward) {
    if (!dailyReward.lastClaimUnix) {
        dailyReward.lastClaimUnix = 0;
    }
    var d = new Date();
    d.setHours(0, 0, 0, 0);
    return dailyReward.lastClaimUnix < msecToSec(d.getTime());
}
function getTotalDayConnected(dailyReward) {
    if (!dailyReward.totalDay) {
        dailyReward.totalDay = 0;
    }
    return dailyReward.totalDay;
}
function rpcCanClaimDailyReward(context, logger, nk, payload) {
    var dailyReward = getLastDailyRewardObject(context, logger, nk, payload);
    var response = {
        canClaimDailyReward: canUserClaimDailyReward(dailyReward),
        totalDayConnected: dailyReward.totalDay,
    };
    var result = JSON.stringify(response);
    logger.debug('rpcCanClaimDailyReward response: %q', result);
    return result;
}
function rpcClaimDailyReward(context, logger, nk, payload) {
    var reward = {
        coinsReceived: 0,
        gemsReceived: 0,
        blastReceived: null,
        itemReceived: null,
    };
    var dailyReward = getLastDailyRewardObject(context, logger, nk, payload);
    if (canUserClaimDailyReward(dailyReward)) {
        var totalDay = getTotalDayConnected(dailyReward);
        reward = getDayReward(totalDay);
        var notification = {
            code: notificationOpCodes.CURENCY,
            content: reward,
            persistent: false,
            subject: "You've received a new item",
            userId: context.userId,
        };
        if (reward.coinsReceived != 0) {
            updateWalletWithCurrency(nk, context.userId, Currency.Coins, reward.coinsReceived);
            notification = {
                code: notificationOpCodes.CURENCY,
                content: reward,
                persistent: false,
                subject: "You've received a new currency",
                userId: context.userId,
            };
            try {
                nk.notificationsSend([notification]);
            }
            catch (error) {
                logger.error('notificationsSend error: %q', error);
                throw error;
            }
        }
        if (reward.gemsReceived != 0) {
            updateWalletWithCurrency(nk, context.userId, Currency.Gems, reward.gemsReceived);
            notification = {
                code: notificationOpCodes.CURENCY,
                content: reward,
                persistent: false,
                subject: "You've received a new currency",
                userId: context.userId,
            };
            try {
                nk.notificationsSend([notification]);
            }
            catch (error) {
                logger.error('notificationsSend error: %q', error);
                throw error;
            }
        }
        if (reward.blastReceived != null) {
            addBlast(nk, logger, context.userId, reward.blastReceived);
        }
        if (reward.itemReceived != null) {
            addItem(nk, logger, context, reward.itemReceived);
        }
        dailyReward.lastClaimUnix = msecToSec(Date.now());
        dailyReward.totalDay = totalDay + 1;
        var write = {
            collection: DailyRewardCollectionName,
            key: DailyRewardCollectionKey,
            permissionRead: DailyRewardPermissionRead,
            permissionWrite: DailyRewardPermissionWrite,
            value: dailyReward,
            userId: context.userId,
        };
        if (dailyReward.version) {
            // Use OCC to prevent concurrent writes.
            write.version = dailyReward.version;
        }
        // Update daily reward storage object for user.
        try {
            nk.storageWrite([write]);
        }
        catch (error) {
            logger.error('storageWrite error: %q', error);
            throw error;
        }
    }
    var result = JSON.stringify(reward);
    logger.debug('rpcClaimDailyReward response: %q', result);
    return result;
}
function getDayReward(totalDay) {
    var reward = {
        coinsReceived: 0,
        gemsReceived: 0,
        blastReceived: null,
        itemReceived: null,
    };
    switch (totalDay % 7) {
        case 0:
            reward = Reward0;
            break;
        case 1:
            reward = Reward1;
            break;
        case 2:
            reward = Reward2;
            break;
        case 3:
            reward = Reward3;
            break;
        case 4:
            reward = Reward4;
            break;
        case 5:
            reward = Reward5;
            break;
        case 6:
            reward = Reward6;
            break;
    }
    return reward;
}
// Data
const Reward0 = {
    coinsReceived: 500,
    gemsReceived: 0,
    blastReceived: null,
    itemReceived: null,
};
const Reward1 = {
    coinsReceived: 0,
    gemsReceived: 10,
    blastReceived: null,
    itemReceived: null,
};
const Reward2 = {
    coinsReceived: 1000,
    gemsReceived: 0,
    blastReceived: null,
    itemReceived: null,
};
const Reward3 = {
    coinsReceived: 0,
    gemsReceived: 15,
    blastReceived: null,
    itemReceived: null,
};
const Reward4 = {
    coinsReceived: 2000,
    gemsReceived: 0,
    blastReceived: null,
    itemReceived: null,
};
const Reward5 = {
    coinsReceived: 0,
    gemsReceived: 30,
    blastReceived: null,
    itemReceived: null,
};
const Reward6 = {
    coinsReceived: 5000,
    gemsReceived: 0,
    blastReceived: null,
    itemReceived: null,
};
const allReward = [
    Reward0,
    Reward1,
    Reward2,
    Reward3,
    Reward4,
    Reward5,
    Reward6,
];
const rpcLoadAllDailyReward = function () {
    return JSON.stringify(allReward);
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
const rpcSwapBlastMove = function (ctx, logger, nk, payload) {
    const request = JSON.parse(payload);
    let userCards;
    userCards = loadUserBlast(nk, logger, ctx.userId);
    let isInDeck = false;
    let selectedBlast = {
        uuid: "",
        data_id: 0,
        exp: 0,
        iv: 0,
        hp: 0,
        maxHp: 0,
        mana: 0,
        maxMana: 0,
        attack: 0,
        defense: 0,
        speed: 0,
        status: Status.NONE,
        activeMoveset: []
    };
    if (userCards.deckBlasts.find(blast => blast.uuid === request.uuidBlast) != null) {
        selectedBlast = userCards.deckBlasts.find(blast => blast.uuid === request.uuidBlast);
        isInDeck = true;
    }
    else if (userCards.storedBlasts.find(blast => blast.uuid === request.uuidBlast) != null) {
        selectedBlast = userCards.deckBlasts.find(blast => blast.uuid === request.uuidBlast);
        isInDeck = false;
    }
    if (isInDeck) {
        userCards.deckBlasts.find(blast => blast.uuid === request.uuidBlast);
        selectedBlast.activeMoveset[request.outMoveIndex] = getMoveById(getBlastDataById(selectedBlast.data_id).movepool[request.newMoveIndex].move_id).id;
    }
    else {
        userCards.storedBlasts.find(blast => blast.uuid === request.uuidBlast);
        selectedBlast.activeMoveset[request.outMoveIndex] = getMoveById(getBlastDataById(selectedBlast.data_id).movepool[request.newMoveIndex].move_id).id;
    }
    storeUserBlasts(nk, logger, ctx.userId, userCards);
    return JSON.stringify(userCards);
};
const rpcSwapDeckBlast = function (ctx, logger, nk, payload) {
    const request = JSON.parse(payload);
    const userBlasts = loadUserBlast(nk, logger, ctx.userId);
    logger.debug("Payload on server '%s'", request);
    if (userBlasts.deckBlasts[request.outIndex] == null) {
        throw Error('invalid out card');
    }
    if (userBlasts.storedBlasts[request.inIndex] == null) {
        throw Error('invalid in card');
    }
    let outCard = userBlasts.deckBlasts[request.outIndex];
    let inCard = userBlasts.storedBlasts[request.inIndex];
    userBlasts.deckBlasts[request.outIndex] = inCard;
    userBlasts.storedBlasts[request.inIndex] = outCard;
    storeUserBlasts(nk, logger, ctx.userId, userBlasts);
    logger.debug("user '%s' deck card '%s' swapped with '%s'", ctx.userId);
    return JSON.stringify(userBlasts);
};
const rpcUpgradeBlast = function (ctx, logger, nk, payload) {
    var _a, _b;
    const uuid = JSON.parse(payload);
    let userCards;
    userCards = loadUserBlast(nk, logger, ctx.userId);
    let isInDeck = false;
    let selectedBlast = {
        uuid: "",
        data_id: 0,
        exp: 0,
        iv: 0,
        hp: 0,
        maxHp: 0,
        mana: 0,
        maxMana: 0,
        attack: 0,
        defense: 0,
        speed: 0,
        status: Status.NONE,
        activeMoveset: []
    };
    if (userCards.deckBlasts.find(blast => blast.uuid === uuid) != null) {
        selectedBlast = userCards.deckBlasts.find(blast => blast.uuid === uuid);
        isInDeck = true;
    }
    else if (userCards.storedBlasts.find(blast => blast.uuid === uuid) != null) {
        selectedBlast = userCards.storedBlasts.find(blast => blast.uuid === uuid);
        isInDeck = false;
    }
    let blastdata = getBlastDataById(selectedBlast.data_id);
    if (isInDeck) {
        if (blastdata.nextEvolution !== null) {
            if (((_a = blastdata.nextEvolution) === null || _a === void 0 ? void 0 : _a.levelRequired) >= calculateLevelFromExperience(selectedBlast.exp)) {
                // Check si assez d'argent
                // If true check if assez d'argent par rapport rareté + ratio IV de base
                userCards.deckBlasts.find(blast => blast.uuid === uuid).data_id = getBlastDataById(blastdata.nextEvolution.id).id;
            }
        }
    }
    else {
        if (blastdata.nextEvolution != null) {
            if (((_b = blastdata.nextEvolution) === null || _b === void 0 ? void 0 : _b.levelRequired) >= calculateLevelFromExperience(selectedBlast.exp)) {
                // Check si assez d'argent
                // If true check if assez d'argent par rapport rareté + ratio IV de base
                userCards.storedBlasts.find(blast => blast.uuid === uuid).data_id = getBlastDataById(blastdata.nextEvolution.id).id;
            }
        }
    }
    storeUserBlasts(nk, logger, ctx.userId, userCards);
    logger.debug("user '%s' upgraded card '%s'", ctx.userId, selectedBlast.uuid);
    return JSON.stringify(userCards);
};
function addBlast(nk, logger, userId, newBlastToAdd) {
    newBlastToAdd.hp = newBlastToAdd.maxHp;
    newBlastToAdd.mana = newBlastToAdd.maxMana;
    newBlastToAdd.status = Status.NONE;
    let userCards;
    userCards = loadUserBlast(nk, logger, userId);
    if (userCards.deckBlasts.length < 3) {
        userCards.deckBlasts[userCards.deckBlasts.length] = newBlastToAdd;
    }
    else {
        userCards.storedBlasts[userCards.storedBlasts.length] = newBlastToAdd;
    }
    storeUserBlasts(nk, logger, userId, userCards);
    logger.debug("user '%s' succesfully add blast with id '%s'", userId, newBlastToAdd.data_id);
    return userCards;
}
function addExpOnBlast(nk, logger, userId, uuid, expToAdd) {
    let userCards;
    userCards = loadUserBlast(nk, logger, userId);
    let isInDeck = false;
    let selectedBlast = {
        uuid: "",
        data_id: 0,
        exp: 0,
        iv: 0,
        hp: 0,
        maxHp: 0,
        mana: 0,
        maxMana: 0,
        attack: 0,
        defense: 0,
        speed: 0,
        status: Status.NONE,
        activeMoveset: []
    };
    if (userCards.deckBlasts.find(blast => blast.uuid === uuid) != null) {
        selectedBlast = userCards.deckBlasts.find(blast => blast.uuid === uuid);
        isInDeck = true;
    }
    else if (userCards.storedBlasts.find(blast => blast.uuid === uuid) != null) {
        selectedBlast = userCards.deckBlasts.find(blast => blast.uuid === uuid);
        isInDeck = false;
    }
    if (isInDeck) {
        userCards.deckBlasts.find(blast => blast.uuid === uuid).exp += expToAdd;
    }
    else {
        userCards.storedBlasts.find(blast => blast.uuid === uuid).exp += expToAdd;
    }
    storeUserBlasts(nk, logger, userId, userCards);
    logger.debug("user '%s' succesfully add exp on blast with uuid '%s'", userId, uuid);
    return userCards.deckBlasts;
}
function getDeckBlast(nk, logger, userId) {
    let userCards;
    userCards = loadUserBlast(nk, logger, userId);
    logger.debug("user '%s' successfully get deck blast", userId);
    return userCards.deckBlasts;
}
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
const BagPermissionRead = 2;
const BagPermissionWrite = 0;
const BagCollectionName = 'item_collection';
const BagCollectionKey = 'user_items';
const DefaultDeckItems = [
    {
        data_id: healthPotionData.id,
        amount: 5,
    },
    {
        data_id: manaPotionData.id,
        amount: 5,
    },
    {
        data_id: blastTrapData.id,
        amount: 10,
    },
];
const rpcSwapDeckItem = function (ctx, logger, nk, payload) {
    const request = JSON.parse(payload);
    const userItems = loadUserItems(nk, logger, ctx.userId);
    logger.debug("Payload on server '%s'", request);
    if (userItems.deckItems[request.outIndex] == null) {
        throw Error('invalid out card');
    }
    if (userItems.storedItems[request.inIndex] == null) {
        throw Error('invalid in card');
    }
    let outCard = userItems.deckItems[request.outIndex];
    let inCard = userItems.storedItems[request.inIndex];
    userItems.deckItems[request.outIndex] = inCard;
    userItems.storedItems[request.inIndex] = outCard;
    storeUserItems(nk, logger, ctx.userId, userItems);
    logger.debug("user '%s' deck card '%s' swapped with '%s'", ctx.userId);
    return JSON.stringify(userItems);
};
const rpcLoadUserItems = function (ctx, logger, nk, payload) {
    return JSON.stringify(loadUserItems(nk, logger, ctx.userId));
};
function addItem(nk, logger, ctx, newItemToAdd) {
    let userItems;
    try {
        userItems = loadUserItems(nk, logger, ctx.userId);
    }
    catch (error) {
        logger.error('error loading user cards: %s', error);
        throw Error('Internal server error');
    }
    var continueScan = true;
    if (continueScan) {
        for (let i = 0; i < userItems.deckItems.length; i++) {
            if (userItems.deckItems[i].data_id == newItemToAdd.data_id) {
                userItems.deckItems[i].amount += newItemToAdd.amount;
                continueScan = false;
            }
        }
    }
    if (continueScan) {
        for (let i = 0; i < userItems.storedItems.length; i++) {
            if (userItems.storedItems[i].data_id == newItemToAdd.data_id) {
                userItems.storedItems[i].amount += newItemToAdd.amount;
                continueScan = false;
            }
        }
    }
    if (continueScan) {
        if (userItems.deckItems.length < 3) {
            userItems.deckItems[userItems.deckItems.length] = newItemToAdd;
        }
        else {
            userItems.storedItems[userItems.storedItems.length] = newItemToAdd;
        }
    }
    try {
        storeUserItems(nk, logger, ctx.userId, userItems);
    }
    catch (error) {
        logger.error('error buying card: %s', error);
        throw error;
    }
    return userItems;
}
function useItem(nk, logger, userId, itemToUse) {
    let userItems;
    userItems = loadUserItems(nk, logger, userId);
    for (let i = 0; i < userItems.deckItems.length; i++) {
        if (userItems.deckItems[i].data_id == itemToUse.data_id) {
            userItems.deckItems[i].amount--;
            if (userItems.deckItems[i].amount < 0) {
                userItems.deckItems[i].amount = 0;
            }
        }
    }
    storeUserItems(nk, logger, userId, userItems);
    logger.debug('user %s successfully use item', userId);
    return userItems;
}
function loadUserItems(nk, logger, userId) {
    let storageReadReq = {
        key: BagCollectionKey,
        collection: BagCollectionName,
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
    let storedItemCollection = objects[0].value;
    return storedItemCollection;
}
function storeUserItems(nk, logger, userId, cards) {
    try {
        nk.storageWrite([
            {
                key: BagCollectionKey,
                collection: BagCollectionName,
                userId: userId,
                value: cards,
                permissionRead: BagPermissionRead,
                permissionWrite: BagPermissionWrite,
            }
        ]);
    }
    catch (error) {
        logger.error('storageWrite error: %s', error);
        throw error;
    }
}
function defaultItemsCollection(nk, logger, userId) {
    let cards = {
        deckItems: DefaultDeckItems,
        storedItems: [],
    };
    storeUserItems(nk, logger, userId, cards);
    return {
        deckItems: DefaultDeckItems,
        storedItems: [],
    };
}
var OpCodes;
(function (OpCodes) {
    OpCodes[OpCodes["MATCH_START"] = 10] = "MATCH_START";
    OpCodes[OpCodes["PLAYER_WAIT"] = 15] = "PLAYER_WAIT";
    OpCodes[OpCodes["PLAYER_ATTACK"] = 20] = "PLAYER_ATTACK";
    OpCodes[OpCodes["PLAYER_USE_ITEM"] = 30] = "PLAYER_USE_ITEM";
    OpCodes[OpCodes["PLAYER_CHANGE_BLAST"] = 40] = "PLAYER_CHANGE_BLAST";
    OpCodes[OpCodes["PLAYER_READY"] = 50] = "PLAYER_READY";
    OpCodes[OpCodes["ENEMY_READY"] = 55] = "ENEMY_READY";
    OpCodes[OpCodes["MATCH_ROUND"] = 60] = "MATCH_ROUND";
    OpCodes[OpCodes["PLAYER_MUST_CHANGE_BLAST"] = 61] = "PLAYER_MUST_CHANGE_BLAST";
    OpCodes[OpCodes["MATCH_END"] = 100] = "MATCH_END";
    OpCodes[OpCodes["ERROR_SERV"] = 404] = "ERROR_SERV";
    OpCodes[OpCodes["DEBUG"] = 500] = "DEBUG";
})(OpCodes || (OpCodes = {}));
var notificationOpCodes;
(function (notificationOpCodes) {
    notificationOpCodes[notificationOpCodes["CURENCY"] = 1000] = "CURENCY";
    notificationOpCodes[notificationOpCodes["BLAST"] = 1010] = "BLAST";
    notificationOpCodes[notificationOpCodes["ITEM"] = 1020] = "ITEM";
})(notificationOpCodes || (notificationOpCodes = {}));
const LeaderboardTrophyId = "leaderboard_trophy";
const LeaderboardBlastDefeated = "leaderboard_blast_defeated";
function createTrophyLeaderboard(nk, logger, ctx) {
    let id = LeaderboardTrophyId;
    let authoritative = true;
    let sort = "descending" /* nkruntime.SortOrder.DESCENDING */;
    let operator = "best" /* nkruntime.Operator.BEST */;
    let reset = "0 0 1 */2 *";
    try {
        nk.leaderboardCreate(id, authoritative, sort, operator, reset, undefined);
    }
    catch (error) {
        // Handle error
    }
}
function createBlastDefeatedLeaderboard(nk, logger, ctx) {
    let id = LeaderboardBlastDefeated;
    let authoritative = true;
    let sort = "descending" /* nkruntime.SortOrder.DESCENDING */;
    let operator = "increment" /* nkruntime.Operator.INCREMENTAL */;
    let reset = '0 0 1 */2 *';
    try {
        nk.leaderboardCreate(id, authoritative, sort, operator, reset, undefined);
    }
    catch (error) {
        // Handle error
    }
}
const rpcGetAroundTrophyLeaderboard = function (ctx, logger, nk) {
    let result;
    let id = LeaderboardTrophyId;
    let ownerIds = [ctx.userId];
    let limit = 100;
    let cursor = '';
    let overrideExpiry = 3600;
    try {
        result = nk.leaderboardRecordsList(id, ownerIds, limit, cursor, overrideExpiry);
    }
    catch (error) {
        // Handle error
    }
    return JSON.stringify(loadUserBlast(nk, logger, ctx.userId));
};
let leaderboardReset = function (ctx, logger, nk, leaderboard, reset) {
    if (leaderboard.id !== LeaderboardTrophyId) {
        return;
    }
    let result = nk.leaderboardRecordsList(leaderboard.id, undefined, undefined, undefined, reset);
    // If leaderboard is top tier and has 10 or more players, relegate bottom 3 players
    result.records.forEach(function (r) {
        // Enlever /2 au dessus de 400 tr
        // nk.leaderboardRecordWrite(bottomTierId, r.ownerId, r.username, r.score, r.subscore, null, null);
        // nk.leaderboardRecordDelete(topTierId, r.ownerId);
    });
};
function writeRecordTrophyLeaderboard(nk, logger, ctx) {
    let id = LeaderboardTrophyId;
    let ownerID = ctx.userId;
    let username = ctx.username;
    let score = getCurrencyInWallet(nk, ctx.userId, Currency.Trophies);
    let result;
    try {
        result = nk.leaderboardRecordWrite(id, ownerID, username, score, undefined, undefined);
    }
    catch (error) {
        // Handle error
    }
}
function writeRecordBlastDefeatedLeaderboard(nk, logger, ctx, amount) {
    let id = LeaderboardBlastDefeated;
    let ownerID = ctx.userId;
    let username = ctx.username;
    let score = amount;
    let result;
    try {
        result = nk.leaderboardRecordWrite(id, ownerID, username, score, undefined, undefined);
    }
    catch (error) {
        // Handle error
    }
}
var BattleState;
(function (BattleState) {
    BattleState[BattleState["NONE"] = 0] = "NONE";
    BattleState[BattleState["WAITING"] = 1] = "WAITING";
    BattleState[BattleState["READY"] = 2] = "READY";
    BattleState[BattleState["START"] = 3] = "START";
    BattleState[BattleState["WAITFORPLAYERSWAP"] = 4] = "WAITFORPLAYERSWAP";
    BattleState[BattleState["END"] = 5] = "END";
})(BattleState || (BattleState = {}));
var PlayerState;
(function (PlayerState) {
    PlayerState[PlayerState["NONE"] = 0] = "NONE";
    PlayerState[PlayerState["BUSY"] = 1] = "BUSY";
    PlayerState[PlayerState["READY"] = 2] = "READY";
})(PlayerState || (PlayerState = {}));
var TurnType;
(function (TurnType) {
    TurnType[TurnType["NONE"] = 0] = "NONE";
    TurnType[TurnType["ATTACK"] = 1] = "ATTACK";
    TurnType[TurnType["ITEM"] = 2] = "ITEM";
    TurnType[TurnType["SWAP"] = 3] = "SWAP";
    TurnType[TurnType["WAIT"] = 4] = "WAIT";
})(TurnType || (TurnType = {}));
function rpcFindOrCreateWildBattle(context, logger, nk) {
    var matchId = nk.matchCreate('wildBattle', {});
    return JSON.stringify(matchId);
}
const matchInit = function (ctx, logger, nk, params) {
    const wildBattleData = {
        emptyTicks: 0,
        presences: {},
        battle_state: BattleState.START,
        player1_state: PlayerState.BUSY,
        player1_id: "",
        player1_current_blast: null,
        player1_blasts: [],
        player1_items: [],
        wild_blast: null,
        TurnStateData: {
            p_move_damage: 0,
            p_move_status: Status.NONE,
            wb_turn_type: TurnType.NONE,
            wb_move_index: 0,
            wb_move_damage: 0,
            wb_move_status: Status.NONE,
            catched: false
        },
    };
    return {
        state: wildBattleData,
        tickRate: 2,
        label: ''
    };
};
const matchJoinAttempt = function (ctx, logger, nk, dispatcher, tick, state, presence, metadata) {
    logger.debug('%q attempted to join Lobby match', ctx.userId);
    return {
        state,
        accept: true
    };
};
const matchJoin = function (ctx, logger, nk, dispatcher, tick, state, presences) {
    for (const presence of presences) {
        state.emptyTicks = 0;
        state.presences[presence.userId] = presence;
    }
    return {
        state
    };
};
const matchLeave = function (ctx, logger, nk, dispatcher, tick, state, presences) {
    for (let presence of presences) {
        logger.info("Player: %s left match: %s.", presence.userId, ctx.matchId);
        state.presences[presence.userId] = null;
    }
    for (let userID in state.presences) {
        if (state.presences[userID] === null) {
            delete state.presences[userID];
        }
    }
    if (connectedPlayers(state) === 0) {
        return null;
    }
    return {
        state
    };
};
const matchLoop = function (ctx, logger, nk, dispatcher, tick, state, messages) {
    // logger.info('Current state : %d', state.battle_state);
    switch (state.battle_state) {
        case BattleState.START:
            logger.debug('______________ START BATTLE ______________');
            const keys = Object.keys(state.presences);
            const player1_presence = state.presences[keys[0]];
            state.player1_id = player1_presence.userId;
            var allPlayer1BlastInBattle = getDeckBlast(nk, logger, state.player1_id);
            state.player1_current_blast = allPlayer1BlastInBattle[0];
            state.player1_blasts = allPlayer1BlastInBattle;
            var allPlayer1Items = getDeckItem(nk, logger, state.player1_id);
            state.player1_items = allPlayer1Items;
            state.wild_blast = getRandomBlastInPlayerArea(state.player1_id, logger, nk);
            const StartData = {
                id: state.wild_blast.data_id,
                exp: state.wild_blast.exp,
                iv: state.wild_blast.iv,
                status: Status.NONE,
                activeMoveset: state.wild_blast.activeMoveset,
            };
            logger.debug('Random blast: %d, with level: %l appeared', getBlastDataById(state.wild_blast.data_id).name, calculateLevelFromExperience(state.wild_blast.exp));
            state.battle_state = BattleState.WAITING;
            dispatcher.broadcastMessage(OpCodes.MATCH_START, JSON.stringify(StartData));
            logger.debug('______________ END START BATTLE ______________');
            break;
        case BattleState.WAITING:
            messages.forEach(function (message) {
                switch (message.opCode) {
                    case OpCodes.PLAYER_READY:
                        state.player1_state = PlayerState.READY;
                        logger.debug('______________ PLAYER 1 READY ______________');
                        break;
                }
            });
            if (state.player1_state == PlayerState.READY) {
                dispatcher.broadcastMessage(OpCodes.ENEMY_READY);
                state.battle_state = BattleState.READY;
                logger.debug('______________ EVERYONE"S READY ______________');
            }
            break;
        case BattleState.READY:
            messages.forEach(function (message) {
                // faire que si c'est un autre OP code que ceux d'en dessous faire un errorfunc
                var _a, _b, _c, _d, _e, _f;
                logger.debug('______________ LOOP BATTLE ______________');
                message.data == null ? logger.debug('Receive Op code : %d', message.opCode) : logger.debug('Receive Op code : %d, with data : %e', message.opCode, JSON.parse(nk.binaryToString(message.data)));
                state.player1_state = PlayerState.BUSY;
                state.TurnStateData = {
                    p_move_damage: 0,
                    p_move_status: Status.NONE,
                    wb_move_index: getRandomNumber(0, state.wild_blast.activeMoveset.length - 1),
                    wb_move_damage: 0,
                    wb_move_status: Status.NONE,
                    wb_turn_type: TurnType.NONE,
                    catched: false
                };
                switch (message.opCode) {
                    case OpCodes.PLAYER_ATTACK:
                        let attackIndex = clamp(JSON.parse(nk.binaryToString(message.data)), 0, 3);
                        let move = getMoveById(state.player1_current_blast.activeMoveset[attackIndex]);
                        if (move == null) {
                            ({ state } = ErrorFunc(state, "Player 1 move null", dispatcher));
                            return;
                        }
                        state = performAttackSequence(state, move, dispatcher, nk, logger);
                        break;
                    case OpCodes.PLAYER_USE_ITEM:
                        let msgItem = {};
                        msgItem = JSON.parse(nk.binaryToString(message.data));
                        msgItem.index_item = clamp(msgItem.index_item, 0, state.player1_items.length - 1);
                        let item = state.player1_items[msgItem.index_item];
                        useItem(nk, logger, state.player1_id, item);
                        if (item == null) {
                            ({ state } = ErrorFunc(state, "Item to use is null", dispatcher));
                            return;
                        }
                        if (item.amount <= 0) {
                            ({ state } = ErrorFunc(state, "U don't have enough item", dispatcher));
                            return;
                        }
                        var itemData = getItemDataById(item.data_id);
                        switch (itemData.behaviour) {
                            case ItemBehaviour.HEAL:
                                state.player1_blasts[msgItem.index_blast] = healHealthBlast(state.player1_blasts[msgItem.index_blast], itemData.gain_amount);
                                break;
                            case ItemBehaviour.MANA:
                                state.player1_blasts[msgItem.index_blast] = healManaBlast(state.player1_blasts[msgItem.index_blast], itemData.gain_amount);
                                break;
                            case ItemBehaviour.STATUS:
                                break;
                            case ItemBehaviour.CATCH:
                                var wildBlastCaptured = false;
                                wildBlastCaptured = isBlastCaptured(state.wild_blast.hp, state.wild_blast.maxHp, getBlastDataById(state.wild_blast.data_id).catchRate, itemData.catchRate, 1);
                                if (wildBlastCaptured) {
                                    logger.debug('Wild blast Captured !', wildBlastCaptured);
                                    addBlast(nk, logger, state.player1_id, state.wild_blast);
                                    state.battle_state = BattleState.END;
                                }
                                state.TurnStateData.catched = wildBlastCaptured;
                                break;
                            default:
                        }
                        ({ state } = executeWildBlastAttack(state, dispatcher));
                        break;
                    case OpCodes.PLAYER_CHANGE_BLAST:
                        var msgChangeBlast = clamp(JSON.parse(nk.binaryToString(message.data)), 0, state.player1_blasts.length - 1);
                        if (state.player1_current_blast == state.player1_blasts[msgChangeBlast]) {
                            ErrorFunc(state, "Cannot change actual blast with actual blast", dispatcher);
                            return;
                        }
                        if (!isBlastAlive(state.player1_blasts[msgChangeBlast])) {
                            ({ state } = ErrorFunc(state, "Cannot change actual blast with dead blast in Ready", dispatcher));
                            return;
                        }
                        state.player1_current_blast = state.player1_blasts[msgChangeBlast];
                        ({ state } = executeWildBlastAttack(state, dispatcher));
                        break;
                    case OpCodes.PLAYER_WAIT:
                        state.player1_state = PlayerState.BUSY;
                        state.player1_current_blast.mana = calculateStaminaRecovery(state.player1_current_blast.maxMana, state.player1_current_blast.mana, true);
                        ({ state } = executeWildBlastAttack(state, dispatcher));
                        break;
                }
                if (state.battle_state == BattleState.WAITFORPLAYERSWAP) {
                    if (state.TurnStateData.wb_turn_type != TurnType.WAIT)
                        state.wild_blast.mana = calculateStaminaRecovery(state.wild_blast.maxMana, state.wild_blast.mana, false);
                    logger.debug('Wild blast : %d, HP : %h, Mana : %m', getBlastDataById(state.wild_blast.data_id).name, (_a = state.wild_blast) === null || _a === void 0 ? void 0 : _a.hp, (_b = state.wild_blast) === null || _b === void 0 ? void 0 : _b.mana);
                    dispatcher.broadcastMessage(OpCodes.MATCH_ROUND, JSON.stringify(state.TurnStateData));
                    return;
                }
                else if (state.battle_state == BattleState.END) {
                    dispatcher.broadcastMessage(OpCodes.MATCH_ROUND, JSON.stringify(state.TurnStateData));
                    return;
                }
                else {
                    state.battle_state = BattleState.WAITING;
                    if (message.opCode != OpCodes.PLAYER_WAIT)
                        state.player1_current_blast.mana = calculateStaminaRecovery(state.player1_current_blast.maxMana, state.player1_current_blast.mana, false);
                    if (state.TurnStateData.wb_turn_type != TurnType.WAIT)
                        state.wild_blast.mana = calculateStaminaRecovery(state.wild_blast.maxMana, state.wild_blast.mana, false);
                    //Send matchTurn
                    dispatcher.broadcastMessage(OpCodes.MATCH_ROUND, JSON.stringify(state.TurnStateData));
                    logger.debug('Wild blast : %d, HP : %h, Mana : %m', getBlastDataById(state.wild_blast.data_id).name, (_c = state.wild_blast) === null || _c === void 0 ? void 0 : _c.hp, (_d = state.wild_blast) === null || _d === void 0 ? void 0 : _d.mana);
                    logger.debug('P1 blast : %d, HP : %h, Mana : %m', getBlastDataById(state.player1_current_blast.data_id).name, (_e = state.player1_current_blast) === null || _e === void 0 ? void 0 : _e.hp, (_f = state.player1_current_blast) === null || _f === void 0 ? void 0 : _f.mana);
                }
                logger.debug('______________ END LOOP BATTLE ______________');
            });
            break;
        case BattleState.WAITFORPLAYERSWAP:
            messages.forEach(function (message) {
                var _a, _b, _c, _d;
                logger.debug('______________ PLAYER SWAP BLAST ______________');
                message.data == null ? logger.debug('Receive Op code : %d', message.opCode) : logger.debug('Receive Op code : %d, with data : %e', message.opCode, JSON.parse(nk.binaryToString(message.data)));
                if (message.opCode == OpCodes.PLAYER_CHANGE_BLAST) {
                    state.player1_state = PlayerState.BUSY;
                    var msgChangeBlast = clamp(JSON.parse(nk.binaryToString(message.data)), 0, state.player1_blasts.length - 1);
                    if (state.player1_current_blast == state.player1_blasts[msgChangeBlast]) {
                        ErrorFunc(state, "Cannot change actual blast with actual blast", dispatcher);
                        return;
                    }
                    if (!isBlastAlive(state.player1_blasts[msgChangeBlast])) {
                        ({ state } = ErrorFunc(state, "Cannot change actual blast with dead blast", dispatcher));
                        return;
                    }
                    state.player1_current_blast = state.player1_blasts[msgChangeBlast];
                }
                logger.debug('Wild blast : %d, HP : %h, Mana : %m', getBlastDataById(state.wild_blast.data_id).name, (_a = state.wild_blast) === null || _a === void 0 ? void 0 : _a.hp, (_b = state.wild_blast) === null || _b === void 0 ? void 0 : _b.mana);
                logger.debug('P1 blast : %d, HP : %h, Mana : %m', getBlastDataById(state.player1_current_blast.data_id).name, (_c = state.player1_current_blast) === null || _c === void 0 ? void 0 : _c.hp, (_d = state.player1_current_blast) === null || _d === void 0 ? void 0 : _d.mana);
                state.battle_state = BattleState.WAITING;
                logger.debug('______________ END PLAYER SWAP BLAST ______________');
            });
            break;
        case BattleState.END:
            updateWalletWithCurrency(nk, state.player1_id, Currency.Coins, 200);
            state.battle_state = BattleState.START;
            state.player1_state = PlayerState.BUSY;
            logger.debug('______________ END BATTLE ______________');
            return null;
    }
    if (connectedPlayers(state) === 0) {
        logger.debug('Running empty ticks: %d', state.emptyTicks);
        state.emptyTicks++;
    }
    if (state.emptyTicks > 100) {
        return null;
    }
    return {
        state
    };
};
const matchSignal = function (ctx, logger, nk, dispatcher, tick, state, data) {
    logger.debug('Lobby match signal received: ' + data);
    return {
        state,
        data: "Lobby match signal received: " + data
    };
};
const matchTerminate = function (ctx, logger, nk, dispatcher, tick, state, graceSeconds) {
    logger.debug('Lobby match terminated');
    return {
        state
    };
};
function ErrorFunc(state, error, dispatcher) {
    state.battle_state = BattleState.READY;
    state.player1_state = PlayerState.READY;
    dispatcher.broadcastMessage(OpCodes.ERROR_SERV, JSON.stringify(error));
    return { state };
}
function connectedPlayers(s) {
    let count = 0;
    for (const p of Object.keys(s.presences)) {
        if (s.presences[p] !== null) {
            count++;
        }
    }
    return count;
}
//#region  Attack Logic
function applyBlastAttack(attacker, defender, move, state) {
    let damage = calculateDamage(calculateLevelFromExperience(attacker.exp), attacker.attack, defender.defense, move.type, getBlastDataById(defender.data_id).type, move.power);
    defender.hp = clamp(defender.hp - damage, 0, Number.POSITIVE_INFINITY);
    attacker.mana = clamp(attacker.mana - move.cost, 0, Number.POSITIVE_INFINITY);
    return damage;
}
function executePlayerAttack(state, move, dispatcher) {
    if (state.player1_current_blast.mana < move.cost) {
        return { state };
    }
    const damage = applyBlastAttack(state.player1_current_blast, state.wild_blast, move, state);
    state.TurnStateData.p_move_damage = damage;
    state.TurnStateData.p_move_status = Status.NONE;
    return { state };
}
function executeWildBlastAttack(state, dispatcher) {
    let wb_move = getMoveById(state.wild_blast.activeMoveset[state.TurnStateData.wb_move_index]);
    if (state.wild_blast.mana < wb_move.cost) {
        state.TurnStateData.wb_move_index = -1;
    }
    if (state.TurnStateData.wb_move_index < 0) {
        state.wild_blast.mana = calculateStaminaRecovery(state.wild_blast.maxMana, state.wild_blast.mana, true);
        state.TurnStateData.wb_turn_type = TurnType.WAIT;
    }
    else {
        const damage = applyBlastAttack(state.wild_blast, state.player1_current_blast, wb_move, state);
        state.TurnStateData.wb_move_damage = damage;
        state.TurnStateData.wb_move_status = Status.NONE;
        state.TurnStateData.wb_turn_type = TurnType.ATTACK;
    }
    return { state };
}
function handleAttackTurn(isPlayerFaster, state, move, dispatcher, nk, logger) {
    ({ state } = isPlayerFaster ? executePlayerAttack(state, move, dispatcher) : executeWildBlastAttack(state, dispatcher));
    ({ state } = checkIfMatchContinue(nk, logger, state, dispatcher));
    return { state };
}
function performAttackSequence(state, playerMove, dispatcher, nk, logger) {
    ({ state } = handleAttackTurn(getFasterBlast(state.player1_current_blast, state.wild_blast), state, playerMove, dispatcher, nk, logger));
    if (state.battle_state == BattleState.READY)
        ({ state } = handleAttackTurn(!getFasterBlast(state.player1_current_blast, state.wild_blast), state, playerMove, dispatcher, nk, logger));
    return state;
}
function checkIfMatchContinue(nk, logger, state, dispatcher) {
    if (!isBlastAlive(state.wild_blast)) {
        dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(true));
        addExpOnBlastInGame(nk, logger, state.player1_id, state.player1_current_blast, state.wild_blast);
        state.battle_state = BattleState.END;
    }
    if (isAllBlastDead(state.player1_blasts)) {
        dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(false));
        state.battle_state = BattleState.END;
    }
    else if (!isBlastAlive(state.player1_current_blast)) {
        state.battle_state = BattleState.WAITFORPLAYERSWAP;
    }
    return { state };
}
//#endregion
