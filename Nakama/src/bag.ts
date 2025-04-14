const BagPermissionRead = 2;
const BagPermissionWrite = 0;
const BagCollectionName = 'item_collection';
const BagCollectionKey = 'user_items';

const DefaultDeckItems: Item[] = [
    {
        data_id: potionData.id,
        amount: 5,
    },
    {
        data_id: elixirData.id,
        amount: 5,
    },
    {
        data_id: blastTrapData.id,
        amount: 10,
    },
];

interface ItemCollection {
    deckItems: Item[]
    storedItems: Item[]
}

const rpcSwapDeckItem: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
        const request: SwapDeckRequest = JSON.parse(payload);

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
    }


const rpcLoadUserItems: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
        return JSON.stringify(loadUserItems(nk, logger, ctx.userId));
    }

function addItem(nk: nkruntime.Nakama, logger: nkruntime.Logger, ctx: nkruntime.Context, newItemToAdd: Item): ItemCollection {

    let userItems: ItemCollection;

    try {
        userItems = loadUserItems(nk, logger, ctx.userId);
    } catch (error) {
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
        } else {
            userItems.storedItems[userItems.storedItems.length] = newItemToAdd;
        }
    }

    try {
        storeUserItems(nk, logger, ctx.userId, userItems);
    } catch (error) {
        logger.error('error buying card: %s', error);
        throw error;
    }

    return userItems;
}

function useItem(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string, itemToUse: Item): ItemCollection {

    let userItems: ItemCollection;
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

function loadUserItems(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string): ItemCollection {
    let storageReadReq: nkruntime.StorageReadRequest = {
        key: BagCollectionKey,
        collection: BagCollectionName,
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

    let storedItemCollection = objects[0].value as ItemCollection;
    return storedItemCollection;
}

function storeUserItems(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string, cards: ItemCollection) {
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
    } catch (error) {
        logger.error('storageWrite error: %s', error);
        throw error;
    }
}

function defaultItemsCollection(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string): ItemCollection {


    let cards: ItemCollection = {
        deckItems: DefaultDeckItems,
        storedItems: [],
    }

    storeUserItems(nk, logger, userId, cards);

    return {
        deckItems: DefaultDeckItems,
        storedItems: [],
    }
}