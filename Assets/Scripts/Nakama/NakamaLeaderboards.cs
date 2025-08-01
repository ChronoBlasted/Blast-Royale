using Nakama;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaLeaderboards : MonoBehaviour
{
    public const string LeaderboardTrophyId = "leaderboard_trophy";
    public const string LeaderboardBlastDefeated = "leaderboard_blast_defeated";
    public const string LeaderboardBlastDefeatedAreaId = "leaderboard_blast_defeated_area_";
    public const string LeaderboardBestStageAreaId = "leaderboard_best_stage_area_";

    IClient _client;
    ISession _session;

    int _amountToLoad = 20;

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await UpdateLeaderboards();
    }

    public async Task UpdateLeaderboards()
    {
        await GetTrophyLeaderboard();
        await GetTrophyAroundMeLeaderboard();
        //await GetTrophyFriendLeaderboard();

        await GetBlastDefeatedLeaderboard();
        await GetBlastDefeatedAroundMeLeaderboard();
        //await GetBlastDefeatedFriendLeaderboard();

        await GetAllBlastDefeatedByAreaLeaderboard();
        await GetAllBestStageByAreaLeaderboard();
    }

    public async Task GetTrophyLeaderboard()
    {
        var result = await _client.ListLeaderboardRecordsAsync(_session, LeaderboardTrophyId, null, null, _amountToLoad);

        UIManager.Instance.RegularLeaderboardView.SetTrophyTopLeaderboardData(result);
    }

    public async Task GetTrophyAroundMeLeaderboard()
    {
        var result = await _client.ListLeaderboardRecordsAroundOwnerAsync(_session, LeaderboardTrophyId, _session.UserId, null, _amountToLoad);

        UIManager.Instance.RegularLeaderboardView.SetTrophyAroundMeLeaderboardData(result);
    }

    public async Task GetTrophyFriendLeaderboard()
    {
        var friendsList = await _client.ListFriendsAsync(_session, 0, _amountToLoad, cursor: null);
        var userIds = friendsList.Friends.Select(f => f.User.Id);
        var recordList = await _client.ListLeaderboardRecordsAsync(_session, LeaderboardTrophyId, userIds, null, _amountToLoad);

        UIManager.Instance.RegularLeaderboardView.SetTrophyFriendsLeaderboardData(recordList);
    }

    public async Task GetBlastDefeatedLeaderboard()
    {
        var result = await _client.ListLeaderboardRecordsAsync(_session, LeaderboardBlastDefeated, null, null, _amountToLoad);

        UIManager.Instance.RegularLeaderboardView.SetBlastDefeatedTopLeaderboardData(result);
    }

    public async Task GetBlastDefeatedAroundMeLeaderboard()
    {
        var result = await _client.ListLeaderboardRecordsAroundOwnerAsync(_session, LeaderboardBlastDefeated, _session.UserId, null, _amountToLoad);

        UIManager.Instance.RegularLeaderboardView.SetBlastDefeatedAroundMeLeaderboardData(result);
    }

    public async Task GetBlastDefeatedFriendLeaderboard()
    {
        int amount = 100;
        var friendsList = await _client.ListFriendsAsync(_session, 0, amount, cursor: null);
        var userIds = friendsList.Friends.Select(f => f.User.Id);
        var recordList = await _client.ListLeaderboardRecordsAsync(_session, LeaderboardBlastDefeated, userIds, null, amount);

        UIManager.Instance.RegularLeaderboardView.SetBlastDefeatedFriendsLeaderboardData(recordList);
    }


    public async Task GetAllBlastDefeatedByAreaLeaderboard()
    {
        var allArea = NakamaData.Instance.AreaCollection;

        List<IApiLeaderboardRecordList> result = new List<IApiLeaderboardRecordList>();

        foreach (var area in allArea)
        {
            var currentLeaderboard = await _client.ListLeaderboardRecordsAroundOwnerAsync(_session, LeaderboardBlastDefeatedAreaId + area.id, _session.UserId, null, _amountToLoad);

            result.Add(currentLeaderboard);
        }

        UIManager.Instance.AreaLeaderboardView.SetAreaBlastDefeatedLeaderboardData(result);
    }

    public async Task GetAllBestStageByAreaLeaderboard()
    {
        var allArea = NakamaData.Instance.AreaCollection;

        List<IApiLeaderboardRecordList> result = new List<IApiLeaderboardRecordList>();

        foreach (var area in allArea)
        {
            var currentLeaderboard = await _client.ListLeaderboardRecordsAroundOwnerAsync(_session, LeaderboardBestStageAreaId + area.id, _session.UserId, null, _amountToLoad);

            result.Add(currentLeaderboard);
        }

        UIManager.Instance.AreaLeaderboardView.SetBestStageLeaderboardData(result);
    }

}
