using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LinkLogPopup : Popup
{



    public void HandleOnLinkDevice()
    {
        NakamaManager.Instance.NakamaAuth.LinkDevice();
    }

    public void HandleOnLinkEmail()
    {
        UIManager.Instance.ConfirmPopup.OpenPopup();

        UIManager.Instance.ConfirmPopup.UpdateDataWithTwoInputField("Enter mail", "Enter your credentials to log in via mail", "E-mail", TMP_InputField.ContentType.EmailAddress, "Password", TMP_InputField.ContentType.Password, (x, y) =>
        {
            NakamaManager.Instance.NakamaAuth.LinkMail(x, y);

        });
    }

    public void HandleOnLinkGoogle()
    {
        NakamaManager.Instance.NakamaAuth.LinkInWithGoogle();
    }


    public void HandleOnLinkFacebook()
    {
       // NakamaManager.Instance.NakamaAuth.LinkFacebook();
    }

    public void HandleOnLogOut()
    {
        NakamaManager.Instance.NakamaAuth.Logout();

        GameManager.Instance.ReloadScene();
    }

    public void HandleOnDeleteAccount()
    {
    }

}
