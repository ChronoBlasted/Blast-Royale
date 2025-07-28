using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameLogicContext
{

    public Blast Attacker;
    public Blast Defender;
    public List<PlayerBattleInfo> Players;

    public int MoveIndex;
    public int MoveDamage;
    public List<MoveEffectData> MoveEffects;

    public int ItemIndex;
    public int SelectedBlastIndex;
    public bool IsCatched;

    public GameLogicContext(Blast attacker, Blast defender, List<PlayerBattleInfo> players, int moveIndex, int moveDamage, List<MoveEffectData> moveEffects, int itemIndex, int selectedBlastIndex, bool isCatched)
    {
        Attacker = attacker;
        Defender = defender;
        Players = players;
        MoveIndex = moveIndex;
        MoveDamage = moveDamage;
        MoveEffects = moveEffects;
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
            int expYield = NakamaData.Instance.GetBlastDataById(Defender.data_id).expYield;
            int amountExp = Mathf.FloorToInt(NakamaLogic.CalculateExpGain(expYield, Attacker.Level, Defender.Level));

            Attacker.exp += amountExp;

            if (CurrentPlayerDefender.OwnerType != BlastOwner.Me) NakamaManager.Instance.NakamaUserAccount.AddPlayerBlastExp(Attacker.uuid, amountExp);

            switch (CurrentPlayerDefender.OwnerType)
            {
                case BlastOwner.Me:
                    await UIManager.Instance.GameView.BlastFainted(true, Defender, amountExp);
                    break;
                case BlastOwner.Opponent:
                case BlastOwner.Wild:
                    await UIManager.Instance.GameView.BlastFainted(false, Defender, amountExp);
                    break;
            }


        }

        if (NakamaLogic.IsAllBlastFainted(CurrentPlayerDefender.Blasts))
        {
            if (NakamaLogic.IsAllBlastFainted(CurrentPlayerDefender.Blasts) && CurrentPlayerDefender.OwnerType != BlastOwner.Wild)
            {
                GameStateManager.Instance.UpdateStateToEnd();
            }
        }

        return isBlastAlive;
    }
}
