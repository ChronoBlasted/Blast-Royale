using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndViewPvP : View
{
    [SerializeField] TMP_Text _p1VictoryTxt, _p2VictoryTxt, _p1Username, _p2Username, _p1TrophyGain;

    [SerializeField] Image _p1Glow, _p2Glow;

    [SerializeField] GameObject _rewardTitleLayout;

    [SerializeField] List<EndBlastLayout> _p1Blasts, _p2Blasts;

    [SerializeField] Transform _rewardContentTransform;
    [SerializeField] RewardEndGameLayout _rewardEndGameLayout;

    [SerializeField] ChronoTweenSequence _chronoTweenSequence;
    [SerializeField] ChronoTweenObject _claimBtn;

    public override void OpenView(bool _instant = false)
    {
        PvPBattleManager battleManager = NakamaManager.Instance.NakamaBattleManager.PvpBattle.BattleManager as PvPBattleManager;

        foreach (Transform transform in _rewardContentTransform)
        {
            Destroy(transform.gameObject);
        }

        UpdateEndGame(battleManager.IsMatchWin);

        _p1Username.text = battleManager.PlayerMeInfo.Username;
        _p2Username.text = battleManager.PlayerOpponentInfo.Username;

        _chronoTweenSequence.ObjectsToTween.Clear();

        for (int i = 0; i < battleManager.OpponentSquad.Count; i++)
        {
            _p2Blasts[i].Init(battleManager.OpponentSquad[i]);

            _chronoTweenSequence.ObjectsToTween.Add(_p2Blasts[i].GetComponent<ChronoTweenObject>());
        }

        for (int i = 0; i < battleManager.PlayerSquad.Count; i++)
        {
            _p1Blasts[i].Init(battleManager.PlayerSquad[i]);

            _chronoTweenSequence.ObjectsToTween.Add(_p1Blasts[i].GetComponent<ChronoTweenObject>());
        }

        if (battleManager.BattleReward.Count > 0)
        {
            _rewardTitleLayout.SetActive(true);

            foreach (Offer offer in battleManager.BattleReward)
            {
                var currentRerward = Instantiate(_rewardEndGameLayout, _rewardContentTransform);
                currentRerward.Init(offer);
                _chronoTweenSequence.ObjectsToTween.Add(currentRerward.GetComponent<ChronoTweenObject>());
            }
        }
        else
        {
            _rewardTitleLayout.SetActive(false);
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
        _p1VictoryTxt.enabled = isWin;
        _p2VictoryTxt.enabled = !isWin;

        _p1Glow.enabled = isWin;
        _p2Glow.enabled = !isWin;

        UIManager.Instance.DoSmoothTextInt(_p1TrophyGain, 0, isWin ? +20 : -20, isWin ? "+" : "");
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
