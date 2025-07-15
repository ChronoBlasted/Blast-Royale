using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using System.Collections;

public class RewardedAdsButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] AdsLayout _adsLayout;
    [SerializeField] Image _background;
    [SerializeField] Sprite _regularBG;
    [SerializeField] Sprite _activeBG;
    [SerializeField] ParticleSystem _adsReadyParticle;

    [Header("Events")]
    [SerializeField] UnityEvent onAdsCompleted;

    bool _isAdsActive = false;

    public void Init()
    {
        _adsLayout.gameObject.SetActive(false);
        AdsManager.Instance.OnRewardedAdLoaded.AddListener(OnAdLoaded);
    }

    private void OnDestroy()
    {
        AdsManager.Instance.OnRewardedAdLoaded.RemoveListener(OnAdLoaded);
    }

    private void OnEnable()
    {
        if (_isAdsActive)
        {
            if (_adsReadyParticle != null) StartCoroutine(PlayParticleNextFrame());
        }
    }

    private IEnumerator PlayParticleNextFrame()
    {
        yield return new WaitForEndOfFrame();
        _adsReadyParticle.Play();
    }

    private void OnAdLoaded()
    {
        if (!_isAdsActive)
        {
            _adsLayout.gameObject.SetActive(true);
        }
    }

    public void RefreshAd()
    {
        SetAdsOff();

        if (AdsManager.Instance.RewardedAd.CanShowAd())
        {
            OnAdLoaded();
        }
    }

    public void ShowAd()
    {
        AdsManager.Instance.ShowRewardedAd(onAdsCompleted);
        _adsLayout.gameObject.SetActive(false);
    }

    public void SetAdsOff()
    {
        if (_background == null || _adsReadyParticle == null) return;

        _background.sprite = _regularBG;
        _adsReadyParticle.Stop();
        _isAdsActive = false;
    }

    public void SetAdsOn()
    {
        if (_background == null || _adsReadyParticle == null) return;

        _adsLayout.gameObject.SetActive(false);

        _background.sprite = _activeBG;
        _adsReadyParticle.Play();
        _isAdsActive = true;
    }

    public void Disable()
    {
        AdsManager.Instance.OnRewardedAdLoaded.RemoveListener(OnAdLoaded);

        _adsLayout.gameObject.SetActive(false);
    }
}
