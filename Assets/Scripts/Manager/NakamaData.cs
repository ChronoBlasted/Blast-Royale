using BaseTemplate.Behaviours;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class NakamaData : MonoSingleton<NakamaData>
{
    [SerializeField] List<BlastDataRef> _allBlastData;
    [SerializeField] List<ItemDataRef> _allItemData;
    [SerializeField] List<MoveDataRef> _allMoveData;
    [SerializeField] List<AreaDataRef> _allAreaDataRef;
    [SerializeField] List<StoreOfferDataRef> _allStoreOfferDataRef;
    [SerializeField] List<QuestDataRef> _allQuestDataRef;

    BlastCollection _blastCollection;
    ItemCollection _itemCollection;
    List<AreaData> _areaCollection;
    Dictionary<string, BlastTrackerEntry> _blastTracker = new Dictionary<string, BlastTrackerEntry>();

    public ItemCollection ItemCollection { get => _itemCollection; set => _itemCollection = value; }
    public BlastCollection BlastCollection { get => _blastCollection; set => _blastCollection = value; }
    public List<AreaData> AreaCollection { get => _areaCollection; set => _areaCollection = value; }


    List<BlastData> _blastPedia = new List<BlastData>();
    List<ItemData> _itemPedia = new List<ItemData>();
    List<Move> _movePedia = new List<Move>();
    public List<BlastData> BlastPedia { get => _blastPedia; set => _blastPedia = value; }
    public List<ItemData> ItemPedia { get => _itemPedia; set => _itemPedia = value; }
    public List<Move> MovePedia { get => _movePedia; set => _movePedia = value; }
    public Dictionary<string, BlastTrackerEntry> BlastTracker { get => _blastTracker; set => _blastTracker = value; }



    public BlastDataRef GetBlastDataRef(int id)
    {
        return _allBlastData.Find(x => x.DataID == id);
    }

    public Sprite GetSpriteWithBlast(Blast blast)
    {
        Sprite sprite;

        BlastDataRef dataRef = GetBlastDataRef(blast.data_id);

        if (blast.shiny) sprite = dataRef.ShinySprite;
        else if (blast.boss) sprite = dataRef.BossSprite;
        else sprite = dataRef.Sprite;

        return sprite;
    }

    public ItemDataRef GetItemDataRef(int id)
    {
        return _allItemData.Find(x => x.DataID == id);
    }

    public MoveDataRef GetMoveDataRef(int id)
    {
        return _allMoveData.Find(x => x.DataID == id);
    }

    public AreaDataRef GetAreaDataRef(int id)
    {
        return _allAreaDataRef.Find(x => x.DataID == id);
    }

    public StoreOfferDataRef GetStoreOfferDataRef(int id)
    {
        return _allStoreOfferDataRef.Find(x => x.DataID == id);
    }
    public QuestDataRef GetQuestDataRefByIds(string id)
    {
        return _allQuestDataRef.Find(x => x.QuestIds.ToString() == id);
    }


    public BlastData GetBlastDataById(int id)
    {
        return _blastPedia.Find(x => x.id == id);
    }
    public ItemData GetItemDataById(int id)
    {
        return _itemPedia.Find(x => x.id == id);
    }
    public Move GetMoveById(int id)
    {
        return _movePedia.Find(x => x.id == id);
    }



#if UNITY_EDITOR
    [ContextMenu("Load All Ref")]
    public void LoadAllResourcesInEditor()
    {
        _allBlastData = AssetDatabase.FindAssets("t:BlastDataRef")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<BlastDataRef>)
            .ToList();

        _allItemData = AssetDatabase.FindAssets("t:ItemDataRef")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ItemDataRef>)
            .ToList();

        _allMoveData = AssetDatabase.FindAssets("t:MoveDataRef")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<MoveDataRef>)
            .ToList();

        _allAreaDataRef = AssetDatabase.FindAssets("t:AreaDataRef")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<AreaDataRef>)
            .ToList();

        _allStoreOfferDataRef = AssetDatabase.FindAssets("t:StoreOfferDataRef")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<StoreOfferDataRef>)
            .ToList();

        _allQuestDataRef = AssetDatabase.FindAssets("t:QuestDataRef")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<QuestDataRef>)
            .ToList();

        EditorUtility.SetDirty(this);
    }
#endif
}
