using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PediaItemLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _nameTxt, _behaviourTxt, _descTxt;
    [SerializeField] Image _itemBG, _itemImg;

    ItemData _data;
    public void Init(ItemData item)
    {
        _data = item;

        _nameTxt.text = NakamaData.Instance.GetItemDataRef(_data.id).Name.GetLocalizedString();
        _behaviourTxt.text = _data.behaviour.ToString();

        switch (_data.behaviour)
        {
            case ItemBehaviour.Heal:
                _descTxt.text = "Restore " + _data.gain_amount + " health points";
                break;
            case ItemBehaviour.Mana:
                _descTxt.text = "Restore " + _data.gain_amount + " mana points";
                break;
            case ItemBehaviour.Status:
                _descTxt.text = "Remove " + _data.status + " from a Blast";
                break;
            case ItemBehaviour.Catch:
                _descTxt.text = "Try to catch with rate  : " + _data.catchRate;
                break;
        }

        _itemImg.sprite = NakamaData.Instance.GetItemDataRef(_data.id).Sprite;
        _itemBG.color = ColorManager.Instance.GetItemColor(_data.behaviour);
    }
}



