using Chrono.UI;
using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopLayout : MonoBehaviour
{
    [SerializeField] Image _ico, _border, _priceIco;
    [SerializeField] TMP_Text _nameTxt, _descTxt, _priceAmount;
    [SerializeField] GameObject _buyedBlackShade;

    [SerializeField] CustomButton _buyButton;

    StoreOffer _offer;

    public void Init(StoreOffer storeOffer)
    {
        _offer = storeOffer;

        _nameTxt.text = _offer.name;
        _descTxt.text = _offer.desc;
        _priceAmount.text = _offer.price.ToString();


        if (_offer.coinsAmount > 0)
        {
            _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin).Sprite;
        }
        if (_offer.gemsAmount > 0)
        {
            _ico.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem).Sprite;
        }
        if (_offer.blast != null)
        {
            _ico.sprite = NakamaData.Instance.GetBlastDataRef(_offer.blast.data_id).Sprite;
            _border.color = ColorManager.Instance.GetTypeColor(NakamaData.Instance.GetBlastDataById(_offer.blast.data_id).type);
        }
        if (_offer.item != null)
        {
            ItemData itemData = NakamaData.Instance.GetItemDataById(_offer.item.data_id);

            _ico.sprite = NakamaData.Instance.GetItemDataRef(_offer.item.data_id).Sprite;
            _border.color = ColorManager.Instance.GetItemColor(itemData.behaviour);
        }

        switch (_offer.currency)
        {
            case Currency.coins:
                _priceIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin).Sprite;
                break;
            case Currency.gems:
                _priceIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Gem).Sprite;
                break;
            default:
                _priceIco.gameObject.SetActive(false);
                break;
        }

        IsBuyable();
    }

    private void IsBuyable()
    {
        if (_offer.isAlreadyBuyed)
        {
            _priceAmount.text = "BUYED";
            _buyButton.interactable = false;
            _buyedBlackShade.SetActive(true);
        }
        else
        {
            _buyButton.interactable = true;
            _buyedBlackShade.SetActive(false);
        }
    }

    public async void HandleOnBuyBlastTrapOffer(int index)
    {
        if (_offer.price > NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.coins.ToString()]) return;

        await NakamaManager.Instance.NakamaStore.BuyTrapOffer(index);
    }
    public async void HandleOnBuyCoinOffer(int index)
    {
        if (_offer.price > NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.gems.ToString()]) return;

        RewardCollection reward = new RewardCollection();

        reward.coinsReceived = _offer.coinsAmount;

        UIManager.Instance.RewardPopup.OpenPopup();
        UIManager.Instance.RewardPopup.UpdateData(reward);

        await NakamaManager.Instance.NakamaStore.BuyCoinOffer(index);
    }

    public async void HandleOnBuyGemOffer(int index)
    {
        await NakamaManager.Instance.NakamaStore.BuyGemOffer(index);

        RewardCollection reward = new RewardCollection();

        reward.gemsReceived = _offer.gemsAmount;

        UIManager.Instance.RewardPopup.OpenPopup();
        UIManager.Instance.RewardPopup.UpdateData(reward);
    }
    public async void HandleOnBuyDailyShopOffer(int index)
    {
        if (_offer.price > NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.coins.ToString()]) return;
        if (_offer.isAlreadyBuyed) return;

        try
        {
            await NakamaManager.Instance.NakamaStore.BuyDailyShopOffer(index);

            _offer.isAlreadyBuyed = true;

            IsBuyable();
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
}
