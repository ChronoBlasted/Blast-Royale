using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text sliderValue;

    Tweener _fillTween;

    public Tweener FillTween { get => _fillTween; }

    public void Init(float value, float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = value;
    }

    public void SetValue(float newValue)
    {
        slider.value = newValue;
    }

    public void SetValueSmooth(float newValue, float duration = 0.2f, float delay = 0f, Ease ease = Ease.OutSine)
    {
        if (_fillTween != null)
        {
            _fillTween.Kill(true);
            _fillTween = null;
        }

        _fillTween = slider.DOValue(newValue, duration).SetEase(ease).SetDelay(delay);
    }


    #region TextUpdateOnValueChange
    public void UpdateTextWithSlash() => sliderValue.text = Mathf.RoundToInt(slider.value) + "/" + slider.maxValue; // Pour l'inspecteur onchange du slider
    public void UpdateTextValue() => sliderValue.text = Mathf.RoundToInt(slider.value).ToString(); // Pour l'inspecteur onchange du slider
    public void UpdateTextValueWithSuffixe(string suffixe) => sliderValue.text = Mathf.RoundToInt(slider.value) + suffixe; // Pour l'inspecteur onchange du slider
    public void UpdateTextValueWithPrefix(string prefix) => sliderValue.text = prefix + Mathf.RoundToInt(slider.value); // Pour l'inspecteur onchange du slider
    public void UpdateText(string prefix = "", string suffixe = "", bool slash = false) => sliderValue.text = prefix + Mathf.RoundToInt(slider.value) + (slash ? "/" + slider.maxValue : "") + suffixe; // Cas precis
    #endregion
}