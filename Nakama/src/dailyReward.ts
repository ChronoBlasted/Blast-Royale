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
            addItem(nk, logger, context, reward.itemReceived);
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
    var reward: Reward = {
        coinsReceived: 0,
        gemsReceived: 0,
        blastReceived: null,
        itemReceived: null,
    };

    switch (totalDay % 7) {
        case 0:
            reward = Reward0
            break;
        case 1:
            reward = Reward1
            break;
        case 2:
            reward = Reward2
            break;
        case 3:
            reward = Reward3
            break;
        case 4:
            reward = Reward4
            break;
        case 5:
            reward = Reward5
            break;
        case 6:
            reward = Reward6
            break;
    }

    return reward;
}

// Data

const Reward0: Reward = {
    coinsReceived: 500,
    gemsReceived: 0,
    blastReceived: null,
    itemReceived: null,
};

const Reward1: Reward = {
    coinsReceived: 0,
    gemsReceived: 10,
    blastReceived: null,
    itemReceived: null,
};

const Reward2: Reward = {
    coinsReceived: 1000,
    gemsReceived: 0,
    blastReceived: null,
    itemReceived: null,
};

const Reward3: Reward = {
    coinsReceived: 0,
    gemsReceived: 15,
    blastReceived: null,
    itemReceived: null,
};

const Reward4: Reward = {
    coinsReceived: 2000,
    gemsReceived: 0,
    blastReceived: null,
    itemReceived: null,
};

const Reward5: Reward = {
    coinsReceived: 0,
    gemsReceived: 30,
    blastReceived: null,
    itemReceived: null,
};

const Reward6: Reward = {
    coinsReceived: 5000,
    gemsReceived: 0,
    blastReceived: null,
    itemReceived: null,
};


const allReward: Reward[] = [
    Reward0,
    Reward1,
    Reward2,
    Reward3,
    Reward4,
    Reward5,
    Reward6,
]


const rpcLoadAllDailyReward: nkruntime.RpcFunction =
    function (): string {
        return JSON.stringify(allReward);
    }

