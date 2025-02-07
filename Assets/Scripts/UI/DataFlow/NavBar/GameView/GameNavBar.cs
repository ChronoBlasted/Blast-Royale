using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNavBar : NavBar
{
    [SerializeField] CanvasGroup _cg;

    Sequence _showHideBarTween;
    public void Show()
    {
        if (_showHideBarTween.IsActive()) _showHideBarTween.Kill(true);

        _showHideBarTween = DOTween.Sequence();

        RectTransform rectTransform = (RectTransform)transform;

        _cg.interactable = true;
        _cg.blocksRaycasts = true;

        _showHideBarTween
            .Join(rectTransform.DOAnchorPosY(150, .2f).SetEase(Ease.OutBack))
            .Join(_cg.DOFade(1, .1f));

    }

    public void Hide()
    {
        if (_showHideBarTween.IsActive()) _showHideBarTween.Kill(true);

        _showHideBarTween = DOTween.Sequence();

        RectTransform rectTransform = (RectTransform)transform;

        _cg.interactable = false;
        _cg.blocksRaycasts = false;

        _showHideBarTween
            .Join(rectTransform.DOAnchorPosY(-150, .2f).SetEase(Ease.OutBack))
            .Join(_cg.DOFade(0, .1f));
    }
}
