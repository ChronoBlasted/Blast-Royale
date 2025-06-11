using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPopup : Popup
{
    [SerializeField] List<QuestLayout> _questLayouts;

    public void Init(List<DailyQuestData> dailyQuests)
    {
        for (int i = 0; i < _questLayouts.Count; i++)
        {
            _questLayouts[i].gameObject.SetActive(true);
            _questLayouts[i].Init(dailyQuests[i]);
        }
    }
    public async void HandleOnClaimReward()
    {
        await NakamaManager.Instance.NakamaQuest.ClaimQuestReward();
    }
}
