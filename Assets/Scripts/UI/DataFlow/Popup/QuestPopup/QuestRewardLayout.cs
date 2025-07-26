using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardLayout : MonoBehaviour
{
    [SerializeField] Image _ico, _check;
    [SerializeField] TMP_Text _rewardAmount;
    [SerializeField] Button _rewardButton;
    [SerializeField] NotifChild _notifChild;

    Tween _tweenScale;
    public void Init(Reward reward, bool canBeClaim, bool isCollected)
    {
        if (reward.type == RewardType.Coin)
        {
            _rewardAmount.text = UIManager.GetFormattedInt(reward.amount);

            if (reward.amount <= 1000)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin).Sprite;
            }
            else if (reward.amount <= 3000)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinThree).Sprite;
            }
            else if (reward.amount <= 3000)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinLots).Sprite;
            }
            else
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinMega).Sprite;
            }
        }
        else if (reward.type == RewardType.Gem)
        {
            _rewardAmount.text = UIManager.GetFormattedInt(reward.amount);

            if (reward.amount <= 5)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem).Sprite;
            }
            else if (reward.amount <= 10)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemThree).Sprite;
            }
            else if (reward.amount <= 20)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemLots).Sprite;
            }
            else
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemMega).Sprite;
            }
        }

        _rewardButton.interactable = !isCollected && canBeClaim;


        if (_tweenScale != null && _tweenScale.IsActive())
            _tweenScale.Kill(true);

        if (isCollected)
        {
            _check.enabled = true;
            _ico.DOFade(0.5f, 0f);
            _ico.transform.localScale = Vector3.one;
            _rewardButton.interactable = false;

            _notifChild.Unregister();
        }
        else if (canBeClaim)
        {
            _check.enabled = false;
            _ico.DOFade(1f, 0f);
            _rewardButton.interactable = true;
            _tweenScale = _ico.transform.DOScale(Vector3.one * 1.2f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            _notifChild.Register();
        }
        else
        {
            _check.enabled = false;
            _ico.DOFade(1f, 0f);
            _rewardButton.interactable = false;
            _ico.transform.localScale = Vector3.one;

            _notifChild.Unregister();
        }
    }
}
