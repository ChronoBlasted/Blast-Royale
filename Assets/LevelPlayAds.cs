using com.unity3d.mediation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayAds : MonoBehaviour
{
    private void Start()
    {
        IronSource.Agent.init("22271ee65");
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
}
