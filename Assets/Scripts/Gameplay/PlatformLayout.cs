using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlatformLayout : MonoBehaviour
{
    [SerializeField] SpriteRenderer _circle1, _circle2, _circle3, _circle4;
    [SerializeField] SpriteRenderer _outlineCircle1, _outlineCircle2, _outlineCircle3;

    List<SpriteRenderer> _circles;
    List<SpriteRenderer> _outlineCircles;

    List<Type> _platformType = new List<Type>();

    readonly float[] _scalesX = { 2f, 3f, 4, 6 };
    readonly float[] _scalesY = { 1f, 1.5f, 2f, 3f };

    readonly float[] _scalesOutlineX = { 1f, 1.5f, 2f };
    readonly float[] _scalesOutlineY = { 0.5f, .75f, 1f };
    readonly float _fadeDuration = .5f;

    public void Init()
    {
        _circles = new List<SpriteRenderer> { _circle1, _circle2, _circle3, _circle4 };
        _outlineCircles = new List<SpriteRenderer> { _outlineCircle1, _outlineCircle2, _outlineCircle3 };

        foreach (var circle in _circles)
        {
            circle.transform.localScale = Vector3.zero;
            circle.color = new Color(1, 1, 1, 0);
        }

        for (int i = 0; i < _outlineCircles.Count; i++)
        {
            _outlineCircles[i].transform.localScale = new Vector3(_scalesOutlineX[i], _scalesOutlineY[i], 0);
        }

        _platformType.Clear();
    }

    public void AddEnergy(Type type)
    {
        if (_platformType.Count == 4)
        {
            _platformType.RemoveAt(_platformType.Count - 1);
        }

        _platformType.Insert(0, type);
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

    public void RemoveEnergyByType(Type type, int amount)
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
            SpriteRenderer circle = _circles[i];

            if (i < _platformType.Count)
            {
                Type type = _platformType[i];
                Color baseColor = ResourceObjectHolder.Instance.GetTypeDataByType(type).Color;
                float tintFactor = Mathf.Clamp01(i * .2f);

                Color targetColor = Color.Lerp(baseColor, Color.black, tintFactor);
                circle.DOColor(targetColor, 0.3f);

                circle.sortingOrder = _circles.Count - i;

                if (i == 0)
                {
                    circle.transform.DOScale(new Vector3(_scalesX[i], _scalesY[i], 1), 0.3f).SetEase(Ease.OutBack);
                }
                else
                {
                    circle.transform.DOScale(new Vector3(_scalesX[i], _scalesY[i], 1), 0.3f).SetEase(Ease.Linear);
                }

                if (i == 3)
                {
                    circle.DOFade(0f, _fadeDuration).SetDelay(0.5f);

                    _circles.RemoveAt(i);
                    _circles.Insert(0, circle);

                }

                circle.transform.SetSiblingIndex(_circles.Count - 1 - i);
            }
            else
            {
                circle.transform.DOScale(0f, 0.2f);
                circle.DOFade(0f, 0.2f);
            }
        }
    }

    public int GetAmountOfType(Type type)
    {
        return _platformType.Count(item => EqualityComparer<Type>.Default.Equals(item, type));
    }

    public async Task CatchAnimation(int amount)
    {
        int count = Mathf.Min(amount, _outlineCircles.Count);

        bool shouldReset = amount < 4;

        for (int i = count - 1; i >= 0; i--)
        {
            if (i < _platformType.Count)
            {
                _circles[i].transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
                _circles[i].DOFade(0.3f, 0.2f);
            }

            _outlineCircles[i].transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);

            await Task.Delay(500);
        }

        await Task.Delay(300);

        if (shouldReset)
        {
            for (int i = 0; i < count; i++)
            {
                if (i < _platformType.Count)
                {
                    _circles[i].DOFade(1f, 0.3f);
                }
                _circles[i].transform.DOScale(new Vector3(_scalesX[i], _scalesY[i], 1), 0.3f).SetEase(Ease.OutBack);

                _outlineCircles[i].transform.DOScale(new Vector3(_scalesOutlineX[i], _scalesOutlineY[i], 1), 0.3f).SetEase(Ease.OutBack);
            }
        }
    }

}
