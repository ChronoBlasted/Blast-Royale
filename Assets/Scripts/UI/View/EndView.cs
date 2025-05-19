using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndView : View
{
    [SerializeField] TMP_Text _title, _amountRegularBlastTxt, _amountBossDefeatedTxt, _amountShinyDefeatedTxt;
    [SerializeField] ProgressionSlotLayout _progressionSlotLayout;
    [SerializeField] Transform _rewardContentTransform;
    [SerializeField] RewardEndGameLayout _rewardEndGameLayout;
    [SerializeField] ChronoTweenSequence _chronoTweenSequence;
    [SerializeField] ChronoTweenObject _claimBtn, _claimX2Btn;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        _ = NakamaManager.Instance.NakamaUserAccount.GetPlayerMetadata(); // TODO Just update locally
        _ = NakamaManager.Instance.NakamaLeaderboards.UpdateLeaderboards(); // TODO Just update correct leaderboard

        _progressionSlotLayout.InitSmooth(WildBattleManager.Instance.IndexProgression);

        UIManager.Instance.DoSmoothTextInt(_amountRegularBlastTxt, 0, WildBattleManager.Instance.BlastDefeated, "<sprite name=\"RegularBlast\">");
        UIManager.Instance.DoSmoothTextInt(_amountBossDefeatedTxt, 0, WildBattleManager.Instance.BossEncounter, "<sprite name=\"BossBlast\">");
        UIManager.Instance.DoSmoothTextInt(_amountShinyDefeatedTxt, 0, WildBattleManager.Instance.ShinyEncounter, "<sprite name=\"Luck\">");

        foreach (Transform transform in _rewardContentTransform)
        {
            Destroy(transform.gameObject);
        }

        _chronoTweenSequence.ObjectsToTween.Clear();

        foreach (Offer offer in WildBattleManager.Instance.WildBattleReward)
        {
            var currentRerward = Instantiate(_rewardEndGameLayout, _rewardContentTransform);
            currentRerward.Init(offer);
            _chronoTweenSequence.ObjectsToTween.Add(currentRerward.GetComponent<ChronoTweenObject>());
        }

        _chronoTweenSequence.ObjectsToTween.Add(_claimBtn);
        _chronoTweenSequence.ObjectsToTween.Add(_claimX2Btn);

        _chronoTweenSequence.Init();

        base.OpenView(_instant);
    }

    public override void CloseView()
    {
        base.CloseView();
    }

    public void UpdateEndGame(bool isWin)
    {
        _title.text = isWin ? "GOOD GAME" : "YOU LOOSE";
    }

    public void HandleOnClaim()
    {
        GameStateManager.Instance.UpdateStateToMenu();
    }

    public void HandleOnClaimAds()
    {
        GameStateManager.Instance.UpdateStateToMenu();
    }
}
