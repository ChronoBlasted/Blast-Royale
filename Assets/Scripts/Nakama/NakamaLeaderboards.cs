using Nakama;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaLeaderboards : MonoBehaviour
{
    public const string LeaderboardTrophyId = "leaderboard_trophy";
    public const string LeaderboardBlastDefeated = "leaderboard_blast_defeated";

    IClient _client;
    ISession _session;

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await GetTrophyLeaderboard();
        await GetTrophyAroundMeLeaderboard();
        await GetTrophyFriendLeaderboard();

        await GetBlastDefeatedLeaderboard();
        await GetBlastDefeatedAroundMeLeaderboard();
        await GetBlastDefeatedFriendLeaderboard();
    }

    public async Task GetTrophyLeaderboard()
    {
        var result = await _client.ListLeaderboardRecordsAsync(_session, LeaderboardTrophyId, null, null, 100);

        UIManager.Instance.LeaderboardView.UpdateTrophyLeaderboard(result);
    }

    public async Task GetTrophyAroundMeLeaderboard()
    {
        var limit = 20;
        var result = await _client.ListLeaderboardRecordsAroundOwnerAsync(_session, LeaderboardTrophyId, _session.UserId, null, limit);

        UIManager.Instance.LeaderboardView.UpdateTrophyAroundMeLeaderboard(result);
    }

    public async Task GetTrophyFriendLeaderboard()
    {
        int amount = 100;
        var friendsList = await _client.ListFriendsAsync(_session, 0, amount, cursor: null);
        var userIds = friendsList.Friends.Select(f => f.User.Id);
        var recordList = await _client.ListLeaderboardRecordsAsync(_session, LeaderboardTrophyId, userIds, null, amount);

        UIManager.Instance.LeaderboardView.UpdateTrophyFriendLeaderboard(recordList);
    }

    public async Task GetBlastDefeatedLeaderboard()
    {
        var result = await _client.ListLeaderboardRecordsAsync(_session, LeaderboardBlastDefeated);

        UIManager.Instance.LeaderboardView.UpdateBlastDefeatedLeaderboard(result);
    }

    public async Task GetBlastDefeatedAroundMeLeaderboard()
    {
        var limit = 20;
        var result = await _client.ListLeaderboardRecordsAroundOwnerAsync(_session, LeaderboardBlastDefeated, _session.UserId, null, limit);

        UIManager.Instance.LeaderboardView.UpdateBlastDefeatedAroundMeLeaderboard(result);
    }

    public async Task GetBlastDefeatedFriendLeaderboard()
    {
        int amount = 100;
        var friendsList = await _client.ListFriendsAsync(_session, 0, amount, cursor: null);
        var userIds = friendsList.Friends.Select(f => f.User.Id);
        var recordList = await _client.ListLeaderboardRecordsAsync(_session, LeaderboardBlastDefeated, userIds, null, amount);

        UIManager.Instance.LeaderboardView.UpdateBlastDefeatedFriendLeaderboard(recordList);
    }
}
