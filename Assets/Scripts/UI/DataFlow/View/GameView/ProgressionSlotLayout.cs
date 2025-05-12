using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionSlotLayout : MonoBehaviour
{
    [SerializeField] Image _ico, _bg, _gradient;
    [SerializeField] TMP_Text _progressionTxt;
    [SerializeField] Color _normalColor, _activeColor;
    [SerializeField] Color _normalGradient, _activeGradient;

    public void Init(int indexProgression)
    {
        _progressionTxt.text = indexProgression.ToString();
        _bg.color = _normalColor;
        _gradient.color = _normalGradient;
        transform.localScale = Vector3.one * .8f;
    }

    public void SetActive()
    {
        _bg.DOColor(_activeColor, 0.2f);
        _gradient.DOColor(_activeGradient, 0.2f);
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }
}
