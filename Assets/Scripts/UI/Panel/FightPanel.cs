using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightPanel : Panel
{
    [SerializeField] ProfileLayout _profileLayout;

    [SerializeField] SettingLayout _settingLayout;
    [SerializeField] SquadLayout _squadLayout;
    [SerializeField] AreaLayoutFightPanel _areaLayoutFightPanel;
    [SerializeField] RewardedAdsButton _pveBattleBonusAds, _pvpBattleBonusAds;


    public ProfileLayout ProfileLayout { get => _profileLayout; }
    public AreaLayoutFightPanel AreaLayoutFightPanel { get => _areaLayoutFightPanel; }
    public RewardedAdsButton PvEBattleBonusAds { get => _pveBattleBonusAds; }
    public RewardedAdsButton PvPBattleBonusAds { get => _pvpBattleBonusAds; }
    public SettingLayout SettingLayout { get => _settingLayout; }

    public override void Init()
    {
        base.Init();

        NakamaManager.Instance.NakamaPvEBattle.OnPvEBattleEnd.AddListener(() =>
        {
            _pveBattleBonusAds.RefreshAd();
        });

        NakamaManager.Instance.NakamaPvPBattle.OnPvPBattleEnd.AddListener(() =>
        {
            _pvpBattleBonusAds.RefreshAd();
        });

        _pveBattleBonusAds.Init();
        _pvpBattleBonusAds.Init();
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

    public void HandleOnPvEBattle()
    {
        NakamaManager.Instance.NakamaPvEBattle.FindBattle();
    }

    public void HandleOnPvPBattle()
    {
        NakamaManager.Instance.NakamaPvPBattle.FindBattle();
    }

    public void HandleOnBonusPvERewardsAds()
    {
        _pveBattleBonusAds.SetAdsOn();

        NakamaManager.Instance.NakamaPvEBattle.HandleOnPvERewardsAds();
    }

    public void HandleOnBonusPvPRewardsAds()
    {
        _pvpBattleBonusAds.SetAdsOn();

        NakamaManager.Instance.NakamaPvPBattle.HandleOnPvERewardsAds();
    }

    public void HandleOnOpenQuest()
    {
        UIManager.Instance.QuestPopup.OpenPopup();
    }

}
