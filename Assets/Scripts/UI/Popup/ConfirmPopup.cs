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

    public override void Init()
    {
        base.Init();
    }

    public override void OpenPopup()
    {
        base.OpenPopup();
    }

    public override void ClosePopup()
    {
        base.ClosePopup();
    }

    public void UpdateData(string title, string desc, UnityAction acceptAction)
    {
        _titleTxt.text = title;
        _descTxt.text = desc;

        _inputField.gameObject.SetActive(false);

        _acceptButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        _acceptButton.onClick.AddListener(acceptAction);
        _cancelButton.onClick.AddListener(ClosePopup);
    }

    public void UpdateDataWithInputField(string title, string desc, string placeHolder, UnityAction<string> acceptAction)
    {
        _titleTxt.text = title;
        _descTxt.text = desc;
        _placeHolderTxt.text = placeHolder;

        _acceptButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        _inputField.gameObject.SetActive(true);

        _inputField.text = "";

        _acceptButton.onClick.AddListener(() => acceptAction.Invoke(_inputField.text));

        _cancelButton.onClick.AddListener(ClosePopup);
    }

}
