using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Slider delayedSlider;
    [SerializeField] Slider recoverSlider;

    [SerializeField] TMP_Text sliderValue;

    Tweener _fillTween;
    Tweener _fillDelayedTween;
    Tweener _fillRecoverTween;


    public void Init(float value, float maxValue)
    {
        KillTweens();

        slider.maxValue = maxValue;
        slider.value = value;

        delayedSlider.maxValue = maxValue;
        delayedSlider.value = value;

        recoverSlider.maxValue = maxValue;
        recoverSlider.value = value;
    }

    public void Init(float value)
    {
        Init(value, value);
    }

    public void SetValue(float newValue)
    {
        KillTweens();

        slider.value = newValue;
        delayedSlider.value = newValue;
        recoverSlider.value = newValue;
    }

    public void SetMaxValue(float newValue)
    {
        KillTweens();

        slider.maxValue = newValue;
        delayedSlider.maxValue = newValue;
        recoverSlider.maxValue = newValue;
    }

    public void SetValueSmooth(float newValue, float duration = 0.2f, float delay = 0f, Ease ease = Ease.OutCirc)
    {
        KillTweens();

        if (newValue > slider.value)
        {
            _fillRecoverTween = recoverSlider.DOValue(newValue, duration).SetDelay(delay).SetEase(ease);

            _fillTween = slider.DOValue(newValue, duration * 2f).SetDelay(delay).SetEase(Ease.Linear);
            _fillDelayedTween = delayedSlider.DOValue(newValue, duration * 2f).SetDelay(delay).SetEase(Ease.Linear);
        }
        else
        {
            _fillTween = slider.DOValue(newValue, duration).SetDelay(delay).SetEase(ease);
            _fillRecoverTween = recoverSlider.DOValue(newValue, duration).SetDelay(delay).SetEase(ease);

            _fillDelayedTween = delayedSlider.DOValue(newValue, duration * 2f).SetDelay(delay).SetEase(Ease.Linear);
        }
    }

    private void KillTweens()
    {
        if (_fillTween != null)
        {
            _fillTween.Kill(true);
            _fillTween = null;
        }

        if (_fillDelayedTween != null)
        {
            _fillDelayedTween.Kill();
            _fillDelayedTween = null;
        }

        if (_fillRecoverTween != null)
        {
            _fillRecoverTween.Kill();
            _fillRecoverTween = null;
        }
    }


    #region TextUpdateOnValueChange
    public void UpdateTextWithSlash() => sliderValue.text = Mathf.RoundToInt(slider.value) + "/" + slider.maxValue; // Pour l'inspecteur onchange du slider
    public void UpdateTextValue() => sliderValue.text = Mathf.RoundToInt(slider.value).ToString(); // Pour l'inspecteur onchange du slider
    public void UpdateTextValueWithSuffixe(string suffixe) => sliderValue.text = Mathf.RoundToInt(slider.value) + suffixe; // Pour l'inspecteur onchange du slider
    public void UpdateTextValueWithPrefix(string prefix) => sliderValue.text = prefix + Mathf.RoundToInt(slider.value); // Pour l'inspecteur onchange du slider
    public void UpdateText(string prefix = "", string suffixe = "", bool slash = false) => sliderValue.text = prefix + Mathf.RoundToInt(slider.value) + (slash ? "/" + slider.maxValue : "") + suffixe; // Cas precis
    #endregion
}