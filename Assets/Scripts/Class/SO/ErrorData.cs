using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewErrorData", menuName = "ScriptableObjects/NewErrorData", order = 0)]

public class ErrorData : ScriptableObject
{
    public ErrorType ErrorID;
    public LocalizedString ErrorMsg;
}
