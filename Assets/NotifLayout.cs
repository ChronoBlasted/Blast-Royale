using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotifLayout : MonoBehaviour
{
    [SerializeField] Image _notifImage;
    [SerializeField] TMP_Text _notifText;

    Tween _tween;

    public void Init(int amountIndex)
    {
        if (amountIndex == -1)
        {
            _notifText.text = "";
        }
        else
        {
            _notifText.text = amountIndex.ToString();
        }

        _tween.Kill();

        transform.localScale = Vector3.one;

        _tween = transform.DOScale(Vector3.one * 1.2f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void Remove()
    {
        _tween.Kill();

        gameObject.SetActive(false);
    }
}
