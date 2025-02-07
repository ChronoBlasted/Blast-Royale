using Nakama;
using Nakama.TinyJson;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum BattleState
{
    WAITING,
    READY,
    START,
    END,
}

public enum PlayerState
{
    BUSY,
    READY,
    WAITFORATTACK,
    WAITFORCHANGEBLAST,
    WAITFORUSEITEM,
    WAITTURN,
}

public class NakamaWildBattle : MonoBehaviour
{
    IClient _client;
    ISession _session;
    ISocket _socket;
    IMatch _match;

    Action<IMatchState> _matchStateHandler;

    string _matchId;
    PlayerState _playerState;

    public PlayerState PlayerState { get => _playerState; }

    public void Init(IClient client, ISession session, ISocket socket)
    {
        _client = client;
        _session = session;
        _socket = socket;

        _matchStateHandler = m => UnityMainThreadDispatcher.Instance().Enqueue(() => OnReceivedMatchState(m));
    }

    public async void FindWildBattle()
    {
        try
        {
            UIManager.Instance.ChangeView(UIManager.Instance.LoadingBattleView);

            var response = await _client.RpcAsync(_session, "findWildBattle");

            _matchId = response.Payload.FromJson<string>();
            _match = await _socket.JoinMatchAsync(_matchId);

            StartWildBattle();
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Could not join / find match: " + e.Message);
        }
    }

    void StartWildBattle()
    {
        _socket.ReceivedMatchState += _matchStateHandler;
    }

    public async Task CancelMatchmaking()
    {
        await _socket.LeaveMatchAsync(_matchId);

        _socket.ReceivedMatchState -= _matchStateHandler;

        _matchId = null;
    }


    private void OnReceivedMatchState(IMatchState matchState)
    {
        string messageJson = Encoding.UTF8.GetString(matchState.State);

        if (messageJson == "") Debug.Log("OpCodes recu : " + matchState.OpCode);
        else Debug.Log("OpCodes recu : " + matchState.OpCode + ",Message en Json : " + messageJson);

/*        switch (matchState.OpCode)
        {
            // TODO add match logic
        }*/
    }
}


// Receive data

[Serializable]
public class StartStateData
{
    public int id;
    public int exp;
    public int iv;
    public Status status;
    public List<int> activeMoveset;
}

[Serializable]
public class TurnStateData
{
    public int p_move_damage;
    public Status p_move_status;

    public TurnType wb_turn_type;
    public int wb_move_index;
    public int wb_move_damage;
    public Status wb_move_status;

    public bool catched;
}

// Send data

public class ItemUseJSON
{
    public int index_item;
    public int index_blast;
}

public enum TurnType { NONE, ATTACK, ITEM, SWAP, WAIT }