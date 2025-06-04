using Nakama;
using Nakama.TinyJson;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class NakamaPedia : MonoBehaviour
{
    IClient _client;
    ISession _session;

    List<BlastData> _blastPedia = new List<BlastData>();
    List<ItemData> _itemPedia = new List<ItemData>();
    List<Move> _movePedia = new List<Move>();

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await LoadBlastPedia();
        await LoadItemPedia();
        await LoadMovePedia();
    }

    public async Task LoadBlastPedia()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadBlastPedia");

            _blastPedia = response.Payload.FromJson<List<BlastData>>();

            NakamaData.Instance.BlastPedia = _blastPedia;

            //Update Squad Panel
            UIManager.Instance.PediaView.UpdateBlastPedia(_blastPedia);

        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async Task LoadItemPedia()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadItemPedia");

            _itemPedia = response.Payload.FromJson<List<ItemData>>();

            NakamaData.Instance.ItemPedia = _itemPedia;

            //Update Squad Panel
            UIManager.Instance.PediaView.UpdateItemPedia(_itemPedia);

        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async Task LoadMovePedia()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadMovePedia");

            _movePedia = response.Payload.FromJson<List<Move>>();

            NakamaData.Instance.MovePedia = _movePedia;

            UIManager.Instance.PediaView.UpdateMovePedia(_movePedia);

        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
}
