using Chrono.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeBlastPopup : Popup
{
    [SerializeField] List<ChangeBlastLayout> changeBlastLayouts;
    [SerializeField] List<CustomButton> customButtons;

    [SerializeField] CustomButton closeButton;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenPopup()
    {
        base.OpenPopup();

        UpdateData(WildBattleManager.Instance.PlayerSquads);
    }

    public override void ClosePopup()
    {
        base.ClosePopup();
    }

    public void UpdateData(List<Blast> blasts)
    {
        for (int i = 0; i < changeBlastLayouts.Count; i++)
        {
            changeBlastLayouts[i].Init(blasts[i]);
        }
    }

    public void UpdateAction(List<UnityAction<int>> actions, CHANGE_REASON changeReason)
    {
        for (int i = 0; i < customButtons.Count; i++)
        {
            int buttonIndex = i;
            customButtons[i].onClick.RemoveAllListeners();

            if (changeBlastLayouts[i].IsUnlocked(changeReason))
            {
                foreach (UnityAction<int> action in actions)
                {
                    customButtons[i].onClick.AddListener(() => action.Invoke(buttonIndex));
                }
                customButtons[i].onClick.AddListener(ClosePopup);
            }
        }
    }

    public void UpdateClose(UnityAction action, bool canClose = true)
    {
        UIManager.Instance.BlackShadeView.CloseButton.onClick.RemoveAllListeners();

        if (canClose) UIManager.Instance.BlackShadeView.HideCloseButton();
        else UIManager.Instance.BlackShadeView.ShowCloseButton();

        if (canClose)
        {
            UIManager.Instance.BlackShadeView.CloseButton.onClick.AddListener(action);
            UIManager.Instance.BlackShadeView.CloseButton.onClick.AddListener(ClosePopup);

            UIManager.Instance.BlackShadeView.ShadeButton.onClick.AddListener(action);
        }
        else
        {
            UIManager.Instance.BlackShadeView.ShadeButton.onClick.RemoveAllListeners();
        }
    }
}

public enum CHANGE_REASON
{
    HP,
    MANA,
    STATUS,
    KO,
    SWAP
}
