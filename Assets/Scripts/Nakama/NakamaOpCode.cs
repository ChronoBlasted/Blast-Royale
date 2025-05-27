using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NakamaOpCode
{
    public const long PING = 1;
    public const long PONG = 2;

    public const long MATCH_START = 10;

    public const long PLAYER_WAIT = 15;
    public const long PLAYER_ATTACK = 20;
    public const long PLAYER_USE_ITEM = 30;
    public const long PLAYER_CHANGE_BLAST = 40;


    public const long PLAYER_READY = 50;
    public const long PLAYER_LEAVE = 51;

    public const long ENEMY_READY = 55;

    public const long MATCH_ROUND = 60;
    public const long PLAYER_MUST_CHANGE_BLAST = 61;

    public const long NEW_OFFER_TURN = 80;
    public const long PLAYER_CHOOSE_OFFER = 81;

    public const long NEW_BLAST = 90;

    public const long MATCH_END = 100;

    public const long ERROR_SERV = 404;

    public const long DEBUG = 500;
}

public static class NotificationOpCodes
{
    public const int CURENCY = 1000;
    public const int BLAST = 1010;
    public const int ITEM = 1020;
}