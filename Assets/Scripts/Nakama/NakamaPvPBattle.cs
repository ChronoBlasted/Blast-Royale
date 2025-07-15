using BaseTemplate.Behaviours;
using Nakama;
using Nakama.TinyJson;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class NakamaPvPBattle : MonoSingleton<NakamaPvPBattle>
{
    IClient _client;
    ISession _session;
    ISocket _socket;
    IMatch _match;

    Action<IMatchState> _matchStateHandler;

    string _matchId;

    PlayerState _player1State;
    PlayerState _player2State;


    PvPBattleManager _battleManager;

    StartStateData _startStateData;

    public UnityEvent OnPvPBattleEnd;

    public PlayerState Player1State { get => _player1State; }
    public PlayerState Player2State { get => _player2State; }

    public void Init(IClient client, ISession session, ISocket socket)
    {
        _client = client;
        _session = session;
        _socket = socket;
        _battleManager = PvPBattleManager.Instance;

        _matchStateHandler = m => UnityMainThreadDispatcher.Instance().Enqueue(() => OnReceivedMatchState(m));
    }

    public async void FindBattle()
    {
        try
        {
            UIManager.Instance.ChangeView(UIManager.Instance.LoadingBattleView);

            var response = await _client.RpcAsync(_session, "findPvPBattle");

            _matchId = response.Payload.FromJson<string>();
            _match = await _socket.JoinMatchAsync(_matchId);

            StartBattle();
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Could not join / find match: " + e.Message);
        }
    }

    void StartBattle()
    {
        _socket.ReceivedMatchState += _matchStateHandler;
    }

    public async Task CancelMatchmaking()
    {
        if (_matchId == null) return;

        await _socket.LeaveMatchAsync(_matchId);

        _socket.ReceivedMatchState -= _matchStateHandler;

        _matchId = null;
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        string messageJson = Encoding.UTF8.GetString(matchState.State);

        Debug.Log("OpCodes recu : " + matchState.OpCode + ",Message en Json : " + messageJson);

        switch (matchState.OpCode)
        {
            case NakamaOpCode.MATCH_START:
                _startStateData = JsonUtility.FromJson<StartStateData>(messageJson);

                _battleManager.StartBattle(_startStateData);
                break;

            case NakamaOpCode.ENEMY_READY:
                _battleManager.StartNewTurn();
                break;

            case NakamaOpCode.MATCH_ROUND:
                var _turnState = messageJson.FromJson<TurnStateData>();

                _battleManager.PlayTurn(_turnState);
                break;
            case NakamaOpCode.MATCH_END:
                PlayerLeaveMatch(Boolean.Parse(messageJson));
                break;

            case NakamaOpCode.ERROR_SERV:
                _battleManager.StartNewTurn();

                ErrorManager.Instance.ShowError(ErrorType.SERVER_ERROR);
                break;
            case NakamaOpCode.NEW_BLAST:
                var newBlast = JsonUtility.FromJson<NewBlastData>(messageJson);
                _battleManager.SetNewOpponentBlast(newBlast);
                break;
            case NakamaOpCode.DEBUG:
                break;
            default:
                break;
        }
    }

    public async void PlayerReady()
    {
        try
        {
            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_READY, "", null);
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error player Ready : " + e.Message);
        }
    }

    public async void LeaveMatch()
    {
        try
        {
            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_LEAVE, "");
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error Player Leave: " + e.Message);
        }
    }

    public async void PlayerLeaveMatch(bool isWin)
    {
        try
        {
            await _socket.LeaveMatchAsync(_matchId);

            _socket.ReceivedMatchState -= _matchStateHandler;
            _matchId = null;

            await NakamaManager.Instance.NakamaUserAccount.GetPlayerMetadata(); // TODO Just update locally
            await NakamaManager.Instance.NakamaLeaderboards.UpdateLeaderboards(); // TODO Just update correct leaderboard

            OnPvPBattleEnd?.Invoke();

            _battleManager.MatchEnd(isWin.ToString());
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Leave: " + e.Message);
        }
    }

    public async void HandleOnPvERewardsAds()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "watchPvPBattleAds");

            PvPBattleManager.Instance.BonusAds = true;
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Could not join / find match: " + e.Message);
        }
    }

}
