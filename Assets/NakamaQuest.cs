using Nakama;
using Nakama.TinyJson;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaQuest : MonoBehaviour
{
    IClient _client;
    ISession _session;

    List<DailyQuestData> _dailyQuests = new List<DailyQuestData>();

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await LoadDailyQuest();
    }

    public async Task LoadDailyQuest()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadDailyQuest");

            _dailyQuests = response.Payload.FromJson<List<DailyQuestData>>();


            UIManager.Instance.QuestPopup.Init(_dailyQuests);
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

            var reward = response.Payload.FromJson<RewardCollection>();

            UIManager.Instance.RewardPopup.OpenPopup();
            UIManager.Instance.RewardPopup.UpdateData(reward);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
}

public class DailyQuestData
{
    public string id;
    public int goal;
    public int progress;
}