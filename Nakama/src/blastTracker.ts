const BlastTrackerCollection = "blasts_tracker_collection";
const BlastTrackerKey = "blasts_tracker_collection_key";
const PermissionRead = 2;
const PermissionWrite = 0;

interface BlastTrackerData {
    [monsterId: string]: BlastTrackerEntry;
}

interface BlastTrackerEntry {
    versions: { [version: string]: BlastVersionData };
}

interface BlastVersionData {
    catched: boolean;
    rewardClaimed: boolean;
}


function initializeBlastTrackerData(userId: string, nk: nkruntime.Nakama, logger: nkruntime.Logger): void {
  const records = nk.storageRead([{ collection: BlastTrackerCollection, key: BlastTrackerKey, userId }]);

  if (records.length === 0) {
    const initialData: BlastTrackerData = {};

    for (const blast of blastPedia) {
      initialData[blast.id] = { versions: {
        1: { catched: false, rewardClaimed: false },
        2: { catched: false, rewardClaimed: false },
        3: { catched: false, rewardClaimed: false }
      }};
    }

    try {
      nk.storageWrite([{
        collection: BlastTrackerCollection,
        key: BlastTrackerKey,
        userId,
        value: initialData,
        permissionRead: PermissionRead,
        permissionWrite: PermissionWrite,
      }]);
    } catch (e) {
      logger.error("initializePlayerBlastData storageWrite error: %q", e);
      throw e;
    }
  }
}

function markMonsterCaptured(userId: string, monsterId: string, version: number, nk: nkruntime.Nakama, logger: nkruntime.Logger): void {
  const records = nk.storageRead([{ collection: BlastTrackerCollection, key: BlastTrackerKey, userId }]);
  if (records.length === 0) {
    throw new Error("No monster capture data found for user");
  }

  const playerData = records[0].value as BlastTrackerData;
  const versionData = playerData[monsterId]?.versions;

  if (!versionData) {
    throw new Error("Monster data not found");
  }

  if (!versionData[version]) {
    versionData[version] = { catched: false, rewardClaimed: false };
  }

  if (versionData[version].catched) {
    return;
  }

  versionData[version].catched = true;

  try {
    nk.storageWrite([{
      collection: BlastTrackerCollection,
      key: BlastTrackerKey,
      userId,
      value: playerData,
      version: records[0].version,
      permissionRead: PermissionRead,
      permissionWrite: PermissionWrite,
    }]);
  } catch (e) {
    logger.error("markMonsterCaptured storageWrite error: %q", e);
    throw e;
  }
}

function rpcGetAllBlastTrackerData(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    const userId = context.userId;
    if (!userId) {
        throw new Error("User not authenticated");
    }

    const records = nk.storageRead([{ collection: BlastTrackerCollection, key: BlastTrackerKey, userId }]);
    if (records.length === 0) {
        throw new Error("No monster capture data found");
    }

    return JSON.stringify(records[0].value as BlastTrackerData);
}


function rpcClaimFirstCaptureReward(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    const userId = context.userId;
    if (!userId) {
        throw new Error("User not authenticated");
    }

    const dataInput = JSON.parse(payload) as { monsterId: string, version: number };

    if (!dataInput.monsterId || dataInput.version === undefined) {
        throw new Error("Invalid input");
    }

    const records = nk.storageRead([{ collection: BlastTrackerCollection, key: BlastTrackerKey, userId }]);
    if (records.length === 0) {
        throw new Error("No monster capture data found");
    }

    const playerData = records[0].value;

    if (!playerData[dataInput.monsterId] || !playerData[dataInput.monsterId].versions) {
        throw new Error("Monster data not found");
    }

    const monsterVersions = playerData[dataInput.monsterId].versions as { [version: string]: BlastVersionData };

    if (!monsterVersions[dataInput.version] || !monsterVersions[dataInput.version].catched) {
        throw new Error("Monster/version not captured");
    }

    if (monsterVersions[dataInput.version].rewardClaimed) {
        throw new Error("Reward already claimed for this monster/version");
    }

    monsterVersions[dataInput.version].rewardClaimed = true;

    try {
        nk.storageWrite([{
            collection: BlastTrackerCollection,
            key: BlastTrackerKey,
            userId: userId,
            value: playerData,
            version: records[0].version,
            permissionRead: PermissionRead,
            permissionWrite: PermissionWrite,
        }]);
    } catch (e) {
        logger.error("storageWrite error: %q", e);
        throw e;
    }

    const reward = getRewardForMonsterVersion(dataInput.version);

    return JSON.stringify({ success: true, reward });
}

function getRewardForMonsterVersion(version: number): Reward {

    switch (version) {
        case 1:
            return {
                coinsReceived: 200,
                gemsReceived: 0,
                blastReceived: null,
                itemReceived: null,
            };
        case 2:
            return {
                coinsReceived: 1000,
                gemsReceived: 0,
                blastReceived: null,
                itemReceived: null,
            };
        case 3:
            return {
                coinsReceived: 0,
                gemsReceived: 10,
                blastReceived: null,
                itemReceived: null,
            };
        default:
            return {
                coinsReceived: 0,
                gemsReceived: 0,
                blastReceived: null,
                itemReceived: null,
            };
    }
}
