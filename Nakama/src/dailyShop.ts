const DailyShopPermissionRead = 2;
const DailyShopPermissionWrite = 0;
const DailyShopCollectionName = 'shop';
const DailyShopCollectionKey = 'daily';

interface CanClaimDailyShop {
    canClaimDailyShop: boolean
    lastDailyShop: StoreOffer[]
}

function getLastDailyShopObject(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): any {
    if (!context.userId) {
        throw Error('No user ID in context');
    }

    var objectId: nkruntime.StorageReadRequest = {
        collection: DailyShopCollectionName,
        key: DailyShopCollectionKey,
        userId: context.userId,
    }

    var objects: nkruntime.StorageObject[];
    try {
        objects = nk.storageRead([objectId]);
    } catch (error) {
        logger.error('storageRead error: %s', error);
        throw error;
    }

    var dailyShop: any = {
        lastClaimUnix: 0,
        lastDailyShop: generateRandomDailyShop(nk, context.userId),
    }

    objects.forEach(function (object) {
        if (object.key == DailyShopCollectionKey) {
            dailyShop = object.value;
        }
    });

    return dailyShop;
}

function canUserClaimDailyShop(dailyShop: any) {
    if (!dailyShop.lastClaimUnix) {
        dailyShop.lastClaimUnix = 0;
    }

    var d = new Date();
    d.setHours(0, 0, 0, 0);

    return dailyShop.lastClaimUnix < msecToSec(d.getTime());
}

function rpcCanClaimDailyShop(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    var dailyShop = getLastDailyShopObject(context, logger, nk);

    var response = {
        canClaimDailyShop: canUserClaimDailyShop(dailyShop),
        lastDailyShop: dailyShop.lastDailyShop,
    }

    var result = JSON.stringify(response);

    return result;
}

function rpcGetDailyShopOffer(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {

    var dailyShop = getLastDailyShopObject(context, logger, nk);

    if (canUserClaimDailyShop(dailyShop)) {

        var newShop: StoreOffer[] = generateRandomDailyShop(nk, context.userId);

        dailyShop.lastClaimUnix = msecToSec(Date.now());
        dailyShop.lastDailyShop = newShop;

        var write: nkruntime.StorageWriteRequest = {
            collection: DailyShopCollectionName,
            key: DailyShopCollectionKey,
            permissionRead: DailyShopPermissionRead,
            permissionWrite: DailyShopPermissionWrite,
            value: dailyShop,
            userId: context.userId,
        }

        if (dailyShop.version) {
            // Use OCC to prevent concurrent writes.
            write.version = dailyShop.version
        }

        // Update daily reward storage object for user.
        try {
            nk.storageWrite([write])
        } catch (error) {
            logger.error('storageWrite error: %q', error);
            throw error;
        }
    }

    var result = JSON.stringify(dailyShop);

    return result;
}

function rpcBuyDailyShopOffer(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {

    var dailyShop: CanClaimDailyShop = getLastDailyShopObject(context, logger, nk);
    var indexOffer = JSON.parse(payload);

    if (dailyShop.lastDailyShop[indexOffer].isAlreadyBuyed == true) {
        throw Error('Daily shop offer already buyed');
    }

    try {
        nk.walletUpdate(context.userId, { [dailyShop.lastDailyShop[indexOffer].currency]: -dailyShop.lastDailyShop[indexOffer].price });
    } catch (error) {
        logger.error('error buying blast trap: %s', error);
        throw error;
    }

    dailyShop.lastDailyShop[indexOffer].isAlreadyBuyed = true;

    var write: nkruntime.StorageWriteRequest = {
        collection: DailyShopCollectionName,
        key: DailyShopCollectionKey,
        permissionRead: DailyShopPermissionRead,
        permissionWrite: DailyShopPermissionWrite,
        value: dailyShop,
        userId: context.userId,
    }

    // Update daily reward storage object for user.
    try {
        nk.storageWrite([write])
    } catch (error) {
        logger.error('storageWrite error: %q', error);
        throw error;
    }

    if (dailyShop.lastDailyShop[indexOffer].blast != null) {
        addBlast(nk, logger, context.userId, dailyShop.lastDailyShop[indexOffer].blast!)
    }

    if (dailyShop.lastDailyShop[indexOffer].item != null) {
        addItem(nk, logger, context, dailyShop.lastDailyShop[indexOffer].item!)
    }

    var result = JSON.stringify(dailyShop);
    logger.debug('Succefuly buy daily shop offer response: %q', result)
    return result;
}

function generateRandomDailyShop(nk: nkruntime.Nakama, userId: string): StoreOffer[] {

    var dailyShop: StoreOffer[];

    dailyShop = [
        getRandomStoreOffer(nk, userId),
        getRandomStoreOffer(nk, userId),
        getRandomStoreOffer(nk, userId),
        getRandomStoreOffer(nk, userId),
        getRandomStoreOffer(nk, userId),
        getRandomStoreOffer(nk, userId),
    ]

    return dailyShop;
}

function getRandomOfferType(): OfferType {
    const offerTypeValues = Object.values(OfferType);
    const randomIndex = Math.floor(Math.random() * offerTypeValues.length);
    return offerTypeValues[randomIndex] as OfferType;
}

function getRandomStoreOffer(nk: nkruntime.Nakama, userId: string): StoreOffer {

    let offer: StoreOffer = {
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
    } else {
        offer.item = getRandomItem(5);
        
        var itemData = getItemDataById(offer.item.data_id)
        
        offer.price = getItemPrice(offer.item);
        offer.currency = Currency.Coins;
        offer.desc = offer.item.amount.toString();
        offer.name = itemData.name;
    }

    return offer;
}

function getBlastPrice(blast: Blast): number {

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

function getItemPrice(item: Item): number {

    var coeffRarity = 1;
    var itemData = getItemDataById(item.data_id)

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

