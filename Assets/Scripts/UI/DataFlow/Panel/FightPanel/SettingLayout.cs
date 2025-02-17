using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingLayout : MonoBehaviour
{
    [SerializeField] CanvasGroup _settingMenu;

    Tweener _settingFadeTweener;
    public void OpenSettingMenu()
    {
        UIManager.Instance.ShowBlackShade(CloseSettingMenu);

        _settingMenu.gameObject.SetActive(true);

        if (_settingFadeTweener.IsActive()) _settingFadeTweener.Kill();

        _settingFadeTweener = _settingMenu.DOFade(1, .2f).OnComplete(() =>
                    {
                        _settingMenu.blocksRaycasts = true;
                        _settingMenu.interactable = true;
                    }).SetUpdate(UpdateType.Normal, true);

        _settingMenu.gameObject.transform.localScale = Vector3.zero;
        _settingMenu.gameObject.transform.DOScale(new Vector3(1, 1, 1), .2f).SetEase(Ease.OutBack);
    }

    public void CloseSettingMenu()
    {
        UIManager.Instance.HideBlackShade();

        if (_settingFadeTweener.IsActive()) _settingFadeTweener.Kill();

        _settingMenu.blocksRaycasts = false;
        _settingMenu.interactable = false;

        _settingFadeTweener = _settingMenu.DOFade(0, 0f)
            .OnComplete(() =>
            {
                _settingMenu.gameObject.SetActive(false);
            })
            .SetUpdate(UpdateType.Normal, true);
    }

    public void HandleOpenDailyReward()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.DailyRewardView);
    }
    public void HandleOpenPedia()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.PediaView);
    }
    public void HandleOpenFriend()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.FriendView);
    }
    public void HandleOpenLeaderboard()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.LeaderboardView);
    }
    public void HandleOpenSetting()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.SettingView);
    }
}
