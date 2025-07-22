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

        base.StartBattle(startData);

        _gameView.BagPanel.HandleOnPvPBattle(true);
    }

    public override async Task PlayerLeave(bool leaveMatch)
    {
        await base.PlayerLeave(leaveMatch);

        if (BonusAds)
        {
            UIManager.Instance.MenuView.FightPanel.PvPBattleBonusAds.RefreshAd();
        }
    }
}
