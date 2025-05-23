using UnityEngine;
using UnityEngine.Localization;

public abstract class NakamaDataRef : ScriptableObject
{
    public LocalizedString Name;
    public LocalizedString Desc;
    public Sprite Sprite;
    public int DataID;
}
