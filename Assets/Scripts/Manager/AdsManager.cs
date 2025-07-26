using BaseTemplate.Behaviours;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public class AdsManager : MonoSingleton<AdsManager>
{
#if UNITY_ANDROID
    string _adUnitId = "ca-app-pub-1529838381890617/9990832209";
#elif UNITY_IPHONE
    string _adUnitId = "ca-app-pub-1529838381890617/9990832209";
#else
    string _adUnitId = "unused";
#endif

    public UnityEvent OnRewardedAdLoaded;
    public RewardedAd RewardedAd;

    private bool _adsEnabled = false;

    public void Init()
    {
#if UNITY_ANDROID || UNITY_IOS
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            _adsEnabled = true;
            LoadRewardedAd();
        });
#else
        Debug.Log("AdsManager: Mobile Ads not supported on this platform.");
#endif
    }

    public void LoadRewardedAd()
    {
        if (!_adsEnabled)
        {
            Debug.LogWarning("AdsManager: Ads not enabled. Skipping ad load.");
            return;
        }

        if (RewardedAd != null)
        {
            RewardedAd.Destroy();
            RewardedAd = null;
        }

        var adRequest = new AdRequest();

        RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error: " + error);
                return;
            }

            RewardedAd = ad;

            RegisterReloadHandler(RewardedAd);

            OnRewardedAdLoaded?.Invoke();
        });
    }

    public void ShowRewardedAd(UnityEvent onAdsComplete)
    {
        if (!_adsEnabled)
        {
            Debug.LogWarning("AdsManager: Ads not available on this platform.");
            return;
        }

        if (RewardedAd != null && RewardedAd.CanShowAd())
        {
            RewardedAd.Show((GoogleMobileAds.Api.Reward reward) =>
            {
                onAdsComplete.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("AdsManager: Rewarded ad not ready.");
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
            Debug.LogError("Rewarded ad failed to open full screen content with error: " + error);
            LoadRewardedAd();
        };
    }
}
