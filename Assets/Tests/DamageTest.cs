using Nakama;
using Nakama.TinyJson;
using NUnit.Framework;
using System.Collections;
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
    public IEnumerator CalculateBasicAttackLevelOneDamage()
    {
        CalculateDamageParams payload = new CalculateDamageParams
        {
            attackerLevel = 1,
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
    public IEnumerator CalculateBasicAttackLevelHundredDamage()
    {
        CalculateDamageParams payload = new CalculateDamageParams
        {
            attackerLevel = 100,
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
    public IEnumerator CalculateBasicDefenseLowAttackDamage()
    {
        CalculateDamageParams payload = new CalculateDamageParams
        {
            attackerLevel = 50,
            attackerAttack = 100,
            defenderDefense = 20,
            attackType = Type.Normal,
            defenderType = Type.Normal,
            movePower = 80,
            meteo = Meteo.None
        };

        yield return TestAttack(payload);
    }
    [UnityTest]
    public IEnumerator CalculateBasicDefenseHighAttackDamage()
    {
        CalculateDamageParams payload = new CalculateDamageParams
        {
            attackerLevel = 50,
            attackerAttack = 100,
            defenderDefense = 200,
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

        var rpcTask = _utilsTest.Client.RpcAsync(_utilsTest.Session, "calculateAttackDamage", payload.ToJson());
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
