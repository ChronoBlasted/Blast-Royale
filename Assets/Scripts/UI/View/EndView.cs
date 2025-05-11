using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndView : View
{
    [SerializeField] TMP_Text _title, _desc;
    [SerializeField] RewardEndGameLayout _rewardEndGameLayout;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);

        Debug.Log("OPEN END");

        _ = NakamaManager.Instance.NakamaUserAccount.GetPlayerMetadata(); // TODO Just update locally
        _ = NakamaManager.Instance.NakamaLeaderboards.UpdateLeaderboards(); // TODO Just update correct leaderboard
    }

    public override void CloseView()
    {
        base.CloseView();
    }

    public void UpdateEndGame(bool isWin)
    {
        _title.text = isWin ? "YOU WIN" : "YOU LOOSE";

        _desc.text = "You win " + 100 + " coins"; // TODO Add more complexity
    }

    public void HandleOnReturnButton()
    {
        NakamaManager.Instance.NakamaWildBattle.LeaveMatch();
    }
}
