using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using static DamageTest;

public class ItemTest
{
    UtilsTest _utilsTest;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _utilsTest = new UtilsTest();

        yield return _utilsTest.ConnectToServer();
    }

    [UnityTest]
    public IEnumerator CalculateCatchRate()
    {
        yield return TestCatch();
    }

    private IEnumerator TestCatch()
    {
        //int localResult = NakamaLogic.CalculateDamage(
        //    payload.attackerLevel,
        //    payload.attackerAttack,
        //    payload.defenderDefense,
        //    payload.attackType,
        //    payload.defenderType,
        //    payload.movePower,
        //    payload.meteo
        //);

        //var rpcTask = _utilsTest.Client.RpcAsync(_utilsTest.Session, "calculateAttackDamage", payload.ToJson());
        //while (!rpcTask.IsCompleted) yield return null;

        //var response = rpcTask.Result;
        //var serverResult = int.Parse(response.Payload);

        //Assert.AreEqual(serverResult, localResult);
        yield return null;
    }
}
