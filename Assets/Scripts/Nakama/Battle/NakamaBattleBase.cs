using Nakama;
using Nakama.TinyJson;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class NakamaBattleBase : MonoBehaviour
{
    [SerializeField] protected string _matchName = "findPvPBattle";
    [SerializeField] protected string _watchAdName = "watchPvEBattleAds";

    protected IClient _client;
    protected ISession _session;
    protected ISocket _socket;
    protected IMatch _match;
    protected string _matchId;
    protected StartStateData _startStateData;

    protected Action<IMatchState> _matchStateHandler;

    public UnityEvent OnBattleEnd;
    public BattleBase BattleManager;

    public virtual void Init(IClient client, ISession session, ISocket socket)
    {
        _client = client;
        _session = session;
        _socket = socket;

        BattleManager.Init();

        _matchStateHandler = m => UnityMainThreadDispatcher.Instance().Enqueue(() => HandleMatchState(m));
    }
    public async void FindBattle()
    {
        try
        {
            UIManager.Instance.ChangeView(UIManager.Instance.LoadingBattleView);

            var response = await _client.RpcAsync(_session, _matchName);
            _matchId = response.Payload.FromJson<string>();

            await JoinMatchById(_matchId);
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("PvE: Could not join / find match: " + e.Message);
        }
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

    public async void PlayerLeaveMatch(bool isWin)
    {
        try
        {
            await LeaveMatch();

            await NakamaManager.Instance.NakamaUserAccount.GetPlayerMetadata();
            await NakamaManager.Instance.NakamaLeaderboards.UpdateLeaderboards();

            OnBattleEnd?.Invoke();

            BattleManager.MatchEnd(isWin.ToString());
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("PvP: Error Player Leave: " + e.Message);
        }
    }


    public async Task PlayerAttack(int indexAttack)
    {
        try
        {
            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_ATTACK, indexAttack.ToJson(), null);

            BattleManager.WaitForOpponent();
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Attack: " + e.Message);
        }
    }

    public async Task PlayerUseItem(int indexItem, int indexSelectedBlast = 0)
    {
        try
        {
            ItemUseJSON itemUseJson = new ItemUseJSON();

            itemUseJson.index_item = indexItem;
            itemUseJson.index_blast = indexSelectedBlast;

            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_USE_ITEM, itemUseJson.ToJson(), null);

            BattleManager.WaitForOpponent();
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Use Item: " + e.Message);
        }

    }

    public async void PlayerChangeBlast(int indexSelectedBlast)
    {
        try
        {
            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_CHANGE_BLAST, indexSelectedBlast.ToJson(), null);

            BattleManager.WaitForOpponent();
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Change Blast: " + e.Message);
        }

    }

    public async Task PlayerWait()
    {
        try
        {
            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_WAIT, "");

            BattleManager.WaitForOpponent();
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Wait : " + e.Message);
        }
    }
    public async Task PlayerChooseOffer(int indexOffer)
    {
        try
        {
            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_CHOOSE_OFFER, indexOffer.ToJson(), null);

            BattleManager.WaitForOpponent();
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Choose Offer: " + e.Message);
        }
    }

    public async void HandleOnRewardsAds()
    {
        try
        {
            var response = await _client.RpcAsync(_session, _watchAdName);
            BattleManager.BonusAds = true;
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("PvE: Could not reward ad: " + e.Message);
        }
    }

    public virtual void HandleMatchState(IMatchState matchState)
    {
        string messageJson = DecodeMatchState(matchState);

        Debug.Log($" OpCode: {matchState.OpCode} | Data: {messageJson}");

        switch (matchState.OpCode)
        {
            case NakamaOpCode.MATCH_START:
                _startStateData = JsonUtility.FromJson<StartStateData>(messageJson);
                BattleManager.StartBattle(_startStateData);
                break;

            case NakamaOpCode.ENEMY_READY:
                BattleManager.StartNewTurn();
                break;

            case NakamaOpCode.MATCH_ROUND:
                var turnState = messageJson.FromJson<TurnStateData>();
                BattleManager.PlayTurn(turnState);
                break;

            case NakamaOpCode.MATCH_END:
                PlayerLeaveMatch(bool.Parse(messageJson));
                break;

            case NakamaOpCode.ERROR_SERV:
                BattleManager.StartNewTurn();
                ErrorManager.Instance.ShowError(ErrorType.SERVER_ERROR);
                break;

            case NakamaOpCode.NEW_BLAST:
                var newBlast = JsonUtility.FromJson<NewBlastData>(messageJson);
                BattleManager.SetOpponent(newBlast);
                break;
        }
    }

    private void OnApplicationQuit()
    {
        if (_matchId != null) _ = LeaveMatch();
    }
}
