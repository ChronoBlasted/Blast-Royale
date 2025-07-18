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

        UIManager.Instance.RegularLeaderboardView.LeaderBoardFilter = _type;

        UIManager.Instance.RegularLeaderboardView.UpdateActiveLeaderboard();

        _bg.sprite = ColorManager.Instance.ActiveSprite;
    }

    public override void HandleOnReset()
    {
        base.HandleOnReset();

        _bg.sprite = ColorManager.Instance.InactiveSprite;
    }
}
