using Chrono.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChangeBlastPopup : Popup
{
    [SerializeField] List<ChangeBlastLayout> changeBlastLayouts;

    [SerializeField] CustomButton closeButton;
    [SerializeField] TMP_Text _title;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenPopup()
    {
        base.OpenPopup();

        UpdateData(PvEBattleManager.Instance.PlayerSquads);
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

    public void UpdateAction(List<UnityAction<int>> actions, CHANGE_REASON changeReason, string itemName = "")
    {
        switch (changeReason)
        {
            case CHANGE_REASON.HP:
            case CHANGE_REASON.MANA:
            case CHANGE_REASON.STATUS:
                _title.text = "Use " + itemName + " On";
                break;
            case CHANGE_REASON.KO:
            case CHANGE_REASON.SWAP:
                _title.text = "Swap Blast With";
                break;
        }


        for (int i = 0; i < changeBlastLayouts.Count; i++)
        {
            int buttonIndex = i;
            changeBlastLayouts[i].Button.onClick.RemoveAllListeners();

            if (changeBlastLayouts[i].IsUnlocked(changeReason))
            {
                foreach (UnityAction<int> action in actions)
                {
                    changeBlastLayouts[i].Button.onClick.AddListener(() => action.Invoke(buttonIndex));
                }
                changeBlastLayouts[i].Button.onClick.AddListener(ClosePopup);
            }
        }
    }

    public void UpdateClose(UnityAction action, bool canClose = true)
    {
        UIManager.Instance.BlackShadeView.CloseButton.onClick.RemoveAllListeners();

        if (canClose) UIManager.Instance.BlackShadeView.ShowCloseButton();
        else UIManager.Instance.BlackShadeView.HideCloseButton();

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
