using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class NakamaStore : MonoBehaviour
{
    IClient _client;
    ISession _session;

    CanClaimDailyShop _canClaimDailyShop;

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await GetAllBlastTrapOffer();
        await GetAllCoinOffer();
        await GetAllGemOffer();

        if (await CanClaimDailyShop()) await ClaimDailyShop();
    }

    #region Offer

    async Task GetAllBlastTrapOffer()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadBlastTrapOffer");

            List<StoreOffer> allBlastTrapOfferList = response.Payload.FromJson<List<StoreOffer>>();

            UIManager.Instance.MenuView.ShopPanel.UpdateBlastTrapOffer(allBlastTrapOfferList);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async Task BuyTrapOffer(int index)
    {
        try
        {
            var response = await _client.RpcAsync(_session, "buyTrapOffer", index.ToJson());
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    async Task GetAllCoinOffer()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadCoinOffer");

            List<StoreOffer> allCoinOfferList = response.Payload.FromJson<List<StoreOffer>>();

            UIManager.Instance.MenuView.ShopPanel.UpdateCoinOffer(allCoinOfferList);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async Task BuyCoinOffer(int index)
    {
        try
        {
            var response = await _client.RpcAsync(_session, "buyCoinOffer", index.ToJson());
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    async Task GetAllGemOffer()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadGemOffer");

            List<StoreOffer> allGemOfferList = response.Payload.FromJson<List<StoreOffer>>();

            UIManager.Instance.MenuView.ShopPanel.UpdateGemOffer(allGemOfferList);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async Task BuyGemOffer(int index)
    {
        try
        {
            var response = await _client.RpcAsync(_session, "buyGemOffer", index.ToJson());
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    #endregion

    private async Task<bool> CanClaimDailyShop()
    {
        try
        {
            var canClaimRewardResponse = await _client.RpcAsync(_session, "canClaimDailyShop");

            _canClaimDailyShop = canClaimRewardResponse.Payload.FromJson<CanClaimDailyShop>();

            UIManager.Instance.MenuView.ShopPanel.UpdateDailyShopOffer(_canClaimDailyShop.lastDailyShop);

            return _canClaimDailyShop.canClaimDailyShop;
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }

        return false;
    }

    public async Task ClaimDailyShop()
    {
        if (_canClaimDailyShop.canClaimDailyShop == false) return;

        try
        {
            var claimRewardResponse = await _client.RpcAsync(_session, "claimDailyShop");

            var canClaimRewardDict = claimRewardResponse.Payload.FromJson<CanClaimDailyShop>();

            UIManager.Instance.MenuView.ShopPanel.UpdateDailyShopOffer(_canClaimDailyShop.lastDailyShop);

            await CanClaimDailyShop();
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async Task BuyDailyShopOffer(int index)
    {
        try
        {
            if (_canClaimDailyShop.lastDailyShop[index].isAlreadyBuyed == true) return;

            var response = await _client.RpcAsync(_session, "buyDailyShopOffer", index.ToJson());

            Dictionary<string, int> changeset = new Dictionary<string, int>
            {
                 { Currency.Coins.ToString(), -_canClaimDailyShop.lastDailyShop[index].price },
            };

            NakamaManager.Instance.NakamaUserAccount.UpdateWalletData(changeset);

            if (_canClaimDailyShop.lastDailyShop[index].reward.type == RewardType.Blast) NakamaManager.Instance.NakamaUserAccount.AddPlayerBlast(_canClaimDailyShop.lastDailyShop[index].reward.blast);
            if (_canClaimDailyShop.lastDailyShop[index].reward.type == RewardType.Item) NakamaManager.Instance.NakamaUserAccount.AddOrUpdateItem(_canClaimDailyShop.lastDailyShop[index].reward.item);

        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }



    public async Task RefreshDailyShop()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "watchRefreshShopAds");

            var newDailyShopRequest = response.Payload.FromJson<CanClaimDailyShop>();

            UIManager.Instance.MenuView.ShopPanel.UpdateDailyShopOffer(newDailyShopRequest.lastDailyShop);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
}

[Serializable]
public class StoreOffer
{
    public int offer_id;
    public Reward reward;

    public int price;
    public Currency currency;

    public bool isAlreadyBuyed;
}

[Serializable]
public class Reward
{
    public RewardType type;
    public int amount;
    public Blast blast;
    public Item item;
    public bool isBonus;
}

public class CanClaimDailyShop
{
    public bool canClaimDailyShop;
    public List<StoreOffer> lastDailyShop;
}