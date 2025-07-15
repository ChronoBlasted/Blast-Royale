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
            _session = Session.Restore(token);

            if (_session.IsExpired)
            {
                try
                {
                    _session = await _client.SessionRefreshAsync(_session);

                    SaveSession();

                    await AfterAuth();

                    return;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Échec du rafraîchissement de la session : " + ex.Message);
                }
            }
            else
            {
                Debug.Log("Session restaurée automatiquement.");

                await AfterAuth();
                return;
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

            await AfterAuth();
        }
        catch (Exception ex)
        {
            UIManager.Instance.ConfirmPopup.OpenPopup();
            UIManager.Instance.ConfirmPopup.UpdateData("Erreur de connexion", ex.Message, GameManager.Instance.ReloadScene, false);
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
            await AfterAuth();
        }
        catch (Exception ex)
        {
            UIManager.Instance.ConfirmPopup.OpenPopup();
            UIManager.Instance.ConfirmPopup.UpdateData("Erreur de connexion", ex.Message, GameManager.Instance.ReloadScene, false);
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

        _session = null;

        UIManager.Instance.OpeningView.ShowLogOption(true);
    }

    #region Linking Accounts

    public async void LinkDevice(string email, string password)
    {
        try
        {
            var deviceId = PlayerPrefs.GetString("deviceId", SystemInfo.deviceUniqueIdentifier);

            if (deviceId == SystemInfo.unsupportedIdentifier)
            {
                deviceId = Guid.NewGuid().ToString();
            }

            PlayerPrefs.SetString("deviceId", deviceId);

            await _client.LinkDeviceAsync(_session, deviceId);
        }
        catch (Exception ex)
        {
            UIManager.Instance.ConfirmPopup.OpenPopup();
            UIManager.Instance.ConfirmPopup.UpdateData("Erreur de connexion", ex.Message, GameManager.Instance.ReloadScene, false);
        }
    }

    public async void LinkMail(string email, string password)
    {
        try
        {
            await _client.LinkEmailAsync(_session, email, password);
        }
        catch (Exception ex)
        {
            UIManager.Instance.ConfirmPopup.OpenPopup();
            UIManager.Instance.ConfirmPopup.UpdateData("Erreur de connexion", ex.Message, GameManager.Instance.ReloadScene, false);
        }
    }

    public async void LinkGoogle()
    {
        try
        {
            await _client.LinkGoogleAsync(_session, "");
        }
        catch (Exception ex)
        {
            UIManager.Instance.ConfirmPopup.OpenPopup();
            UIManager.Instance.ConfirmPopup.UpdateData("Erreur de connexion", ex.Message, GameManager.Instance.ReloadScene, false);
        }
    }

    #endregion
}
