using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BaseTemplate.Behaviours;
using UnityEditor;

public class ResourceObjectHolder : MonoSingleton<ResourceObjectHolder>
{
    [SerializeField] List<ResourceData> _resources;
    [SerializeField] List<TypeData> _typeData;

    public ResourceData GetResourceByType(ResourceType resourceType)
    {
        return _resources.First(resource => resource.Type == resourceType);
    }

    public TypeData GetTypeDataByType(Type type)
    {
        return _typeData.First(resource => resource.Type == type);
    }

#if UNITY_EDITOR
    [ContextMenu("Load All Data")]
    public void LoadAllResourcesInEditor()
    {
        _resources = AssetDatabase.FindAssets("t:ResourceData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ResourceData>)
            .ToList();

        _typeData = AssetDatabase.FindAssets("t:TypeData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<TypeData>)
            .ToList();

        EditorUtility.SetDirty(this);
    }
#endif
}
