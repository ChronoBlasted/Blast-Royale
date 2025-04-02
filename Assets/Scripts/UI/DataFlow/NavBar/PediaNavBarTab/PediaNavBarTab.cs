using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PediaNavBarTab : NavBarTab
{
    [SerializeField] Image _bg;
    [SerializeField] GameObject _tab;
    public override void HandleOnPress()
    {
        base.HandleOnPress();

        _tab.SetActive(true);

        _bg.sprite = ColorManager.Instance.ActiveSprite;
    }

    public override void HandleOnReset()
    {
        base.HandleOnReset();

        _tab.SetActive(false);

        _bg.sprite = ColorManager.Instance.InactiveSprite;
    }
}
