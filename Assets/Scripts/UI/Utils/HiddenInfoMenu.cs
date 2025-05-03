using Chrono.UI;
using UnityEngine;
using UnityEngine.UI;

public class HiddenInfoMenu : MonoBehaviour
{
    [SerializeField] GameObject _hiddenMenu;
    [SerializeField] CustomButton _button;
    [SerializeField] Canvas _canvas;

    public void HandleOnClick()
    {
        _button.interactable = false;

        _hiddenMenu.SetActive(true);

        if (UIManager.Instance.MenuView.SquadPanel.LastHiddenInfoMenu != null)
        {
            UIManager.Instance.MenuView.SquadPanel.LastHiddenInfoMenu.HandleOnClose();
        }

        UIManager.Instance.MenuView.SquadPanel.LastHiddenInfoMenu = this;

        _canvas.overrideSorting = true;
    }

    public void HandleOnClose()
    {
        _button.interactable = true;

        _hiddenMenu.SetActive(false);

        UIManager.Instance.MenuView.SquadPanel.LastHiddenInfoMenu = null;

        _canvas.overrideSorting = false;
    }
}
