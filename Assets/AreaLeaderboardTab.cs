using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaLeaderboardTab : NavBarTab
{
    [SerializeField] LeaderboardType _type;

    [SerializeField] Image _bg;


    public override void HandleOnPress()
    {
        base.HandleOnPress();

        UIManager.Instance.AreaLeaderboardView.LeaderboardType = _type;

        UIManager.Instance.AreaLeaderboardView.UpdateActiveLeaderboard();

        _bg.sprite = ColorManager.Instance.ActiveSprite;
    }

    public override void HandleOnReset()
    {
        base.HandleOnReset();

        _bg.sprite = ColorManager.Instance.InactiveSprite;
    }
}
