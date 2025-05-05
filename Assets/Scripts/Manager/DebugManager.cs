using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoSingleton<DebugManager>
{
    [Header("Attack")]
    [SerializeField] int _moveId;
    [SerializeField] int _moveDamage;
    [SerializeField] float _effectiveness;

#if UNITY_EDITOR
    [ContextMenu("Do attack debug")]
    public void TestPlayerAttack()
    {
        Move move = NakamaData.Instance.GetMoveById(_moveId);
        Blast target = null;
        HUDLayout defenderHUD = null;


        switch (move.target)
        {
            case Target.Opponent:
                target = WildBattleManager.Instance.WildBlast;
                defenderHUD = UIManager.Instance.GameView.OpponentHUD;

                break;
            case Target.Self:
                target = WildBattleManager.Instance.PlayerBlast;
                defenderHUD = UIManager.Instance.GameView.PlayerHUD;
                break;
        }


        _ = UIManager.Instance.GameView.PlayerHUD.DoAttackAnimAsync(defenderHUD, target, move, _moveDamage, 1);
    }

#endif
}
