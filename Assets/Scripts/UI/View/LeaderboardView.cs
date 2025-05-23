using Nakama;
using UnityEngine;

public enum LeaderboardType { Trophy, BlastDefeated }
public enum LeaderboardFilter { Top, Me, Friend }

public class LeaderboardView : View
{
    [SerializeField] Transform _trophyTransform;
    [SerializeField] Transform _trophyAroundMeTransform;
    [SerializeField] Transform _trophyFriendTransform;

    [SerializeField] Transform _blastDefeatedTranform;
    [SerializeField] Transform _blastDefeatedAroundMeTranform;
    [SerializeField] Transform _blastDefeatedFriendTranform;

    [SerializeField] LeaderboardRowLayout _leaderboardRowLayout;

    [SerializeField] NavBar _leaderboardNavbar;
    [SerializeField] NavBar _leaderboardSubNavbar;

    LeaderboardType _leaderboardType = LeaderboardType.Trophy;
    LeaderboardFilter _leaderBoardFilter = LeaderboardFilter.Top;

    GameObject _lastTab;

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

    public void UpdateTrophyLeaderboard(IApiLeaderboardRecordList trophyLeaderboard)
    {
        foreach (Transform child in _trophyTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in trophyLeaderboard.Records)
        {
            var currentTrophyLayout = Instantiate(_leaderboardRowLayout, _trophyTransform);
            currentTrophyLayout.Init(player, LeaderboardType.Trophy);
        }
    }

    public void UpdateTrophyAroundMeLeaderboard(IApiLeaderboardRecordList trophyLeaderboard)
    {
        foreach (Transform child in _trophyAroundMeTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in trophyLeaderboard.Records)
        {
            var currentTrophyLayout = Instantiate(_leaderboardRowLayout, _trophyAroundMeTransform);
            currentTrophyLayout.Init(player, LeaderboardType.Trophy);
        }
    }

    public void UpdateTrophyFriendLeaderboard(IApiLeaderboardRecordList trophyLeaderboard)
    {
        foreach (Transform child in _trophyFriendTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (var player in trophyLeaderboard.Records)
        {
            var currentTrophyLayout = Instantiate(_leaderboardRowLayout, _trophyFriendTransform);
            currentTrophyLayout.Init(player, LeaderboardType.Trophy);
        }
    }

    public void UpdateBlastDefeatedLeaderboard(IApiLeaderboardRecordList trophyLeaderboard)
    {
        foreach (Transform child in _blastDefeatedTranform)
        {
            Destroy(child.gameObject);
        }
        foreach (var player in trophyLeaderboard.Records)
        {
            var currentTrophyLayout = Instantiate(_leaderboardRowLayout, _blastDefeatedTranform);
            currentTrophyLayout.Init(player, LeaderboardType.BlastDefeated);
        }
    }

    public void UpdateBlastDefeatedAroundMeLeaderboard(IApiLeaderboardRecordList trophyLeaderboard)
    {
        foreach (Transform child in _blastDefeatedAroundMeTranform)
        {
            Destroy(child.gameObject);
        }
        foreach (var player in trophyLeaderboard.Records)
        {
            var currentTrophyLayout = Instantiate(_leaderboardRowLayout, _blastDefeatedAroundMeTranform);
            currentTrophyLayout.Init(player, LeaderboardType.BlastDefeated);
        }
    }

    public void UpdateBlastDefeatedFriendLeaderboard(IApiLeaderboardRecordList trophyLeaderboard)
    {
        foreach (Transform child in _blastDefeatedFriendTranform)
        {
            Destroy(child.gameObject);
        }
        foreach (var player in trophyLeaderboard.Records)
        {
            var currentTrophyLayout = Instantiate(_leaderboardRowLayout, _blastDefeatedFriendTranform);
            currentTrophyLayout.Init(player, LeaderboardType.BlastDefeated);
        }
    }

    #endregion

    public void UpdateActiveLeaderboard()
    {
        if (_lastTab != null) _lastTab.gameObject.SetActive(false);

        switch (_leaderboardType)
        {
            case LeaderboardType.Trophy:
                switch (_leaderBoardFilter)
                {
                    case LeaderboardFilter.Top:
                        _lastTab = _trophyTransform.parent.gameObject;
                        _trophyTransform.parent.gameObject.SetActive(true);
                        break;
                    case LeaderboardFilter.Me:
                        _lastTab = _trophyAroundMeTransform.parent.gameObject;
                        _trophyAroundMeTransform.parent.gameObject.SetActive(true);
                        break;
                    case LeaderboardFilter.Friend:
                        _lastTab = _trophyFriendTransform.parent.gameObject;
                        _trophyFriendTransform.parent.gameObject.SetActive(true);
                        break;
                }
                break;
            case LeaderboardType.BlastDefeated:
                switch (_leaderBoardFilter)
                {
                    case LeaderboardFilter.Top:
                        _lastTab = _blastDefeatedTranform.parent.gameObject;
                        _blastDefeatedTranform.parent.gameObject.SetActive(true);
                        break;
                    case LeaderboardFilter.Me:
                        _lastTab = _blastDefeatedAroundMeTranform.parent.gameObject;
                        _blastDefeatedAroundMeTranform.parent.gameObject.SetActive(true);
                        break;
                    case LeaderboardFilter.Friend:
                        _lastTab = _blastDefeatedFriendTranform.parent.gameObject;
                        _blastDefeatedFriendTranform.parent.gameObject.SetActive(true);
                        break;
                }
                break;
        }
    }
}
