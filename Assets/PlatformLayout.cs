using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformLayout : MonoBehaviour
{
    [SerializeField] Image _circle1, _circle2, _circle3, _circle4;

    List<Image> _circles;
    List<TYPE> _platformType = new List<TYPE>();

    readonly float[] _scales = { 0.33f, 0.66f, 1f, 1.33f };
    readonly float _fadeDuration = .5f;

    private void Awake()
    {
        _circles = new List<Image> { _circle1, _circle2, _circle3, _circle4 };

        foreach (var circle in _circles)
        {
            circle.transform.localScale = Vector3.zero;
            circle.color = new Color(1, 1, 1, 0);
        }
    }

    public void AddEnergy(TYPE type)
    {
        if (_platformType.Count == 4)
        {
            _platformType.RemoveAt(0);
        }

        _platformType.Add(type);
        UpdateVisuals();
    }

    public void RemoveEnergy()
    {
        if (_platformType.Count == 0) return;

        int lastIndex = _platformType.Count - 1;
        _platformType.RemoveAt(lastIndex);

        _circles[lastIndex].transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
        _circles[lastIndex].DOFade(0f, 0.2f);

        UpdateVisuals();
    }

    public void RemoveEnergyByType(TYPE type, int amount)
    {
        int removedCount = 0;

        for (int i = _platformType.Count - 1; i >= 0 && removedCount < amount; i--)
        {
            if (_platformType[i] == type)
            {
                _platformType.RemoveAt(i);

                _circles[i].transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
                _circles[i].DOFade(0f, 0.2f);

                removedCount++;
            }
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < _circles.Count; i++)
        {
            Image circle = _circles[i];

            if (i < _platformType.Count)
            {
                TYPE type = _platformType[i];

                circle.color = ResourceObjectHolder.Instance.GetTypeDataByType(type).Color;

                float targetScale = _scales[i];
                circle.transform.DOScale(Vector3.one * targetScale, 0.3f).SetEase(Ease.OutBack);

                circle.DOFade(1f, 0.2f);

                if (i == 3)
                {
                    circle.DOFade(0f, _fadeDuration).SetDelay(0.5f);
                }

                circle.transform.SetSiblingIndex(i);
            }
            else
            {
                circle.transform.DOScale(0f, 0.2f);
                circle.DOFade(0f, 0.2f);
            }
        }
    }
}
