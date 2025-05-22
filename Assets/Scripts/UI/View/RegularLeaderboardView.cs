using Nakama;
using UnityEngine;

public enum LeaderboardType { Trophy, BlastDefeated, BestStage }
public enum LeaderboardFilter { Top, Me, Friend }

public class RegularLeaderboardView : View
{
    [SerializeField] Transform _contentTransform;

    [SerializeField] LeaderboardRowLayout _leaderboardRowLayout;

    [SerializeField] NavBar _leaderboardNavbar;
    [SerializeField] NavBar _leaderboardSubNavbar;

    LeaderboardType _leaderboardType = LeaderboardType.Trophy;
    LeaderboardFilter _leaderBoardFilter = LeaderboardFilter.Top;

    IApiLeaderboardRecordList _trophyTopLeaderboard;
    IApiLeaderboardRecordList _trophyFriendsLeaderboard;
    IApiLeaderboardRecordList _trophyAroundMeLeaderboard;

    IApiLeaderboardRecordList _blastDefeatedTopLeaderboard;
    IApiLeaderboardRecordList _blastDefeatedFriendsLeaderboard;
    IApiLeaderboardRecordList _blastDefeatedAroundMeLeaderboard;


    public LeaderboardFilter LeaderBoardFilter { get => _leaderBoardFilter; set => _leaderBoardFilter = value; }
    public LeaderboardType LeaderboardType { get => _leaderboardType; set => _leaderboardType = value; }

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);

        _leaderboardNavbar.Init();
        _leaderboardSubNavbar.Init();
    }

    public override void CloseView()
    {
        base.CloseView();
    }

    public void Close()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.MenuView);
    }

    #region LoadLeaderboard

    public void UpdateLeaderboard(IApiLeaderboardRecordList leaderboardData)
    {
        foreach (Transform child in _contentTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in leaderboardData.Records)
        {
            var currentLayout = Instantiate(_leaderboardRowLayout, _contentTransform);
            currentLayout.Init(player, LeaderboardType);
        }
    }

    #endregion
    #region SetData

    public void SetTrophyTopLeaderboardData(IApiLeaderboardRecordList leaderboardData)
    {
        _trophyTopLeaderboard = leaderboardData;
    }

    public void SetTrophyFriendsLeaderboardData(IApiLeaderboardRecordList leaderboardData)
    {
        _trophyFriendsLeaderboard = leaderboardData;
    }

    public void SetTrophyAroundMeLeaderboardData(IApiLeaderboardRecordList leaderboardData)
    {
        _trophyAroundMeLeaderboard = leaderboardData;
    }

    public void SetBlastDefeatedTopLeaderboardData(IApiLeaderboardRecordList leaderboardData)
    {
        _blastDefeatedTopLeaderboard = leaderboardData;
    }

    public void SetBlastDefeatedFriendsLeaderboardData(IApiLeaderboardRecordList leaderboardData)
    {
        _blastDefeatedFriendsLeaderboard = leaderboardData;
    }

    public void SetBlastDefeatedAroundMeLeaderboardData(IApiLeaderboardRecordList leaderboardData)
    {
        _blastDefeatedAroundMeLeaderboard = leaderboardData;
    }

    #endregion

    public void UpdateActiveLeaderboard()
    {
        switch (_leaderboardType)
        {
            case LeaderboardType.Trophy:
                switch (_leaderBoardFilter)
                {
                    case LeaderboardFilter.Top:
                        UpdateLeaderboard(_trophyTopLeaderboard);
                        break;
                    case LeaderboardFilter.Me:
                        UpdateLeaderboard(_trophyAroundMeLeaderboard);
                        break;
                    case LeaderboardFilter.Friend:
                        UpdateLeaderboard(_trophyFriendsLeaderboard);
                        break;
                }
                break;
            case LeaderboardType.BlastDefeated:
                switch (_leaderBoardFilter)
                {
                    case LeaderboardFilter.Top:
                        UpdateLeaderboard(_blastDefeatedTopLeaderboard);
                        break;
                    case LeaderboardFilter.Me:
                        UpdateLeaderboard(_blastDefeatedAroundMeLeaderboard);
                        break;
                    case LeaderboardFilter.Friend:
                        UpdateLeaderboard(_blastDefeatedFriendsLeaderboard);
                        break;
                }
                break;
        }
    }
}
