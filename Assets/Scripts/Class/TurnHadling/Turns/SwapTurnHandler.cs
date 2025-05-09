using System.Threading.Tasks;

public class SwapTurnHandler : TurnActionHandler
{
    public SwapTurnHandler(GameLogicContext context, GameView gameView, NakamaData nakamaData)
        : base(context, gameView, nakamaData) { }

    public override async Task<bool> HandleTurn()
    {
        PlayerBattleInfo currentPlayer = NakamaLogic.Instance.GetBlastOwner(context.Attacker, context.Players);

        await gameView.BlastSwap(context.Attacker, currentPlayer.Blasts[context.SelectedBlastIndex]);

        currentPlayer.ActiveBlast = currentPlayer.Blasts[context.SelectedBlastIndex];

        await Task.Delay(500);

        return false;
    }
}
