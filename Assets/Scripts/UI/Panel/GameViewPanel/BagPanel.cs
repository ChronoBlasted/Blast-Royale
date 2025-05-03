using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class BagPanel : Panel
{
    [SerializeField] List<ItemLayout> itemInBattleLayouts;

    int _lastItemIndex;

    WildBattleManager _wildBattleManager;
    List<Item> _items;

    public override void Init()
    {
        base.Init();

        _wildBattleManager = WildBattleManager.Instance;
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
    }

    public void UpdateItem(List<Item> items)
    {
        _items = items;
        for (int i = 0; i < itemInBattleLayouts.Count; i++)
        {
            if (i < items.Count()) itemInBattleLayouts[i].Init(items[i]);
            else itemInBattleLayouts[i].gameObject.SetActive(false);
        }
    }

    public void UpdateUI(List<Item> items)
    {
        for (int i = 0; i < itemInBattleLayouts.Count; i++)
        {
            if (i < _items.Count()) itemInBattleLayouts[i].UpdateUI(_items[i].amount);
            else itemInBattleLayouts[i].gameObject.SetActive(false);
        }
    }

    public void HandleOnUseItem(int indexItem)
    {
        _lastItemIndex = indexItem;

        if (itemInBattleLayouts[_lastItemIndex].Item.amount <= 0)
        {
            Debug.Log("Cannot use this item : Amount negative"); // TODO faire une popup d'erreur
            return;
        }

        List<UnityAction<int>> actions = new List<UnityAction<int>>()
        {
            UseItemOnBlast,
        };

        ItemData itemData = NakamaData.Instance.GetItemDataById(itemInBattleLayouts[_lastItemIndex].Item.data_id);

        switch (itemData.behaviour)
        {
            case ItemBehaviour.HEAL:
                UIManager.Instance.ChangeBlastPopup.OpenPopup();

                UIManager.Instance.ChangeBlastPopup.UpdateAction(actions, CHANGE_REASON.HP, NakamaData.Instance.GetItemDataRef(itemData.id).Name.GetLocalizedString());
                break;

            case ItemBehaviour.MANA:
                UIManager.Instance.ChangeBlastPopup.OpenPopup();

                UIManager.Instance.ChangeBlastPopup.UpdateAction(actions, CHANGE_REASON.MANA, NakamaData.Instance.GetItemDataRef(itemData.id).Name.GetLocalizedString());
                break;
            case ItemBehaviour.STATUS:
                UIManager.Instance.ChangeBlastPopup.OpenPopup();

                UIManager.Instance.ChangeBlastPopup.UpdateAction(actions, CHANGE_REASON.STATUS, NakamaData.Instance.GetItemDataRef(itemData.id).Name.GetLocalizedString());
                break;
            case ItemBehaviour.CATCH:
                UseItemOnBlast(_lastItemIndex);
                break;
        }
    }

    void UseItemOnBlast(int indexBlast)
    {
        _wildBattleManager.PlayerUseItem(_lastItemIndex, indexBlast);
    }
}
