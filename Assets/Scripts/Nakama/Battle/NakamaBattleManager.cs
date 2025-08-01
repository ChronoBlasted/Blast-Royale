using Nakama;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum BattleMode { PvP, PvE }

public class NakamaBattleManager : MonoBehaviour
{
    [SerializeField] NakamaPvEBattle _pveBattle;
    [SerializeField] NakamaPvPBattle _pvpBattle;

    NakamaBattleBase _activeBattle;
    public NakamaBattleBase CurrentBattle => _activeBattle;

    public NakamaPvPBattle PvpBattle { get => _pvpBattle; }
    public NakamaPvEBattle PveBattle { get => _pveBattle; }

    public void Init(IClient client, ISession session, ISocket socket)
    {
        _pveBattle.Init(client, session, socket);
        _pvpBattle.Init(client, session, socket);
    }

    public void StartBattle(BattleMode mode)
    {
        switch (mode)
        {
            case BattleMode.PvP:
                _activeBattle = _pvpBattle;
                _pvpBattle.FindBattle();
                break;
            case BattleMode.PvE:
                _activeBattle = _pveBattle;
                _pveBattle.FindBattle();
                break;
        }
    }
}


[Serializable]
public class StartStateData
{
    public Blast[] newBlastSquad;
    public string opponentName;
    public int opponentTrophy;
    public PlayerStat opponentStats;
    public Meteo meteo;
    public int turnDelay;
}

[Serializable]
public class NewBlastData
{
    public int id;
    public int exp;
    public int iv;
    public bool boss;
    public bool shiny;
    public Status status;
    public List<int> activeMoveset;
}

[Serializable]
public class PlayerTurnData
{
    public TurnType type;
    public int index;
    public int moveDamage;
    public List<MoveEffectData> moveEffects;
    public ItemUseJSON itemUse;

}

[Serializable]
public class TurnStateData
{
    public bool p1TurnPriority;
    public PlayerTurnData p1TurnData;
    public PlayerTurnData p2TurnData;
    public bool catched;
}

[Serializable]
public class EndStateData
{
    public bool win;
    public bool opponentSurrender;
    public int trophyRewards;
}


// Send data

public class ItemUseJSON
{
    public int index_item;
    public int index_blast;
}

[Serializable]
public class OfferTurnStateData
{
    public Reward offerOne;
    public Reward offerTwo;
    public Reward offerThree;
}
