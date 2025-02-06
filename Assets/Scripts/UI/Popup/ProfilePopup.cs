using Nakama;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfilePopup : Popup
{
    [SerializeField] TMP_Text _winTxt, _looseTxt, _blastDefeatedTxt, _blastCapturedtxt;

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

    public void UpdateData(string metadata)
    {
        var data = Nakama.TinyJson.JsonParser.FromJson<Metadata>(metadata);

        _winTxt.text = data.win.ToString();
        _looseTxt.text = data.loose.ToString();
        _blastDefeatedTxt.text = data.blast_defeated.ToString();
        _blastCapturedtxt.text = data.blast_captured.ToString();
    }
}
