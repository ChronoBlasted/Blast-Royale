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

const rpcLoadUserBlast: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
        return JSON.stringify(loadUserBlast(nk, logger, ctx.userId));
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
