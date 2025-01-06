using Chrono.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenInfoMenu : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _hiddenMenu;
    [SerializeField] CustomButton _button;
    public void HandleOnClick()
    {
        _button.interactable = false;

        UIManager.Instance.ShowBlackShade(HandleOnClose);

        _hiddenMenu.SetActive(true);
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = 5;
    }

    public void HandleOnClose()
    {
        _button.interactable = true;

        UIManager.Instance.HideBlackShade();

        _hiddenMenu.SetActive(false);
        _canvas.overrideSorting = false;
        _canvas.sortingOrder = 0;
    }

    public void HandleOnContinueFlow()
    {
        _button.interactable = true;

        _hiddenMenu.SetActive(false);
        _canvas.overrideSorting = false;
        _canvas.sortingOrder = 0;
    }
}
