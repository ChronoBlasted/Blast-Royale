const DeckPermissionRead = 2;
const DeckPermissionWrite = 0;
const DefaultBlastLevel = 5;
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
        } else if (i === 14) {
            uuid += '4'; // Le 14e caractère est toujours "4" pour un UUID v4
        } else {
            random = Math.random() * 16 | 0;
            if (i === 19) {
                uuid += (random & 0x3 | 0x8).toString(16); // Le 19e caractère est limité à 8, 9, A, ou B
            } else {
                uuid += random.toString(16);
            }
        }
    }
    return uuid;
}
const DefaultDeckBlast: Blast[] = [
    (() => {
        const iv = getRandomIV(10, MaxIV);
        const exp = calculateExperienceFromLevel(DefaultBlastLevel);
        const blast: Blast = {
            uuid: generateUUID(),
            data_id: Lizzy.id,
            exp,
            iv,
            boss: false,
            shiny: isShiny(),
            activeMoveset: getRandomActiveMoveset(Lizzy, exp),
        };
        return blast;
    })(),
    (() => {
        const iv = getRandomIV(10, MaxIV);
        const exp = calculateExperienceFromLevel(DefaultBlastLevel);
        const blast: Blast = {
            uuid: generateUUID(),
            data_id: Punchball.id,
            exp,
            iv,
            boss: false,
            shiny: isShiny(),
            activeMoveset: getRandomActiveMoveset(Punchball, exp),
        };
        return blast;
    })(),
    (() => {
        const iv = getRandomIV(10, MaxIV);
        const exp = calculateExperienceFromLevel(DefaultBlastLevel);
        const blast: Blast = {
            uuid: generateUUID(),
            data_id: Jellys.id,
            exp,
            iv,
            boss: false,
            shiny: isShiny(),
            activeMoveset: getRandomActiveMoveset(Jellys, exp),
        };
        return blast;
    })()
];

interface BlastCollection {
    deckBlasts: Blast[]
    storedBlasts: Blast[]
}

interface SwapDeckRequest {
    inIndex: number
    outIndex: number
}

interface SwapMoveRequest {
    uuidBlast: string
    outMoveIndex: number
    newMoveIndex: number
}

const rpcLoadUserBlast: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
        return JSON.stringify(loadUserBlast(nk, logger, ctx.userId));
    }

const rpcSwapBlastMove: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
        const request: SwapMoveRequest = JSON.parse(payload);

        const userCards = loadUserBlast(nk, logger, ctx.userId);

        let selectedBlast: Blast | undefined = userCards.deckBlasts.find(blast => blast.uuid === request.uuidBlast) || userCards.storedBlasts.find(blast => blast.uuid === request.uuidBlast);

        if (!selectedBlast) {
            throw Error("Blast not found.");
        }

        const blastData = getBlastDataById(selectedBlast.data_id);
        const newMove = blastData.movepool[request.newMoveIndex];

        if (!newMove) {
            throw Error("Invalid new move index.");
        }

        const newMoveId = getMoveById(newMove.move_id).id;
        const activeMoveset = selectedBlast.activeMoveset!;

        const currentIndex = activeMoveset.indexOf(newMoveId);

        if (currentIndex !== -1) {
            if (currentIndex === request.outMoveIndex) {
                throw Error("Cannot swap a move with itself.");
            }

            const temp = activeMoveset[request.outMoveIndex];
            activeMoveset[request.outMoveIndex] = activeMoveset[currentIndex];
            activeMoveset[currentIndex] = temp;
        } else {
            activeMoveset[request.outMoveIndex] = newMoveId;
        }

        storeUserBlasts(nk, logger, ctx.userId, userCards);

        return JSON.stringify(userCards);
    }


