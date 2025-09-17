using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingPopup : Popup
{
    [SerializeField] ChronoTweenHelper tweenHelper;

    public override void OpenPopup(bool triggerBlackShade = true, bool openCloseButton = true, bool instant = false)
    {
        base.OpenPopup(triggerBlackShade, openCloseButton, instant);

        tweenHelper.TweenAction?.Invoke();
    }
}
