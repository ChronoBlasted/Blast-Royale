using System.Collections;
using Nakama.TinyJson;
using NUnit.Framework;
using UnityEngine.TestTools;

public class ExperienceTest
{
    UtilsTest _utilsTest;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _utilsTest = new UtilsTest();
        yield return _utilsTest.ConnectToServer();
    }

    [UnityTest]
    public IEnumerator RunExperienceGainTests()
    {
        var testCases = new[]
        {
            new ExpGainParams { expYield = 100, enemyLevel = 5, yourLevel = 5 },
            new ExpGainParams { expYield = 300, enemyLevel = 5, yourLevel = 5 },
            new ExpGainParams { expYield = 100, enemyLevel = 5, yourLevel = 100 },
            new ExpGainParams { expYield = 100, enemyLevel = 100, yourLevel = 5 }
        };

        foreach (var testCase in testCases)
        {
            yield return TestExperienceGain(testCase);
        }
    }

    private IEnumerator TestExperienceGain(ExpGainParams payload)
    {
        int localResult = NakamaLogic.CalculateExpGain(
            payload.expYield,
            payload.yourLevel,
            payload.enemyLevel
        );

        var rpcTask = _utilsTest.Client.RpcAsync(_utilsTest.Session, "calculateExpGain", payload.ToJson());
        while (!rpcTask.IsCompleted) yield return null;

        var serverResult = int.Parse(rpcTask.Result.Payload);
        Assert.AreEqual(serverResult, localResult);
    }

    [UnityTest]
    public IEnumerator RunLevelFromExperienceTests()
    {
        int[] testExpValues = { 100, 1000, 10000 };

        foreach (var exp in testExpValues)
        {
            yield return TestLevelForExp(exp);
        }
    }

    private IEnumerator TestLevelForExp(int exp)
    {
        int localResult = NakamaLogic.CalculateLevelFromExperience(exp);

        var rpcTask = _utilsTest.Client.RpcAsync(_utilsTest.Session, "calculateLevelFromExp", exp.ToJson());
        while (!rpcTask.IsCompleted) yield return null;

        var serverResult = int.Parse(rpcTask.Result.Payload);
        Assert.AreEqual(serverResult, localResult);
    }

    [UnityTest]
    public IEnumerator RunExperienceFromLevelTests()
    {
        int[] testLevels = { 5, 50, 100 };

        foreach (var level in testLevels)
        {
            yield return TestExpForLevel(level);
        }
    }

    private IEnumerator TestExpForLevel(int level)
    {
        int localResult = NakamaLogic.CalculateExperienceFromLevel(level);

        var rpcTask = _utilsTest.Client.RpcAsync(_utilsTest.Session, "calculateExpFromLevel", level.ToJson());
        while (!rpcTask.IsCompleted) yield return null;

        var serverResult = int.Parse(rpcTask.Result.Payload);
        Assert.AreEqual(serverResult, localResult);
    }

    public struct ExpGainParams
    {
        public int expYield;
        public int enemyLevel;
        public int yourLevel;
    }
}
