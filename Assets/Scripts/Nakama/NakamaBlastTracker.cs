using Nakama;
using Nakama.TinyJson;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaBlastTracker : MonoBehaviour
{
    IClient _client;
    ISession _session;


    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await LoadBlastTracker();
    }

    async Task LoadBlastTracker()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadBlastTracker");

            var _blastTracker = response.Payload.FromJson<Dictionary<string, BlastTrackerEntry>>();

            NakamaData.Instance.BlastTracker = _blastTracker;
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public void AddBlastTrackerEntry(string blastId, string version)
    {
        NakamaData.Instance.BlastTracker[blastId].versions[version].catched = true;

        UIManager.Instance.PediaView.UpdateBlastPediaByBlastID(blastId);
    }


    public async Task ClaimFirstCatchReward(string blastId, string blastVersion)
    {
        try
        {
            var payloadDict = new Dictionary<string, object>
            {
                { "monsterId", blastId },
                { "version", blastVersion }
            };

            var response = await _client.RpcAsync(_session, "claimFirstCatch", payloadDict.ToJson());
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
}

public class BlastVersionData
{
    public bool catched;
    public bool rewardClaimed;
}

public class BlastTrackerEntry
{
    public Dictionary<string, BlastVersionData> versions;
}