using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BaseTemplate.Behaviours;

public class ResourceObjectHolder : MonoSingleton<ResourceObjectHolder>
{
    public List<ResourceData> resources;

    public ResourceData GetResourceByType(ResourceType resourceType)
    {
        return resources.First(resource => resource.type == resourceType);
    }
}
