using System;

public static class TurnHandlerFactory
{
    public static TurnActionHandler CreateHandler(TurnType turnType, GameLogicContext context, GameView view, NakamaData nakamaData)
    {
        return turnType switch
        {
            TurnType.Attack => new AttackTurnHandler(context, view, nakamaData),
            TurnType.Item => new ItemTurnHandler(context, view, nakamaData),
            TurnType.Swap => new SwapTurnHandler(context, view, nakamaData),
            TurnType.Wait => new WaitTurnHandler(context, view, nakamaData),
            TurnType.Status => new StatusEffectTurnHandler(context, view, nakamaData),
            TurnType.None => new NoneTurnHandler(context, view, nakamaData),
            _ => throw new NotImplementedException(),
        };
    }
}
