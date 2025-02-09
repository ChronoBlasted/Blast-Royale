using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BaseTemplate.Behaviours;
using UnityEditor;

public class ResourceObjectHolder : MonoSingleton<ResourceObjectHolder>
{
    public List<ResourceData> resources;

    public ResourceData GetResourceByType(ResourceType resourceType)
    {
        return resources.First(resource => resource.type == resourceType);
    }

#if UNITY_EDITOR
    [ContextMenu("Load All ResourceData")]
    public void LoadAllResourcesInEditor()
    {
        resources = AssetDatabase.FindAssets("t:ResourceData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ResourceData>)
            .ToList();

        EditorUtility.SetDirty(this);
    }
#endif
}
