using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaLeaderboardView : View
{
    [SerializeField] Transform _contentTransform;
    [SerializeField] LeaderboardRowLayout _leaderboardRowLayout;
    [SerializeField] NavBar _leaderboardNavbar;
    [SerializeField] TMP_Text _areaName, _resetTimerTxt;

    LeaderboardType _leaderboardType = LeaderboardType.Trophy;

    List<IApiLeaderboardRecordList> _allBestStageLeaderboards;
    List<IApiLeaderboardRecordList> _allBlastDefeatedLeaderboards;

    int _currentIndex = 0;
    TimeSpan timeRemaining;

    public LeaderboardType LeaderboardType { get => _leaderboardType; set => _leaderboardType = value; }
    public int CurrentIndex { get => _currentIndex; set => _currentIndex = value; }


    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);

        _leaderboardNavbar.Init();

        UpdateResetTime();
    }

    public override void CloseView()
    {
        base.CloseView();
    }

    public void Close()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.AllAreaView);
    }

    void UpdateResetTime()
    {
        DateTime now = DateTime.Now;
        DateTime nextMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
        timeRemaining = nextMonth - now;

        _resetTimerTxt.text = $"Reset in {timeRemaining.Days} days";
    }

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

    public void SetBestStageLeaderboardData(List<IApiLeaderboardRecordList> leaderboardData)
    {
        _allBestStageLeaderboards = leaderboardData;
    }

    public void SetAreaBlastDefeatedLeaderboardData(List<IApiLeaderboardRecordList> leaderboardData)
    {
        _allBlastDefeatedLeaderboards = leaderboardData;
    }

    public void UpdateActiveLeaderboard()
    {
        switch (_leaderboardType)
        {
            case LeaderboardType.BestStage:
                UpdateLeaderboard(_allBestStageLeaderboards[_currentIndex]);
                break;
            case LeaderboardType.BlastDefeated:
                UpdateLeaderboard(_allBlastDefeatedLeaderboards[_currentIndex]);
                break;
        }

        _areaName.text = NakamaData.Instance.GetAreaDataRef(NakamaData.Instance.AreaCollection[_currentIndex].id).Name.GetLocalizedString();
    }

    public void ShowPreviousArea()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
        }
        else
        {
            _currentIndex = NakamaData.Instance.AreaCollection.Count - 1;
        }
        UpdateActiveLeaderboard();
    }

    public void ShowNextArea()
    {
        if (_currentIndex < NakamaData.Instance.AreaCollection.Count - 1)
        {
            _currentIndex++;
        }
        else
        {
            _currentIndex = 0;
        }
        UpdateActiveLeaderboard();
    }
}
