using System.Threading.Tasks;

public class StatusEffectTurnHandler : TurnActionHandler
{
    public StatusEffectTurnHandler(GameLogicContext context, GameView gameView, NakamaData nakamaData)
        : base(context, gameView, nakamaData) { }

    public override async Task<bool> HandleTurn()
    {
        PlayerBattleInfo currentPlayer = NakamaLogic.Instance.GetBlastOwner(context.Defender, context.Players);

        NakamaLogic.ApplyStatusEffectAtEndOfTurn(context.Defender, context.Attacker);

        await gameView.ApplyStatusEndTurn(currentPlayer.OwnerType == BlastOwner.Me, context.Defender, context.Attacker);

        if (await context.CheckIfAlive() == false) return true;

        return false;
    }
}
