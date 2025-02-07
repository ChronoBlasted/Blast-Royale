using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyRewardView : View
{
    [SerializeField] TMP_Text _totalDayTxt;
    [SerializeField] List<DailyRewardLayout> _allDailyRewardLayout;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);
    }


    public override void CloseView()
    {
        base.CloseView();
    }

    public void UpdateTotalDay(int day)
    {
        _totalDayTxt.text = "TOTAL DAY : " + day;

        int multiplicateur = day / 7;

        for (int i = 0; i < _allDailyRewardLayout.Count; i++)
        {
            _allDailyRewardLayout[i].UpdateDay(multiplicateur + i + 1);
        }
    }

    public void UpdateDailyRewards(List<RewardCollection> rewardCollections)
    {
        for (int i = 0; i < _allDailyRewardLayout.Count; i++)
        {
            _allDailyRewardLayout[i].Init(rewardCollections[i]);
        }
    }

    public void SetActiveReward(int currentDay, bool canClaimDailyReward)
    {
        int multiplicateur = currentDay / 7;

        for (int i = 0; i < _allDailyRewardLayout.Count; i++)
        {
            if (multiplicateur + i < currentDay) _allDailyRewardLayout[i].Unlock();
            else if (multiplicateur + i == currentDay) _allDailyRewardLayout[i].Collectable(canClaimDailyReward);
            else _allDailyRewardLayout[i].Lock();
        }
    }
}