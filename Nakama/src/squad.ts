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

const DefaultDeckBlasts: Blast[] = [
    (() => {
        const iv = getRandomIV(10, MaxIV);
        return {
            uuid: generateUUID(),
            data_id: Florax.id,
            exp: calculateExperienceFromLevel(5),
            iv: iv, // Utiliser l'IV généré
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
            iv: iv, // Utiliser l'IV généré
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
            iv: iv, // Utiliser l'IV généré
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

        let userCards: BlastCollection;
        userCards = loadUserBlast(nk, logger, ctx.userId);

        let isInDeck: boolean = false;
        let selectedBlast: Blast = {
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
            selectedBlast = userCards.deckBlasts.find(blast => blast.uuid === request.uuidBlast)!;
            isInDeck = true;
        }
        else if (userCards.storedBlasts.find(blast => blast.uuid === request.uuidBlast) != null) {
            selectedBlast = userCards.deckBlasts.find(blast => blast.uuid === request.uuidBlast)!;
            isInDeck = false;
        }

        if (isInDeck) {
            userCards.deckBlasts.find(blast => blast.uuid === request.uuidBlast);

            selectedBlast.activeMoveset![request.outMoveIndex] = getMoveById(getBlastDataById(selectedBlast.data_id).movepool[request.newMoveIndex].move_id).id;
        } else {
            userCards.storedBlasts.find(blast => blast.uuid === request.uuidBlast);

            selectedBlast.activeMoveset![request.outMoveIndex] = getMoveById(getBlastDataById(selectedBlast.data_id).movepool[request.newMoveIndex].move_id).id
        }

        storeUserBlasts(nk, logger, ctx.userId, userCards);

        return JSON.stringify(userCards);
    }

const rpcSwapDeckBlast: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
        const request: SwapDeckRequest = JSON.parse(payload);

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
    }

const rpcUpgradeBlast: nkruntime.RpcFunction =
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

    newBlastToAdd.hp = newBlastToAdd.maxHp;
    newBlastToAdd.mana = newBlastToAdd.maxMana;
    newBlastToAdd.status = Status.NONE;

    let userCards: BlastCollection;

    userCards = loadUserBlast(nk, logger, userId);

    if (userCards.deckBlasts.length < 3) {
        userCards.deckBlasts[userCards.deckBlasts.length] = newBlastToAdd;
    } else {
        userCards.storedBlasts[userCards.storedBlasts.length] = newBlastToAdd;
    }

    storeUserBlasts(nk, logger, userId, userCards);

    logger.debug("user '%s' succesfully add blast with id '%s'", userId, newBlastToAdd.data_id);

    return userCards;
}


function addExpOnBlast(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string, uuid: string, expToAdd: number): Blast[] {

    let userCards: BlastCollection;
    userCards = loadUserBlast(nk, logger, userId);

    let isInDeck: boolean = false;
    let selectedBlast: Blast = {
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
        selectedBlast = userCards.deckBlasts.find(blast => blast.uuid === uuid)!;
        isInDeck = true;
    }
    else if (userCards.storedBlasts.find(blast => blast.uuid === uuid) != null) {
        selectedBlast = userCards.deckBlasts.find(blast => blast.uuid === uuid)!;
        isInDeck = false;
    }

    if (isInDeck) {
        userCards.deckBlasts.find(blast => blast.uuid === uuid)!.exp += expToAdd;
    } else {
        userCards.storedBlasts.find(blast => blast.uuid === uuid)!.exp += expToAdd;
    }

    storeUserBlasts(nk, logger, userId, userCards);

    logger.debug("user '%s' succesfully add exp on blast with uuid '%s'", userId, uuid);

    return userCards.deckBlasts;
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
        deckBlasts: DefaultDeckBlasts,
        storedBlasts: DefaultDeckBlasts,
    }

    storeUserBlasts(nk, logger, userId, cards);

    return {
        deckBlasts: DefaultDeckBlasts,
        storedBlasts: DefaultDeckBlasts,
    }
}
