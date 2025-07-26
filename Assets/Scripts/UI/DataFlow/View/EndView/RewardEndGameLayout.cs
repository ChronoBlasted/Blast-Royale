using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardEndGameLayout : MonoBehaviour
{
    [SerializeField] Image _ico, _bg;
    [SerializeField] TMP_Text _amount;
    [SerializeField] GameObject _bonusLayout;

    Reward _currentOffer;
    public void Init(Reward offer)
    {
        _currentOffer = offer;

        _bonusLayout.SetActive(offer.isBonus);

        ResourceData resourceData;

        switch (_currentOffer.type)
        {
            case RewardType.Blast:

                BlastDataRef blastData = NakamaData.Instance.GetBlastDataRef(_currentOffer.blast.data_id);

                _ico.sprite = NakamaData.Instance.GetSpriteWithBlast(_currentOffer.blast);

                _amount.text = "lvl." + NakamaLogic.CalculateLevelFromExperience(_currentOffer.blast.exp);
                break;
            case RewardType.Item:
                ItemDataRef itemDataRef = NakamaData.Instance.GetItemDataRef(_currentOffer.item.data_id);

                ItemData itemData = NakamaData.Instance.GetItemDataById(_currentOffer.item.data_id);

                _ico.sprite = itemDataRef.Sprite;

                _amount.text = "x" + _currentOffer.item.amount;
                break;
            case RewardType.Coin:
                resourceData = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin);

                _ico.sprite = resourceData.Sprite;

                _amount.text = "x" + UIManager.GetFormattedInt(_currentOffer.amount);
                break;
            case RewardType.Gem:
                resourceData = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem);

                _ico.sprite = resourceData.Sprite;

                _amount.text = "x" + UIManager.GetFormattedInt(_currentOffer.amount);
                break;

            default:
                break;
        }
    }
}
