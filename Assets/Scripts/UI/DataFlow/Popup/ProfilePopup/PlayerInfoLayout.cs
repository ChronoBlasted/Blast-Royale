using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _winTxt, _looseTxt, _blastDefeatedTxt, _blastCapturedtxt, _winRateTxt;

    public void UpdateData(PlayerStat stat)
    {
        if (_winTxt != null)
            _winTxt.text = stat.win.ToString();

        if (_looseTxt != null)
            _looseTxt.text = stat.loose.ToString();

        if (_blastDefeatedTxt != null)
            _blastDefeatedTxt.text = stat.blast_defeated.ToString();

        if (_blastCapturedtxt != null)
            _blastCapturedtxt.text = stat.blast_captured.ToString();

        int total = stat.win + stat.loose;
        int winRate = total > 0 ? (stat.win * 100) / total : 0;

        if (_winRateTxt != null)
            _winRateTxt.text = winRate + " %";

    }

}
