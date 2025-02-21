using BaseTemplate.Behaviours;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

public class ErrorManager : MonoSingleton<ErrorManager>
{
    [SerializeField] List<ErrorData> errorDatas;

    ErrorData GetErrorDataWithID(ErrorType errorID = ErrorType.NONE)
    {
        return errorDatas.First(resource => resource.ErrorID == errorID);
    }

    public void ShowError(ErrorType errorType)
    {
        ErrorData errorData = GetErrorDataWithID(errorType);

        UIManager.Instance.ErrorView.AddError(errorData.ErrorMsg.GetLocalizedString());
    }

#if UNITY_EDITOR
    [ContextMenu("Load All Error Data")]
    public void LoadAllResourcesInEditor()
    {
        errorDatas = AssetDatabase.FindAssets("t:ErrorData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ErrorData>)
            .ToList();

        EditorUtility.SetDirty(this);
    }
#endif
}

