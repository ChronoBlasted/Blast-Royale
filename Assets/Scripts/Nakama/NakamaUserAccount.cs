using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaUserAccount : MonoBehaviour
{
    IClient _client;
    ISession _session;

    IApiAccount _lastAccount;

    BlastCollection _lastBlastCollection;
    ItemCollection _lastItemCollection;

    Metadata _lastData;
    Dictionary<string, int> _lastWalletData;

    public Dictionary<string, int> LastWalletData { get => _lastWalletData; set => _lastWalletData = value; }
    public BlastCollection LastBlastCollection { get => _lastBlastCollection; }
    public ItemCollection LastItemCollection { get => _lastItemCollection; }
    public IApiAccount LastAccount { get => _lastAccount; }

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await GetPlayerData();
        await GetWalletData();
        await GetPlayerBlast();
        await GetPlayerBag();
    }

    async Task GetPlayerData()
    {
        _lastAccount = await _client.GetAccountAsync(_session);
        _lastData = JsonParser.FromJson<Metadata>(_lastAccount.User.Metadata);

        var username = _lastAccount.User.Username;
        var avatarUrl = _lastAccount.User.AvatarUrl;

        UIManager.Instance.ProfilePopup.UpdateData(_lastData, username);

        UIManager.Instance.FriendView.UpdateUsername(username);
        UIManager.Instance.MenuView.FightPanel.ProfileLayout.UpdateUsername(username);
    }

    #region ApiAccountUpdate

    public async Task UpdateUsername(string newUsername)
    {
        await _client.UpdateAccountAsync(
            _session,
            newUsername,
            null,
            null,
            null,
            null,
            null
        );

        HaveUpdateDisplayName();

        UIManager.Instance.FriendView.UpdateUsername(newUsername);
        UIManager.Instance.MenuView.FightPanel.ProfileLayout.UpdateUsername(newUsername);
    }

    public async Task UpdateAvatarUrl(string newAvatarUrl)
    {
        await _client.UpdateAccountAsync(
            _session,
            null,
            null,
            newAvatarUrl,
            null,
            null,
            null
        );
    }



    public async Task UpdateLangTag(string userId, string newLangTag)
    {
        await _client.UpdateAccountAsync(
            _session,
            null,
            null,
            null,
            newLangTag,
            null,
            null
        );
    }

    public async Task UpdateLocation(string userId, string newLocation)
    {
        await _client.UpdateAccountAsync(
            _session,
            null,
            null,
            null,
            null,
            newLocation,
            null
        );
    }

    public async Task UpdateTimezone(string userId, string newTimezone)
    {
        await _client.UpdateAccountAsync(
            _session,
            null,
            null,
            null,
            null,
            null,
            newTimezone
        );
    }

    #endregion

    public async Task GetWalletData()
    {
        _lastAccount = await _client.GetAccountAsync(_session);

        LastWalletData = JsonParser.FromJson<Dictionary<string, int>>(_lastAccount.Wallet);

        foreach (var currency in LastWalletData.Keys)
        {
            if (currency == Currency.Coins.ToString())
            {
                UIManager.Instance.MenuView.TopBar.UpdateCoin(LastWalletData[currency]);
            }
            if (currency == Currency.Gems.ToString())
            {
                UIManager.Instance.MenuView.TopBar.UpdateGem(LastWalletData[currency]);
            }
            if (currency == Currency.Trophies.ToString())
            {
                UIManager.Instance.MenuView.TopBar.UpdateTrophy(LastWalletData[currency]);
            }
        }
    }

    public async Task GetPlayerBlast()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadUserBlast");

            _lastBlastCollection = response.Payload.FromJson<BlastCollection>();

            NakamaData.Instance.BlastCollection = _lastBlastCollection;

            //Update Squad Panel
            UIManager.Instance.MenuView.SquadPanel.UpdateDeckBlast(_lastBlastCollection.deckBlasts);
            UIManager.Instance.MenuView.SquadPanel.UpdateStoredBlast(_lastBlastCollection.storedBlasts);

            //Update Fight Panel
            UIManager.Instance.MenuView.FightPanel.UpdateDeckBlast(_lastBlastCollection.deckBlasts);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async void SwitchPlayerBlast(int indexDeckBlast, int indexStoredBlast)
    {
        try
        {
            SwapDeckRequest swapDeckBlastRequest = new SwapDeckRequest();

            swapDeckBlastRequest.outIndex = indexDeckBlast;
            swapDeckBlastRequest.inIndex = indexStoredBlast;

            var response = await _client.RpcAsync(_session, "swapDeckBlast", swapDeckBlastRequest.ToJson());

            _lastBlastCollection = response.Payload.FromJson<BlastCollection>();

            //Update Squad Panel
            UIManager.Instance.MenuView.SquadPanel.UpdateDeckBlast(_lastBlastCollection.deckBlasts);
            UIManager.Instance.MenuView.SquadPanel.UpdateStoredBlast(_lastBlastCollection.storedBlasts);

            //Update Fight Panel
            UIManager.Instance.MenuView.FightPanel.UpdateDeckBlast(_lastBlastCollection.deckBlasts);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
    public async void EvolveBlast(string uuidBlast)
    {
        try
        {
            var response = await _client.RpcAsync(_session, "evolveBlast", uuidBlast.ToJson());

            _lastBlastCollection = response.Payload.FromJson<BlastCollection>();

            //Update Squad Panel
            UIManager.Instance.MenuView.SquadPanel.UpdateDeckBlast(_lastBlastCollection.deckBlasts);
            UIManager.Instance.MenuView.SquadPanel.UpdateStoredBlast(_lastBlastCollection.storedBlasts);

            //Update Fight Panel
            UIManager.Instance.MenuView.FightPanel.UpdateDeckBlast(_lastBlastCollection.deckBlasts);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async void SwitchMoveBlast(string uuidBlast, int outMoveIndex, int newMoveIndex)
    {
        try
        {
            SwapMoveRequest swapMoveRequest = new SwapMoveRequest();

            swapMoveRequest.uuidBlast = uuidBlast;
            swapMoveRequest.outMoveIndex = outMoveIndex;
            swapMoveRequest.newMoveIndex = newMoveIndex;

            var response = await _client.RpcAsync(_session, "swapMove", swapMoveRequest.ToJson());

            _lastBlastCollection = response.Payload.FromJson<BlastCollection>();

            //Update Squad Panel
            UIManager.Instance.MenuView.SquadPanel.UpdateDeckBlast(_lastBlastCollection.deckBlasts);
            UIManager.Instance.MenuView.SquadPanel.UpdateStoredBlast(_lastBlastCollection.storedBlasts);

            //Update Fight Panel
            UIManager.Instance.MenuView.FightPanel.UpdateDeckBlast(_lastBlastCollection.deckBlasts);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async void PrestigeBlast(int indexDeckBlast)
    {
        try
        {
            var response = await _client.RpcAsync(_session, "prestigeBlast", indexDeckBlast.ToString());

            _lastBlastCollection = response.Payload.FromJson<BlastCollection>();

            //Update Squad Panel
            UIManager.Instance.MenuView.SquadPanel.UpdateDeckBlast(_lastBlastCollection.deckBlasts);
            UIManager.Instance.MenuView.SquadPanel.UpdateStoredBlast(_lastBlastCollection.storedBlasts);

            //Update Fight Panel
            UIManager.Instance.MenuView.FightPanel.UpdateDeckBlast(_lastBlastCollection.deckBlasts);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }


    public async Task GetPlayerBag()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "loadUserItem");

            _lastItemCollection = response.Payload.FromJson<ItemCollection>();

            NakamaData.Instance.ItemCollection = _lastItemCollection;

            //Update Squad Panel
            UIManager.Instance.MenuView.SquadPanel.UpdateDeckItem(_lastItemCollection.deckItems);
            UIManager.Instance.MenuView.SquadPanel.UpdateStoredItem(_lastItemCollection.storedItems);

        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async void SwitchPlayerItem(int indexDeckItem, int indexStoredItem)
    {
        try
        {
            SwapDeckRequest swapDeckItemRequest = new SwapDeckRequest();

            swapDeckItemRequest.outIndex = indexDeckItem;
            swapDeckItemRequest.inIndex = indexStoredItem;

            var response = await _client.RpcAsync(_session, "swapDeckItem", swapDeckItemRequest.ToJson());

            _lastItemCollection = response.Payload.FromJson<ItemCollection>();

            //Update Squad Panel
            UIManager.Instance.MenuView.SquadPanel.UpdateDeckItem(_lastItemCollection.deckItems);
            UIManager.Instance.MenuView.SquadPanel.UpdateStoredItem(_lastItemCollection.storedItems);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async void DeleteAccount()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "deleteAccount");

            PlayerPrefs.DeleteAll();

            GameManager.Instance.ReloadScene();
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async void HaveUpdateDisplayName()
    {
        try
        {
            var response = await _client.RpcAsync(_session, "updateNicknameStatus");
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }

    public async void UsePromoCode(string promoCode)
    {
        try
        {
            var response = await _client.RpcAsync(_session, "usePromoCode", promoCode.ToJson());

        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }
}


[Serializable]
public class BlastCollection
{
    public List<Blast> deckBlasts;
    public List<Blast> storedBlasts;
}

[Serializable]
public class ItemCollection
{
    public List<Item> deckItems;
    public List<Item> storedItems;
}

public class SwapDeckRequest
{
    public int inIndex;
    public int outIndex;
}

public class SwapMoveRequest
{
    public string uuidBlast;
    public int outMoveIndex;
    public int newMoveIndex;
}

public class Metadata
{
    public bool battle_pass;
    public bool updated_nickname;
    public int area;
    public int win;
    public int loose;
    public int blast_captured;
    public int blast_defeated;
}
