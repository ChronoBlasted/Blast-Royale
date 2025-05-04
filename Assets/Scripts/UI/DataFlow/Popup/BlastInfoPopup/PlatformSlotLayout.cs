using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformSlotLayout : MonoBehaviour
{
    [SerializeField] Image _bg, _border;
    [SerializeField] int _index;
    [SerializeField] Sprite _circle4, _circle8, _circle16;

    Color _currentColor;

    public void Init(Type newType, int amount)
    {
        _currentColor = ResourceObjectHolder.Instance.GetTypeDataByType(newType).Color;

        _bg.color = _currentColor;

        if (amount == 1) _border.sprite = _circle4;
        else if (amount == 2) _border.sprite = _circle8;
        else if (amount == 3) _border.sprite = _circle16;
    }
}
