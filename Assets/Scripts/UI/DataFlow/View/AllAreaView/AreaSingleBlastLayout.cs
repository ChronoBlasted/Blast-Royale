using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaSingleBlastLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _blastNameTxt;
    [SerializeField] Image _bg, _blastIco;

    BlastData _blast;
    bool isDiscovered;

    public void Init(BlastData blast)
    {
        _blast = blast;

        isDiscovered = NakamaManager.Instance.NakamaBlastTracker.BlastTracker[_blast.id.ToString()].versions["1"].catched;

        _blastIco.sprite = NakamaData.Instance.GetBlastDataRef(_blast.id).Sprite;

        if (isDiscovered)
        {
            _blastNameTxt.text = NakamaData.Instance.GetBlastDataRef(_blast.id).Name.GetLocalizedString();

            TypeData typeData = ResourceObjectHolder.Instance.GetTypeDataByType(_blast.type);

            _blastIco.color = Color.white;

            _bg.color = typeData.Color;
        }
        else
        {
            _blastNameTxt.text = "—";

            _blastIco.color = Color.black;

            _bg.color = Color.white;
        }
    }

    public void HandleOnOpenInfo()
    {
        if (isDiscovered)
        {
            UIManager.Instance.BlastInfoPopup.UpdateData(_blast);

            UIManager.Instance.BlastInfoPopup.OpenPopup();
        }
    }
}
