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

    Tween _tweenScale;
    public void Init(RewardCollection reward, bool canBeClaim, bool isCollected)
    {
        if (reward.coinsReceived > 0)
        {
            _rewardAmount.text = UIManager.GetFormattedInt(reward.coinsReceived);

            if (reward.coinsReceived <= 1000)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin).Sprite;
            }
            else if (reward.coinsReceived <= 3000)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinThree).Sprite;
            }
            else if (reward.coinsReceived <= 5000)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinLots).Sprite;
            }
            else
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinMega).Sprite;
            }
        }
        else if (reward.gemsReceived > 0)
        {
            _rewardAmount.text = UIManager.GetFormattedInt(reward.gemsReceived);

            if (reward.gemsReceived <= 5)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem).Sprite;
            }
            else if (reward.gemsReceived <= 10)
            {
                _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemThree).Sprite;
            }
            else if (reward.gemsReceived <= 20)
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
            _rewardButton.interactable = false;
            transform.localScale = Vector3.one;
        }
        else if (canBeClaim)
        {
            _check.enabled = false;
            _ico.DOFade(1f, 0f);
            _rewardButton.interactable = true;
            _tweenScale = transform.DOScale(Vector3.one * 1.2f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            _check.enabled = false;
            _ico.DOFade(1f, 0f);
            _rewardButton.interactable = false;
            transform.localScale = Vector3.one;
        }
    }
}
