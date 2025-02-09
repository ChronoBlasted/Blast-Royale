using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemLayout : MonoBehaviour
{
    [SerializeField] Image _itemBorderBG, _itemIco;
    [SerializeField] TMP_Text _amount, _name;

    Item _item;
    int _index;

    public Item Item { get => _item; }

    public void Init(Item item, int index = -1)
    {
        _item = item;
        _index = index;

        ItemData itemData = DataUtils.Instance.GetItemDataById(item.data_id);

        _itemBorderBG.color = ColorManager.Instance.GetItemColor(itemData.behaviour);
        _itemIco.sprite = DataUtils.Instance.GetItemImgByID(itemData.id);
        _amount.text = "X" + _item.amount;
        _name.text = itemData.name;
    }

    public void UpdateUI(int amount)
    {
        if (amount == 0)
        {
            _amount.color = Color.red;
        }
        else if (amount <= 2)
        {
            _amount.color = Color.yellow;
        }
        else
        {
            _amount.color = Color.white;
        }

        _amount.text = "X" + amount;
    }

    public void HandleOnSwapEnable()
    {
        UIManager.Instance.MenuView.SquadPanel.IsSwapMode = true;
        UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored = _index;
        // Do feedback on select

        UIManager.Instance.MenuView.SquadPanel.SwitchToSoloItem(_item);
    }

    public void HandleOnSwapDisable()
    {
        UIManager.Instance.MenuView.SquadPanel.IsSwapMode = false;
        UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored = -1;

        // Do feedback on select
        UIManager.Instance.MenuView.SquadPanel.QuitSoloItem();
    }
}
