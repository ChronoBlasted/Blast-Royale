using BaseTemplate.Behaviours;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NakamaData : MonoSingleton<NakamaData>
{
    [SerializeField] List<BlastDataRef> _allBlastData;
    [SerializeField] List<ItemDataRef> _allItemData;

    List<BlastData> _blastPedia = new List<BlastData>();
    List<ItemData> _itemPedia = new List<ItemData>();
    List<Move> _movePedia = new List<Move>();

    BlastCollection _blastCollection;
    ItemCollection _itemCollection;

    public List<BlastData> BlastPedia { get => _blastPedia; set => _blastPedia = value; }
    public List<ItemData> ItemPedia { get => _itemPedia; set => _itemPedia = value; }
    public List<Move> MovePedia { get => _movePedia; set => _movePedia = value; }

    public ItemCollection ItemCollection { get => _itemCollection; set => _itemCollection = value; }
    public BlastCollection BlastCollection { get => _blastCollection; set => _blastCollection = value; }


    public BlastDataRef GetBlastDataRef(int id)
    {
        return _allBlastData.Find(x => x.ID == id);
    }

    public ItemDataRef GetItemDataRef(int id)
    {
        return _allItemData.Find(x => x.ID == id);
    }

    public Sprite GetItemImgByID(int id)
    {
        return _allItemData.Find(x => x.ID == id).Sprite;
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

#if UNITY_EDITOR
    [ContextMenu("Load All Ref")]
    public void LoadAllResourcesInEditor()
    {
        _allBlastData = AssetDatabase.FindAssets("t:BlastDataRef")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<BlastDataRef>)
            .ToList();

        EditorUtility.SetDirty(this);

        _allItemData = AssetDatabase.FindAssets("t:ItemDataRef")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ItemDataRef>)
            .ToList();

        EditorUtility.SetDirty(this);
    }
#endif
}
