using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdsManager : MonoSingleton<AdsManager>
{
    //    [SerializeField] string _androidGameId;
    //    [SerializeField] string _iOSGameId;
    //    [SerializeField] bool _testMode = true;
    //    string _gameId;


    public void Init()
    {
        //InitializeAds();
    }

    //    public void InitializeAds()
    //    {
    //#if UNITY_IOS
    //       _gameId = _iOSGameId;  
    //#elif UNITY_ANDROID
    //       _gameId = _androidGameId;  
    //#elif UNITY_EDITOR
    //        _gameId = _androidGameId;
    //#endif

    //        if (!Advertisement.isInitialized && Advertisement.isSupported)
    //        {
    //            Advertisement.Initialize(_gameId, _testMode, this);
    //        }
    //    }

    //    public void OnInitializationComplete()
    //    {
    //    }


    //    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    //    {
    //        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    //    }
}
