using Nakama;
using Nakama.TinyJson;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class DamageTest
{
    UtilsTest _utilsTest;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _utilsTest = new UtilsTest();
        yield return _utilsTest.ConnectToServer();
    }

    [UnityTest]
    public IEnumerator RunDamageVariations()
    {
        var testCases = new[]
        {
            CreatePayload(1),                                  // Level 1
            CreatePayload(100),                                // Level 100
            CreatePayload(50, 100, 20),                         // Faible défense
            CreatePayload(50, 100, 200),                        // Forte défense
            CreatePayload(50, 100, 50, Type.Fire, Type.Grass),  // Très efficace
            CreatePayload(50, 100, 50, Type.Water, Type.Fire),  // Peu efficace
            CreatePayload(50, 100, 50, Type.Fire, Type.Normal, 80, Meteo.Sun), // Bonus météo
        };

        foreach (var payload in testCases)
        {
            yield return TestAttack(payload);
        }
    }

    private CalculateDamageParams CreatePayload(
        int attackerLevel = 50,
        int attackerAttack = 100,
        int defenderDefense = 50,
        Type attackType = Type.Normal,
        Type defenderType = Type.Normal,
        int movePower = 80,
        Meteo meteo = Meteo.None)
    {
        return new CalculateDamageParams
        {
            attackerLevel = attackerLevel,
            attackerAttack = attackerAttack,
            defenderDefense = defenderDefense,
            attackType = attackType,
            defenderType = defenderType,
            movePower = movePower,
            meteo = meteo
        };
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

        Debug.Log($"[Local Calculation] {localResult} for {payload.ToJson()}");

        var rpcTask = _utilsTest.Client.RpcAsync(_utilsTest.Session, "calculateAttackDamage", payload.ToJson());
        while (!rpcTask.IsCompleted) yield return null;

        var response = rpcTask.Result;
        var serverResult = int.Parse(response.Payload);

        Debug.Log($"[Server Response] {serverResult}");

        Assert.AreEqual(serverResult, localResult, "Server and local damage calculation mismatch.");
        Assert.Greater(localResult, 0, "Damage should be greater than 0");
        Assert.LessOrEqual(localResult, 999, "Damage should not exceed reasonable maximum");
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
