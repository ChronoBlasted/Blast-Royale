using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferLayout : MonoBehaviour
{
    [SerializeField] Image _ico;
    [SerializeField] TMP_Text _nameTxt, _descTxt;

    Reward _currentReward;

    public void Init(Reward reward)
    {
        _currentReward = reward;

        ResourceData resourceData;

        switch (_currentReward.type)
        {
            case RewardType.Blast:

                BlastDataRef blastData = NakamaData.Instance.GetBlastDataRef(_currentReward.blast.data_id);

                _ico.sprite = NakamaData.Instance.GetSpriteWithBlast(_currentReward.blast);

                _nameTxt.text = blastData.Name.GetLocalizedString();
                _descTxt.text = "lvl." + NakamaLogic.CalculateLevelFromExperience(_currentReward.blast.exp);
                break;
            case RewardType.Item:
                ItemDataRef itemDataRef = NakamaData.Instance.GetItemDataRef(_currentReward.item.data_id);

                ItemData itemData = NakamaData.Instance.GetItemDataById(_currentReward.item.data_id);

                _ico.sprite = itemDataRef.Sprite;

                _nameTxt.text = itemDataRef.Name.GetLocalizedString();
                _descTxt.text = "x" + _currentReward.item.amount;
                break;
            case RewardType.Coin:
                resourceData = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin);

                _ico.sprite = resourceData.Sprite;

                _nameTxt.text = resourceData.Name.GetLocalizedString();
                _descTxt.text = "x" + UIManager.GetFormattedInt(_currentReward.amount);
                break;
            case RewardType.Gem:
                resourceData = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem);

                _ico.sprite = resourceData.Sprite;

                _nameTxt.text = resourceData.Name.GetLocalizedString();
                _descTxt.text = "x" + UIManager.GetFormattedInt(_currentReward.amount);
                break;
            default:
                break;
        }
    }

    public void Init(StoreOffer storeOffer)
    {
        Init(storeOffer.reward);

        StoreOfferDataRef storeOfferRef;

        switch (storeOffer.reward.type)
        {
            case RewardType.Coin:
                storeOfferRef = NakamaData.Instance.GetStoreOfferDataRef(storeOffer.offer_id);

                _ico.sprite = storeOfferRef.Sprite;

                _nameTxt.text = storeOfferRef.Name.GetLocalizedString();
                _descTxt.text = "x" + UIManager.GetFormattedInt(storeOffer.reward.amount);
                break;
            case RewardType.Gem:
                storeOfferRef = NakamaData.Instance.GetStoreOfferDataRef(storeOffer.offer_id);

                _ico.sprite = storeOfferRef.Sprite;

                _nameTxt.text = storeOfferRef.Name.GetLocalizedString();
                _descTxt.text = "x" + UIManager.GetFormattedInt(storeOffer.reward.amount);
                break;
        }
    }
}
