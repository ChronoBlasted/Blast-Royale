using BaseTemplate.Behaviours;
using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NakamaBattle : MonoSingleton<NakamaBattle>
{
    IClient _client;
    ISession _session;
    ISocket _socket;
    IMatch _match;

    Action<IMatchState> _matchStateHandler;

    string _matchId;

    PlayerState _player1State;
    PlayerState _player2State;

    StartStateData _startStateData;

    public UnityEvent OnBattleEnd;

    public PlayerState Player1State { get => _player1State; }
    public PlayerState Player2State { get => _player2State; }


}
