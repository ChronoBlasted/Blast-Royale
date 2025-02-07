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
    [SerializeField] Image _rewardImg, _stateImg;
    [SerializeField] CustomButton _rewardButton;

    ResourceObjectHolder resource;

    public void Init(RewardCollection reward)
    {
        resource = ResourceObjectHolder.Instance;

        if (reward.coinsReceived > 0)
        {
            _rewardAmount.text = reward.coinsReceived.ToString();
            _rewardImg.sprite = resource.GetResourceByType(ResourceType.Coin).sprite;
        }
        else if (reward.gemsReceived > 0)
        {
            _rewardAmount.text = reward.gemsReceived.ToString();
            _rewardImg.sprite = resource.GetResourceByType(ResourceType.Gem).sprite;
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
        _stateImg.sprite = resource.GetResourceByType(ResourceType.Unlock).sprite;
    }

    public void Collectable(bool canClaimReward)
    {
        if (canClaimReward)
        {
            _stateImg.sprite = resource.GetResourceByType(ResourceType.ArrowLeft).sprite;
            _rewardButton.interactable = true;
        }
        else Lock();
    }

    public void Lock()
    {
        _stateImg.sprite = resource.GetResourceByType(ResourceType.Lock).sprite;
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
