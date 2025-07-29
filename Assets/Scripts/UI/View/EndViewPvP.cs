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

    PvPBattleManager battleManager;

    public override void OpenView(bool _instant = false)
    {
        battleManager = NakamaManager.Instance.NakamaBattleManager.PvpBattle.BattleManager as PvPBattleManager;


        foreach (Transform transform in _rewardContentTransform)
        {
            Destroy(transform.gameObject);
        }

        UpdateEndGame(battleManager.EndStateData.win);

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

            foreach (Reward reward in battleManager.BattleReward)
            {
                var currentRerward = Instantiate(_rewardEndGameLayout, _rewardContentTransform);
                currentRerward.Init(reward);
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

        UIManager.Instance.DoSmoothTextInt(_p1TrophyGain, 0, battleManager.EndStateData.trophyRewards, isWin ? "+" : "");
    }

    public async void HandleOnClaim()
    {
        Dictionary<string, int> changeset = new Dictionary<string, int>
        {
            { Currency.Coins.ToString(), battleManager.CoinGenerated },
            { Currency.Gems.ToString(), battleManager.GemGenerated },
            { Currency.Trophies.ToString(), battleManager.EndStateData.trophyRewards },
        };

        NakamaManager.Instance.NakamaUserAccount.UpdateWalletData(changeset);

        await NakamaManager.Instance.NakamaLeaderboards.UpdateLeaderboards();

        GameStateManager.Instance.UpdateStateToMenu();
    }
}
