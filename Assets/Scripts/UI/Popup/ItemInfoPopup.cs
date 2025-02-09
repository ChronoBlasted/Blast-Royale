using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPopup : Popup
{
    [SerializeField] TMP_Text _itemNameTxt, _itemDescTxt, _itemAmount, _itemBehaviour;
    [SerializeField] Image _itemImg, _itemTypeColorImg, _borderImg;


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

    public void UpdateData(Item item)
    {
        ItemData itemData = NakamaData.Instance.GetItemDataById(item.data_id);

        _itemNameTxt.text = itemData.name;
        _itemDescTxt.text = itemData.desc;
        _itemAmount.text = "X" + item.amount;

        _itemBehaviour.text = itemData.behaviour.ToString();

        _itemImg.sprite = NakamaData.Instance.GetItemImgByID(item.data_id);
        _itemTypeColorImg.color = ColorManager.Instance.GetItemColor(itemData.behaviour);
        _borderImg.color = ColorManager.Instance.GetItemColor(itemData.behaviour);
    }
}
