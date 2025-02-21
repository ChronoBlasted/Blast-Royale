enum OfferType {
    COINS,
    GEMS,
    BLAST,
    ITEM,
}

interface StoreOffer {
    offer_id: number
    type: OfferType

    coinsAmount: number
    gemsAmount: number
    blast: Blast | null
    item: Item | null

    price: number
    currency: Currency
    isAlreadyBuyed:boolean
}

//#region BlastTrap Offer

const blastTrap: Item = {
    data_id: blastTrapData.id,
    amount: 1,
};

const blastTrapOffer: StoreOffer = {
    offer_id: 1,
    type: OfferType.ITEM,
    currency: Currency.Coins,

    coinsAmount: 0,
    gemsAmount: 0,
    blast: null,
    item: blastTrap,

    price: 100,
    isAlreadyBuyed:false,
};

const superBlastTrap: Item = {
    data_id: superBlastTrapData.id,
    amount: 1,
};

const superBlastTrapOffer: StoreOffer = {
    offer_id: 2,
    type: OfferType.ITEM,
    currency: Currency.Coins,

    coinsAmount: 0,
    gemsAmount: 0,
    blast: null,
    item: superBlastTrap,

    price: 250,
    isAlreadyBuyed:false,
};

const hyperBlastTrap: Item = {
    data_id: hyperBlastTrapData.id,
    amount: 1,
};

const hyperBlastTrapOffer: StoreOffer = {
    offer_id: 3,
    type: OfferType.ITEM,
    currency: Currency.Coins,

    coinsAmount: 0,
    gemsAmount: 0,
    blast: null,
    item: hyperBlastTrap,

    price: 500,
    isAlreadyBuyed:false,
};

const blastTrapOffers: StoreOffer[] = [
    blastTrapOffer,
    superBlastTrapOffer,
    hyperBlastTrapOffer,
];

const rpcLoadBlastTrapOffer: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
        return JSON.stringify(blastTrapOffers);
    }

const rpcBuyTrapOffer: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string) {
        var indexOffer = JSON.parse(payload);

        var storeOffer = blastTrapOffers[indexOffer];

        try {
            nk.walletUpdate(ctx.userId, { [storeOffer.currency]: -storeOffer.price });
        } catch (error) {
            logger.error('error buying blast trap: %s', error);
            throw error;
        }

        addItem(nk, logger, ctx, storeOffer.item!)

        // return playerWallet and Wallets
    }

//#endregion

//#region Coins Offer

const coinsOffer1: StoreOffer = {
    offer_id: 4,
    type: OfferType.ITEM,
    currency: Currency.Gems,

    coinsAmount: 20000,
    gemsAmount: 0,
    blast: null,
    item: null,

    price: 100,
    isAlreadyBuyed:false,
};

const coinsOffer2: StoreOffer = {
    offer_id: 5,
    type: OfferType.ITEM,
    currency: Currency.Gems,

    coinsAmount: 65000,
    gemsAmount: 0,
    blast: null,
    item: null,

    price: 300,
    isAlreadyBuyed:false,
};

const coinsOffer3: StoreOffer = {
    offer_id: 6,
    type: OfferType.ITEM,
    currency: Currency.Gems,

    coinsAmount: 140000,
    gemsAmount: 0,
    blast: null,
    item: null,

    price: 600,
    isAlreadyBuyed:false,
};

const coinsOffer: StoreOffer[] = [
    coinsOffer1,
    coinsOffer2,
    coinsOffer3,
];

const rpcLoadCoinsOffer: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
        return JSON.stringify(coinsOffer);
    }

const rpcBuyCoinOffer: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string) {
        var indexOffer = JSON.parse(payload);

        var storeOffer = coinsOffer[indexOffer];

        try {
            nk.walletUpdate(ctx.userId, { [storeOffer.currency]: -storeOffer.price });

            nk.walletUpdate(ctx.userId, { [Currency.Coins]: storeOffer.coinsAmount });
        } catch (error) {
            logger.error('error buying blast trap: %s', error);
            throw error;
        }
    }

//#endregion

//#region Gems Offer

const gemsOffer1: StoreOffer = {
    offer_id: 7,
    type: OfferType.GEMS,
    currency: Currency.Hard,

    coinsAmount: 0,
    gemsAmount: 100,
    blast: null,
    item: null,

    price: 0,
    isAlreadyBuyed:false,
};

const gemsOffer2: StoreOffer = {
    offer_id: 8,
    type: OfferType.GEMS,
    currency: Currency.Hard,

    coinsAmount: 0,
    gemsAmount: 200,
    blast: null,
    item: null,

    price: 0,
    isAlreadyBuyed:false,
};

const gemsOffer3: StoreOffer = {
    offer_id: 9,
    type: OfferType.GEMS,
    currency: Currency.Hard,

    coinsAmount: 0,
    gemsAmount: 300,
    blast: null,
    item: null,

    price: 0,
    isAlreadyBuyed:false,
};

const gemsOffer4: StoreOffer = {
    offer_id: 10,
    type: OfferType.GEMS,
    currency: Currency.Hard,

    coinsAmount: 0,
    gemsAmount: 400,
    blast: null,
    item: null,

    price: 0,
    isAlreadyBuyed:false,
};

const gemsOffer5: StoreOffer = {
    offer_id: 11,
    type: OfferType.GEMS,
    currency: Currency.Hard,

    coinsAmount: 0,
    gemsAmount: 500,
    blast: null,
    item: null,

    price: 0,
    isAlreadyBuyed:false,
};

const gemsOffer6: StoreOffer = {
    offer_id: 12,
    type: OfferType.GEMS,
    currency: Currency.Hard,

    coinsAmount: 0,
    gemsAmount: 600,
    blast: null,
    item: null,

    price: 0,
    isAlreadyBuyed:false,
};

const gemsOffer: StoreOffer[] = [
    gemsOffer1,
    gemsOffer2,
    gemsOffer3,
    gemsOffer4,
    gemsOffer5,
    gemsOffer6,
];

const rpcLoadGemsOffer: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
        return JSON.stringify(gemsOffer);
    }

const rpcBuyGemOffer: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string) {
        var indexOffer = JSON.parse(payload);

        var storeOffer = gemsOffer[indexOffer];

        try {
            // Verif
            // Achat in app

            nk.walletUpdate(ctx.userId, { [Currency.Gems]: storeOffer.gemsAmount });
        } catch (error) {
            logger.error('error buying blast trap: %s', error);
            throw error;
        }
    }

//#endregion