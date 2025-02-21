using Chrono.UI;
using Nakama;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _dayTxt, _rewardAmount;
    [SerializeField] Image _rewardImg, _stateImg,_alreadyCollectBG;
    [SerializeField] CustomButton _rewardButton;

    public void Init(RewardCollection reward)
    {
        if (reward.coinsReceived > 0)
        {
            _rewardAmount.text = reward.coinsReceived.ToString();
            _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin).Sprite;
        }
        else if (reward.gemsReceived > 0)
        {
            _rewardAmount.text = reward.gemsReceived.ToString();
            _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem).Sprite;
        }
        else if (reward.blastReceived != null)
        {

        }
    }

    public void UpdateDay(int day)
    {
        _dayTxt.text = "DAY " + day;
    }

    public void Unlock()
    {
        _stateImg.enabled = false;
        _alreadyCollectBG.enabled = true;
    }

    public void Collectable(bool canClaimReward)
    {
        if (canClaimReward)
        {
            _stateImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Unlock).Sprite;
            _rewardButton.interactable = true;
        }
        else Lock();
    }

    public void Lock()
    {
        _stateImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Lock).Sprite;
        _alreadyCollectBG.enabled = false;
    }

    public async void HandleOnCollectDailyReward()
    {
        _rewardButton.interactable = false;

        try
        {
            await NakamaManager.Instance.NakamaDailyReward.ClaimDailyReward();
        }
        catch (ApiResponseException ex)
        {
            _rewardButton.interactable = true;

            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
}
