using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollStepIndicator : MonoBehaviour
{
    [SerializeField] Image _bg, _activeDot;

    [SerializeField] Sprite _visualIndicatorSelectedSprite, _visualIndicatorUnselectSprite;

    public void SetActive()
    {
        _bg.sprite = _visualIndicatorSelectedSprite;
    }

    public void SetUnActive()
    {
        _bg.sprite = _visualIndicatorUnselectSprite;
    }

    public void SetSelected()
    {
        _activeDot.enabled = true;
    }

    public void SetUnSelected()
    {
        _activeDot.enabled = false;
    }
}