const rpcSwapStoredToDeckBlast: nkruntime.RpcFunction = function (
    ctx: nkruntime.Context,
    logger: nkruntime.Logger,
    nk: nkruntime.Nakama,
    payload: string
): string {
    const request: SwapDeckRequest = JSON.parse(payload);
    const userBlasts = loadUserBlast(nk, logger, ctx.userId);

    if (userBlasts.deckBlasts[request.outIndex] == null) {
        throw Error('invalid out card (deck)');
    }

    if (userBlasts.storedBlasts[request.inIndex] == null) {
        throw Error('invalid in card (stored)');
    }

    const outCard = userBlasts.deckBlasts[request.outIndex];
    const inCard = userBlasts.storedBlasts[request.inIndex];

    userBlasts.deckBlasts[request.outIndex] = inCard;
    userBlasts.storedBlasts[request.inIndex] = outCard;

    storeUserBlasts(nk, logger, ctx.userId, userBlasts);
    logger.debug("user '%s' swapped STORED card '%d' with DECK card at index %d", ctx.userId, request.inIndex, request.outIndex);

    return JSON.stringify(userBlasts);
};

const rpcSwapDeckToDeckBlast: nkruntime.RpcFunction = function (
    ctx: nkruntime.Context,
    logger: nkruntime.Logger,
    nk: nkruntime.Nakama,
    payload: string
): string {
    const request: SwapDeckRequest = JSON.parse(payload);
    const userBlasts = loadUserBlast(nk, logger, ctx.userId);

    if (userBlasts.deckBlasts[request.outIndex] == null || userBlasts.deckBlasts[request.inIndex] == null) {
        throw Error('invalid deck card index');
    }

    const outCard = userBlasts.deckBlasts[request.outIndex];
    const inCard = userBlasts.deckBlasts[request.inIndex];

    userBlasts.deckBlasts[request.outIndex] = inCard;
    userBlasts.deckBlasts[request.inIndex] = outCard;

    storeUserBlasts(nk, logger, ctx.userId, userBlasts);
    logger.debug("user '%s' swapped DECK card '%d' with DECK card at index %d", ctx.userId, request.inIndex, request.outIndex);

    return JSON.stringify(userBlasts);
};




const rpcEvolveBlast: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {

        const uuid: string = JSON.parse(payload);

        let userCards: BlastCollection;
        userCards = loadUserBlast(nk, logger, ctx.userId);

        let isInDeck: boolean = false;
        let selectedBlast: Blast = {
            uuid: "",
            data_id: 0,
            exp: 0,
            iv: 0,
            boss: false,
            shiny: false,
            activeMoveset: [],
        };

        if (userCards.deckBlasts.find(blast => blast.uuid === uuid) != null) {
            selectedBlast = userCards.deckBlasts.find(blast => blast.uuid === uuid)!;
            isInDeck = true;
        }
        else if (userCards.storedBlasts.find(blast => blast.uuid === uuid) != null) {
            selectedBlast = userCards.storedBlasts.find(blast => blast.uuid === uuid)!;
            isInDeck = false;
        }

        let blastdata = getBlastDataById(selectedBlast.data_id)

        if (isInDeck) {
            if (blastdata.nextEvolution !== null) {
                if (blastdata.nextEvolution?.levelRequired! >= calculateLevelFromExperience(selectedBlast.exp)) {

                    // Check si assez d'argent
                    // If true check if assez d'argent par rapport rareté + ratio IV de base

                    userCards.deckBlasts.find(blast => blast.uuid === uuid)!.data_id = getBlastDataById(blastdata.nextEvolution.id).id;

                }
            }
        } else {
            if (blastdata.nextEvolution != null) {
                if (blastdata.nextEvolution?.levelRequired! >= calculateLevelFromExperience(selectedBlast.exp)) {

                    // Check si assez d'argent
                    // If true check if assez d'argent par rapport rareté + ratio IV de base

                    userCards.storedBlasts.find(blast => blast.uuid === uuid)!.data_id = getBlastDataById(blastdata.nextEvolution.id).id;
                }
            }
        }

        storeUserBlasts(nk, logger, ctx.userId, userCards);

        logger.debug("user '%s' upgraded card '%s'", ctx.userId, selectedBlast.uuid);

        return JSON.stringify(userCards);
    }

