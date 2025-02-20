using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaSingleBlastLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _blastNameTxt;
    [SerializeField] Image _borderBG, _ico,_typeIco;

    BlastData _blast;

    public void Init(BlastData blast)
    {
        _blast = blast;

        _blastNameTxt.text = NakamaData.Instance.GetBlastDataRef(_blast.id).Name.GetLocalizedString();

        _ico.sprite = NakamaData.Instance.GetBlastDataRef(_blast.id).Sprite;
        _typeIco.sprite = ResourceObjectHolder.Instance.GetTypeDataByType(_blast.type).Sprite;
        _borderBG.color = ResourceObjectHolder.Instance.GetTypeDataByType(_blast.type).Color;
    }

    public void HandleOnOpenInfo()
    {
        UIManager.Instance.BlastInfoPopup.UpdateData(_blast);

        UIManager.Instance.BlastInfoPopup.OpenPopup();
    }
}
