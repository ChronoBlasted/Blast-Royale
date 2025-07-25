using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ErrorLayout : MonoBehaviour
{
    [SerializeField] TMP_Text errorText;
    [SerializeField] Image _glow;

    Sequence _tween;

    public void Init(string errorMsg, Transform endTransform)
    {
        RectTransform rectTransform = (RectTransform)gameObject.transform;
        rectTransform.sizeDelta = new Vector2(0, 256);

        errorText.alpha = 1;
        _glow.DOFade(.5f, 0);
        transform.localScale = Vector3.one;
        errorText.text = errorMsg;

        if (_tween.IsActive()) _tween.Kill(true);

        _tween = DOTween.Sequence()
            .Append(transform.DOPunchScale(new Vector3(.3f, .3f, .3f), .5f, 1))
            .AppendInterval(2f)
            .Append(transform.DOMoveY(endTransform.position.y, 3f).SetEase(Ease.InSine))
            .Join(errorText.DOFade(0, .5f).SetDelay(2.5f))
            .Join(_glow.DOFade(0, .5f).SetDelay(.5f))
            .OnComplete(() =>
            {
                PoolManager.Instance[ResourceType.ErrorMsg].Release(gameObject);
            });

    }
}
