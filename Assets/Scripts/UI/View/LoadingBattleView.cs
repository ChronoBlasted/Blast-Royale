using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBattleView : View
{
    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(true);
    }

    public override void CloseView()
    {
        base.CloseView();
    }

    public void UpdateData()
    {

    }

    public async void HandleCancelMatchMaking()
    {
        await NakamaManager.Instance.NakamaBattleManager.CurrentBattle.CancelMatchmaking();

        UIManager.Instance.ChangeView(UIManager.Instance.MenuView);
    }
}
