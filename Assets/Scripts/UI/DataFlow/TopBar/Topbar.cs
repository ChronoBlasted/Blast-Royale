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

    int _lastTrophy;
    int _lastCoin;
    int _lastGem;

    public void Init()
    {
        _trophyTxt.text = _lastTrophy.ToString();
        _coinTxt.text = _lastCoin.ToString();
        _gemTxt.text = _lastGem.ToString();
    }

    public void UpdateTrophy(int amount)
    {
        UIManager.Instance.DoSmoothTextInt(_trophyTxt, _lastTrophy, amount);
        _lastTrophy = amount;
    }

    public void UpdateCoin(int amount)
    {
        UIManager.Instance.DoSmoothTextInt(_coinTxt, _lastCoin, amount);
        _lastCoin = amount;
    }

    public void UpdateGem(int amount)
    {
        UIManager.Instance.DoSmoothTextInt(_gemTxt, _lastGem, amount);
        _lastGem = amount;
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
