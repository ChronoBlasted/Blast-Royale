using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlatformLayout : MonoBehaviour
{
    [SerializeField] SpriteRenderer _circle1, _circle2, _circle3, _circle4;
    [SerializeField] SpriteRenderer _outlineCircle1, _outlineCircle2, _outlineCircle3;

    List<SpriteRenderer> _activeCircles = new();
    List<SpriteRenderer> _availableCircles = new();
    List<Type> _platformType = new();

    readonly float[] _scalesX = { 2f, 3f, 4f };
    readonly float[] _scalesY = { 1f, 1.5f, 2f };
    readonly float _fadeDuration = 0.5f;

    readonly float[] _scalesOutlineX = { 1f, 1.5f, 2f };
    readonly float[] _scalesOutlineY = { 0.5f, 0.75f, 1f };

    List<SpriteRenderer> _outlineCircles;

    public void Init()
    {
        _activeCircles.Clear();
        _availableCircles = new List<SpriteRenderer> { _circle1, _circle2, _circle3 };
        _outlineCircles = new List<SpriteRenderer> { _outlineCircle1, _outlineCircle2, _outlineCircle3 };

        _platformType.Clear();

        foreach (var circle in new[] { _circle1, _circle2, _circle3, _circle4 })
        {
            circle.color = new Color(1, 1, 1, 0);
            circle.transform.localScale = Vector3.zero;
        }

        for (int i = 0; i < _outlineCircles.Count; i++)
        {
            _outlineCircles[i].transform.localScale = new Vector3(_scalesOutlineX[i], _scalesOutlineY[i], 1);
        }
    }

    public void AddEnergy(Type type)
    {
        if (_platformType.Count == 3)
        {
            Type removedType = _platformType[2];
            Color baseColor = ResourceObjectHolder.Instance.GetTypeDataByType(removedType).Color;
            float tintFactor = Mathf.Clamp01(3 * 0.2f);
            Color fadedColor = Color.Lerp(baseColor, Color.black, tintFactor);

            _circle4.color = fadedColor;
            _circle4.transform.localScale = new Vector3(6f, 3f, 1f);
            _circle4.sortingOrder = 4;
            _circle4.DOFade(0f, _fadeDuration).SetDelay(0.2f);
            _circle4.transform.DOScale(Vector3.zero, 0.2f).SetDelay(0.2f);

            // Remove from active
            _platformType.RemoveAt(2);
            _availableCircles.Add(_activeCircles[2]);
            _activeCircles.RemoveAt(2);
        }

        // Add new type
        var newCircle = _availableCircles[0];
        _availableCircles.RemoveAt(0);
        _activeCircles.Insert(0, newCircle);
        _platformType.Insert(0, type);

        UpdateVisuals();
    }

    public void RemoveEnergy()
    {
        if (_platformType.Count == 0) return;

        int lastIndex = _platformType.Count - 1;

        var circle = _activeCircles[lastIndex];
        circle.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
        circle.DOFade(0f, 0.2f);

        _platformType.RemoveAt(lastIndex);
        _availableCircles.Add(circle);
        _activeCircles.RemoveAt(lastIndex);

        UpdateVisuals();
    }

    public void RemoveEnergyByType(Type type, int amount)
    {
        int removedCount = 0;

        for (int i = _platformType.Count - 1; i >= 0 && removedCount < amount; i--)
        {
            if (_platformType[i] == type)
            {
                var circle = _activeCircles[i];
                circle.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
                circle.DOFade(0f, 0.2f);

                _platformType.RemoveAt(i);
                _availableCircles.Add(circle);
                _activeCircles.RemoveAt(i);

                removedCount++;
            }
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < _activeCircles.Count; i++)
        {
            var circle = _activeCircles[i];
            var type = _platformType[i];

            Color baseColor = ResourceObjectHolder.Instance.GetTypeDataByType(type).Color;
            float tintFactor = Mathf.Clamp01(i * 0.2f);
            Color color = Color.Lerp(baseColor, Color.black, tintFactor);

            circle.DOColor(color, 0.2f);
            circle.sortingOrder = 4 - i;

            var scale = new Vector3(_scalesX[i], _scalesY[i], 1f);
            circle.transform.DOScale(scale, 0.2f).SetEase(i == 0 ? Ease.OutBack : Ease.Linear);
            circle.DOFade(1f, 0.2f);
        }
    }

    public int GetAmountOfType(Type type)
    {
        return _platformType.Count(t => EqualityComparer<Type>.Default.Equals(t, type));
    }

    public async Task CatchAnimation(int amount, float delay)
    {
        int count = Mathf.Min(amount, _outlineCircles.Count);
        bool shouldReset = amount < 4;

        for (int i = count - 1; i >= 0; i--)
        {
            if (i < _platformType.Count)
            {
                _activeCircles[i].transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
                _activeCircles[i].DOFade(0.3f, 0.2f);
            }

            await _outlineCircles[i].transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).AsyncWaitForCompletion();
            await Task.Delay((int)(delay * 1000));
        }

        if (shouldReset)
        {
            ResetPlatform();
        }
    }

    public void ResetPlatform()
    {
        for (int i = 0; i < _outlineCircles.Count; i++)
        {
            if (i < _platformType.Count)
            {
                _activeCircles[i].DOFade(1f, 0.3f);
                _activeCircles[i].transform.DOScale(new Vector3(_scalesX[i], _scalesY[i], 1f), 0.3f).SetEase(Ease.OutBack);
            }
            else
            {
                if (i < _activeCircles.Count)
                {
                    _activeCircles[i].DOFade(0f, 0.2f);
                    _activeCircles[i].transform.DOScale(Vector3.zero, 0.2f);
                }
            }

            _outlineCircles[i].transform.DOScale(new Vector3(_scalesOutlineX[i], _scalesOutlineY[i], 1f), 0.3f).SetEase(Ease.OutBack);
        }
    }
}
