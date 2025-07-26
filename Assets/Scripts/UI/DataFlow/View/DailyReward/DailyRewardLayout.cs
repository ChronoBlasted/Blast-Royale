using Chrono.UI;
using GoogleMobileAds.Api;
using Nakama;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class DailyRewardLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _dayTxt, _rewardAmount;
    [SerializeField] Image _rewardImg, _bg, _glow;
    [SerializeField] GameObject _alreadyCollected, _nextReward, _focusLayout;
    [SerializeField] CustomButton _rewardButton;
    [SerializeField] ParticleSystem _specialRewardFX;
    [SerializeField] NotifChild _notifChild;
    [SerializeField] LocalizedString _dayTrad;

    [SerializeField] Color _regularColorBG, _specialColorBG;
    [SerializeField] Color _regularColorTxt, _specialColorTxt;
    [SerializeField] Color _regularColorGlow, _specialColorGlow;

    int _index;
    Reward _reward;
    public void Init(Reward reward, int index)
    {
        _index = index;
        _reward = reward;

        if (reward.type == RewardType.Coin)
        {
            _rewardAmount.text = UIManager.GetFormattedInt(reward.amount);

            if (reward.amount <= 1000)
            {
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin).Sprite;
            }
            else if (reward.amount <= 3000)
            {
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinThree).Sprite;
            }
            else if (reward.amount <= 5000)
            {
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinLots).Sprite;
            }
            else
            {
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinMega).Sprite;
            }
        }
        else if (reward.type == RewardType.Gem)
        {
            _rewardAmount.text = UIManager.GetFormattedInt(reward.amount);

            if (reward.amount <= 5)
            {
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem).Sprite;
            }
            else if (reward.amount <= 10)
            {
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemThree).Sprite;
            }
            else if (reward.amount <= 20)
            {
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemLots).Sprite;
            }
            else
            {
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemMega).Sprite;
            }
        }
        else if (reward.type == RewardType.Blast)
        {
            _rewardAmount.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(reward.blast.exp);

            var dataRef = NakamaData.Instance.GetBlastDataRef(reward.blast.data_id);

            if (reward.blast.shiny) _rewardImg.sprite = dataRef.ShinySprite;
            else if (reward.blast.boss) _rewardImg.sprite = dataRef.BossSprite;
            else _rewardImg.sprite = dataRef.Sprite;
        }
        else if (reward.type == RewardType.Item)
        {
            _rewardAmount.text = reward.item.amount.ToString();

            var dataRef = NakamaData.Instance.GetItemDataRef(reward.item.data_id);

            _rewardImg.sprite = dataRef.Sprite;
        }

        if ((_index + 1) % 7 == 0)
        {
            _bg.color = _specialColorBG;
            _dayTxt.color = _specialColorTxt;
            _glow.color = _specialColorGlow;

            _specialRewardFX.gameObject.SetActive(true);
        }
        else
        {
            _bg.color = _regularColorBG;
            _dayTxt.color = _regularColorTxt;
            _glow.color = _regularColorGlow;

            _specialRewardFX.gameObject.SetActive(false);
        }

        _nextReward.SetActive(false);
        _alreadyCollected.SetActive(false);
        _focusLayout.SetActive(false);

        _rewardButton.interactable = false;

        _notifChild.Unregister();

        UpdateDay(_index + 1);
    }

    public void UpdateDay(int day)
    {
        _dayTxt.text = _dayTrad.GetLocalizedString() + " " + day;
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
            _rewardButton.interactable = true;

            _notifChild.Register();

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

            UIManager.Instance.RewardPopup.OpenPopup();
            UIManager.Instance.RewardPopup.UpdateData(_reward);

            _notifChild.Unregister();
        }
        catch (ApiResponseException ex)
        {
            _rewardButton.interactable = true;

            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
}
