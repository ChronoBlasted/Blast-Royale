using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferLayout : MonoBehaviour
{
    [SerializeField] Image _ico;
    [SerializeField] TMP_Text _nameTxt, _descTxt;

    Offer _currentOffer;

    public void Init(Offer offer)
    {
        _currentOffer = offer;

        ResourceData resourceData;

        switch (_currentOffer.type)
        {
            case OfferType.BLAST:

                BlastDataRef blastData = NakamaData.Instance.GetBlastDataRef(_currentOffer.blast.data_id);

                _ico.sprite = NakamaData.Instance.GetSpriteWithBlast(_currentOffer.blast);

                _nameTxt.text = blastData.Name.GetLocalizedString();
                _descTxt.text = "lvl." + NakamaLogic.CalculateLevelFromExperience(_currentOffer.blast.exp);
                break;
            case OfferType.ITEM:
                ItemDataRef itemDataRef = NakamaData.Instance.GetItemDataRef(_currentOffer.item.data_id);

                ItemData itemData = NakamaData.Instance.GetItemDataById(_currentOffer.item.data_id);

                _ico.sprite = itemDataRef.Sprite;

                _nameTxt.text = itemDataRef.Name.GetLocalizedString();
                _descTxt.text = "x" + _currentOffer.item.amount;
                break;
            case OfferType.COIN:
                resourceData = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin);

                _ico.sprite = resourceData.Sprite;

                _nameTxt.text = resourceData.Name.GetLocalizedString();
                _descTxt.text = "x" + UIManager.GetFormattedInt(_currentOffer.coinsAmount);
                break;
            case OfferType.GEM:
                resourceData = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem);

                _ico.sprite = resourceData.Sprite;

                _nameTxt.text = resourceData.Name.GetLocalizedString();
                _descTxt.text = "x" + UIManager.GetFormattedInt(_currentOffer.gemsAmount);
                break;
            default:
                break;
        }
    }

    public void Init(StoreOffer storeOffer)
    {
        Init(storeOffer.offer);

        StoreOfferDataRef storeOfferRef;

        switch (storeOffer.offer.type)
        {
            case OfferType.COIN:
                storeOfferRef = NakamaData.Instance.GetStoreOfferDataRef(storeOffer.offer_id);

                _ico.sprite = storeOfferRef.Sprite;

                _nameTxt.text = storeOfferRef.Name.GetLocalizedString();
                _descTxt.text = "x" + UIManager.GetFormattedInt(storeOffer.offer.coinsAmount);
                break;
            case OfferType.GEM:
                storeOfferRef = NakamaData.Instance.GetStoreOfferDataRef(storeOffer.offer_id);

                _ico.sprite = storeOfferRef.Sprite;

                _nameTxt.text = storeOfferRef.Name.GetLocalizedString();
                _descTxt.text = "x" + UIManager.GetFormattedInt(storeOffer.offer.gemsAmount);
                break;
        }
    }
}
