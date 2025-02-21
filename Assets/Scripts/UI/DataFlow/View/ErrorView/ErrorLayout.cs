using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;


public class ErrorLayout : MonoBehaviour
{
    [SerializeField] TMP_Text errorText;

    Sequence _tween;

    public void Init(string errorMsg, Transform endTransform)
    {
        RectTransform rectTransform = (RectTransform)gameObject.transform;
        rectTransform.sizeDelta = new Vector2(0, 256);

        errorText.alpha = 1;
        transform.localScale = Vector3.one;
        errorText.text = errorMsg;

        if (_tween.IsActive()) _tween.Kill(true);

        _tween = DOTween.Sequence()
            .Join(transform.DOPunchScale(new Vector3(.3f, .3f, .3f), .5f, 1))
            .Join(transform.DOMoveY(endTransform.position.y, 3f).SetEase(Ease.InBack).SetDelay(.5f))
            .Join(errorText.DOFade(0, .5f).SetDelay(2.5f).OnComplete(() =>
            {
                PoolManager.Instance[ResourceType.ErrorMsg].Release(gameObject);
            }));
    }
}
