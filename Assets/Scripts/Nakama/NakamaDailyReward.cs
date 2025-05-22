using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaDailyReward : MonoBehaviour
{
    IClient _client;
    ISession _session;

    CanClaimDailyReward _canClaimDailyReward;

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await LoadAllDailyReward();
        await CanClaimDailyReward();
    }

    private async Task<bool> CanClaimDailyReward()
    {
        try
        {
            var canClaimRewardResponse = await _client.RpcAsync(_session, "canClaimDailyReward");

            _canClaimDailyReward = canClaimRewardResponse.Payload.FromJson<CanClaimDailyReward>();

            UIManager.Instance.DailyRewardView.UpdateTotalDay(_canClaimDailyReward.totalDayConnected);
            UIManager.Instance.DailyRewardView.SetActiveReward(_canClaimDailyReward.totalDayConnected, _canClaimDailyReward.canClaimDailyReward);

            return _canClaimDailyReward.canClaimDailyReward;
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }

        return false;
    }

    public async Task ClaimDailyReward()
    {
        if (_canClaimDailyReward.canClaimDailyReward == false) return;

        try
        {
            var claimRewardResponse = await _client.RpcAsync(_session, "claimDailyReward");

            // TODO Add reward popup

            await CanClaimDailyReward();

        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async Task LoadAllDailyReward()
    {
        try
        {
            var loadAllDailyReward = await _client.RpcAsync(_session, "loadAllDailyReward");

            var loadAllDailyRewardList = loadAllDailyReward.Payload.FromJson<List<RewardCollection>>();

            UIManager.Instance.DailyRewardView.UpdateDailyRewards(loadAllDailyRewardList);

        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

}

[Serializable]
public class RewardCollection
{
    public int offer_id = -1;
    public int coinsReceived;
    public int gemsReceived;
    public Blast blastReceived;
    public Item itemReceived;
}

public class CanClaimDailyReward
{
    public bool canClaimDailyReward;
    public int totalDayConnected;
}

