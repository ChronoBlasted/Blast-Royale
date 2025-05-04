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
    [SerializeField] HiddenInfoMenu _hiddenInfoMenu;
    [SerializeField] bool _isDeckItem;

    Item _item;
    int _index;

    public Item Item { get => _item; }

    public void Init(Item item, int index = -1)
    {
        _item = item;
        _index = index;

        ItemData itemData = NakamaData.Instance.GetItemDataById(item.data_id);

        _itemBorderBG.color = ColorManager.Instance.GetItemColor(itemData.behaviour);
        _itemIco.sprite = NakamaData.Instance.GetItemDataRef(itemData.id).Sprite;
        _amount.text = "X" + _item.amount;
        _name.text = NakamaData.Instance.GetItemDataRef(itemData.id).Name.GetLocalizedString();
    }

    public void HandleOnInfoButton()
    {
        UIManager.Instance.ItemInfoPopup.UpdateData(_item);

        UIManager.Instance.ItemInfoPopup.OpenPopup();
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

    public void HandleOnSwap()
    {
        UIManager.Instance.MenuView.SquadPanel.IsSwapMode = true;
        UIManager.Instance.MenuView.SquadPanel.IsDeckSwap = _isDeckItem;
        UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored = _index;
        // Do feedback on select

        UIManager.Instance.MenuView.SquadPanel.SwitchToSoloItem(_item);
    }

    public void DisableSwap()
    {
        UIManager.Instance.MenuView.SquadPanel.IsSwapMode = false;
        UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored = -1;

        // Do feedback on select
        UIManager.Instance.MenuView.SquadPanel.QuitSoloItem();
    }

    public void HandleOnClick()
    {
        if (UIManager.Instance.MenuView.SquadPanel.IsSwapMode)
        {
            NakamaManager.Instance.NakamaUserAccount.SwitchPlayerItem(_index, UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored, UIManager.Instance.MenuView.SquadPanel.IsDeckSwap);

            foreach (Transform child in UIManager.Instance.MenuView.SquadPanel.StoredItemTransform.transform)
            {
                Destroy(child.gameObject);
            }

            DisableSwap();
        }
        else
        {
            _hiddenInfoMenu.HandleOnClick();
        }
    }
}
