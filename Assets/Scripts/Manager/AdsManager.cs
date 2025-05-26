using BaseTemplate.Behaviours;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdsManager : MonoSingleton<AdsManager>
{
#if UNITY_ANDROID
    string _adUnitId = "ca-app-pub-1529838381890617/9990832209";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-1529838381890617/9990832209";
#else
  private string _adUnitId = "unused";
#endif

    public UnityEvent OnRewardedAdLoaded;

    public RewardedAd RewardedAd;

    public void Init()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("Mobile Ads Initialized");

            LoadRewardedAd();
        });

        RewardedAd.CanShowAd();
    }

    public void LoadRewardedAd()
    {
        if (RewardedAd != null)
        {
            RewardedAd.Destroy();
            RewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        var adRequest = new AdRequest();

        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                RewardedAd = ad;

                RegisterReloadHandler(RewardedAd);

                OnRewardedAdLoaded?.Invoke();

            });
    }

    public void ShowRewardedAd(UnityEvent onAdsComplete)
    {
        if (RewardedAd != null && RewardedAd.CanShowAd())
        {
            RewardedAd.Show((Reward reward) =>
            {
                onAdsComplete.Invoke();
            });
        }
    }

    void RegisterReloadHandler(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            LoadRewardedAd();
        };
    }
}
