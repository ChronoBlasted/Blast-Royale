using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StartGameLayout : MonoBehaviour
{
    [SerializeField] CanvasGroup _cg;
    [SerializeField] ProfileLayout _p1Profile, _p2Profile;
    [SerializeField] PlayerInfoLayout _p1PlayerInfo, _p2PlayerInfo;

    [SerializeField] RectTransform _p1GameObject, _p2GameObject;

    Vector2 _p1OnScreenPosition;
    Vector2 _p1OffScreenPosition;

    Vector2 _p2OnScreenPosition;
    Vector2 _p2OffScreenPosition;

    private void Awake()
    {
        _p1OnScreenPosition = _p1GameObject.anchoredPosition;
        _p1OffScreenPosition = _p1OnScreenPosition + Vector2.left * 256f;

        _p1GameObject.anchoredPosition = _p1OffScreenPosition;

        _p2OnScreenPosition = _p2GameObject.anchoredPosition;
        _p2OffScreenPosition = _p2OnScreenPosition + Vector2.right * 256f;

        _p2GameObject.anchoredPosition = _p2OffScreenPosition;

        _cg.alpha = 0f;
        _cg.interactable = false;
        _cg.blocksRaycasts = false;
    }

    private void SetDefault()
    {
        _p1GameObject.anchoredPosition = _p1OffScreenPosition;
        _p2GameObject.anchoredPosition = _p2OffScreenPosition;

        _cg.alpha = 0f;
        _cg.interactable = false;
        _cg.blocksRaycasts = false;
    }

    public async Task DoStartAnim()
    {
        SetDefault();

        _cg.DOFade(1, .2f);
        _cg.interactable = true;
        _cg.blocksRaycasts = true;

        _p1GameObject.DOAnchorPos(_p1OnScreenPosition, 1f).SetEase(Ease.OutCirc);
        _p2GameObject.DOAnchorPos(_p2OnScreenPosition, 1f).SetEase(Ease.OutCirc);

        await Task.Delay(2500);

        _p1GameObject.DOAnchorPos(_p1OffScreenPosition, 1f).SetEase(Ease.InSine);
        _p2GameObject.DOAnchorPos(_p2OffScreenPosition, 1f).SetEase(Ease.InSine);

        await Task.Delay(500);

        _cg.DOFade(0f, .2f);
        _cg.interactable = false;
        _cg.blocksRaycasts = false;
    }

    public void UpdateData(string p2Name, int p2Trophy, PlayerStat p2Stat)
    {
        _p1Profile.UpdateUsername(NakamaManager.Instance.NakamaUserAccount.Username);
        _p1Profile.UpdateTrophy(NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.Trophies.ToString()].ToString());
        _p1PlayerInfo.UpdateData(NakamaManager.Instance.NakamaUserAccount.LastData.playerStats);

        _p2Profile.UpdateUsername(p2Name);
        _p2Profile.UpdateTrophy(p2Trophy.ToString());
        _p2PlayerInfo.UpdateData(p2Stat);
    }
}
