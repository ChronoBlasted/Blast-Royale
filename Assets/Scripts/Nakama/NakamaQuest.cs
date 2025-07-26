using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaQuest : MonoBehaviour
{
    IClient _client;
    ISession _session;

    List<DailyQuest> _dailyQuests = new List<DailyQuest>();
    DailyQuestRewardData _dailyQuestRewards;

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await LoadDailyQuest();
        await LoadDailyQuestRewards();
    }

    async Task LoadDailyQuest()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadDailyQuest");

            _dailyQuests = response.Payload.FromJson<List<DailyQuest>>();

            UIManager.Instance.QuestPopup.Init(_dailyQuests);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public void UpdateQuest(QuestType type, int amount = 1)
    {
        int questIndex = _dailyQuests.FindIndex((x) => x.type == type);

        _dailyQuests[questIndex].progress += amount;

        if (_dailyQuests[questIndex].progress > _dailyQuests[questIndex].goal)
        {
            _dailyQuests[questIndex].progress = _dailyQuests[questIndex].goal;
        }

        UIManager.Instance.QuestPopup.Init(_dailyQuests);

        if (_dailyQuests[questIndex].progress == _dailyQuests[questIndex].goal)
        {
            UIManager.Instance.QuestPopup.RefreshRewards(_dailyQuestRewards);
        }
    }

    async Task LoadDailyQuestRewards()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadDailyQuestRewards");

            _dailyQuestRewards = response.Payload.FromJson<DailyQuestRewardData>();

            UIManager.Instance.QuestPopup.InitRewards(_dailyQuestRewards);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async Task ClaimQuestReward()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "claimDailyQuest");

            var reward = response.Payload.FromJson<Reward>();

            UIManager.Instance.RewardPopup.OpenPopup(false);
            UIManager.Instance.RewardPopup.UpdateData(reward);

            _dailyQuestRewards.rewardCount++;

            UIManager.Instance.QuestPopup.RefreshRewards(_dailyQuestRewards);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async Task ClaimAdQuest()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "claimAdQuest");

            UpdateQuest(QuestType.WatchAd);

            await LoadDailyQuestRewards();
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
}

[Serializable]
public class DailyQuest
{
    public QuestType type;
    public int goal;
    public int progress;
}

[Serializable]
public class DailyQuestRewardData
{
    public List<Reward> rewards;
    public int rewardCount;
}

public enum QuestType
{
    Login,
    DefeatBlast,
    CatchBlast,
    WatchAd,
}