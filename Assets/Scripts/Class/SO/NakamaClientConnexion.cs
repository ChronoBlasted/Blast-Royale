using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNakamaClientConnexion", menuName = "ScriptableObjects/NewNakamaClientConnexion", order = 0)]
public class NakamaClientConnexion : ScriptableObject
{
    public string Scheme = "http";
    public string Host = "127.0.0.1";
    public string ServerKey = "defaultkey";
    public int Port = 7350;
}
