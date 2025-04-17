using Chrono.UI;
using UnityEngine;
using UnityEngine.UI;

public class HiddenInfoMenu : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _hiddenMenu;
    [SerializeField] CustomButton _button;

    public void HandleOnClick()
    {
        _button.interactable = false;

        UIManager.Instance.BlackShadeView.ShowBlackShade(HandleOnClose);

        _hiddenMenu.SetActive(true);
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = 5;
    }

    public void HandleOnClose()
    {
        UIManager.Instance.BlackShadeView.HideBlackShade();

        HandleOnContinueFlow();
    }

    public void HandleOnContinueFlow()
    {
        _button.interactable = true;

        _hiddenMenu.SetActive(false);
        _canvas.overrideSorting = false;
        _canvas.sortingOrder = 0;
    }
}
