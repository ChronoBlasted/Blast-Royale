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
    wildBattleButtonAds: boolean;
}

const DefaultMetadata: PlayerMetadata = {
    battle_pass: false,
    updated_nickname: false,
    area: 0,
    win: 0,
    loose: 0,
    blast_catched: 0,
    blast_defeated: 0,
    wildBattleButtonAds: false,
};

function afterAuthenticate(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, data: nkruntime.Session) {

    createDailyQuestStorageIfNeeded(ctx.userId, nk, logger);
    incrementQuest(ctx.userId, QuestIds.LOGIN, 1, nk, logger);

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

    initializeBlastTrackerData(ctx.userId, nk, logger);

    markMonsterCaptured(ctx.userId, Lizzy.id.toString(), 1, nk, logger);
    markMonsterCaptured(ctx.userId, Punchball.id.toString(), 1, nk, logger);
    markMonsterCaptured(ctx.userId, Jellys.id.toString(), 1, nk, logger);

    logger.debug('new user id: %s account data initialised', ctx.userId);
}


function rpcDeleteAccount(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama) {
    if (!ctx.userId) {
        throw new Error("Authentication required.");
    }

    nk.accountDeleteId(ctx.userId);

    return JSON.stringify({ success: true, message: "Account deleted." });
};


// region Metadata


function rpcUpdateNicknameStatus(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama) {
    const account = nk.accountGetId(ctx.userId);
    const metadata = account.user.metadata as PlayerMetadata;

    metadata.updated_nickname = true;

    nk.accountUpdateId(ctx.userId, null, null, null, null, null, null, metadata);

    try {
        const leaderboardsList = nk.leaderboardList(100);
        if (leaderboardsList.leaderboards && Array.isArray(leaderboardsList.leaderboards)) {
            for (const lb of leaderboardsList.leaderboards) {
                nk.leaderboardRecordWrite(
                    lb.id,
                    ctx.userId,
                    account.user.username,
                    0, // score
                    0, // subscore
                    undefined,
                    nkruntime.OverrideOperator.SET
                );
            }
        }
    } catch (e) {
        logger.error("Failed to write initial leaderboard scores: %s", e);
    }
}

function incrementMetadataStat(nk: nkruntime.Nakama, userId: string, statKey: keyof PlayerMetadata, increment: number) {

    const account = nk.accountGetId(userId);
    const metadata = account.user.metadata as PlayerMetadata;

    (metadata[statKey] as number) = (metadata[statKey] as number) + increment;

    nk.accountUpdateId(userId, "", null, null, null, null, null, metadata);
}

function setMetadataStat(
    nk: nkruntime.Nakama,
    userId: string,
    statKey: keyof PlayerMetadata,
    value: number | boolean
) {
    const account = nk.accountGetId(userId);
    const metadata = account.user.metadata as PlayerMetadata;

    (metadata as any)[statKey] = value;

    nk.accountUpdateId(userId, "", null, null, null, null, null, metadata);
}

function getMetadataStat(
    nk: nkruntime.Nakama,
    userId: string,
    statKey: keyof PlayerMetadata
): number {
    const account = nk.accountGetId(userId);
    const metadata = account.user.metadata as PlayerMetadata;
    return metadata[statKey] as number;
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

