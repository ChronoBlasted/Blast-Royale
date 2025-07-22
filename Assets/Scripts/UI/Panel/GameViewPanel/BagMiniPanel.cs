using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BagMiniPanel : Panel
{
    [SerializeField] Button _tabButton;
    [SerializeField] List<ItemLayout> itemInBattleLayouts;

    int _lastItemIndex;

    List<Item> _items;

    public override void Init()
    {
        base.Init();
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
            case ItemBehaviour.Heal:
                UIManager.Instance.ChangeBlastPopup.OpenPopup();

                UIManager.Instance.ChangeBlastPopup.UpdateAction(actions, CHANGE_REASON.HP, NakamaData.Instance.GetItemDataRef(itemData.id).Name.GetLocalizedString());
                break;

            case ItemBehaviour.Mana:
                UIManager.Instance.ChangeBlastPopup.OpenPopup();

                UIManager.Instance.ChangeBlastPopup.UpdateAction(actions, CHANGE_REASON.MANA, NakamaData.Instance.GetItemDataRef(itemData.id).Name.GetLocalizedString());
                break;
            case ItemBehaviour.Status:
                UIManager.Instance.ChangeBlastPopup.OpenPopup();

                UIManager.Instance.ChangeBlastPopup.UpdateAction(actions, CHANGE_REASON.STATUS, NakamaData.Instance.GetItemDataRef(itemData.id).Name.GetLocalizedString());
                break;
            case ItemBehaviour.Catch:
                UseItemOnBlast(_lastItemIndex);
                break;
        }
    }

    void UseItemOnBlast(int indexBlast)
    {
        NakamaManager.Instance.NakamaBattleManager.CurrentBattle.BattleManager.PlayerUseItem(_lastItemIndex, indexBlast);
    }

    public void HandleOnPvPBattle(bool isPvPBattle)
    {
        _tabButton.interactable = !isPvPBattle;
    }

}
