using Nakama;
using Nakama.TinyJson;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class NakamaArea : MonoBehaviour
{
    IClient _client;
    ISession _session;

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await GetAllArea();
    }

    async Task GetAllArea()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadAllArea");

            List<AreaData> allAreaList = response.Payload.FromJson<List<AreaData>>();

            NakamaData.Instance.AreaCollection = allAreaList;

            UIManager.Instance.AllAreaView.UpdateAllArea(allAreaList);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async Task SelectArea(int areaID)
    {
        try
        {
            var response = await _client.RpcAsync(_session, "selectArea", areaID.ToString());
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
}

public class AreaData
{
    public int id;
    public int trophyRequired;
    public int[] blastIds;
    public int[] blastLevels;
}
