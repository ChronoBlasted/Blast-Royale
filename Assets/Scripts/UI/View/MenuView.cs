using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuView : View
{
    [SerializeField] NavBar navBar;

    [field: SerializeField] public ShopPanel ShopPanel { get; protected set; }
    [field: SerializeField] public SquadPanel SquadPanel { get; protected set; }
    [field: SerializeField] public FightPanel FightPanel { get; protected set; }
    [field: SerializeField] public ClanPanel ClanPanel { get; protected set; }
    [field: SerializeField] public EventPanel EventPanel { get; protected set; }


    public override void Init()
    {
        base.Init();

        ShopPanel.Init();
        SquadPanel.Init();
        FightPanel.Init();
        ClanPanel.Init();
        EventPanel.Init();
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
