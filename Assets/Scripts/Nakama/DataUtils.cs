using BaseTemplate.Behaviours;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUtils : MonoSingleton<DataUtils>
{
    [SerializeField] List<Sprite> _allBlastSprite, _allItemSprite;
    [SerializeField] Color _normalColor, _fireColor, _waterColor, _grassColor, _groundColor, _flyColor, _electricColor, _lightColor, _darkColor;
    [SerializeField] Color _healColor, _manaColor, _catchColor, _statusColor;

    [Header("Debug")]
    [SerializeField] Sprite _coinIco;
    [SerializeField] Sprite _gemIco, _trophyIco;
    [SerializeField] Sprite _blastPediaSprite, _itemPediaSprite, _blastIcoSprite, _itemIcoSprite;

    List<BlastData> _blastPedia;
    List<ItemData> _itemPedia;
    List<Move> _movePedia;

    BlastCollection _blastCollection;
    ItemCollection _itemCollection;

    public Sprite CoinIco { get => _coinIco; }
    public Sprite GemIco { get => _gemIco; }
    public Sprite TrophyIco { get => _trophyIco; }
    public Sprite BlastPediaSprite { get => _blastPediaSprite; }
    public Sprite ItemPediaSprite { get => _itemPediaSprite; }
    public Sprite BlastIcoSprite { get => _blastIcoSprite; }
    public Sprite ItemIcoSprite { get => _itemIcoSprite; }

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

    public Color GetTypeColor(TYPE type)
    {
        Color colorToReturn = new Color();

        switch (type)
        {
            case TYPE.NORMAL:
                colorToReturn = _normalColor;
                break;
            case TYPE.FIRE:
                colorToReturn = _fireColor;
                break;
            case TYPE.WATER:
                colorToReturn = _waterColor;
                break;
            case TYPE.GRASS:
                colorToReturn = _grassColor;
                break;
            case TYPE.GROUND:
                colorToReturn = _groundColor;
                break;
            case TYPE.FLY:
                colorToReturn = _flyColor;
                break;
            case TYPE.ELECTRIC:
                colorToReturn = _electricColor;
                break;
            case TYPE.LIGHT:
                colorToReturn = _lightColor;
                break;
            case TYPE.DARK:
                colorToReturn = _darkColor;
                break;
        }

        return colorToReturn;
    }

    public Color GetItemColor(ItemBehaviour behaviour)
    {
        Color colorToReturn = new Color();

        switch (behaviour)
        {
            case ItemBehaviour.HEAL:
                colorToReturn = _healColor;
                break;
            case ItemBehaviour.MANA:
                colorToReturn = _manaColor;
                break;
            case ItemBehaviour.STATUS:
                colorToReturn = _statusColor;
                break;
            case ItemBehaviour.CATCH:
                colorToReturn = _catchColor;
                break;
        }

        return colorToReturn;
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
