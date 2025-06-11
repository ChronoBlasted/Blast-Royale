const WildBattleAds = 'wildBattleButtonAds';

function rpcWatchWildBattleAds(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string) {
    const userId = context.userId;

    setMetadataStat(nk, userId, WildBattleAds, true);
}

function rpcWatchRefreshShopAds(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    if (!context.userId) {
        throw Error('No user ID in context');
    }

    const newShop: StoreOffer[] = generateRandomDailyShop(nk, context.userId, logger);

    const dailyShop = {
        lastClaimUnix: msecToSec(Date.now()),
        lastDailyShop: newShop,
    };

    const write: nkruntime.StorageWriteRequest = {
        collection: DailyShopCollectionName,
        key: DailyShopCollectionKey,
        permissionRead: DailyShopPermissionRead,
        permissionWrite: DailyShopPermissionWrite,
        value: dailyShop,
        userId: context.userId,
    };

    try {
        nk.storageWrite([write]);
    } catch (error) {
        logger.error('storageWrite error: %q', error);
        throw error;
    }

    return JSON.stringify(dailyShop);
}

function rpcWatchQuestAds(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string){
    if (!context.userId) {
        throw Error('No user ID in context');
    }

    incrementQuest(context.userId, QuestIds.WATCH_AD, 1, nk, logger);
}