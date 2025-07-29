using Nakama;
using Nakama.TinyJson;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class NakamaBattleBase : MonoBehaviour
{
    [SerializeField] protected string _matchName = "findPvPBattle";
    [SerializeField] protected string _watchAdName = "watchPvEBattleAds";

    protected IClient _client;
    protected ISession _session;
    protected ISocket _socket;
    protected IMatch _match;
    protected BattleState _battleState;
    protected string _matchId;
    protected StartStateData _startStateData;
    protected EndStateData _endStateData;

    protected Action<IMatchState> _matchStateHandler;

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
            _startStateData = new();
            _endStateData = new();
            _matchId = null;
            _battleState = BattleState.None;

            UIManager.Instance.ChangeView(UIManager.Instance.LoadingBattleView);

            await Task.Delay(100);

            var response = await _client.RpcAsync(_session, _matchName);
            _matchId = response.Payload.FromJson<string>();

            await EnsureSocketConnected();
            await JoinMatchById(_matchId);
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Could not join / find match: " + e.Message);
        }
        catch (SocketException ex)
        {
            Debug.LogError("Erreur socket : " + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Erreur inattendue dans FindBattle : " + ex.Message);
        }
    }


    public async Task JoinMatchById(string matchId)
    {
        _matchId = matchId;
        _match = await _socket.JoinMatchAsync(_matchId);
        _socket.ReceivedMatchState += _matchStateHandler;
    }

    async Task EnsureSocketConnected()
    {
        if (_socket == null || !_socket.IsConnected)
        {
            _socket = _client.NewSocket();
            _socket.ReceivedMatchState += _matchStateHandler;

            await _socket.ConnectAsync(_session);
            Debug.Log("Socket reconnecté.");
        }
    }


    public async void PlayerReady()
    {
        try
        {
            if (_endStateData.opponentSurrender)
            {
                BattleManager.GetBattleReward();
                return;
            }

            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_READY, "", null);

            _battleState = BattleState.Ready;

        }
        catch (Exception e)
        {
            Debug.LogWarning("Error player Ready : " + e.Message);
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



    public async Task PlayerAttack(int indexAttack)
    {
        try
        {
            PlayerActionData playerActionData = new PlayerActionData
            {
                type = TurnType.Attack,
                data = indexAttack
            };
            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_ATTACK, playerActionData.ToJson(), null);

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

            PlayerActionData playerActionData = new PlayerActionData
            {
                type = TurnType.Item,
                data = itemUseJson
            };

            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_USE_ITEM, playerActionData.ToJson());

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
            PlayerActionData playerActionData = new PlayerActionData
            {
                type = TurnType.Swap,
                data = indexSelectedBlast
            };

            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_CHANGE_BLAST, playerActionData.ToJson());

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
            PlayerActionData playerActionData = new PlayerActionData
            {
                type = TurnType.Wait
            };

            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_WAIT, playerActionData.ToJson());

            BattleManager.WaitForOpponent();
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Wait : " + e.Message);
        }
    }

    public async Task PlayerLeave()
    {
        try
        {
            PlayerActionData playerActionData = new PlayerActionData
            {
                type = TurnType.Leave
            };

            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_LEAVE, playerActionData.ToJson());
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Leave : " + e.Message);
        }
    }

    public void MatchEnd(EndStateData endStateData)
    {
        BattleManager.EndStateData = endStateData;

        _socket.ReceivedMatchState -= _matchStateHandler;
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

        TurnStateData turnState = new();

        switch (matchState.OpCode)
        {
            case NakamaOpCode.MATCH_START:
                _startStateData = JsonUtility.FromJson<StartStateData>(messageJson);
                BattleManager.StartBattle(_startStateData);
                _battleState = BattleState.Start;
                break;

            case NakamaOpCode.ENEMY_READY:
                BattleManager.StartNewTurn();
                _battleState = BattleState.Waiting;
                break;

            case NakamaOpCode.MATCH_ROUND:
                turnState = messageJson.FromJson<TurnStateData>();
                BattleManager.PlayTurn(turnState);
                BattleManager.WaitForOpponent();
                UIManager.Instance.GameView.DialogLayout.Hide();
                _battleState = BattleState.ResolveTurn;
                break;

            case NakamaOpCode.MATCH_END:
                _endStateData = messageJson.FromJson<EndStateData>();
                BattleManager.EndStateData = _endStateData;

                MatchEnd(_endStateData);
                _battleState = BattleState.End;
                break;

            case NakamaOpCode.OPPONENT_LEAVE:
                _endStateData = messageJson.FromJson<EndStateData>();
                _endStateData.opponentSurrender = true;

                _ = UIManager.Instance.GameView.DoShowMessage("Opponent surrender");

                MatchEnd(_endStateData);

                if (_battleState != BattleState.ResolveTurn)
                {
                    BattleManager.GetBattleReward();
                }
                break;
            case NakamaOpCode.ERROR_SERV:
                BattleManager.StartNewTurn();
                ErrorManager.Instance.ShowError(ErrorType.SERVER_ERROR);
                break;

            case NakamaOpCode.NEW_BLAST:
                var newBlast = JsonUtility.FromJson<NewBlastData>(messageJson);
                BattleManager.SetOpponent(newBlast);
                break;

            case NakamaOpCode.PLAYER_READY_MUST_CHANGE:
                BattleManager.StartNewMustSwapTurn();
                if (!NakamaLogic.IsBlastAlive(BattleManager.PlayerBlast)) BattleManager.PlayerMustChangeBlast();
                break;

            case NakamaOpCode.PLAYER_MUST_CHANGE_BLAST:
                UIManager.Instance.GameView.DialogLayout.Hide();
                turnState = messageJson.FromJson<TurnStateData>();
                BattleManager.PlayMustSwapTurn(turnState);
                UIManager.Instance.ChangeBlastPopup.ClosePopup();
                break;
        }
    }

    private void OnApplicationQuit()
    {
        _ = CancelMatchmaking();
    }
}

[Serializable]
public class PlayerActionData
{
    public TurnType type;
    public object data;
}
public enum BattleState
{
    None,
    Start,
    Waiting,
    Ready,
    ResolveTurn,
    WaitingForPlayerSwap,
    ReadyForPlayerSwap,
    ResolvePlayerSwap,
    WaitForPlayerChooseOffer,
    End,
}