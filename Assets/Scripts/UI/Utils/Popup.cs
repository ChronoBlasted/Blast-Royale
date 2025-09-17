using DG.Tweening;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] protected CanvasGroup _canvasGroup;
    [SerializeField] protected ChronoTweenSequence _tweenSequence;

    bool _triggerBlackShade;

    Tween _tween;

    public virtual void Init()
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0;

        gameObject.SetActive(false);
    }

    public virtual void OpenPopup(bool triggerBlackShade = true, bool openCloseButton = true, bool instant = false)
    {
        _triggerBlackShade = triggerBlackShade;

        if (_triggerBlackShade) UIManager.Instance.BlackShadeView.ShowBlackShade(ClosePopup, openCloseButton);

        _tween.Kill(true);

        gameObject.SetActive(true);

        float time = instant ? 0 : .2f;

        transform.localScale = Vector3.zero;
        transform.DOScale(1, time).SetEase(Ease.OutBack);

        _canvasGroup.blocksRaycasts = true;

        _tween = _canvasGroup.DOFade(1, time).OnComplete(() =>
        {
            _canvasGroup.interactable = true;
        }).SetUpdate(UpdateType.Normal, true);

        if (_tweenSequence != null) _tweenSequence.Init();
    }

    public virtual void OpenPopup() => OpenPopup(true);

    public virtual void ClosePopup(bool instant = false)
    {
        if (_triggerBlackShade) UIManager.Instance.BlackShadeView.HideBlackShade();

        float time = instant ? 0 : .1f;

        _tween.Kill(true);

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _tween = _canvasGroup.DOFade(0, time)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            })
            .SetUpdate(UpdateType.Normal, true);
    }

    public virtual void ClosePopup() => ClosePopup(false);
}
