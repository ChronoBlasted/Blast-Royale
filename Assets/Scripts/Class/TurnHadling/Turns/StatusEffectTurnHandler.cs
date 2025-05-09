using System.Threading.Tasks;

public class StatusEffectTurnHandler : TurnActionHandler
{
    public StatusEffectTurnHandler(GameLogicContext context, GameView gameView, NakamaData nakamaData)
        : base(context, gameView, nakamaData) { }

    public override async Task<bool> HandleTurn()
    {
        NakamaLogic.ApplyStatusEffectAtEndOfTurn(context.Attacker, context.Defender);

        await gameView.ApplyStatusEndTurn(true, context.Attacker, context.Defender);

        if (await context.CheckIfAlive() == false) return true;

        return false;
    }
}
