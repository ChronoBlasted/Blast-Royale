let InitModule: nkruntime.InitModule = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer) {

    // Set up hooks.
    initializer.registerAfterAuthenticateDevice(afterAuthenticate);
    initializer.registerAfterAuthenticateEmail(afterAuthenticate);

    initializer.registerRpc('deleteAccount', rpcDeleteAccount);

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

    logger.debug('new user id: %s account data initialised', ctx.userId);
}



function rpcDeleteAccount(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama) {
    nk.accountDeleteId(ctx.userId);
}