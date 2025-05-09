using System.Threading.Tasks;

public abstract class TurnActionHandler
{
    protected GameView gameView;
    protected NakamaData nakamaData;
    protected GameLogicContext context;

    public TurnActionHandler(GameLogicContext context, GameView gameView, NakamaData nakamaData)
    {
        this.context = context;
        this.gameView = gameView;
        this.nakamaData = nakamaData;
    }

    public abstract Task<bool> HandleTurn();
}
