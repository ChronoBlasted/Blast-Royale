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

    [SerializeField] Sprite _coinsSpr, _gemsSpr;
    [SerializeField] Sprite _lockSpr, _collectableSpr, _unlockSpr;

    public void Init(RewardCollection reward)
    {
        if (reward.coinsReceived > 0)
        {
            _rewardAmount.text = reward.coinsReceived.ToString();
            _rewardImg.sprite = _coinsSpr;
        }
        else if (reward.gemsReceived > 0)
        {
            _rewardAmount.text = reward.gemsReceived.ToString();
            _rewardImg.sprite = _gemsSpr;
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
        _stateImg.sprite = _unlockSpr;
    }

    public void Collectable(bool canClaimReward)
    {
        if (canClaimReward)
        {
            _stateImg.sprite = _collectableSpr;
            _rewardButton.interactable = true;
        }
        else Lock();
    }

    public void Lock()
    {
        _stateImg.sprite = _lockSpr;
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
