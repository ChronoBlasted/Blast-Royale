using Chrono.UI;
using Nakama;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _priceAmount;
    [SerializeField] GameObject _buyedBlackShade;
    [SerializeField] OfferLayout _offerLayout;
    [SerializeField] bool _isIAP;
    [SerializeField] IAPPriceDisplay _iapPriceDisplay;
    [SerializeField] CustomButton _buyButton;

    StoreOffer _storeOffer;

    public void Init(StoreOffer storeOffer)
    {
        _storeOffer = storeOffer;

        _offerLayout.Init(storeOffer);

        if (_isIAP) _iapPriceDisplay.Init();
        else
        {
            switch (_storeOffer.currency)
            {
                case Currency.Coins:

                    _priceAmount.text = "<sprite name=\"Coin\">";
                    break;
                case Currency.Gems:

                    _priceAmount.text = "<sprite name=\"Gem\">";
                    break;
                default:
                    _priceAmount.text = "";
                    break;
            }

            _priceAmount.text += _storeOffer.price.ToString();
        }

        IsBuyable();
    }

    private void IsBuyable()
    {
        if (_storeOffer.isAlreadyBuyed)
        {
            _buyedBlackShade.SetActive(true);
        }
        else
        {
            _buyedBlackShade.SetActive(false);
        }
    }

    public async void HandleOnBuyBlastTrapOffer(int index)
    {
        if (_storeOffer.price > NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.Coins.ToString()])
        {
            ErrorManager.Instance.ShowError(ErrorType.NOT_ENOUGH_COIN);
            return;
        }

        await NakamaManager.Instance.NakamaStore.BuyTrapOffer(index);

        Dictionary<string, int> changeset = new Dictionary<string, int>
        {
            { Currency.Coins.ToString(), -_storeOffer.price },
        };
        
        NakamaManager.Instance.NakamaUserAccount.UpdateWalletData(changeset);

        await NakamaManager.Instance.NakamaUserAccount.GetPlayerBag();

        ShowReward();
    }


    public async void HandleOnBuyCoinOffer(int index)
    {
        if (_storeOffer.price > NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.Gems.ToString()])
        {
            ErrorManager.Instance.ShowError(ErrorType.NOT_ENOUGH_GEMS);
            return;
        }

        await NakamaManager.Instance.NakamaStore.BuyCoinOffer(index);

        Dictionary<string, int> changeset = new Dictionary<string, int>
        {
            { Currency.Coins.ToString(), _storeOffer.offer.amount },
            { Currency.Gems.ToString(), -_storeOffer.price },
        };

        NakamaManager.Instance.NakamaUserAccount.UpdateWalletData(changeset);

        ShowReward();

    }

    public async void HandleOnBuyGemOffer(int index)
    {
        await NakamaManager.Instance.NakamaStore.BuyGemOffer(index);

        Dictionary<string, int> changeset = new Dictionary<string, int>
        {
             { Currency.Gems.ToString(), _storeOffer.offer.amount },
        };

        NakamaManager.Instance.NakamaUserAccount.UpdateWalletData(changeset);

        ShowReward();

    }
    public async void HandleOnBuyDailyShopOffer(int index)
    {
        if (_storeOffer.isAlreadyBuyed)
        {
            ErrorManager.Instance.ShowError(ErrorType.SHOP_ALREADY_BUYED);
            return;
        }
        else if (_storeOffer.price > NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.Coins.ToString()])
        {
            ErrorManager.Instance.ShowError(ErrorType.NOT_ENOUGH_COIN);
            return;
        }

        try
        {
            await NakamaManager.Instance.NakamaStore.BuyDailyShopOffer(index);

            ShowReward();

            _storeOffer.isAlreadyBuyed = true;

            IsBuyable();
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }


    private void ShowReward()
    {
        Reward reward = new Reward();

        reward.type = _storeOffer.offer.type;

        if (_storeOffer.offer.blast != null) reward.blast = _storeOffer.offer.blast;
        if (_storeOffer.offer.item != null) reward.item = _storeOffer.offer.item;

        UIManager.Instance.RewardPopup.OpenPopup();
        UIManager.Instance.RewardPopup.UpdateData(reward);
    }
}
