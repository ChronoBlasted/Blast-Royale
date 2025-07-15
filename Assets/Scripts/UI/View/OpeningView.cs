using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpeningView : View
{
    [SerializeField] GameObject _logLayout;
    [SerializeField] Image _loadingLayout;
    public override void Init()
    {
        base.Init();

        ShowLogOption(false);
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);
    }

    public override void CloseView()
    {
        base.CloseView();
    }

    public void ShowLogOption(bool show)
    {
        if (show)
        {
            _logLayout.SetActive(true);
            _loadingLayout.enabled = false;
        }
        else
        {
            _logLayout.SetActive(false);
            _loadingLayout.enabled = true;
        }
    }

    public void HandleOnGuestLog()
    {
        NakamaManager.Instance.NakamaAuth.AuthenticateWithDevice();
    }

    public void HandleOnMailLog()
    {
        UIManager.Instance.ConfirmPopup.OpenPopup();

        UIManager.Instance.ConfirmPopup.UpdateDataWithTwoInputField("Enter mail", "Enter your credentials to log in via mail", "E-mail", TMP_InputField.ContentType.EmailAddress, "Password", TMP_InputField.ContentType.Password, (x, y) =>
        {
            NakamaManager.Instance.NakamaAuth.AuthenticateWithMail(x, y);

        });
    }

    public void HandleOnGoogleLog()
    {
        NakamaManager.Instance.NakamaAuth.SignInWithGoogle();
    }
}
