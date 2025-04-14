using Chrono.UI;
using Nakama;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopLayout : MonoBehaviour
{
    [SerializeField] Image _ico, _priceIco;
    [SerializeField] TMP_Text _nameTxt, _descTxt, _priceAmount;
    [SerializeField] GameObject _buyedBlackShade;

    [SerializeField] CustomButton _buyButton;

    StoreOffer _offer;

    public void Init(StoreOffer storeOffer)
    {
        _offer = storeOffer;

        _priceAmount.text = _offer.price.ToString();

        StoreOfferDataRef storeOfferRef;


        if (_offer.coinsAmount > 0)
        {
            storeOfferRef = NakamaData.Instance.GetStoreOfferDataRef(_offer.offer_id);

            _ico.sprite = storeOfferRef.Sprite;

            _nameTxt.text = storeOfferRef.Name.GetLocalizedString();
            _descTxt.text = "x" + UIManager.GetFormattedInt(storeOffer.coinsAmount);
        }
        if (_offer.gemsAmount > 0)
        {
            storeOfferRef = NakamaData.Instance.GetStoreOfferDataRef(_offer.offer_id);

            _ico.sprite = storeOfferRef.Sprite;

            _nameTxt.text = storeOfferRef.Name.GetLocalizedString();
            _descTxt.text = "x" + UIManager.GetFormattedInt(storeOffer.gemsAmount);
        }
        if (_offer.blast != null)
        {
            BlastDataRef blastData = NakamaData.Instance.GetBlastDataRef(storeOffer.blast.data_id);

            _nameTxt.text = blastData.Name.GetLocalizedString();
            _descTxt.text = "lvl." + NakamaLogic.CalculateLevelFromExperience(storeOffer.blast.exp);
        }
        if (_offer.item != null)
        {
            ItemDataRef itemDataRef = NakamaData.Instance.GetItemDataRef(storeOffer.item.data_id);

            ItemData itemData = NakamaData.Instance.GetItemDataById(_offer.item.data_id);

            _ico.sprite = itemDataRef.Sprite;

            _nameTxt.text = itemDataRef.Name.GetLocalizedString();
            _descTxt.text = "x1";
        }

        switch (_offer.currency)
        {
            case Currency.Coins:
                _priceIco.gameObject.SetActive(true);

                _priceIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin).Sprite;
                break;
            case Currency.Gems:
                _priceIco.gameObject.SetActive(true);

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
            _buyedBlackShade.SetActive(true);
        }
        else
        {
            _buyedBlackShade.SetActive(false);
        }
    }

    public async void HandleOnBuyBlastTrapOffer(int index)
    {
        if (_offer.price > NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.Coins.ToString()])
        {
            ErrorManager.Instance.ShowError(ErrorType.NOT_ENOUGH_COIN);
            return;
        }

        await NakamaManager.Instance.NakamaStore.BuyTrapOffer(index);

        ShowReward();
    }


    public async void HandleOnBuyCoinOffer(int index)
    {
        if (_offer.price > NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.Gems.ToString()])
        {
            ErrorManager.Instance.ShowError(ErrorType.NOT_ENOUGH_GEMS);
            return;
        }

        await NakamaManager.Instance.NakamaStore.BuyCoinOffer(index);

        ShowReward();

    }

    public async void HandleOnBuyGemOffer(int index)
    {
        await NakamaManager.Instance.NakamaStore.BuyGemOffer(index);

        ShowReward();

    }
    public async void HandleOnBuyDailyShopOffer(int index)
    {
        if (_offer.isAlreadyBuyed)
        {
            ErrorManager.Instance.ShowError(ErrorType.SHOP_ALREADY_BUYED);
            return;
        }
        else if (_offer.price > NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.Coins.ToString()])
        {
            ErrorManager.Instance.ShowError(ErrorType.NOT_ENOUGH_COIN);
            return;
        }

        try
        {
            await NakamaManager.Instance.NakamaStore.BuyDailyShopOffer(index);

            ShowReward();

            _offer.isAlreadyBuyed = true;

            IsBuyable();
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }


    private void ShowReward()
    {
        RewardCollection reward = new RewardCollection();

        reward.coinsReceived = _offer.coinsAmount;
        reward.gemsReceived = _offer.gemsAmount;

        if (_offer.blast != null) reward.blastReceived = _offer.blast;
        if (_offer.item != null) reward.itemReceived = _offer.item;

        UIManager.Instance.RewardPopup.OpenPopup();
        UIManager.Instance.RewardPopup.UpdateData(reward);
    }
}
