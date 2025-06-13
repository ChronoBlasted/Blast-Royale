using DG.Tweening;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] protected CanvasGroup _canvasGroup;
    [SerializeField] protected ChronoTweenSequence _tweenSequence;

    bool _triggerBlackShade;

    public virtual void Init()
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0;

        gameObject.SetActive(false);
    }

    public virtual void OpenPopup(bool triggerBlackShade = true, bool openCloseButton = true)
    {
        _triggerBlackShade = triggerBlackShade;

        if (_triggerBlackShade) UIManager.Instance.BlackShadeView.ShowBlackShade(ClosePopup, openCloseButton);

        gameObject.SetActive(true);

        transform.localScale = Vector3.zero;
        transform.DOScale(1, .2f).SetEase(Ease.OutBack);

        _canvasGroup.blocksRaycasts = true;

        _canvasGroup.DOFade(1, .2f).OnComplete(() =>
        {
            _canvasGroup.interactable = true;
        }).SetUpdate(UpdateType.Normal, true);

        if (_tweenSequence != null) _tweenSequence.Init();
    }

    public virtual void OpenPopup() => OpenPopup(true);

    public virtual void ClosePopup()
    {
        if (_triggerBlackShade) UIManager.Instance.BlackShadeView.HideBlackShade();

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _canvasGroup.DOFade(0, .1f)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            })
            .SetUpdate(UpdateType.Normal, true);
    }
}
