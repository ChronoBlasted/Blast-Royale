using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlackShadeView : View
{
    [SerializeField] Button _shadeButton, _closeButton;
    [SerializeField] Image _shadeImage, _bgCloseButton;
    [SerializeField] TMP_Text _closeButtonTxt;

    Tweener _blackShadeTweener;
    bool _isCloseButtonOpen = false;

    public Button ShadeButton { get => _shadeButton; }
    public Button CloseButton { get => _closeButton; }

    public void ShowBlackShade(UnityAction _onClickAction, bool showCloseButton = true)
    {
        if (_blackShadeTweener.IsActive()) _blackShadeTweener.Kill();

        _blackShadeTweener = _canvasGroup.DOFade(1f, .1f).OnComplete(() => _isCloseButtonOpen = true);

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        _shadeButton.onClick.AddListener(_onClickAction);

        if (showCloseButton)
        {
            _closeButton.onClick.AddListener(_onClickAction);

            ShowCloseButton();
        }
        else
        {
            _closeButton.onClick.RemoveAllListeners();

            HideCloseButton();
        }
    }

    public void HideBlackShade(bool _instant = true)
    {
        if (_blackShadeTweener.IsActive()) _blackShadeTweener.Kill();

        if (_instant) _blackShadeTweener = _canvasGroup.DOFade(0f, 0).OnComplete(() => _isCloseButtonOpen = false);
        else _blackShadeTweener = _canvasGroup.DOFade(0f, .1f).OnComplete(() => _isCloseButtonOpen = false);

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _shadeButton.onClick.RemoveAllListeners();
    }

    public void ShowCloseButton()
    {
        if (_isCloseButtonOpen == true) return;

        _closeButton.transform.localPosition = new Vector3(0, -256, 0);

        _closeButton.transform.DOLocalMoveY(96, .2f).SetEase(Ease.OutBack);
    }

    public void HideCloseButton()
    {
        _closeButton.transform.localPosition = new Vector3(0, -256, 0);
        _isCloseButtonOpen = false;
    }
}
