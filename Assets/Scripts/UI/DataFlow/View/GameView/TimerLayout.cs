using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerLayout : MonoBehaviour
{
    [SerializeField] CanvasGroup _cg;
    [SerializeField] TMP_Text _timer;

    public void SetTimer(int timer)
    {
        _timer.text = timer.ToString();
    }


    public void Show()
    {
        _cg.DOFade(1f, .2f);
    }

    public void Hide(bool isInstant = false)
    {

        _cg.DOFade(0f, isInstant ? 0 : .2f);
    }
}
