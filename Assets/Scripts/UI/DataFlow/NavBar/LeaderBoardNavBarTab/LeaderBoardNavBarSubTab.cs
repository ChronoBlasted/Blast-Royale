using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LeaderboardSubTab : NavBarTab
{
    [SerializeField] LeaderboardFilter _type;

    [SerializeField] Image _bg;


    public override void HandleOnPress()
    {
        base.HandleOnPress();

        UIManager.Instance.LeaderboardView.LeaderBoardFilter = _type;

        UIManager.Instance.LeaderboardView.UpdateActiveLeaderboard();

        _bg.color = ColorManager.Instance.ActiveColor;
    }

    public override void HandleOnReset()
    {
        base.HandleOnReset();

        _bg.color = ColorManager.Instance.InactiveColor;
    }
}
