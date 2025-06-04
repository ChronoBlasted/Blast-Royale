using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AttackTurnHandler : TurnActionHandler
{
    public AttackTurnHandler(GameLogicContext context, GameView gameView, NakamaData nakamaData) : base(context, gameView, nakamaData) { }

    public override async Task<bool> HandleTurn()
    {
        await Attack();

        if (await context.CheckIfAlive() == false) return true;

        return false;
    }

    public async Task Attack()
    {
        var move = nakamaData.GetMoveById(context.Attacker.activeMoveset[context.MoveIndex]);
        PlayerBattleInfo CurrentPlayerAttacker = NakamaLogic.Instance.GetBlastOwner(context.Attacker, context.Players);
        Blast target = move.target == Target.Opponent ? context.Defender : context.Attacker;

        target.Hp -= context.MoveDamage;

        if (context.MoveEffects != null && context.MoveEffects.Count > 0)
        {
            foreach (var effect in context.MoveEffects)
            {
                target = NakamaLogic.ApplyEffectToBlast(target, move, effect);
            }
        }

        if (move.attackType != AttackType.None)
        {
            if (move.attackType == AttackType.Normal || move.attackType == AttackType.Status)
                context.Attacker.Mana -= move.cost;
        }

        await gameView.BlastAttack(CurrentPlayerAttacker.OwnerType == BlastOwner.Me, context.Attacker, target, move, context.MoveDamage, context.MoveEffects);
    }
}
