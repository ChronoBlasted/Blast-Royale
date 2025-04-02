using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButtonLayout : MonoBehaviour
{
    [SerializeField] Image _switchIco, _switchBG;

    [SerializeField] Sprite _switchBGOn, _switchBGOff;

    Tween _switchTween;
    public void SetOn()
    {
        _switchBG.sprite = _switchBGOn;

        _switchTween.Kill(true);
        _switchTween = _switchIco.rectTransform.DOLocalMoveX(32, .1f);
    }

    public void SetOff()
    {
        _switchBG.sprite = _switchBGOff;

        _switchTween.Kill(true);
        _switchTween = _switchIco.rectTransform.DOLocalMoveX(-32, .1f);
    }
}
