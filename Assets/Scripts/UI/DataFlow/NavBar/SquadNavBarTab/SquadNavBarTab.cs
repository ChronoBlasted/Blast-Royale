using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SquadTabType { NONE, BLAST, ITEM }

public class SquadNavBarTab : NavBarTab
{
    [SerializeField] GameObject _deckTab;
    [SerializeField] GameObject _storedTab;
    [SerializeField] SquadTabType _type;

    public override void HandleOnPress()
    {
        base.HandleOnPress();

        _deckTab.gameObject.SetActive(true);
        _storedTab.gameObject.SetActive(true);

        UIManager.Instance.MenuView.SquadPanel.UpdateMiddleTitle(_type);
    }

    public override void HandleOnReset()
    {
        base.HandleOnReset();

        _deckTab.gameObject.SetActive(false);
        _storedTab.gameObject.SetActive(false);
    }
}
