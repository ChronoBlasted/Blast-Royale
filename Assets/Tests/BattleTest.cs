using Nakama;
using Nakama.TinyJson;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class BattleTest
{
    private IClient _client;
    private ISession _session;
    private ISocket _socket;

    private const string ServerKey = "defaultkey";
    private const string Host = "127.0.0.1";
    //private const string Host = "209.38.212.129";
    private const int Port = 7350;


    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _client = new Client("http", Host, Port, ServerKey);
        var task = _client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
        while (!task.IsCompleted) yield return null;

        _session = task.Result;

        _socket = _client.NewSocket();
        var connectTask = _socket.ConnectAsync(_session);
        while (!connectTask.IsCompleted) yield return null;

        Debug.Log("Connected to Nakama");
    }

    [UnityTest]
    public IEnumerator CalculateBasicAttackDamage()
    {
        CalculateDamageParams payload = new CalculateDamageParams
        {
            attackerLevel = 50,
            attackerAttack = 100,
            defenderDefense = 50,
            attackType = Type.Normal,
            defenderType = Type.Normal,
            movePower = 80,
            meteo = Meteo.None
        };

        yield return TestAttack(payload);
    }

    [UnityTest]
    public IEnumerator CalculateEffectiveAttackDamage()
    {
        CalculateDamageParams payload = new CalculateDamageParams
        {
            attackerLevel = 50,
            attackerAttack = 100,
            defenderDefense = 50,
            attackType = Type.Fire,
            defenderType = Type.Water,
            movePower = 80,
            meteo = Meteo.None
        };

        yield return TestAttack(payload);
    }


    [UnityTest]
    public IEnumerator CalculateNotEffectiveAttackDamage()
    {
        CalculateDamageParams payload = new CalculateDamageParams
        {
            attackerLevel = 50,
            attackerAttack = 100,
            defenderDefense = 50,
            attackType = Type.Water,
            defenderType = Type.Fire,
            movePower = 80,
            meteo = Meteo.None
        };

        yield return TestAttack(payload);
    }

    [UnityTest]
    public IEnumerator CalculateWeatherBoostAttackDamage()
    {
        CalculateDamageParams payload = new CalculateDamageParams
        {
            attackerLevel = 50,
            attackerAttack = 100,
            defenderDefense = 50,
            attackType = Type.Fire,
            defenderType = Type.Normal,
            movePower = 80,
            meteo = Meteo.Sun
        };

        yield return TestAttack(payload);
    }


    private IEnumerator TestAttack(CalculateDamageParams payload)
    {
        int localResult = NakamaLogic.CalculateDamage(
            payload.attackerLevel,
            payload.attackerAttack,
            payload.defenderDefense,
            payload.attackType,
            payload.defenderType,
            payload.movePower,
            payload.meteo
        );

        Debug.Log(payload.ToJson());

        var rpcTask = _client.RpcAsync(_session, "calculateAttackDamage", payload.ToJson());
        while (!rpcTask.IsCompleted) yield return null;

        var response = rpcTask.Result;
        var serverResult = int.Parse(response.Payload);

        Assert.AreEqual(serverResult, localResult);
    }

    public struct CalculateDamageParams
    {
        public int attackerLevel;
        public int attackerAttack;
        public int defenderDefense;
        public Type attackType;
        public Type defenderType;
        public int movePower;
        public Meteo meteo;
    }

}
