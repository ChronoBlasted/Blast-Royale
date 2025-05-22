const DailyRewardPermissionRead = 2;
const DailyRewardPermissionWrite = 0;
const DailyRewardCollectionName = 'reward';
const DailyRewardCollectionKey = 'daily';

interface Reward {
    coinsReceived: number
    gemsReceived: number
    blastReceived: Blast | null
    itemReceived: Item | null
}

function getLastDailyRewardObject(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): any {
    if (!context.userId) {
        throw Error('No user ID in context');
    }

    if (payload) {
        throw Error('No input allowed');
    }

    var objectId: nkruntime.StorageReadRequest = {
        collection: DailyRewardCollectionName,
        key: DailyRewardCollectionKey,
        userId: context.userId,
    }

    var objects: nkruntime.StorageObject[];
    try {
        objects = nk.storageRead([objectId]);
    } catch (error) {
        logger.error('storageRead error: %s', error);
        throw error;
    }

    var dailyReward: any = {
        lastClaimUnix: 0,
        totalDay: 0,
    }

    objects.forEach(function (object) {
        if (object.key == DailyRewardCollectionKey) {
            dailyReward = object.value;
        }
    });

    return dailyReward;
}

function canUserClaimDailyReward(dailyReward: any) {
    if (!dailyReward.lastClaimUnix) {
        dailyReward.lastClaimUnix = 0;
    }

    var d = new Date();
    d.setHours(0, 0, 0, 0);

    return dailyReward.lastClaimUnix < msecToSec(d.getTime());
}

function getTotalDayConnected(dailyReward: any): number {
    if (!dailyReward.totalDay) {
        dailyReward.totalDay = 0;
    }

    return dailyReward.totalDay;
}

function rpcCanClaimDailyReward(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    var dailyReward = getLastDailyRewardObject(context, logger, nk, payload);
    var response = {
        canClaimDailyReward: canUserClaimDailyReward(dailyReward),
        totalDayConnected: dailyReward.totalDay,
    }

    var result = JSON.stringify(response);
    logger.debug('rpcCanClaimDailyReward response: %q', result);

    return result;
}

function rpcClaimDailyReward(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {

    var reward: Reward = {
        coinsReceived: 0,
        gemsReceived: 0,
        blastReceived: null,
        itemReceived: null,
    };

    var dailyReward = getLastDailyRewardObject(context, logger, nk, payload);

    if (canUserClaimDailyReward(dailyReward)) {

        var totalDay = getTotalDayConnected(dailyReward);
        reward = getDayReward(totalDay);

        var notification: nkruntime.NotificationRequest = {
            code: notificationOpCodes.CURENCY,
            content: reward,
            persistent: false,
            subject: "You've received a new item",
            userId: context.userId,
        }

        if (reward.coinsReceived != 0) {
            updateWalletWithCurrency(nk, context.userId, Currency.Coins, reward.coinsReceived);

            notification = {
                code: notificationOpCodes.CURENCY,
                content: reward,
                persistent: false,
                subject: "You've received a new currency",
                userId: context.userId,
            }

                    try {
            nk.notificationsSend([notification]);
        } catch (error) {
            logger.error('notificationsSend error: %q', error);
            throw error;
        }

        }
        if (reward.gemsReceived != 0) {
            updateWalletWithCurrency(nk, context.userId, Currency.Gems, reward.gemsReceived);

            notification = {
                code: notificationOpCodes.CURENCY,
                content: reward,
                persistent: false,
                subject: "You've received a new currency",
                userId: context.userId,
            }

            try {
                nk.notificationsSend([notification]);
            } catch (error) {
                logger.error('notificationsSend error: %q', error);
                throw error;
            }
        }

        if (reward.blastReceived != null) {
            addBlast(nk, logger, context.userId, reward.blastReceived);
        }

        if (reward.itemReceived != null) {
            addItem(nk, logger, context.userId, reward.itemReceived);
        }

        dailyReward.lastClaimUnix = msecToSec(Date.now());
        dailyReward.totalDay = totalDay + 1;

        var write: nkruntime.StorageWriteRequest = {
            collection: DailyRewardCollectionName,
            key: DailyRewardCollectionKey,
            permissionRead: DailyRewardPermissionRead,
            permissionWrite: DailyRewardPermissionWrite,
            value: dailyReward,
            userId: context.userId,
        }

        if (dailyReward.version) {
            // Use OCC to prevent concurrent writes.
            write.version = dailyReward.version
        }

        // Update daily reward storage object for user.
        try {
            nk.storageWrite([write])
        } catch (error) {
            logger.error('storageWrite error: %q', error);
            throw error;
        }
    }

    var result = JSON.stringify(reward);
    logger.debug('rpcClaimDailyReward response: %q', result)

    return result;
}

function getDayReward(totalDay: number): Reward {
    return allReward[totalDay % allReward.length];
}


// Data

const allReward: Reward[] = [
    { coinsReceived: 500, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 10, blastReceived: null, itemReceived: null },
    { coinsReceived: 1000, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 15, blastReceived: null, itemReceived: null },
    { coinsReceived: 2000, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 30, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 0, blastReceived: null, itemReceived: { data_id: 7, amount: 5 } },
    { coinsReceived: 0, gemsReceived: 50, blastReceived: null, itemReceived: null },
    { coinsReceived: 750, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 12, blastReceived: null, itemReceived: null },
    { coinsReceived: 1200, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 18, blastReceived: null, itemReceived: null },
    { coinsReceived: 2500, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 0, blastReceived: { 
        uuid: generateUUID(), 
        data_id: Clawball.id,
        exp: calculateExperienceFromLevel(10), 
        iv: getRandomIV(), 
        boss: false, 
        shiny: true, 
        activeMoveset: getRandomActiveMoveset(Clawball, calculateExperienceFromLevel(10)) 
    }, itemReceived: null },
    { coinsReceived: 6000, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 60, blastReceived: null, itemReceived: null },
    { coinsReceived: 900, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 14, blastReceived: null, itemReceived: null },
    { coinsReceived: 1400, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 20, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 0, blastReceived: null, itemReceived: { data_id: 8, amount: 10 } },
    { coinsReceived: 0, gemsReceived: 40, blastReceived: null, itemReceived: null },
    { coinsReceived: 7000, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 70, blastReceived: null, itemReceived: null },
    { coinsReceived: 1000, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 16, blastReceived: null, itemReceived: null },
    { coinsReceived: 1600, gemsReceived: 0, blastReceived: null, itemReceived: null },
    { coinsReceived: 0, gemsReceived: 0, blastReceived: { 
        uuid: generateUUID(), 
        data_id: Cerberus.id,
        exp: calculateExperienceFromLevel(25), 
        iv: getRandomIV(), 
        boss: false, 
        shiny: true, 
        activeMoveset: getRandomActiveMoveset(Cerberus, calculateExperienceFromLevel(25)) 
    }, itemReceived: null },
];

const rpcLoadAllDailyReward: nkruntime.RpcFunction =
    function (): string {
        return JSON.stringify(allReward);
    }

