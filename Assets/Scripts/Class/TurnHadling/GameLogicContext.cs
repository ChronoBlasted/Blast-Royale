using System.Collections.Generic;
using System.Threading.Tasks;

public class GameLogicContext
{

    public Blast Attacker;
    public Blast Defender;
    public List<PlayerBattleInfo> Players;

    public int MoveIndex;
    public int MoveDamage;
    public MoveEffect MoveEffect;

    public int ItemIndex;
    public int SelectedBlastIndex;
    public bool IsCatched;

    public GameLogicContext(Blast attacker, Blast defender, List<PlayerBattleInfo> players, int moveIndex, int moveDamage, MoveEffect moveEffect, int itemIndex, int selectedBlastIndex, bool isCatched)
    {
        Attacker = attacker;
        Defender = defender;
        Players = players;
        MoveIndex = moveIndex;
        MoveDamage = moveDamage;
        MoveEffect = moveEffect;
        ItemIndex = itemIndex;
        SelectedBlastIndex = selectedBlastIndex;
        IsCatched = isCatched;
    }

    public async Task<bool> CheckIfAlive()
    {
        PlayerBattleInfo CurrentPlayerDefender = NakamaLogic.Instance.GetBlastOwner(Defender, Players);

        bool isBlastAlive = NakamaLogic.IsBlastAlive(Defender);

        if (!isBlastAlive)
        {
            switch (CurrentPlayerDefender.OwnerType)
            {
                case BlastOwner.Opponent:
                    await UIManager.Instance.GameView.BlastFainted(false, Defender);

                    break;
                case BlastOwner.Me:
                    await UIManager.Instance.GameView.BlastFainted(true, Defender);

                    UIManager.Instance.LevelExpPopup.UpdateData(Attacker, Defender);
                    UIManager.Instance.LevelExpPopup.OpenPopup();

                    _ = NakamaManager.Instance.NakamaUserAccount.GetPlayerBlast();
                    _ = NakamaManager.Instance.NakamaUserAccount.GetPlayerBag();

                    UIManager.Instance.LevelExpPopup.UpdateClose(GameStateManager.Instance.UpdateStateToEnd);
                    break;
            }
        }

        if (NakamaLogic.IsAllBlastDead(CurrentPlayerDefender.Blasts))
        {
            await UIManager.Instance.GameView.AllPlayerBlastFainted(CurrentPlayerDefender);
            GameStateManager.Instance.UpdateStateToEnd();
        }
        else
        {
            if (CurrentPlayerDefender.OwnerType == BlastOwner.Me && !isBlastAlive) WildBattleManager.Instance.PlayerMustChangeBlast();
        }

        return isBlastAlive;
    }
}
