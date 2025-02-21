using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButtonLayout : MonoBehaviour
{
    [SerializeField] Image _switchIco;

    public void SetOn()
    {
        _switchIco.color = ColorManager.Instance.ActiveColor;
    }

    public void SetOff()
    {
        _switchIco.color = ColorManager.Instance.InactiveColor;
    }
}
