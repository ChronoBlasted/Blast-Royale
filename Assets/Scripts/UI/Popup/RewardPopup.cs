using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
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

        _canvasGroup.DOFade(1, .2f).OnComplete(() =>
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }).SetUpdate(UpdateType.Normal, true);
    }

    public override void ClosePopup()
    {
        base.ClosePopup();
    }

    public void UpdateData(RewardCollection reward)
    {
        _rewardQueue = new Queue<RewardPopupData>(10);

        if (reward.coinsReceived != 0)
        {
            _rewardQueue.Enqueue(new RewardPopupData(reward.coinsReceived, "Coins", DataUtils.Instance.CoinIco));
        }
        if (reward.gemsReceived != 0)
        {
            _rewardQueue.Enqueue(new RewardPopupData(reward.gemsReceived, "Gems", DataUtils.Instance.GemIco));
        }
        if (reward.blastReceived != null)
        {
            _rewardQueue.Enqueue(new RewardPopupData(0, DataUtils.Instance.GetBlastDataById(reward.blastReceived.data_id).name, DataUtils.Instance.GetBlastImgByID(DataUtils.Instance.GetBlastDataById(reward.blastReceived.data_id).id)));
        }
        if (reward.itemReceived != null)
        {
            ItemData itemData = DataUtils.Instance.GetItemDataById(reward.itemReceived.data_id);

            _rewardQueue.Enqueue(new RewardPopupData(reward.itemReceived.amount, itemData.name, DataUtils.Instance.GetItemImgByID(reward.itemReceived.data_id)));
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

            if (tempReward.Amount > 1) _titleTxt.text = tempReward.Amount + " " + tempReward.Name;
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
