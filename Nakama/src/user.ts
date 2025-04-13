const SYSTEM_USER_ID = "00000000-0000-0000-0000-000000000000";
const FRIEND_CODE_COLLECTION = "system";
const FRIEND_CODE_KEY = "friend_code_counter";

interface PlayerMetadata {
    battle_pass: boolean;
    updated_nickname: boolean;
    area: number;
    win: number;
    loose: number;
    blast_catched: number;
    blast_defeated: number;
}

const DefaultMetadata: PlayerMetadata = {
    battle_pass: false,
    updated_nickname: false,
    area: 0,
    win: 0,
    loose: 0,
    blast_catched: 0,
    blast_defeated: 0,
};


function afterAuthenticate(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, data: nkruntime.Session) {
    if (!data.created) {
        logger.info('User with id: %s account data already existing', ctx.userId);
        return
    }

    let user_id = ctx.userId;
    let username = "Player_" + generateFriendCode(nk);

    try {
        nk.accountUpdateId(user_id, username, null, null, null, null, null, DefaultMetadata);
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

    writeRecordLeaderboard(nk, logger, user_id, LeaderboardTrophyId, 0);
    writeRecordLeaderboard(nk, logger, user_id, LeaderboardBlastDefeatedId, 0);

    logger.debug('new user id: %s account data initialised', ctx.userId);
}


function rpcDeleteAccount(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama) {
    nk.accountDeleteId(ctx.userId);
}

// region Metadata


function rpcUpdateNicknameStatus(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama) {
    const account = nk.accountGetId(ctx.userId);
    const metadata = account.user.metadata as PlayerMetadata;

    metadata.updated_nickname = true;

    nk.accountUpdateId(ctx.userId, null, null, null, null, null, null, metadata);
}

function incrementMetadataStat(nk: nkruntime.Nakama, userId: string, statKey: keyof PlayerMetadata, increment: number) {

    const account = nk.accountGetId(userId);
    const metadata = account.user.metadata as PlayerMetadata;

    (metadata[statKey] as number) = (metadata[statKey] as number) + increment;

    nk.accountUpdateId(userId, "", null, null, null, null, null, metadata);
}

// endregion Metadata



function generateFriendCode(nk: nkruntime.Nakama): string {
    let counter = 1;

    try {
        const result = nk.storageRead([
            {
                collection: FRIEND_CODE_COLLECTION,
                key: FRIEND_CODE_KEY,
                userId: SYSTEM_USER_ID
            }
        ]);

        if (result.length > 0) {
            counter = parseInt(result[0].value.counter) + 1;
        }
    } catch (e) {
        counter = 1;
    }

    nk.storageWrite([
        {
            collection: FRIEND_CODE_COLLECTION,
            key: FRIEND_CODE_KEY,
            userId: SYSTEM_USER_ID,
            value: { counter: counter },
            permissionRead: 0,
            permissionWrite: 0
        }
    ]);

    const friendCode = counter.toString().padStart(8, "0");

    return friendCode;
}
