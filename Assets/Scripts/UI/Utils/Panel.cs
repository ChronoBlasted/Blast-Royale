using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Panel : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;

    Tweener _fadeTweener;

    public virtual void Init()
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0;

        gameObject.SetActive(false);
    }

    public virtual void OpenPanel()
    {
        gameObject.SetActive(true);

        if (_fadeTweener.IsActive())
        {
            _fadeTweener.Kill(true);
            _fadeTweener = null;
        }

        _fadeTweener =_canvasGroup.DOFade(1, 0).OnComplete(() =>
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }).SetUpdate(UpdateType.Normal, true);
    }

    public virtual void ClosePanel()
    {
        if (_fadeTweener.IsActive())
        {
            _fadeTweener.Kill(true);
            _fadeTweener = null;
        }

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _fadeTweener =_canvasGroup.DOFade(0, 0f)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            })
            .SetUpdate(UpdateType.Normal, true);
    }

    public virtual void Disable()
    {
        if (_fadeTweener.IsActive())
        {
            _fadeTweener.Kill(true);
            _fadeTweener = null;
        }

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _fadeTweener = _canvasGroup.DOFade(0.5f, .1f);
    }

    public virtual void Enable()
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

}
