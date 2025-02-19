using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : Panel
{
    [SerializeField] List<ShopLayout> _blastTrapShopLayouts;
    [SerializeField] List<ShopLayout> _coinShopLayouts;
    [SerializeField] List<ShopLayout> _gemShopLayouts;
    [SerializeField] List<ShopLayout> _dailyShopLayouts;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        UIManager.Instance.MenuView.TopBar.ShowTopBar();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
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
}
