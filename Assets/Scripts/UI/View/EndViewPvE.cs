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

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        PvEBattleManager battleManager = NakamaManager.Instance.NakamaBattleManager.PveBattle.BattleManager as PvEBattleManager;


        _progressionSlotLayout.InitSmooth(battleManager.IndexProgression);

        UIManager.Instance.DoSmoothTextInt(_amountRegularBlastTxt, 0, battleManager.BlastDefeated, "<sprite name=\"RegularBlast\">");
        UIManager.Instance.DoSmoothTextInt(_amountBossDefeatedTxt, 0, battleManager.BossEncounter, "<sprite name=\"BossBlast\">");
        UIManager.Instance.DoSmoothTextInt(_amountShinyDefeatedTxt, 0, battleManager.ShinyEncounter, "<sprite name=\"Luck\">");

        foreach (Transform transform in _rewardContentTransform)
        {
            Destroy(transform.gameObject);
        }

        _chronoTweenSequence.ObjectsToTween.Clear();

        foreach (Offer offer in battleManager.BattleReward)
        {
            var currentRerward = Instantiate(_rewardEndGameLayout, _rewardContentTransform);
            currentRerward.Init(offer);
            _chronoTweenSequence.ObjectsToTween.Add(currentRerward.GetComponent<ChronoTweenObject>());
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

    public void UpdateEndGame(bool isWin)
    {
        _title.text = isWin ? "GOOD GAME" : "YOU LOOSE";
    }

    public async void HandleOnClaim()
    {
        await NakamaManager.Instance.NakamaUserAccount.GetWalletData();

        await NakamaManager.Instance.NakamaUserAccount.GetPlayerBlast();
        await NakamaManager.Instance.NakamaUserAccount.GetPlayerBag();

        await NakamaManager.Instance.NakamaLeaderboards.UpdateLeaderboards();

        await NakamaManager.Instance.NakamaBlastTracker.LoadBlastTracker();

        await NakamaManager.Instance.NakamaQuest.LoadDailyQuest();
        await NakamaManager.Instance.NakamaQuest.LoadDailyQuestRewards();

        GameStateManager.Instance.UpdateStateToMenu();
    }
}
