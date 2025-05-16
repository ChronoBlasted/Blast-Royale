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

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        _ = NakamaManager.Instance.NakamaUserAccount.GetPlayerMetadata(); // TODO Just update locally
        _ = NakamaManager.Instance.NakamaLeaderboards.UpdateLeaderboards(); // TODO Just update correct leaderboard

        _progressionSlotLayout.Init(WildBattleManager.Instance.IndexProgression);

        _amountRegularBlastTxt.text = "<sprite name=\"RegularBlast\">" + WildBattleManager.Instance.BlastDefeated;
        _amountBossDefeatedTxt.text = "<sprite name=\"BossBlast\">" + WildBattleManager.Instance.BossEncounter;
        _amountShinyDefeatedTxt.text = "<sprite name=\"Luck\">" + WildBattleManager.Instance.ShinyEncounter;

        foreach (Transform transform in _rewardContentTransform)
        {
            Destroy(transform.gameObject);
        }

        foreach (Offer offer in WildBattleManager.Instance.WildBattleReward)
        {
            var currentRerward = Instantiate(_rewardEndGameLayout, _rewardContentTransform);
            currentRerward.Init(offer);
        }

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
