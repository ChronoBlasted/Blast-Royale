using Nakama;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfilePopup : Popup
{
    [SerializeField] TMP_Text _winTxt, _looseTxt, _blastDefeatedTxt, _blastCapturedtxt;

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

        _winTxt.text = _lastData.win.ToString();
        _looseTxt.text = _lastData.loose.ToString();
        _blastDefeatedTxt.text = _lastData.blast_defeated.ToString();
        _blastCapturedtxt.text = _lastData.blast_captured.ToString();

        
        if (!_lastData.updated_nickname)
        {
            UIManager.Instance.ConfirmPopup.OpenPopup();

            UIManager.Instance.ConfirmPopup.UpdateDataWithInputField("Change username", "Enter your new username", playerName, (x) =>
            {
                _ = NakamaManager.Instance.NakamaUserAccount.UpdateUsername(x); // TODO Secure cette fonction

            },false);
        }
    }
}
