using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class RewardedAdsButton : MonoBehaviour
{
    [SerializeField] UnityEvent _onAdsCompleted;

    [SerializeField] Image _bg;
    [SerializeField] Sprite _regularBG, _activeBG;
    [SerializeField] ParticleSystem _adsReadyParticle;

    bool _isAdsActive = false;

    public void Init()
    {
        transform.gameObject.SetActive(false);

        AdsManager.Instance.OnRewardedAdLoaded.AddListener(LoadedAd);
    }

    public void LoadedAd()
    {
        if (_isAdsActive == false)
        {
            transform.gameObject.SetActive(true);
        }
    }

    public void RefreshAd()
    {
        if (AdsManager.Instance.RewardedAd.CanShowAd())
        {
            LoadedAd();
        }
    }

    public void ShowAd()
    {
        AdsManager.Instance.ShowRewardedAd(_onAdsCompleted);

        transform.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        AdsManager.Instance.OnRewardedAdLoaded.RemoveListener(LoadedAd);
    }

    public void SetAdsOff()
    {
        if (_bg != null)
        {
            _bg.sprite = _regularBG;
            _adsReadyParticle.Stop();

            _isAdsActive = false;
        }
    }

    public void SetAdsOn()
    {
        if (_bg != null)
        {
            _bg.sprite = _activeBG;
            _adsReadyParticle.Play();

            _isAdsActive = true;
        }
    }
}
