using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SquadTabType { NONE, BLAST, ITEM }

public class SquadNavBarTab : NavBarTab
{
    [SerializeField] GameObject _deckTab;
    [SerializeField] GameObject _storedTab;
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] Image _tabIco;
    [SerializeField] TMP_Text _titleTxt;
    [SerializeField] Color _offColor;
    [SerializeField] SquadTabType _type;

    public override void HandleOnPress()
    {
        base.HandleOnPress();

        _deckTab.gameObject.SetActive(true);
        _storedTab.gameObject.SetActive(true);

        _tabIco.enabled = true;

        _titleTxt.color = Color.white;

        UIManager.Instance.MenuView.SquadPanel.QuitSoloBlast();
        UIManager.Instance.MenuView.SquadPanel.QuitSoloItem();

        UIManager.ResetScroll(_scrollRect);

        UIManager.Instance.MenuView.SquadPanel.UpdateMiddleTitle(_type);
    }

    public override void HandleOnReset()
    {
        base.HandleOnReset();

        _deckTab.gameObject.SetActive(false);
        _storedTab.gameObject.SetActive(false);

        _titleTxt.color = _offColor;

        _tabIco.enabled = false;
    }
}
