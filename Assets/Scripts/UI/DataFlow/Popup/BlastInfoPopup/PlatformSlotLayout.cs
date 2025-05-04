using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformSlotLayout : MonoBehaviour
{
    [SerializeField] Image _bg;
    [SerializeField] int _index;

    Color _currentColor;
    Color _activeColor;

    public void Init(Type newType)
    {
        _currentColor = ResourceObjectHolder.Instance.GetTypeDataByType(newType).Color;
        _activeColor = Color.Lerp(_currentColor, Color.black, _index * .2f);

        _bg.color = _activeColor;

        SetOff();
    }

    public void SetOn()
    {
        _bg.DOFade(1, .2f);
    }

    public void SetOff()
    {
        _bg.DOFade(_index * .2f, .2f);
    }
}
