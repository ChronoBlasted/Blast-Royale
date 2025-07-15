using Chrono.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmPopup : Popup
{
    [SerializeField] TMP_Text _titleTxt, _descTxt, _placeHolder1Txt, _placeHolder2Txt;
    [SerializeField] TMP_InputField _inputField1, _inputField2;
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

        _inputField1.gameObject.SetActive(false);
        _inputField2.gameObject.SetActive(false);

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

    public void UpdateDataWithInputField(string title, string desc, string placeHolder, TMP_InputField.ContentType inputType, UnityAction<string> acceptAction, bool canClose = true)
    {
        _lastCanClose = canClose;

        _titleTxt.text = title;
        _descTxt.text = desc;
        _placeHolder1Txt.text = placeHolder;

        _inputField1.gameObject.SetActive(true);
        _inputField2.gameObject.SetActive(false);

        _inputField1.text = "";
        _inputField1.contentType = inputType;

        _acceptButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.AddListener(() => acceptAction.Invoke(_inputField1.text));
        _acceptButton.onClick.AddListener(ClosePopup);

        _cancelButton.gameObject.SetActive(canClose);

        if (canClose)
        {
            _cancelButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.AddListener(ClosePopup);
        }
    }

    public void UpdateDataWithTwoInputField(string title, string desc, string placeHolder1, TMP_InputField.ContentType inputType1, string placeHolder2, TMP_InputField.ContentType inputType2, UnityAction<string, string> acceptAction, bool canClose = true)
    {
        _lastCanClose = canClose;

        _titleTxt.text = title;
        _descTxt.text = desc;
        _placeHolder1Txt.text = placeHolder1;
        _placeHolder2Txt.text = placeHolder2;

        _inputField1.gameObject.SetActive(true);
        _inputField2.gameObject.SetActive(true);

        _inputField1.text = "";
        _inputField2.text = "";

        _inputField1.contentType = inputType1;
        _inputField2.contentType = inputType2;

        _acceptButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.AddListener(() => acceptAction.Invoke(_inputField1.text, _inputField2.text));
        _acceptButton.onClick.AddListener(ClosePopup);

        _cancelButton.gameObject.SetActive(canClose);

        if (canClose)
        {
            _cancelButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.AddListener(ClosePopup);
        }
    }
}
