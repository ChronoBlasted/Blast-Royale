using BaseTemplate.Behaviours;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUtils : MonoSingleton<DataUtils>
{
    [SerializeField] List<Sprite> _allBlastSprite, _allItemSprite;

    List<BlastData> _blastPedia;
    List<ItemData> _itemPedia;
    List<Move> _movePedia;

    BlastCollection _blastCollection;
    ItemCollection _itemCollection;

    public List<BlastData> BlastPedia { get => _blastPedia; set => _blastPedia = value; }
    public List<ItemData> ItemPedia { get => _itemPedia; set => _itemPedia = value; }
    public List<Move> MovePedia { get => _movePedia; set => _movePedia = value; }

    public ItemCollection ItemCollection { get => _itemCollection; set => _itemCollection = value; }
    public BlastCollection BlastCollection { get => _blastCollection; set => _blastCollection = value; }


    public Sprite GetBlastImgByID(int id)
    {
        return _allBlastSprite[id];
    }

    public Sprite GetItemImgByID(int id)
    {
        return _allItemSprite[id];
    }

    public BlastData GetBlastDataById(int id)
    {
        return _blastPedia.Find(x => x.id == id);
    }

    public Move GetMoveById(int id)
    {
        return _movePedia.Find(x => x.id == id);
    }

    public ItemData GetItemDataById(int id)
    {
        return _itemPedia.Find(x => x.id == id);
    }
}
