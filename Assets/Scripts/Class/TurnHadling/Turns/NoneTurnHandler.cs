
using System.Threading.Tasks;

public class NoneTurnHandler : TurnActionHandler
{
    public NoneTurnHandler(GameLogicContext context, GameView gameView, NakamaData nakamaData)
        : base(context, gameView, nakamaData) { }

    public override async Task<bool> HandleTurn()
    {
        await Task.Delay(0);

        return false;
    }
}
