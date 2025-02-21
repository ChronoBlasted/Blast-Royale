using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Topbar : MonoBehaviour
{
    [SerializeField] CanvasGroup _cg;
    [SerializeField] TMP_Text _trophyTxt, _coinTxt, _gemTxt;

    Sequence _showHideTopbarTween;

    public void Init()
    {
    }

    public void UpdateTrophy(int amount)
    {
        _trophyTxt.text = UIManager.GetFormattedInt(amount);
    }

    public void UpdateCoin(int amount)
    {
        _coinTxt.text = UIManager.GetFormattedInt(amount);
    }

    public void UpdateGem(int amount)
    {
        _gemTxt.text = UIManager.GetFormattedInt(amount);
    }

    public void ShowTopBar()
    {
        if (_showHideTopbarTween.IsActive()) _showHideTopbarTween.Kill(true);

        _showHideTopbarTween = DOTween.Sequence();

        RectTransform rectTransform = (RectTransform)transform;

        _cg.interactable = true;
        _cg.blocksRaycasts = true;

        _showHideTopbarTween
            .Join(rectTransform.DOAnchorPosY(-64, .2f).SetEase(Ease.OutBack))
            .Join(_cg.DOFade(1, .1f));

    }

    public void HideTopBar()
    {
        if (_showHideTopbarTween.IsActive()) _showHideTopbarTween.Kill(true);

        _showHideTopbarTween = DOTween.Sequence();

        RectTransform rectTransform = (RectTransform)transform;

        _cg.interactable = false;
        _cg.blocksRaycasts = false;

        _showHideTopbarTween
            .Join(rectTransform.DOAnchorPosY(64, .2f).SetEase(Ease.OutBack))
            .Join(_cg.DOFade(0, .1f));
    }

}
