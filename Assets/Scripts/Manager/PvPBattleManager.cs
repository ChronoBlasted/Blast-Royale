using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PvPBattleManager : BattleBase
{
    public override void StartBattle(StartStateData startData)
    {
        _serverBattle = NakamaManager.Instance.NakamaBattleManager.PvpBattle;
        _gameView.UpdateGameviewState(BattleMode.PvP);

        base.StartBattle(startData);
    }

    public override void StartBattleAnim()
    {
        base.StartBattleAnim();
    }

    public override async Task PlayerLeave()
    {
        if (BonusAds)
        {
            UIManager.Instance.MenuView.FightPanel.PvPBattleBonusAds.RefreshAd();
        }

        await base.PlayerLeave();
    }

    public override Task StopTurnHandler()
    {
        return base.StopTurnHandler();
    }

    public override void PlayerAttack(int indexAttack)
    {
        base.PlayerAttack(indexAttack);

        _ = _gameView.DoShowMessage("Waiting for opponent");
    }

    public override void PlayerChangeBlast(int indexSelectedBlast)
    {
        base.PlayerChangeBlast(indexSelectedBlast);

        _ = _gameView.DoShowMessage("Waiting for opponent");
    }

    public override void PlayerWait()
    {
        base.PlayerWait();
        _ = _gameView.DoShowMessage("Waiting for opponent");

    }

    public override void PlayerUseItem(int indexItem, int indexSelectedBlast = 0)
    {
        base.PlayerUseItem(indexItem, indexSelectedBlast);
        _ = _gameView.DoShowMessage("Waiting for opponent");
    }
}
