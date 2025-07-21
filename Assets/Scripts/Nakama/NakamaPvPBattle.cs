using Nakama;
using Nakama.TinyJson;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaPvPBattle : NakamaBattleBase
{
    private PvPBattleManager _battleManager;
    private StartStateData _startStateData;

    public UnityEngine.Events.UnityEvent OnPvPBattleEnd;

    public override void Init(IClient client, ISession session, ISocket socket)
    {
        base.Init(client, session, socket);
        _battleManager = PvPBattleManager.Instance;
    }

    public async void FindBattle()
    {
        try
        {
            UIManager.Instance.ChangeView(UIManager.Instance.LoadingBattleView);

            var response = await _client.RpcAsync(_session, "findPvPBattle");
            string matchId = response.Payload.FromJson<string>();

            await JoinMatchById(matchId);
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("PvP: Could not join / find match: " + e.Message);
        }
    }

    protected override void HandleMatchState(IMatchState matchState)
    {
        string messageJson = DecodeMatchState(matchState);

        Debug.Log($"[PvP] OpCode: {matchState.OpCode} | Data: {messageJson}");

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
                _battleManager.SetNewOpponentBlast(newBlast);
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

            OnPvPBattleEnd?.Invoke();
            _battleManager.MatchEnd(isWin.ToString());
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("PvP: Error Player Leave: " + e.Message);
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
            Debug.LogWarning("PvP: Reward Ad failed: " + e.Message);
        }
    }
}
