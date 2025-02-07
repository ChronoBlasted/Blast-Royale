using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LeaderboardSubTab : NavBarTab
{
    [SerializeField] LeaderboardFilter _type;

    [SerializeField] Image _bg;
    [SerializeField] Sprite _activeSprite, _inactiveSprite;


    public override void HandleOnPress()
    {
        base.HandleOnPress();

        UIManager.Instance.LeaderboardView.LeaderBoardFilter = _type;

        UIManager.Instance.LeaderboardView.UpdateActiveLeaderboard();

        _bg.sprite = _activeSprite;
    }

    public override void HandleOnReset()
    {
        base.HandleOnReset();

        _bg.sprite = _inactiveSprite;
    }
}
