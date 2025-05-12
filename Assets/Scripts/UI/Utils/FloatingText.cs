using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TMP_Text Text;
    Sequence _tween;

    public void Init(string text, Color color, TextStyle textStyle = TextStyle.Normal, bool isCrit = false)
    {
        Text.alpha = 1;
        Text.transform.localScale = Vector3.one;
        Text.text = "<style=" + textStyle.ToString() + "> " + text + "</style>";
        Text.color = color;

        Vector3 punchScale = isCrit ? new Vector3(1.5f, 1.5f, 1.5f) : new Vector3(1.2f, 1.2f, 1.2f);

        if (_tween.IsActive()) _tween.Kill();

        _tween = DOTween.Sequence();

        _tween
            .Join(Text.transform.DOPunchScale(punchScale, .25f))
            .Join(Text.transform.DOMoveY(transform.position.y + 1f, 1f)).SetEase(Ease.InOutSine)
            .Append(Text.DOFade(0, .2f))
            .OnComplete(() =>
            {
                PoolManager.Instance[ResourceType.FloatingText].Release(gameObject);
            });
    }
}
