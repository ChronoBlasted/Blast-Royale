using Chrono.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmPopup : Popup
{
    [SerializeField] TMP_Text _titleTxt, _descTxt, _placeHolderTxt;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] CustomButton _acceptButton, _cancelButton;

    bool _lastCanClose;
    UnityAction _lastUnityAction;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenPopup()
    {
        base.OpenPopup(true, false);


        if (_lastCanClose == false) UIManager.Instance.BlackShadeView.ShadeButton.onClick.RemoveAllListeners();
    }

    public override void ClosePopup()
    {
        base.ClosePopup();
    }

    public void UpdateData(string title, string desc, UnityAction acceptAction, bool canClose = true)
    {
        _lastCanClose = canClose;
        _lastUnityAction = acceptAction;

        _titleTxt.text = title;
        _descTxt.text = desc;

        _inputField.gameObject.SetActive(false);

        _acceptButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.AddListener(acceptAction);
        _acceptButton.onClick.AddListener(ClosePopup);

        _cancelButton.gameObject.SetActive(canClose);

        if (canClose)
        {
            _cancelButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.AddListener(ClosePopup);
        }
    }

    public void UpdateDataWithInputField(string title, string desc, string placeHolder, UnityAction<string> acceptAction, bool canClose = true)
    {
        _lastCanClose = canClose;

        _titleTxt.text = title;
        _descTxt.text = desc;
        _placeHolderTxt.text = placeHolder;

        _inputField.gameObject.SetActive(true);

        _inputField.text = "";

        _acceptButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.AddListener(() => acceptAction.Invoke(_inputField.text));
        _acceptButton.onClick.AddListener(ClosePopup);

        _cancelButton.gameObject.SetActive(canClose);

        if (canClose)
        {
            _cancelButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.AddListener(ClosePopup);
        }
    }
}
