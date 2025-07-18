using Nakama;
using Nakama.TinyJson;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class NakamaWildBattle : MonoBehaviour
{
    IClient _client;
    ISession _session;
    ISocket _socket;
    IMatch _match;
    Action<IMatchState> _matchStateHandler;

    string _matchId;
    PlayerState _playerState;

    StartStateData _startStateData;

    public UnityEvent OnWildBattleEnd;
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

        Debug.Log("OpCodes recu : " + matchState.OpCode + ",Message en Json : " + messageJson);

        switch (matchState.OpCode)
        {
            case NakamaOpCode.MATCH_START:
                _startStateData = JsonUtility.FromJson<StartStateData>(messageJson);

                WildBattleManager.Instance.StartBattle(_startStateData);
                break;

            case NakamaOpCode.ENEMY_READY:
                WildBattleManager.Instance.StartNewTurn();
                break;

            case NakamaOpCode.MATCH_ROUND:
                var _turnState = messageJson.FromJson<TurnStateData>();

                WildBattleManager.Instance.PlayTurn(_turnState);
                break;
            case NakamaOpCode.MATCH_END:
                PlayerLeaveMatch(Boolean.Parse(messageJson));
                break;

            case NakamaOpCode.ERROR_SERV:
                WildBattleManager.Instance.StartNewTurn();

                ErrorManager.Instance.ShowError(ErrorType.SERVER_ERROR);
                break;
            case NakamaOpCode.NEW_BLAST:
                var newBlast = JsonUtility.FromJson<NewBlastData>(messageJson);
                WildBattleManager.Instance.SetNewWildBlast(newBlast);
                break;
            case NakamaOpCode.NEW_OFFER_TURN:
                var newOffers = JsonUtility.FromJson<OfferTurnStateData>(messageJson);
                var offerList = new List<Offer> { newOffers.offer_one, newOffers.offer_two, newOffers.offer_three };
                WildBattleManager.Instance.SetNewOffers(offerList);
                break;

            case NakamaOpCode.DEBUG:
                break;


            default:
                break;
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

            OnWildBattleEnd?.Invoke();

            WildBattleManager.Instance.MatchEnd(isWin.ToString());
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Leave: " + e.Message);
        }
    }


    public async Task PlayerAttack(int indexAttack)
    {
        try
        {

            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_ATTACK, indexAttack.ToJson(), null);

            WildBattleManager.Instance.WaitForOpponent();
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

            WildBattleManager.Instance.WaitForOpponent();
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

            WildBattleManager.Instance.WaitForOpponent();
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

            WildBattleManager.Instance.WaitForOpponent();
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Wait : " + e.Message);
        }
    }

    public async void HandleOnBonusRewardsAds()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "watchWildBattleAds");

            WildBattleManager.Instance.BonusAds = true;
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Could not join / find match: " + e.Message);
        }
    }

    public async Task PlayerChooseOffer(int indexOffer)
    {
        try
        {

            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_CHOOSE_OFFER, indexOffer.ToJson(), null);

            WildBattleManager.Instance.WaitForOpponent();
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Choose Offer: " + e.Message);
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

    private void OnApplicationQuit()
    {
        if (_matchId != null) LeaveMatch();
    }
}

// Receive data

[Serializable]
public class StartStateData
{
    public NewBlastData newBlastData;
    public Meteo meteo;
}

[Serializable]
public class NewBlastData
{
    public int id;
    public int exp;
    public int iv;
    public bool boss;
    public bool shiny;
    public Status status;
    public List<int> activeMoveset;
}

[Serializable]
public class TurnStateData
{
    public int p_move_damage;
    public List<MoveEffectData> p_move_effects;

    public TurnType wb_turn_type;
    public int wb_move_index;
    public int wb_move_damage;
    public List<MoveEffectData> wb_move_effects;

    public bool catched;
}

// Send data

public class ItemUseJSON
{
    public int index_item;
    public int index_blast;
}

[Serializable]
public class OfferTurnStateData
{
    public Offer offer_one;
    public Offer offer_two;
    public Offer offer_three;
}