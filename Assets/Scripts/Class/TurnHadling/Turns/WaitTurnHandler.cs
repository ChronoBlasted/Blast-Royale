using System.Threading.Tasks;

public class WaitTurnHandler : TurnActionHandler
{
    public WaitTurnHandler(GameLogicContext context, GameView gameView, NakamaData nakamaData)
        : base(context, gameView, nakamaData) { }

    public override async Task<bool> HandleTurn()
    {
        PlayerBattleInfo currentPlayer = NakamaLogic.Instance.GetBlastOwner(context.Attacker, context.Players);

        context.Attacker.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(context.Attacker.MaxMana, context.Attacker.Mana, true);

        await gameView.BlastWait(currentPlayer.OwnerType == BlastOwner.Me, context.Attacker);

        await Task.Delay(500);

        return false;
    }
}
