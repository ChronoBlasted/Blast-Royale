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
    [SerializeField] Image _rewardImg, _bg;
    [SerializeField] GameObject _alreadyCollected, _nextReward, _focusLayout;
    [SerializeField] CustomButton _rewardButton;

    [SerializeField] Color _regularColor, _specialColor;

    int _index;

    public void Init(RewardCollection reward, int index)
    {
        _index = index;

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
            _rewardAmount.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(reward.blastReceived.exp);

            var dataRef = NakamaData.Instance.GetBlastDataRef(reward.blastReceived.data_id);

            if (reward.blastReceived.shiny) _rewardImg.sprite = dataRef.ShinySprite;
            else if (reward.blastReceived.boss) _rewardImg.sprite = dataRef.BossSprite;
            else _rewardImg.sprite = dataRef.Sprite;
        }
        else if (reward.itemReceived != null)
        {
            _rewardAmount.text = reward.itemReceived.amount.ToString();

            var dataRef = NakamaData.Instance.GetItemDataRef(reward.itemReceived.data_id);

            _rewardImg.sprite = dataRef.Sprite;
        }

        if ((_index + 1) % 7 == 0)
        {
            _bg.color = _specialColor;
        }
        else
        {
            _bg.color = _regularColor;
        }

        _nextReward.SetActive(false);
        _alreadyCollected.SetActive(false);
        _focusLayout.SetActive(false);
    }

    public void UpdateDay(int day)
    {
        _dayTxt.text = "DAY " + day;
    }

    public void Unlock()
    {
        _alreadyCollected.SetActive(true);
        _focusLayout.SetActive(false);
    }

    public void Collectable(bool canClaimReward)
    {
        if (canClaimReward)
        {
            _focusLayout.SetActive(true);
        }
        else Lock();
    }

    public void SetIsNextReward()
    {
        _focusLayout.SetActive(true);
        _nextReward.SetActive(true);
    }

    public void Lock()
    {
        _alreadyCollected.SetActive(false);
        _focusLayout.SetActive(false);
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
