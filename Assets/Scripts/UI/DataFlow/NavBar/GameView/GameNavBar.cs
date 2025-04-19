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
            .Join(rectTransform.DOAnchorPosY(120, .2f).SetEase(Ease.OutBack))
            .Join(_cg.DOFade(1, .1f));

    }

    public void Hide(bool instant = false)
    {
        if (_showHideBarTween.IsActive()) _showHideBarTween.Kill(true);

        _showHideBarTween = DOTween.Sequence();

        RectTransform rectTransform = (RectTransform)transform;

        _cg.interactable = false;
        _cg.blocksRaycasts = false;

        if (instant)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -128);
            _cg.alpha = 0;
        }
        else
        {
            _showHideBarTween
                .Join(rectTransform.DOAnchorPosY(-128, .2f).SetEase(Ease.InBack))
                .Join(_cg.DOFade(0, .1f).SetDelay(.1f));
        }
    }
}
