using Chrono.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LinkLogPopup : Popup
{
    [SerializeField] Image _deviceBG, _mailBG, _deviceLogTick, _mailLogTick;
    [SerializeField] CustomButton _linkDeviceButton, _linkMailButton;


    [SerializeField] Sprite _linked, _unlinked;

    public override void Init()
    {
        base.Init();

        UpdateDeviceBG(SaveHandler.LoadValue("deviceLink", false));
        UpdateMailBG(SaveHandler.LoadValue("mailLink", false));
    }

    #region Link
    public void HandleOnLinkDevice()
    {
        NakamaManager.Instance.NakamaAuth.LinkDevice();
    }

    public void HandleOnLinkEmail()
    {
        UIManager.Instance.ConfirmPopup.OpenPopup(false);

        UIManager.Instance.ConfirmPopup.UpdateDataWithTwoInputField("Enter mail", "Enter your credentials to log in via mail", "E-mail", TMP_InputField.ContentType.EmailAddress, "Password", TMP_InputField.ContentType.Password, (x, y) =>
        {
            NakamaManager.Instance.NakamaAuth.LinkMail(x, y);
        });
    }

    #endregion

    #region Unlink

    public void HandleOnUnlinkDevice()
    {
        UIManager.Instance.ConfirmPopup.OpenPopup(false);
        UIManager.Instance.ConfirmPopup.UpdateData("UNLINK DEVICE", "Are you sure to unlink your device ?", NakamaManager.Instance.NakamaAuth.UnlinkDevice);
    }

    public void HandleOnUnlinkEmail()
    {
        UIManager.Instance.ConfirmPopup.OpenPopup(false);
        UIManager.Instance.ConfirmPopup.UpdateDataWithInputField("UNLINK EMAIL", "Enter the password of " + SaveHandler.LoadValue("mail", "") + ", to unlink mail !", "Password", TMP_InputField.ContentType.Password, x =>
        {
            NakamaManager.Instance.NakamaAuth.UnlinkMail(x);
        });
    }

    #endregion

    #region ButtonState

    public void UpdateDeviceBG(bool isLinked)
    {
        _deviceBG.sprite = isLinked ? _linked : _unlinked;
        _deviceLogTick.enabled = isLinked;

        _linkDeviceButton.onClick.RemoveAllListeners();

        if (isLinked)
        {
            _linkDeviceButton.onClick.AddListener(HandleOnUnlinkDevice);
        }
        else
        {
            _linkDeviceButton.onClick.AddListener(HandleOnLinkDevice);
        }
    }

    public void UpdateMailBG(bool isLinked)
    {
        _mailBG.sprite = isLinked ? _linked : _unlinked;
        _mailLogTick.enabled = isLinked;

        _linkMailButton.onClick.RemoveAllListeners();

        if (isLinked)
        {
            _linkMailButton.onClick.AddListener(HandleOnUnlinkEmail);
        }
        else
        {
            _linkMailButton.onClick.AddListener(HandleOnLinkEmail);
        }
    }

    #endregion


    public void HandleOnLogOut()
    {
        UIManager.Instance.ConfirmPopup.OpenPopup(false);
        UIManager.Instance.ConfirmPopup.UpdateData("LOG OUT", "Are you sure to log out ?", NakamaManager.Instance.NakamaAuth.Logout);


    }

    public void HandleOnDeleteAccount()
    {
        UIManager.Instance.ConfirmPopup.OpenPopup(false);
        UIManager.Instance.ConfirmPopup.UpdateData("DELETE ACCOUNT", "Are you sure to delete your account ?", NakamaManager.Instance.NakamaUserAccount.DeleteAccount);
    }

}
