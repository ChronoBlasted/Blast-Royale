using UnityEngine.Localization;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTypeData", menuName = "ScriptableObjects/NewTypeData", order = 1)]
public class TypeData : ScriptableObject
{
    public Type Type;
    public LocalizedString Name;
    public Color Color;
    public Sprite Sprite;
}