"use strict";
let InitModule = function (ctx, logger, nk, initializer) {
    // Set up hooks.
    initializer.registerAfterAuthenticateDevice(afterAuthenticate);
    initializer.registerAfterAuthenticateEmail(afterAuthenticate);
    initializer.registerRpc('deleteAccount', rpcDeleteAccount);
    logger.info('XXXXXXXXXXXXXXXXXXXX - Blast Royale TypeScript loaded - XXXXXXXXXXXXXXXXXXXX');
};
function afterAuthenticate(ctx, logger, nk, data) {
    if (!data.created) {
        logger.info('User with id: %s account data already existing', ctx.userId);
        return;
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
    }
    catch (error) {
        logger.error('Error init update account : %s', error);
    }
    storeUserWallet(nk, user_id, DefaultWallet, logger);
    logger.debug('new user id: %s account data initialised', ctx.userId);
}
function rpcDeleteAccount(ctx, logger, nk) {
    nk.accountDeleteId(ctx.userId);
}
var Currency;
(function (Currency) {
    Currency["Coins"] = "coins";
    Currency["Gems"] = "gems";
    Currency["Trophies"] = "trophies";
    Currency["Hard"] = "hard";
})(Currency || (Currency = {}));
;
let DefaultWallet = {
    [Currency.Coins]: 1000,
    [Currency.Gems]: 100,
    [Currency.Trophies]: 0,
};
function storeUserWallet(nk, user_id, changeset, logger) {
    try {
        nk.walletUpdate(user_id, changeset);
    }
    catch (error) {
        logger.error('Error storing wallet of player : %s', user_id);
    }
}
