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

        //TODO Update data with current team
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

    public void UpdateAction(List<UnityAction<int>> actions)
    {
        for (int i = 0; i < customButtons.Count; i++)
        {
            int buttonIndex = i;
            customButtons[i].onClick.RemoveAllListeners();

            if (!changeBlastLayouts[i].IsLocked())
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
        closeButton.onClick.RemoveAllListeners();

        closeButton.gameObject.SetActive(canClose);

        if (canClose)
        {
            closeButton.onClick.AddListener(action);
            closeButton.onClick.AddListener(ClosePopup);

            UIManager.Instance.BlackShadeButton.onClick.AddListener(action);
        }
        else
        {
            UIManager.Instance.BlackShadeButton.onClick.RemoveAllListeners();
        }
    }
}
