using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class EndViewPvE : View
{
    [SerializeField] TMP_Text _title, _amountRegularBlastTxt, _amountBossDefeatedTxt, _amountShinyDefeatedTxt;
    [SerializeField] ProgressionSlotLayout _progressionSlotLayout;
    [SerializeField] Transform _rewardContentTransform;
    [SerializeField] RewardEndGameLayout _rewardEndGameLayout;
    [SerializeField] ChronoTweenSequence _chronoTweenSequence;
    [SerializeField] ChronoTweenObject _claimBtn;

    int coinGained;
    int gemGained;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        PvEBattleManager battleManager = NakamaManager.Instance.NakamaBattleManager.PveBattle.BattleManager as PvEBattleManager;

        _title.text = battleManager.EndStateData.win ? "GOOD GAME" : "YOU LOOSE";

        _progressionSlotLayout.InitSmooth(battleManager.IndexProgression);

        UIManager.Instance.DoSmoothTextInt(_amountRegularBlastTxt, 0, battleManager.BlastDefeated, "<sprite name=\"RegularBlast\">");
        UIManager.Instance.DoSmoothTextInt(_amountBossDefeatedTxt, 0, battleManager.BossEncounter, "<sprite name=\"BossBlast\">");
        UIManager.Instance.DoSmoothTextInt(_amountShinyDefeatedTxt, 0, battleManager.ShinyEncounter, "<sprite name=\"Luck\">");

        foreach (Transform transform in _rewardContentTransform)
        {
            Destroy(transform.gameObject);
        }

        _chronoTweenSequence.ObjectsToTween.Clear();

        foreach (Reward reward in battleManager.BattleReward)
        {
            var currentRerward = Instantiate(_rewardEndGameLayout, _rewardContentTransform);
            currentRerward.Init(reward);
            _chronoTweenSequence.ObjectsToTween.Add(currentRerward.GetComponent<ChronoTweenObject>());

            if (reward.type == RewardType.Coin) coinGained += reward.amount;
            if (reward.type == RewardType.Gem) gemGained += reward.amount;
        }

        _chronoTweenSequence.ObjectsToTween.Add(_claimBtn);

        _chronoTweenSequence.Init();

        CameraManager.Instance.SmoothCameraZoom(10, 2f);

        base.OpenView(_instant);
    }

    public override void CloseView()
    {
        base.CloseView();

        CameraManager.Instance.SetCameraZoom(7);
    }

    public async void HandleOnClaim()
    {
        Dictionary<string, int> changeset = new Dictionary<string, int>
        {
            { Currency.Coins.ToString(), coinGained },
            { Currency.Gems.ToString(), gemGained },
        };

        NakamaManager.Instance.NakamaUserAccount.UpdateWalletData(changeset);

        await NakamaManager.Instance.NakamaLeaderboards.UpdateLeaderboards();

        GameStateManager.Instance.UpdateStateToMenu();
    }
}
