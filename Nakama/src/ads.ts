const WildBattleAds = 'wildBattleButtonAds';

function rpcWatchWildBattleAds(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string) {
    const userId = context.userId;

    setMetadataStat(nk, userId, WildBattleAds, true);
}

function rpcWatchEndWildBattleAds(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string) {
    const userId = context.userId;

    // Add bonus rewards to user
}

function rpcWatchRefreshShopAds(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string) {
    const userId = context.userId;

    // Refresh shop
}