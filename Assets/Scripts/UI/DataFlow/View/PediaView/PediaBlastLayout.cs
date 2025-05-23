using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PediaBlastLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _idTxt, _nameTxt, _typeText, _hpTxt, _manaTxt, _attackTxt, _defenseTxt, _speedTxt;
    [SerializeField] Image _mainBG, _blastImg, _typeIcoImg;

    public void Init(BlastData _data)
    {
        _idTxt.text = "ID." + _data.id;
        _nameTxt.text = NakamaData.Instance.GetBlastDataRef(_data.id).Name.GetLocalizedString();
        _typeText.text = _data.type.ToString();
        _hpTxt.text = _data.hp.ToString();
        _manaTxt.text = _data.mana.ToString();
        _attackTxt.text = _data.attack.ToString();
        _defenseTxt.text = _data.defense.ToString();
        _speedTxt.text = _data.speed.ToString();

        TypeData typeData = ResourceObjectHolder.Instance.GetTypeDataByType(_data.type);

        _blastImg.sprite = NakamaData.Instance.GetBlastDataRef(_data.id).Sprite;
        _mainBG.color = typeData.Color;

        _typeIcoImg.sprite = typeData.Sprite;
    }
}
