using System.Collections;
using Nakama.TinyJson;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StatTest
{
    UtilsTest _utilsTest;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _utilsTest = new UtilsTest();
        yield return _utilsTest.ConnectToServer();
    }

    [UnityTest]
    public IEnumerator RunStatTests()
    {
        var testCases = new[]
        {
            CreateBaseStat(50, 10, 5),
            CreateBaseStat(500, 10, 5),   // Test with high base stat
            CreateBaseStat(50, 100, 5),   // Test with high IV
            CreateBaseStat(500, 10, 100), // Test with high level
        };

        foreach (var stats in testCases)
        {
            yield return RunSingleStatTest("calculateBlastStat", stats);
            yield return RunSingleStatTest("calculateBlastHP", stats);
            yield return RunSingleStatTest("calculateBlastMana", stats);
        }
    }

    private BaseStats CreateBaseStat(int baseStat, int iv, int level)
    {
        return new BaseStats
        {
            baseStat = baseStat,
            iv = iv,
            level = level
        };
    }

    private IEnumerator RunSingleStatTest(string rpcFunctionName, BaseStats stats)
    {
        var blast = new Blast();

        int localResult;

        switch (rpcFunctionName)
        {
            case "calculateBlastStat":
                localResult = blast.CalculateBlastStat(stats.baseStat, stats.iv, stats.level);
                break;
            case "calculateBlastHP":
                localResult = blast.CalculateBlastHp(stats.baseStat, stats.iv, stats.level);
                break;
            case "calculateBlastMana":
                localResult = blast.CalculateBlastMana(stats.baseStat, stats.iv, stats.level);
                break;
            default:
                throw new System.ArgumentException($"Unknown RPC function: {rpcFunctionName}");
        }

        Debug.Log($"[Local {rpcFunctionName}] {localResult} for {stats.ToJson()}");

        var rpcTask = _utilsTest.Client.RpcAsync(_utilsTest.Session, rpcFunctionName, stats.ToJson());
        while (!rpcTask.IsCompleted) yield return null;

        var response = rpcTask.Result;
        int serverResult = int.Parse(response.Payload);

        Debug.Log($"[Server {rpcFunctionName}] {serverResult}");

        Assert.AreEqual(serverResult, localResult, $"Mismatch in {rpcFunctionName}");
        Assert.GreaterOrEqual(localResult, 1, "Stat should be at least 1");
        yield return null;
    }


    [System.Serializable]
    public struct BaseStats
    {
        public int baseStat;
        public int iv;
        public int level;
    }
}
