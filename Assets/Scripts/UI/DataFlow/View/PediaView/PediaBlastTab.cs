using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PediaBlastTab : NavBarTab
{
    [SerializeField] Image _bg, _ico;

    [SerializeField] PediaBlastLayout _pediaBlastLayout;
    [SerializeField] BlastType _blastType;

    public override void HandleOnPress()
    {
        base.HandleOnPress();

        _bg.sprite = ColorManager.Instance.ActiveSprite;

        _pediaBlastLayout.SetVersion(_blastType);
    }

    public override void HandleOnReset()
    {
        _bg.sprite = ColorManager.Instance.InactiveSprite;

        base.HandleOnReset();
    }
}
