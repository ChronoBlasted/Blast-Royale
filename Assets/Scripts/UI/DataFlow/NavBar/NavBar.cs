using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavBar : MonoBehaviour
{
    [SerializeField] NavBarTab _firstTab;

    NavBarTab _currentTab;

    public void Init()
    {
        if (_currentTab != null && _currentTab != _firstTab) _currentTab.HandleOnReset();

        _currentTab = null;

        ChangeTab(_firstTab);
    }

    public void ChangeTab(NavBarTab newTab)
    {
        if (_currentTab == newTab) return;
        if (_currentTab != null) _currentTab.HandleOnReset();

        _currentTab = newTab;

        _currentTab.HandleOnPress();
    }

}
