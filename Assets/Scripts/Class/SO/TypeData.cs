using UnityEngine.Localization;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTypeData", menuName = "ScriptableObjects/NewTypeData", order = 1)]
public class TypeData : ScriptableObject
{
    public Type Type;
    public LocalizedString Name;
    public Color Color;
    public Sprite Sprite;
    public ParticleSystem TypeHitFX;
    public List<Type> TypeStronger;
    public List<Type> TypeWeaker;
}