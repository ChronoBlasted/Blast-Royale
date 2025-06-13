const DailyQuestPermissionRead = 2;
const DailyQuestPermissionWrite = 0;
const DailyQuestCollectionName = "daily_quests";
const DailyQuestStorageKey = "daily_quests_key";

enum QuestIds {
  LOGIN = "login",
  DEFEAT_BLAST = "defeat_blast",
  CATCH_BLAST = "catch_blast",
  WATCH_AD = "watch_ad",
}

const QuestDefinitions = {
  [QuestIds.LOGIN]: { goal: 1 } as QuestDefinition,
  [QuestIds.DEFEAT_BLAST]: { goal: 5 } as QuestDefinition,
  [QuestIds.CATCH_BLAST]: { goal: 2 } as QuestDefinition,
  [QuestIds.WATCH_AD]: { goal: 1 } as QuestDefinition,
};

type QuestDefinitionsType = {
  [key in QuestIds]: QuestDefinition;
};

type QuestDefinition = {
  goal: number;
};


interface DailyQuest {
    id: string;    
    goal: number;  
    progress: number; 
}

interface DailyQuestData {
  quests: DailyQuest[];
  lastReset: number;
  rewardCount: number;
}

function generateDailyQuests(): DailyQuestData {
  return {
    quests: Object.entries(QuestDefinitions).map(([id, def]) => ({
      id: id,
      goal: def.goal,
      progress: 0,
    })) as DailyQuest[],
    lastReset: Date.now(),
    rewardCount: 0
  };
}

function createDailyQuestStorageIfNeeded(userId: string, nk: nkruntime.Nakama, logger: nkruntime.Logger): void {
  const records = nk.storageRead([{
    collection: DailyQuestCollectionName,
    key: DailyQuestStorageKey,
    userId
  }]);

  if (records.length === 0) {
    const dailyData = generateDailyQuests();
    nk.storageWrite([{
      collection: DailyQuestCollectionName,
      key: DailyQuestStorageKey,
      userId,
      value: dailyData,
      permissionRead: DailyQuestPermissionRead,
      permissionWrite: DailyQuestPermissionWrite
    }]);
    logger.debug(`createDailyQuestStorageIfNeeded: Created daily quest storage for userId=${userId}`);
  } else {
    logger.debug(`createDailyQuestStorageIfNeeded: Storage already exists for userId=${userId}`);
  }
}

function rpcGetDailyQuests(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
  const userId = context.userId;

  if (!userId) {
    throw new Error("User not authenticated.");
  }

  const records = nk.storageRead([{
    collection: DailyQuestCollectionName,
    key: DailyQuestStorageKey,
    userId
  }]);

  let dailyData: DailyQuestData;

  dailyData = records[0].value as DailyQuestData;

  if (isDailyResetDue(dailyData.lastReset)) {
    dailyData = generateDailyQuests();

    nk.storageWrite([{
      collection: DailyQuestCollectionName,
      key: DailyQuestStorageKey,
      userId,
      value: dailyData,
      permissionRead: DailyQuestPermissionRead,
      permissionWrite: DailyQuestPermissionWrite
    }]);
    logger.debug(`rpcGetDailyQuests: Reset daily quest storage for userId=${userId}`);
  }

  const result = JSON.stringify(dailyData.quests);
  logger.debug("rpcGetDailyQuests response: %q", result);
  return result;
}

function incrementQuest(userId: string, questId: string, amount: number, nk: nkruntime.Nakama, logger: nkruntime.Logger) {
  const records = nk.storageRead([{
    collection: DailyQuestCollectionName,
    key: DailyQuestStorageKey,
    userId
  }]);

  if (!records.length) {
    logger.debug(`incrementQuest: No daily quest record found for userId=${userId}`);
    return;
  }

  const record = records[0];
  const data = record.value as DailyQuestData;
  const version = record.version;

  const quest = data.quests.find((q: DailyQuest) => q.id === questId);
  if (!quest) {
    logger.debug(`incrementQuest: Quest with id=${questId} not found for userId=${userId}`);
    return;
  }

  const oldProgress = quest.progress;
  quest.progress = Math.min(quest.goal, quest.progress + amount);
  logger.debug(`incrementQuest: Updated quest '${questId}' for userId=${userId} from progress=${oldProgress} to progress=${quest.progress} (goal=${quest.goal})`);

  const writeRequest: nkruntime.StorageWriteRequest = {
    collection: DailyQuestCollectionName,
    key: DailyQuestStorageKey,
    userId: userId,
    value: data,
    version: version,
    permissionRead: DailyQuestPermissionRead,
    permissionWrite: DailyQuestPermissionWrite,
  };

  try {
    nk.storageWrite([writeRequest]);
    logger.debug(`incrementQuest: Successfully wrote updated quest data for userId=${userId}`);
  } catch (error) {
    logger.error("incrementQuest storageWrite error: %q", error);
    throw error;
  }
}

function rpcClaimDailyQuestReward(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {

  const records = nk.storageRead([{ collection: DailyQuestCollectionName, key: DailyQuestStorageKey, userId: context.userId }]);
  if (!records.length) throw new Error("No daily quests foundddd");

  const data = records[0].value as DailyQuestData;
  const version = records[0].version;

  const finishedCount = data.quests.filter(q => q.progress >= q.goal).length;

  if (finishedCount <= data.rewardCount) {
    throw new Error("No rewards to claim");
  }

  const reward = rewardList[data.rewardCount];

  if (reward.coinsReceived != 0) {
    updateWalletWithCurrency(nk, context.userId, Currency.Coins, reward.coinsReceived);
  }

  if (reward.gemsReceived != 0) {
    updateWalletWithCurrency(nk, context.userId, Currency.Gems, reward.gemsReceived);
  }

  data.rewardCount++;

  try {
    nk.storageWrite([{
      collection: DailyQuestCollectionName,
      key: DailyQuestStorageKey,
      userId: context.userId,
      value: data,
      version,
      permissionRead: DailyQuestPermissionRead,
      permissionWrite: DailyQuestPermissionWrite,
    }]);
  } catch (error) {
    logger.error("claimReward storageWrite error: %q", error);
    throw error;
  }

  return JSON.stringify(reward);
}

function rpcGetDailyQuestRewards(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    const userId = context.userId;
    if (!userId) throw new Error("User not authenticated.");

    const records = nk.storageRead([{
        collection: DailyQuestCollectionName,
        key: DailyQuestStorageKey,
        userId
    }]);

    if (!records.length) throw new Error("No daily quest record found for userId=" + userId);

    const data = records[0].value as DailyQuestData;

    const response = {
        rewards: rewardList,
        rewardCount: data.rewardCount
    };

    logger.debug(`rpcGetDailyQuestRewards: userId=${userId} rewardCount=${data.rewardCount}`);
    return JSON.stringify(response);
}


const rewardList: Reward[] = [
  { coinsReceived: 0, gemsReceived: 2, blastReceived: null, itemReceived: null },
  { coinsReceived: 2000, gemsReceived: 0, blastReceived: null, itemReceived: null },
  { coinsReceived: 10000, gemsReceived: 0, blastReceived: null, itemReceived: null },
  { coinsReceived: 0, gemsReceived: 10, blastReceived: null, itemReceived: null },
];

