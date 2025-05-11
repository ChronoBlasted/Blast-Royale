using DG.Tweening;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] protected CanvasGroup _canvasGroup;

    public virtual void Init()
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0;

        gameObject.SetActive(false);
    }

    public virtual void OpenPopup(bool openBlackShade = true, bool openCloseButton = true)
    {
        if (openBlackShade) UIManager.Instance.BlackShadeView.ShowBlackShade(ClosePopup, openCloseButton);

        gameObject.SetActive(true);

        transform.localScale = Vector3.zero;
        transform.DOScale(1, .2f).SetEase(Ease.OutBack);

        _canvasGroup.blocksRaycasts = true;

        _canvasGroup.DOFade(1, .2f).OnComplete(() =>
        {
            _canvasGroup.interactable = true;
        }).SetUpdate(UpdateType.Normal, true);
    }

    public virtual void OpenPopup() => OpenPopup(true);

    public virtual void ClosePopup(bool shouldCloseBlackShade = true)
    {
        if (shouldCloseBlackShade) UIManager.Instance.BlackShadeView.HideBlackShade();

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _canvasGroup.DOFade(0, 0f)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            })
            .SetUpdate(UpdateType.Normal, true);
    }

    public virtual void ClosePopup() => ClosePopup(true);

}
