using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameNavBarTab : NavBarTab
{
    [SerializeField] LayoutElement _layoutElement;
    [SerializeField] Image _bg, _ico;
    [SerializeField] TMP_Text _title;

    [SerializeField] Panel _tab;

    Sequence _growSequence;

    public override void HandleOnPress()
    {
        if (_growSequence.IsActive()) _growSequence.Kill();

        _growSequence = DOTween.Sequence();

        _growSequence
            .Join(_ico.rectTransform.DOSizeDelta(new Vector2(192, 192), .2f))
            .Join(_ico.rectTransform.DOAnchorPosY(32, .2f));


        if (_tab != null)
        {
            _tab.OpenPanel();
        }
        else
        {
            UIManager.Instance.GameView.CloseCurrentPanel();
        }
    }

    public override void HandleOnReset()
    {
        if (_growSequence.IsActive()) _growSequence.Kill();

        _growSequence = DOTween.Sequence();

        _growSequence
            .Join(_ico.rectTransform.DOSizeDelta(new Vector2(128, 128), .2f))
            .Join(_ico.rectTransform.DOAnchorPosY(0, .2f));


        if (_tab != null)
        {
            _tab.ClosePanel();
        }
    }

    public void HandleOnOpenSquad()
    {
        UIManager.Instance.ChangeBlastPopup.OpenPopup();

        List<UnityAction<int>> actions = new List<UnityAction<int>>()
        {
            WildBattleManager.Instance.PlayerChangeBlast,
        };

        UIManager.Instance.ChangeBlastPopup.UpdateAction(actions, CHANGE_REASON.SWAP);
    }

    public void HandleOnWait()
    {
        WildBattleManager.Instance.PlayerWait();
    }

    public void HandleOnLeaveBattle()
    {
        UIManager.Instance.ConfirmPopup.UpdateData("LEAVE BATTLE ?", "You will leave the battle, continue ?", NakamaManager.Instance.NakamaWildBattle.LeaveMatch);
        UIManager.Instance.ConfirmPopup.OpenPopup();
    }
}
