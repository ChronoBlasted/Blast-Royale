using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPopup : Popup
{
    [SerializeField] List<QuestLayout> _questLayouts;
    [SerializeField] List<QuestRewardLayout> _questRewardsLayouts;
    [SerializeField] SliderBar _progressBar;

    int amountQuestCompleted;

    public void Init(List<DailyQuestData> dailyQuests)
    {
        amountQuestCompleted = 0;

        for (int i = 0; i < _questLayouts.Count; i++)
        {
            _questLayouts[i].gameObject.SetActive(true);
            _questLayouts[i].Init(dailyQuests[i]);

            if (_questLayouts[i].QuestComplete)
            {
                amountQuestCompleted++;
            }
        }

        _progressBar.Init(_progressBar.Slider.value, _questLayouts.Count);

        _progressBar.SetValueSmooth(amountQuestCompleted);

    }

    public override void OpenPopup()
    {
        base.OpenPopup();

        _progressBar.SetValueSmooth(amountQuestCompleted);
    }

    public void InitRewards(DailyQuestRewardData dailyQuestRewards)
    {
        for (int i = 0; i < _questLayouts.Count; i++)
        {
            _questRewardsLayouts[i].Init(dailyQuestRewards.rewards[i], dailyQuestRewards.rewardCount == i && amountQuestCompleted > i, dailyQuestRewards.rewardCount > i);
        }
    }
    public async void HandleOnClaimReward()
    {
        await NakamaManager.Instance.NakamaQuest.ClaimQuestReward();
    }


}