function addBlast(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string, newBlastToAdd: Blast): BlastCollection {

    let userCards: BlastCollection;

    userCards = loadUserBlast(nk, logger, userId);

    if (userCards.deckBlasts.length < 3) {
        userCards.deckBlasts[userCards.deckBlasts.length] = newBlastToAdd;
    } else {
        userCards.storedBlasts[userCards.storedBlasts.length] = newBlastToAdd;
    }

    let version = getBlastVersion(newBlastToAdd);

    markMonsterCaptured(userId, newBlastToAdd.data_id.toString(), version, nk, logger);

    storeUserBlasts(nk, logger, userId, userCards);

    logger.debug("user '%s' succesfully add blast with id '%s'", userId, newBlastToAdd.data_id);

    return userCards;
}


function addExpOnBlast(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string, uuid: string, expToAdd: number): Blast[] {
    const userCards: BlastCollection = loadUserBlast(nk, logger, userId);

    let blast = userCards.deckBlasts.find(b => b.uuid === uuid);
    if (!blast) blast = userCards.storedBlasts.find(b => b.uuid === uuid);

    if (!blast) {
        logger.error(`Blast with UUID '${uuid}' not found for user '${userId}'`);
        return userCards.deckBlasts;
    }

    const oldLevel = calculateLevelFromExperience(blast.exp);
    blast.exp += expToAdd;
    const newLevel = calculateLevelFromExperience(blast.exp);

    // Si level up et moveset incomplet, tenter d’ajouter un move
    if (newLevel > oldLevel && blast.activeMoveset.length < 4) {
        const blastData = getBlastDataById(blast.data_id);

        const newMoves = blastData.movepool
            .filter(m => m.levelMin <= newLevel)
            .map(m => m.move_id)
            .filter(moveId => !blast!.activeMoveset.includes(moveId));

        if (newMoves.length > 0) {
            blast!.activeMoveset.push(newMoves[0]);
            logger.debug(`Blast '${uuid}' gained new move '${newMoves[0]}' at level ${newLevel}`);
        }
    }

    storeUserBlasts(nk, logger, userId, userCards);

    logger.debug("User '%s' successfully added exp on blast with uuid '%s'", userId, uuid);

    return userCards.deckBlasts;
}


function getDeckBlast(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string): BlastEntity[] {

    let userCards: BlastCollection;
    userCards = loadUserBlast(nk, logger, userId);

    let deckBlasts: BlastEntity[] = [];
    for (let i = 0; i < userCards.deckBlasts.length; i++) {
        var blast = ConvertBlastToBlastEntity(userCards.deckBlasts[i]);
        deckBlasts.push(blast);
    }
    logger.debug("user '%s' successfully get deck blast", userId);

    return deckBlasts;
}



function loadUserBlast(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string): BlastCollection {
    let storageReadReq: nkruntime.StorageReadRequest = {
        key: DeckCollectionKey,
        collection: DeckCollectionName,
        userId: userId,
    }

    let objects: nkruntime.StorageObject[];
    try {
        objects = nk.storageRead([storageReadReq]);
    } catch (error) {
        logger.error('storageRead error: %s', error);
        throw error;
    }

    if (objects.length === 0) {
        throw Error('user cards storage object not found');
    }

    let BlastCollection = objects[0].value as BlastCollection;

    return BlastCollection;
}

function storeUserBlasts(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string, cards: BlastCollection) {
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
    } catch (error) {
        logger.error('storageWrite error: %s', error);
        throw error;
    }
}

function defaultBlastCollection(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string): BlastCollection {

    let cards: BlastCollection = {
        deckBlasts: DefaultDeckBlast,
        storedBlasts: [],
    }

    storeUserBlasts(nk, logger, userId, cards);

    return {
        deckBlasts: DefaultDeckBlast,
        storedBlasts: [],
    }
}
