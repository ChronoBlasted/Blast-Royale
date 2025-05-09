using System;

public static class TurnHandlerFactory
{
    public static TurnActionHandler CreateHandler(TurnType turnType, GameLogicContext context, GameView view, NakamaData nakamaData)
    {
        return turnType switch
        {
            TurnType.ATTACK => new AttackTurnHandler(context, view, nakamaData),
            TurnType.ITEM => new ItemTurnHandler(context, view, nakamaData),
            TurnType.SWAP => new SwapTurnHandler(context, view, nakamaData),
            TurnType.WAIT => new WaitTurnHandler(context, view, nakamaData),
            TurnType.STATUS => new StatusEffectTurnHandler(context, view, nakamaData),
            TurnType.NONE => new WaitTurnHandler(context, view, nakamaData),
            _ => throw new NotImplementedException(),
        };
    }
}
