using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class NakamaPvEBattle : NakamaBattleBase
{
    private PvEBattleManager _battleManager;
    private StartStateData _startStateData;

    public UnityEvent OnPvEBattleEnd;

    public override void Init(IClient client, ISession session, ISocket socket)
    {
        base.Init(client, session, socket);
        _battleManager = PvEBattleManager.Instance;
    }

    public async void FindBattle()
    {
        try
        {
            UIManager.Instance.ChangeView(UIManager.Instance.LoadingBattleView);

            var response = await _client.RpcAsync(_session, "findPvEBattle");
            string matchId = response.Payload.FromJson<string>();

            await JoinMatchById(matchId);
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("PvE: Could not join / find match: " + e.Message);
        }
    }

    protected override void HandleMatchState(IMatchState matchState)
    {
        string messageJson = DecodeMatchState(matchState);

        Debug.Log($"[PvE] OpCode: {matchState.OpCode} | Data: {messageJson}");

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
                var turnState = messageJson.FromJson<TurnStateData>();
                _battleManager.PlayTurn(turnState);
                break;

            case NakamaOpCode.MATCH_END:
                PlayerLeaveMatch(bool.Parse(messageJson));
                break;

            case NakamaOpCode.ERROR_SERV:
                _battleManager.StartNewTurn();
                ErrorManager.Instance.ShowError(ErrorType.SERVER_ERROR);
                break;

            case NakamaOpCode.NEW_BLAST:
                var newBlast = JsonUtility.FromJson<NewBlastData>(messageJson);
                _battleManager.SetNewWildBlast(newBlast);
                break;

            case NakamaOpCode.NEW_OFFER_TURN:
                var newOffers = JsonUtility.FromJson<OfferTurnStateData>(messageJson);
                var offerList = new List<Offer> { newOffers.offerOne, newOffers.offerTwo, newOffers.offerThree };
                _battleManager.SetNewOffers(offerList);
                break;
        }
    }

    public async void PlayerLeaveMatch(bool isWin)
    {
        try
        {
            await LeaveMatch();
            await NakamaManager.Instance.NakamaUserAccount.GetPlayerMetadata();
            await NakamaManager.Instance.NakamaLeaderboards.UpdateLeaderboards();

            OnPvEBattleEnd?.Invoke();
            _battleManager.MatchEnd(isWin.ToString());
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("PvE: Error Player Leave: " + e.Message);
        }
    }

    public async Task PlayerAttack(int indexAttack) =>

        await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_ATTACK, indexAttack.ToJson());

    public async Task PlayerUseItem(int indexItem, int indexSelectedBlast = 0)
    {
        var json = new ItemUseJSON { index_item = indexItem, index_blast = indexSelectedBlast }.ToJson();
        await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_USE_ITEM, json);
    }

    public async Task PlayerChooseOffer(int indexOffer) =>
        await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_CHOOSE_OFFER, indexOffer.ToJson());

    public async Task PlayerWait() =>
        await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_WAIT, "");

    public async void PlayerChangeBlast(int indexSelectedBlast) =>
        await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_CHANGE_BLAST, indexSelectedBlast.ToJson());

    public async void HandleOnPvERewardsAds()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "watchPvEBattleAds");
            PvEBattleManager.Instance.BonusAds = true;
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("PvE: Could not reward ad: " + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        if (_matchId != null) LeaveMatch();
    }
}
