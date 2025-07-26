using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestPopup : Popup
{
    [SerializeField] List<QuestLayout> _questLayouts;
    [SerializeField] List<QuestRewardLayout> _questRewardsLayouts;
    [SerializeField] SliderBar _progressBar;

    [SerializeField] TMP_Text _resetTimerTxt;
    DateTime _nextDailyReset;
    TimeSpan _timeRemaining;

    int _amountQuestCompleted;

    public void Init(List<DailyQuest> dailyQuests)
    {
        _amountQuestCompleted = 0;

        for (int i = 0; i < _questLayouts.Count; i++)
        {
            _questLayouts[i].gameObject.SetActive(true);
            _questLayouts[i].Init(dailyQuests[i]);

            if (_questLayouts[i].QuestComplete)
            {
                _amountQuestCompleted++;
            }
        }

        _progressBar.Init(_progressBar.Slider.value, _questLayouts.Count);

        _progressBar.SetValueSmooth(_amountQuestCompleted);

    }

    public override void OpenPopup()
    {
        base.OpenPopup();

        _progressBar.SetValueSmooth(_amountQuestCompleted);

        DateTime now = DateTime.Now;
        _nextDailyReset = now.Date.AddDays(1);

        UpdateResetTime();
    }

    public void InitRewards(DailyQuestRewardData dailyQuestRewards)
    {
        for (int i = 0; i < _questLayouts.Count; i++)
        {
            _questRewardsLayouts[i].Init(dailyQuestRewards.rewards[i], dailyQuestRewards.rewardCount == i && _amountQuestCompleted > i, dailyQuestRewards.rewardCount > i);
        }
    }

    public void RefreshRewards(DailyQuestRewardData dailyQuestRewards)
    {
        InitRewards(dailyQuestRewards);
    }

    public async void HandleOnClaimReward()
    {
        await NakamaManager.Instance.NakamaQuest.ClaimQuestReward();
    }

    void UpdateResetTime()
    {
        DateTime now = DateTime.Now;
        _timeRemaining = _nextDailyReset - now;

        if (_timeRemaining.TotalSeconds < 0)
        {
            _nextDailyReset = now.Date.AddDays(1);
            _timeRemaining = _nextDailyReset - now;
        }

        if (_timeRemaining.Hours > 0)
        {
            _resetTimerTxt.text = $"Reset in {_timeRemaining.Hours} hours";
        }
        else
        {
            _resetTimerTxt.text = $"Reset in {_timeRemaining.Minutes} min";
        }
    }
}
