using Nakama;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NakamaAuth : MonoBehaviour
{
    [SerializeField] NakamaClientConnexion _nakamaClientConnexion;

    IClient _client;
    ISession _session;

    public void Init()
    {
        _client = new Client(_nakamaClientConnexion.Scheme, _nakamaClientConnexion.Host, _nakamaClientConnexion.Port, _nakamaClientConnexion.ServerKey);

        AuthenticateWithDevice();
    }


    async void AuthenticateWithDevice()
    {
        var deviceId = PlayerPrefs.GetString("deviceId", SystemInfo.deviceUniqueIdentifier);

        if (deviceId == SystemInfo.unsupportedIdentifier)
        {
            deviceId = System.Guid.NewGuid().ToString();
        }

        PlayerPrefs.SetString("deviceId", deviceId);

        var vars = new Dictionary<string, string>();
        vars["DeviceOS"] = SystemInfo.operatingSystem;
        vars["DeviceModel"] = SystemInfo.deviceModel;
        vars["GameVersion"] = Application.version;


        try
        {
            _session = await _client.AuthenticateDeviceAsync(deviceId, null, true, vars);

            ISocket socket = _client.NewSocket();

            bool appearOnline = true;
            int connectionTimeout = 30;

            await socket.ConnectAsync(_session, appearOnline, connectionTimeout);

            NakamaManager.Instance.AuthUser(_client, _session, socket);

        }
        catch (Exception ex)
        {
            UIManager.Instance.ConfirmPopup.OpenPopup();
            UIManager.Instance.ConfirmPopup.UpdateData("Erreur de connexion", ex.Message, GameManager.Instance.ReloadScene, false);
        }
    }
}
