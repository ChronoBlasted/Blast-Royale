using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaSingleBlastLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _blastNameTxt;
    [SerializeField] Image _bg,_ico, _typeIco;

    BlastData _blast;

    public void Init(BlastData blast)
    {
        _blast = blast;

        _blastNameTxt.text = NakamaData.Instance.GetBlastDataRef(_blast.id).Name.GetLocalizedString();

        TypeData typeData = ResourceObjectHolder.Instance.GetTypeDataByType(_blast.type);

        _ico.sprite = NakamaData.Instance.GetBlastDataRef(_blast.id).Sprite;
        _typeIco.sprite = typeData.Sprite;

        _bg.color = typeData.Color;
    }

    public void HandleOnOpenInfo()
    {
        UIManager.Instance.BlastInfoPopup.UpdateData(_blast);

        UIManager.Instance.BlastInfoPopup.OpenPopup();
    }
}
