using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardTab : NavBarTab
{
    [SerializeField] LeaderboardType _type;

    [SerializeField] Image _bg;


    public override void HandleOnPress()
    {
        base.HandleOnPress();

        UIManager.Instance.RegularLeaderboardView.LeaderboardType = _type;

        UIManager.Instance.RegularLeaderboardView.UpdateActiveLeaderboard();

        _bg.sprite = ColorManager.Instance.ActiveSprite;
    }

    public override void HandleOnReset()
    {
        base.HandleOnReset();

        _bg.sprite = ColorManager.Instance.InactiveSprite;
    }
}
