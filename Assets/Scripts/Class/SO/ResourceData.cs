using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewResourceName", menuName = "ScriptableObjects/NewResourceObject", order = 0)]
public class ResourceData : ScriptableObject
{
    public ResourceType Type;
    public LocalizedString Name;
    public Sprite Sprite;
    public GameObject Prefab;
}