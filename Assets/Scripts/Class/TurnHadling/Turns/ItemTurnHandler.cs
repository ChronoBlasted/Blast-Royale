using System.Threading.Tasks;

public class ItemTurnHandler : TurnActionHandler
{
    public ItemTurnHandler(GameLogicContext context, GameView gameView, NakamaData nakamaData)
        : base(context, gameView, nakamaData) { }

    public override async Task<bool> HandleTurn()
    {
        PlayerBattleInfo currentPlayer = NakamaLogic.Instance.GetBlastOwner(context.Attacker, context.Players);

        var itemData = nakamaData.GetItemDataById(currentPlayer.Items[context.ItemIndex].data_id);

        var targetBlast = currentPlayer.Blasts[context.SelectedBlastIndex];

        switch (itemData.behaviour)
        {
            case ItemBehaviour.Heal:
                targetBlast.Hp += itemData.gain_amount;
                break;
            case ItemBehaviour.Mana:
                targetBlast.Mana += itemData.gain_amount;
                break;
            case ItemBehaviour.Status:
                targetBlast.status = itemData.status;
                break;
            case ItemBehaviour.Catch:
                break;
        }

        currentPlayer.Items[context.ItemIndex].amount--;

        await UIManager.Instance.GameView.BlastUseItem(currentPlayer.Items[context.ItemIndex], targetBlast, context.Defender, context.IsCatched);

        UIManager.Instance.GameView.BagPanel.UpdateUI(currentPlayer.Items);
        UIManager.Instance.ChangeBlastPopup.UpdateData(currentPlayer.Blasts);

        await Task.Delay(500);

        return context.IsCatched;
    }
}
