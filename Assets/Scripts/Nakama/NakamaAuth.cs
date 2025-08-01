using Google;
using Nakama;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaAuth : MonoBehaviour
{
    [SerializeField] NakamaClientConnexion _nakamaClientConnexion;

    IClient _client;
    ISession _session;

    Dictionary<string, string> vars = new Dictionary<string, string>();

    public void Init()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = "905607993036-8dirl49g0et7aus50qe5atmbgmhoavq5.apps.googleusercontent.com\r\n",
            RequestIdToken = true
        };

        _client = new Client(_nakamaClientConnexion.Scheme, _nakamaClientConnexion.Host, _nakamaClientConnexion.Port, _nakamaClientConnexion.ServerKey);

        vars["DeviceOS"] = SystemInfo.operatingSystem;
        vars["DeviceModel"] = SystemInfo.deviceModel;
        vars["GameVersion"] = Application.version;

        TryRestoreSession();
    }

    public async void TryRestoreSession()
    {
        var token = PlayerPrefs.GetString("nakama.token", null);

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                _session = Session.Restore(token);

                if (_session.IsExpired)
                {
                    _session = await _client.SessionRefreshAsync(_session);
                    Debug.Log("Session rafra�chie avec succ�s.");
                }

                SaveSession();

                await AfterAuth();
                return;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Session invalide ou expir�e, suppression du token. D�tail : " + ex.Message);
                Logout(); 
            }
        }

        UIManager.Instance.OpeningView.ShowLogOption(true); // Affiche les options de login
    }


    public async void AuthenticateWithDevice()
    {
        try
        {
            UIManager.Instance.OpeningView.ShowLogOption(false);

            var deviceId = PlayerPrefs.GetString("deviceId", SystemInfo.deviceUniqueIdentifier);

            if (deviceId == SystemInfo.unsupportedIdentifier)
            {
                deviceId = Guid.NewGuid().ToString();
            }

            PlayerPrefs.SetString("deviceId", deviceId);

            _session = await _client.AuthenticateDeviceAsync(deviceId, null, true, vars);

            SaveSession();

            SaveHandler.SaveValue("deviceLink", true);

            UIManager.Instance.LinkLogPopup.UpdateDeviceBG(true);

            await AfterAuth();
        }
        catch (Exception ex)
        {
            UIManager.Instance.ErrorView.AddError(ex.Message);
            UIManager.Instance.OpeningView.ShowLogOption(true);
        }
    }

    public async void AuthenticateWithMail(string email, string password)
    {
        try
        {
            UIManager.Instance.OpeningView.ShowLogOption(false);

            _session = await _client.AuthenticateEmailAsync(email, password, null, true, vars);
            SaveSession();

            SaveHandler.SaveValue("mailLink", true);
            SaveHandler.SaveValue("mail", email);

            UIManager.Instance.LinkLogPopup.UpdateMailBG(true);

            await AfterAuth();
        }
        catch (Exception ex)
        {
            UIManager.Instance.ErrorView.AddError(ex.Message);
            UIManager.Instance.OpeningView.ShowLogOption(true);
        }
    }

    public async void AuthenticateWithGoogle(string token)
    {
        try
        {
            UIManager.Instance.OpeningView.ShowLogOption(false);

            _session = await _client.AuthenticateGoogleAsync(token, null, true, vars);

            SaveSession();

            SaveHandler.SaveValue("googleLink", true);

            await AfterAuth();
        }
        catch (Exception ex)
        {
            UIManager.Instance.ErrorView.AddError(ex.Message);
            UIManager.Instance.OpeningView.ShowLogOption(true);
        }
    }


    async Task AfterAuth()
    {
        ISocket socket = _client.NewSocket();

        bool appearOnline = true;
        int connectionTimeout = 30;

        await socket.ConnectAsync(_session, appearOnline, connectionTimeout);

        NakamaManager.Instance.AuthUser(_client, _session, socket);
    }

    void SaveSession()
    {
        SaveHandler.SaveValue("nakama.token", _session.AuthToken);
    }

    public void Logout()
    {
        SaveHandler.DeleteValue("nakama.token");

        PlayerPrefs.DeleteAll();

        _session = null;

        GameManager.Instance.ReloadScene();
    }

    #region Linking Accounts

    public async void LinkDevice()
    {
        Debug.Log("LLINK DEVICE");
        try
        {
            var deviceId = PlayerPrefs.GetString("deviceId", SystemInfo.deviceUniqueIdentifier);

            if (deviceId == SystemInfo.unsupportedIdentifier)
            {
                deviceId = Guid.NewGuid().ToString();
            }

            PlayerPrefs.SetString("deviceId", deviceId);

            await _client.LinkDeviceAsync(_session, deviceId);

            SaveHandler.SaveValue("deviceLink", true);

            UIManager.Instance.LinkLogPopup.UpdateDeviceBG(true);
        }
        catch (Exception ex)
        {
            UIManager.Instance.ErrorView.AddError(ex.Message);
        }
    }

    public async void LinkMail(string email, string password)
    {
        try
        {
            await _client.LinkEmailAsync(_session, email, password);

            SaveHandler.SaveValue("mailLink", true);

            UIManager.Instance.LinkLogPopup.UpdateMailBG(true);
        }
        catch (Exception ex)
        {
            UIManager.Instance.ErrorView.AddError(ex.Message);

        }
    }

    public async void LinkGoogle(string token)
    {
        try
        {
            await _client.LinkGoogleAsync(_session, token);

            SaveHandler.SaveValue("googleLink", true);
        }
        catch (Exception ex)
        {
            UIManager.Instance.ErrorView.AddError(ex.Message);
        }
    }


    #endregion

    #region Unlink Accounts

    public async void UnlinkDevice()
    {
        try
        {
            string deviceId = PlayerPrefs.GetString("deviceId");

            await _client.UnlinkDeviceAsync(_session, deviceId);

            SaveHandler.SaveValue("deviceLink", false);

            UIManager.Instance.LinkLogPopup.UpdateDeviceBG(false);
        }
        catch (Exception ex)
        {
            UIManager.Instance.ErrorView.AddError(ex.Message);
        }
    }

    public async void UnlinkMail(string password)
    {
        try
        {

            string email = SaveHandler.LoadValue("mail", "");

            await _client.UnlinkEmailAsync(_session, email, password);

            SaveHandler.SaveValue("mailLink", false);

            UIManager.Instance.LinkLogPopup.UpdateMailBG(false);
        }
        catch (Exception ex)
        {
            UIManager.Instance.ErrorView.AddError(ex.Message);
        }
    }

    public async void UnlinkGoogle(string token)
    {
        try
        {
            await _client.LinkGoogleAsync(_session, token);

            SaveHandler.SaveValue("googleLink", false);
        }
        catch (Exception ex)
        {
            UIManager.Instance.ErrorView.AddError(ex.Message);
        }
    }


    #endregion

    #region AuthGoogle 

    private GoogleSignInConfiguration configuration;

    public void SignInWithGoogle()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthAuthFinished);
    }

    private void OnGoogleAuthAuthFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError("Google sign-in failed");
        }
        else
        {
            string idToken = task.Result.IdToken;
            Debug.Log("Google Sign-In Successful! ID Token: " + idToken);

            AuthenticateWithGoogle(idToken);
        }
    }

    public void LinkInWithGoogle()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthLinkFinished);
    }

    private void OnGoogleAuthLinkFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError("Google sign-in failed");
        }
        else
        {
            string idToken = task.Result.IdToken;
            Debug.Log("Google Sign-In Successful! ID Token: " + idToken);

            LinkGoogle(idToken);
        }
    }

    #endregion
}
