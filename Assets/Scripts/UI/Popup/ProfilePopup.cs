using Nakama;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfilePopup : Popup
{
    [SerializeField] PlayerInfoLayout playerInfoLayout;

    Metadata _lastData;

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


    public void UpdateData(Metadata metadata, string playerName = null)
    {
        _lastData = metadata;

        playerInfoLayout.UpdateData(_lastData.playerStats);


        if (!_lastData.updated_nickname)
        {
            UIManager.Instance.ConfirmPopup.OpenPopup();

            UIManager.Instance.ConfirmPopup.UpdateDataWithInputField(
                "Change username",
                "Enter your new username",
                playerName,
                TMP_InputField.ContentType.Alphanumeric,
                (newName) =>
                {
                    _ = NakamaManager.Instance.NakamaUserAccount.UpdateUsername(newName);
                },
                false,
                3

            );

        }

    }
}
