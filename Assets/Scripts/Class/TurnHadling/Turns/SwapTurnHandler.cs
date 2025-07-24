using System.Threading.Tasks;

public class SwapTurnHandler : TurnActionHandler
{
    public SwapTurnHandler(GameLogicContext context, GameView gameView, NakamaData nakamaData)
        : base(context, gameView, nakamaData) { }

    public override async Task<bool> HandleTurn()
    {
        PlayerBattleInfo currentPlayer = NakamaLogic.Instance.GetBlastOwner(context.Attacker, context.Players);

        bool isPlayer = currentPlayer.OwnerType == BlastOwner.Me;
        await gameView.BlastSwap(isPlayer, context.Attacker, currentPlayer.Blasts[context.SelectedBlastIndex]);

        currentPlayer.ActiveBlast = currentPlayer.Blasts[context.SelectedBlastIndex];

        await Task.Delay(500);

        return false;
    }
}
