using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : Panel
{
    [SerializeField] ScrollRect _scroll;

    [SerializeField] List<ShopLayout> _blastTrapShopLayouts;
    [SerializeField] List<ShopLayout> _coinShopLayouts;
    [SerializeField] List<ShopLayout> _gemShopLayouts;
    [SerializeField] List<ShopLayout> _dailyShopLayouts;
    [SerializeField] RewardedAdsButton _refreshDailyShop;

    [SerializeField] TMP_Text _resetTimerDailyShopTxt;
    DateTime _nextDailyReset;
    TimeSpan _timeRemainingDailyShop;

    public override void Init()
    {
        base.Init();

        _refreshDailyShop.Init();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        UIManager.ResetScroll(_scroll);

        UIManager.Instance.MenuView.TopBar.ShowTopBar();

        DateTime now = DateTime.Now;
        _nextDailyReset = now.Date.AddDays(1);

        UpdateResetTime();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
    }

    void UpdateResetTime()
    {
        DateTime now = DateTime.Now;
        _timeRemainingDailyShop = _nextDailyReset - now;

        if (_timeRemainingDailyShop.TotalSeconds < 0)
        {
            _nextDailyReset = now.Date.AddDays(1);
            _timeRemainingDailyShop = _nextDailyReset - now;
        }

        if (_timeRemainingDailyShop.Hours > 0)
        {
            _resetTimerDailyShopTxt.text = $"Reset in {_timeRemainingDailyShop.Hours} hours";
        }
        else
        {
            _resetTimerDailyShopTxt.text = $"Reset in {_timeRemainingDailyShop.Minutes} min";
        }
    }


    public void UpdateBlastTrapOffer(List<StoreOffer> allStoreOffer)
    {
        for (int i = 0; i < _blastTrapShopLayouts.Count; i++)
        {
            _blastTrapShopLayouts[i].Init(allStoreOffer[i]);
        }
    }

    public void UpdateCoinOffer(List<StoreOffer> allStoreOffer)
    {
        for (int i = 0; i < _coinShopLayouts.Count; i++)
        {
            _coinShopLayouts[i].Init(allStoreOffer[i]);
        }
    }

    public void UpdateGemOffer(List<StoreOffer> allStoreOffer)
    {
        for (int i = 0; i < _gemShopLayouts.Count; i++)
        {
            _gemShopLayouts[i].Init(allStoreOffer[i]);
        }
    }

    public void UpdateDailyShopOffer(List<StoreOffer> allStoreOffer)
    {
        for (int i = 0; i < _dailyShopLayouts.Count; i++)
        {
            _dailyShopLayouts[i].Init(allStoreOffer[i]);
        }
    }

    public void HandleRefreshDailyShop()
    {
        _ = NakamaManager.Instance.NakamaStore.RefreshDailyShop();
    }
}
