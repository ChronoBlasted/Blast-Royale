using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformSlotLayout : MonoBehaviour
{
    [SerializeField] Image _bg;
    [SerializeField] Image _inner;

    Color _currentColor;
    Color _fadeColor;
    Color _activeColor;

    public void Init(Type newType)
    {
        _currentColor = ResourceObjectHolder.Instance.GetTypeDataByType(newType).Color;
        _fadeColor = Color.Lerp(_currentColor, Color.black, .5f);
        _activeColor = Color.Lerp(_currentColor, Color.white, .5f);

        _bg.color = _currentColor;
        
        SetOff();
    }

    public void SetOn()
    {
        _inner.color = _activeColor;
    }

    public void SetOff()
    {
        _inner.color = _fadeColor;
    }
}
