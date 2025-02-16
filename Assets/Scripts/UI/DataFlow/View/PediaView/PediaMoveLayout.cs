using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PediaMoveLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _idTxt, _nameTxt, _descTxt, _manaCostTxt, _powerTxt;
    [SerializeField] Image _borderImg;

    Move _data;

    public void Init(Move move)
    {
        _data = move;

        _idTxt.text = "ID." + _data.id;
        _nameTxt.text = NakamaData.Instance.GetMoveDataRef(_data.id).Name.GetLocalizedString();
        _descTxt.text = NakamaData.Instance.GetMoveDataRef(_data.id).Desc.GetLocalizedString();

        _manaCostTxt.text = _data.cost.ToString();
        _powerTxt.text = _data.power.ToString();

        _borderImg.color = ColorManager.Instance.GetTypeColor(_data.type);
    }
}
