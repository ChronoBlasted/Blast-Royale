using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] Button _showAdButton;
#if UNITY_IOS
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
#endif

    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] UnityEvent _onAdsCompleted;

    string _adUnitId = null;
    bool _isLoaded = false;



    public void Init()
    {
#if UNITY_IOS
    _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
    _adUnitId = _androidAdUnitId;
#elif UNITY_EDITOR
        _adUnitId = _androidAdUnitId;
#endif

        _showAdButton.gameObject.SetActive(false);
        _isLoaded = false;

        LoadAd();
    }

    void LoadAd()
    {
        if (_isLoaded == false)
        {
            Debug.Log("Start loading rewarded ads");

            Advertisement.Load(_adUnitId, this);
        }
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        if (adUnitId.Equals(_adUnitId))
        {
            Debug.Log("Ads loaded");

            _isLoaded = true;
            _showAdButton.gameObject.SetActive(true);
            _showAdButton.onClick.AddListener(ShowAd);
            _showAdButton.interactable = true;
        }
    }

    public void ShowAd()
    {
        _showAdButton.interactable = false;
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            _isLoaded = false;
            _onAdsCompleted?.Invoke();

            LoadAd();
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        _showAdButton.onClick.RemoveAllListeners();
    }
}
