using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightPanel : Panel
{
    [SerializeField] ProfileLayout _profileLayout;

    [SerializeField] SquadLayout _squadLayout;
    [SerializeField] AreaLayoutFightPanel _areaLayoutFightPanel;
    [SerializeField] RewardedAdsButton _wildBattleBonusAds;


    public ProfileLayout ProfileLayout { get => _profileLayout; }
    public AreaLayoutFightPanel AreaLayoutFightPanel { get => _areaLayoutFightPanel; }
    public RewardedAdsButton WildBattleBonusAds { get => _wildBattleBonusAds; }

    public override void Init()
    {
        base.Init();

        NakamaManager.Instance.NakamaWildBattle.OnWildBattleEnd.AddListener(() =>
        {
            _wildBattleBonusAds.RefreshAd();
        });

        _wildBattleBonusAds.Init();
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

    public void UpdateDeckBlast(List<Blast> decks)
    {
        _squadLayout.UpdateDeckBlast(decks);
    }

    public void HandleOnWildBattle()
    {
        NakamaManager.Instance.NakamaWildBattle.FindWildBattle();
    }

    public void HandleOnPlayerBattle()
    {
        Debug.Log("Coming soon");
    }

    public void HandleOnBonusRewardsAds()
    {
        _wildBattleBonusAds.SetAdsOn();

        NakamaManager.Instance.NakamaWildBattle.HandleOnBonusRewardsAds();
    }

}
