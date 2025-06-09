const DailyQuestPermissionRead = 2;
const DailyQuestPermissionWrite = 0;
const DailyQuestCollectionName = "daily_quests";
const DailyQuestStorageKey = "current";

const QuestIds = {
  LOGIN: "login",
  DEFEAT_MONSTER: "defeat_monster",
  CATCH_BLAST: "catch_blast",
  WATCH_AD: "watch_ad",
} as const;

type QuestIdType = (typeof QuestIds)[keyof typeof QuestIds];

const QuestDefinitions: Record<string, { goal: number }> = {
  [QuestIds.LOGIN]: { goal: 1 },
  [QuestIds.DEFEAT_MONSTER]: { goal: 5 },
  [QuestIds.CATCH_BLAST]: { goal: 2 },
  [QuestIds.WATCH_AD]: { goal: 1 },
};

interface DailyQuestData {
  quests: Array<{
    id: QuestIdType;
    goal: number;
    progress: number;
  }>;
  lastReset: number;
  rewardCount: number;
}

function generateDailyQuests(): DailyQuestData {
  return {
    quests: Object.entries(QuestDefinitions).map(([id, def]) => ({
      id: id as QuestIdType,
      goal: def.goal,
      progress: 0,
    })),
    lastReset: Date.now(),
    rewardCount: 0
  };
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

  if (records.length > 0) {
    dailyData = records[0].value as DailyQuestData;
  } else {
    dailyData = generateDailyQuests();
  }

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
  }

  const result = JSON.stringify(dailyData.quests);
  logger.debug("rpcGetDailyQuests response: %q", result);
  return result;
}

function incrementQuest(userId: string, questId: string, amount: number, nk: nkruntime.Nakama, logger: nkruntime.Logger) {
  const records = nk.storageRead([
    { collection: DailyQuestCollectionName, key: DailyQuestStorageKey, userId }
  ]);

  if (!records.length) return;

  const record = records[0];
  const data = record.value;
  const version = record.version;

  const quest = data.quests.find((q: any) => q.id === questId);
  if (!quest || quest.reward_claimed) return;

  quest.progress = Math.min(quest.goal, quest.progress + amount);

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
  } catch (error) {
    logger.error("incrementQuest storageWrite error: %q", error);
    throw error;
  }
}
function rpcClaimDailyQuestReward(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {

  const records = nk.storageRead([{ collection: DailyQuestCollectionName, key: DailyQuestStorageKey, userId: context.userId }]);
  if (!records.length) throw new Error("No daily quests found");

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


const rewardList: Reward[] = [
  { coinsReceived: 0, gemsReceived: 2, blastReceived: null, itemReceived: null },
  { coinsReceived: 2000, gemsReceived: 0, blastReceived: null, itemReceived: null },
  { coinsReceived: 1000, gemsReceived: 0, blastReceived: null, itemReceived: null },
  { coinsReceived: 0, gemsReceived: 10, blastReceived: null, itemReceived: null },
];

