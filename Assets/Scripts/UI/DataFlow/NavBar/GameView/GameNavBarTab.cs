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
            .Join(DOVirtual.Float(_layoutElement.flexibleWidth, 1.5f, .2f, x => _layoutElement.flexibleWidth = x))
            .Join(_bg.rectTransform.DOSizeDelta(new Vector2(0, 40), .2f))
            .Join(_title.DOFade(1, .2f))
            .Join(_ico.rectTransform.DOSizeDelta(new Vector2(256, 256), .2f))
            .Join(_ico.rectTransform.DOAnchorPosY(10, .2f));

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
            .Join(DOVirtual.Float(_layoutElement.flexibleWidth, 1, .2f, x => _layoutElement.flexibleWidth = x))
            .Join(_bg.rectTransform.DOSizeDelta(new Vector2(0, 0), .2f))
            .Join(_title.DOFade(0, .1f))
            .Join(_ico.rectTransform.DOSizeDelta(new Vector2(128, 128), .2f))
            .Join(_ico.rectTransform.DOAnchorPosY(-100, .2f));

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

        UIManager.Instance.ChangeBlastPopup.UpdateAction(actions);

        UIManager.Instance.ChangeBlastPopup.UpdateClose(UIManager.Instance.GameView.ResetTab);
    }

    public void HandleOnWait()
    {
        WildBattleManager.Instance.PlayerWait();
    }
}
