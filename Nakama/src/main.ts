let InitModule: nkruntime.InitModule = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer) {

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
}

function afterAuthenticate(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, data: nkruntime.Session) {
    if (!data.created) {
        logger.info('User with id: %s account data already existing', ctx.userId);
        return
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
    } catch (error) {
        logger.error('Error init update account : %s', error);
    }

    storeUserWallet(nk, user_id, DefaultWallet, logger);

    const writeBlasts: nkruntime.StorageWriteRequest = {
        collection: DeckCollectionName,
        key: DeckCollectionKey,
        permissionRead: DeckPermissionRead,
        permissionWrite: DeckPermissionWrite,
        value: defaultBlastCollection(nk, logger, ctx.userId),
        userId: ctx.userId,
    }

    try {
        nk.storageWrite([writeBlasts]);
    } catch (error) {
        logger.error('storageWrite error: %q', error);
        throw error;
    }

    const writeItems: nkruntime.StorageWriteRequest = {
        collection: BagCollectionName,
        key: BagCollectionKey,
        permissionRead: BagPermissionRead,
        permissionWrite: BagPermissionWrite,
        value: defaultItemsCollection(nk, logger, ctx.userId),
        userId: ctx.userId,
    }

    try {
        nk.storageWrite([writeItems]);
    } catch (error) {
        logger.error('storageWrite error: %q', error);
        throw error;
    }

    writeRecordTrophyLeaderboard(nk, logger, ctx);
    writeRecordBlastDefeatedLeaderboard(nk, logger, ctx, 0);

    logger.debug('new user id: %s account data initialised', ctx.userId);
}

function rpcDeleteAccount(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama) {
    nk.accountDeleteId(ctx.userId);
}