using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuView : View
{
    [SerializeField] NavBar navBar;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);

        navBar.Init();
    }

    public override void CloseView()
    {
        base.CloseView();
    }
}
