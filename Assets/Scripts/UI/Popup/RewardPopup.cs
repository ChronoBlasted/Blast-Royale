using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopup : Popup
{
    Queue<RewardPopupData> _rewardQueue = new Queue<RewardPopupData>(10);

    Coroutine _openRewardCor;

    [Header("Ref")]
    [SerializeField] Button _continueBtn;
    [SerializeField] TMP_Text _titleTxt, _tapToContinueTxt;
    [SerializeField] Image _ico;


    public override void Init()
    {
        base.Init();
    }

    public override void OpenPopup()
    {
        gameObject.SetActive(true);

        _canvasGroup.blocksRaycasts = true;

        _canvasGroup.DOFade(1, .2f).OnComplete(() =>
        {
            _canvasGroup.interactable = true;
        }).SetUpdate(UpdateType.Normal, true);

        if (_tweenSequence != null) _tweenSequence.Init();
    }

    public override void OpenPopup(bool openBlackShade = true, bool openCloseButton = true)
    {
        base.OpenPopup(openBlackShade, openCloseButton);
    }

    public void UpdateData(Reward reward)
    {
        _rewardQueue = new Queue<RewardPopupData>(10);

        if (reward.type == RewardType.Coin)
        {
            var coinData = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin);

            Sprite sprite = coinData.Sprite;

            if (reward.amount <= 1000)
            {
                sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin).Sprite;
            }
            else if (reward.amount <= 3000)
            {
                sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinThree).Sprite;
            }
            else if (reward.amount <= 3000)
            {
                sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinLots).Sprite;
            }
            else
            {
                sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinMega).Sprite;
            }

            _rewardQueue.Enqueue(new RewardPopupData(reward.amount, coinData.Name.GetLocalizedString(), sprite));
        }
        if (reward.type == RewardType.Gem)
        {
            var gemData = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem);

            Sprite sprite = gemData.Sprite;

            if (reward.amount <= 5)
            {
                sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem).Sprite;
            }
            else if (reward.amount <= 10)
            {
                sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemThree).Sprite;
            }
            else if (reward.amount <= 20)
            {
                sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemLots).Sprite;
            }
            else
            {
                sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemMega).Sprite;
            }
            _rewardQueue.Enqueue(new RewardPopupData(reward.amount, gemData.Name.GetLocalizedString(), sprite));
        }
        if (reward.type == RewardType.Blast)
        {
            _rewardQueue.Enqueue(new RewardPopupData(0, NakamaData.Instance.GetBlastDataRef(reward.blast.data_id).Name.GetLocalizedString(), NakamaData.Instance.GetBlastDataRef(NakamaData.Instance.GetBlastDataById(reward.blast.data_id).id).Sprite));
        }
        if (reward.type == RewardType.Item)
        {
            ItemData itemData = NakamaData.Instance.GetItemDataById(reward.item.data_id);

            _rewardQueue.Enqueue(new RewardPopupData(reward.item.amount, NakamaData.Instance.GetItemDataRef(reward.item.data_id).Name.GetLocalizedString(), NakamaData.Instance.GetItemDataRef(reward.item.data_id).Sprite));
        }

        if (_openRewardCor != null)
        {
            StopCoroutine(_openRewardCor);
            _openRewardCor = null;
        }

        _openRewardCor = StartCoroutine(UpdateWithNextReward());
    }

    IEnumerator UpdateWithNextReward()
    {
        RewardPopupData tempReward;

        if (_rewardQueue.TryDequeue(out tempReward))
        {
            _continueBtn.interactable = false;

            if (tempReward.Amount > 1) _titleTxt.text = UIManager.GetFormattedInt(tempReward.Amount) + " " + tempReward.Name;
            else _titleTxt.text = tempReward.Name;

            _ico.sprite = tempReward.Sprite;
        }
        else
        {
            _rewardQueue.Clear();

            ClosePopup();
        }

        yield return new WaitForSeconds(.2f);

        _continueBtn.interactable = true;
    }

    public void CollectReward()
    {
        if (_openRewardCor != null)
        {
            StopCoroutine(_openRewardCor);
            _openRewardCor = null;
        }

        _openRewardCor = StartCoroutine(UpdateWithNextReward());
    }
}

public struct RewardPopupData
{
    public int Amount;
    public string Name;
    public Sprite Sprite;

    public RewardPopupData(int amount, string name, Sprite sprite)
    {
        Amount = amount;
        Name = name;
        Sprite = sprite;
    }
}
