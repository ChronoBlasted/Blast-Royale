using Nakama;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum BattleMode { PvP, PvE }

public class NakamaBattleManager : MonoBehaviour
{
    [SerializeField] private NakamaPvEBattle _pveBattle;
    [SerializeField] private NakamaPvPBattle _pvpBattle;

    private NakamaBattleBase _activeBattle;

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

    public NakamaBattleBase CurrentBattle => _activeBattle;
}


[Serializable]
public class StartStateData
{
    public NewBlastData newBlastData;
    public Meteo meteo;
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
public class TurnStateData
{
    public TurnType p1TurnType;
    public int p1MoveIndex;
    public int p1MoveDamage;
    public List<MoveEffectData> p1MoveEffects;

    public TurnType p2TurnType;
    public int p2MoveIndex;
    public int p2MoveDamage;
    public List<MoveEffectData> p2MoveEffects;

    public bool catched;
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
    public Offer offerOne;
    public Offer offerTwo;
    public Offer offerThree;
}
