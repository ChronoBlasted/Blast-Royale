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
    [SerializeField] Sprite _activeSprite, _inactiveSprite;


    public override void HandleOnPress()
    {
        base.HandleOnPress();

        UIManager.Instance.LeaderboardView.LeaderboardType = _type;

        UIManager.Instance.LeaderboardView.UpdateActiveLeaderboard();

        _bg.sprite = _activeSprite;
    }

    public override void HandleOnReset()
    {
        base.HandleOnReset();

        _bg.sprite = _inactiveSprite;
    }
}
