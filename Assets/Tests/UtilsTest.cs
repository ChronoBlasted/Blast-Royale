using Nakama;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class UtilsTest
{
    public IClient Client;
    public ISession Session;
    public ISocket Socket;

    private const string ServerKey = "defaultkey";
    //private const string Host = "127.0.0.1";
    private const string Host = "209.38.212.129";
    private const int Port = 7350;

    public IEnumerator ConnectToServer()
    {
        Client = new Client("http", Host, Port, ServerKey);
        var task = Client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
        while (!task.IsCompleted) yield return null;

        Session = task.Result;

        Socket = Client.NewSocket();
        var connectTask = Socket.ConnectAsync(Session);
        while (!connectTask.IsCompleted) yield return null;

        Debug.Log("Connected to Nakama");
    }
}
