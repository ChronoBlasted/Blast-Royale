using System.Collections;
using Nakama;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Threading.Tasks;

public class BattleTest
{
    private IClient _client;
    private ISession _session;
    private ISocket _socket;

    private const string ServerKey = "defaultkey";
    private const string Host = "127.0.0.1"; // ou l'IP de ton serveur
    private const int Port = 7350;
    private const bool UseSSL = false;

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
    public IEnumerator TestSimpleRpcCall()
    {
        //// Appelle la RPC "test_rpc"
        //var payload = "{}";
        //var rpcTask = _client.RpcAsync(_session, "test_rpc", payload);
        //while (!rpcTask.IsCompleted) yield return null;

        //var response = rpcTask.Result;
        //var serverResult = int.Parse(response.Payload); // si le payload est juste un nombre

        //// Fonction locale équivalente
        //int localResult = LocalFunction();

        //Assert.AreEqual(localResult, serverResult);
        yield return null;
    }

    private int LocalFunction()
    {
        return 42; // doit correspondre au retour de "test_rpc"
    }
}
