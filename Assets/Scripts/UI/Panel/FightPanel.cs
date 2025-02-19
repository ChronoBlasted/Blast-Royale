using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightPanel : Panel
{
    [SerializeField] ProfileLayout _profileLayout;

    [SerializeField] SquadLayout _squadLayout;

    public ProfileLayout ProfileLayout { get => _profileLayout; }
    public override void Init()
    {
        base.Init();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        UIManager.Instance.MenuView.TopBar.ShowTopBar();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
    }

    public void UpdateDeckBlast(List<Blast> decks)
    {
        _squadLayout.UpdateDeckBlast(decks);
    }

    public void HandleOnWildBattle()
    {
        NakamaManager.Instance.NakamaWildBattle.FindWildBattle();
    }

    public void HandleOnPlayerBattle()
    {
        Debug.Log("Coming soon");
    }
}
