using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PediaItemLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _idTxt, _nameTxt, _behaviourTxt, _descTxt;
    [SerializeField] Image _borderImg, _itemImg, _behaviourBG;

    ItemData _data;
    public void Init(ItemData item)
    {
        _data = item;

        _idTxt.text = "ID." + _data.id;
        _nameTxt.text = _data.name;
        _behaviourTxt.text = _data.behaviour.ToString();

        switch (_data.behaviour)
        {
            case ItemBehaviour.HEAL:
                _descTxt.text = "Restore " + _data.gain_amount + " health points";
                break;
            case ItemBehaviour.MANA:
                _descTxt.text = "Restore " + _data.gain_amount + " mana points";
                break;
            case ItemBehaviour.STATUS:
                _descTxt.text = "Remove " + _data.status + " from a Blast";
                break;
            case ItemBehaviour.CATCH:
                _descTxt.text = "Try to catch with rate  : " + _data.catchRate;
                break;
        }

        _itemImg.sprite = DataUtils.Instance.GetItemImgByID(_data.id);
        _borderImg.color = ColorManager.Instance.GetItemColor(_data.behaviour);
        _behaviourBG.color = ColorManager.Instance.GetItemColor(_data.behaviour);
    }
}



