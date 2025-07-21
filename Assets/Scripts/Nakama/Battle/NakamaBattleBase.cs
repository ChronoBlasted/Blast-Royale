using Nakama;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class NakamaBattleBase : MonoBehaviour
{
    protected IClient _client;
    protected ISession _session;
    protected ISocket _socket;
    protected IMatch _match;
    protected string _matchId;

    protected Action<IMatchState> _matchStateHandler;

    public virtual void Init(IClient client, ISession session, ISocket socket)
    {
        _client = client;
        _session = session;
        _socket = socket;

        _matchStateHandler = m => UnityMainThreadDispatcher.Instance().Enqueue(() => HandleMatchState(m));
    }

    public async Task JoinMatchById(string matchId)
    {
        _matchId = matchId;
        _match = await _socket.JoinMatchAsync(_matchId);
        _socket.ReceivedMatchState += _matchStateHandler;
    }

    public async Task LeaveMatch()
    {
        if (_matchId == null) return;

        await _socket.LeaveMatchAsync(_matchId);
        _socket.ReceivedMatchState -= _matchStateHandler;
        _matchId = null;
    }

    public async void PlayerReady()
    {
        try
        {
            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_READY, "", null);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error player Ready : " + e.Message);
        }
    }

    public async Task SendMatchState(int opCode, string json)
    {
        try
        {
            await _socket.SendMatchStateAsync(_matchId, opCode, json, null);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error sending match state (OpCode {opCode}): {e.Message}");
        }
    }

    public async Task CancelMatchmaking()
    {
        if (_matchId == null) return;

        await _socket.LeaveMatchAsync(_matchId);

        _socket.ReceivedMatchState -= _matchStateHandler;

        _matchId = null;
    }

    protected string DecodeMatchState(IMatchState matchState)
    {
        return Encoding.UTF8.GetString(matchState.State);
    }

    protected abstract void HandleMatchState(IMatchState matchState);
}
