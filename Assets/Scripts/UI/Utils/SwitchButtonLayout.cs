using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButtonLayout : MonoBehaviour
{
    [SerializeField] Image _switchIco;
    [SerializeField] Color _onColor, _offColor;

    public void SetOn()
    {
        _switchIco.color = _onColor;
    }

    public void SetOff()
    {
        _switchIco.color = _offColor;
    }
}
