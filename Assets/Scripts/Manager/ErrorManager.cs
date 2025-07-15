using BaseTemplate.Behaviours;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

public class ErrorManager : MonoSingleton<ErrorManager>
{
    [SerializeField] List<ErrorData> errorDatas;

    public ErrorData GetErrorDataWithID(ErrorType errorID = ErrorType.NONE)
    {
        return errorDatas.First(resource => resource.ErrorType == errorID);
    }

    public ErrorData GetErrorDataForChangeReason(CHANGE_REASON changeReason,Blast blast = null)
    {
        switch (changeReason)
        {
            case CHANGE_REASON.HP:
                return GetErrorDataWithID(ErrorType.IS_FULL_HP);
            case CHANGE_REASON.MANA:
                return GetErrorDataWithID(ErrorType.IS_FULL_MANA);
            case CHANGE_REASON.KO:
                return GetErrorDataWithID(ErrorType.IS_FAINTED);
            case CHANGE_REASON.SWAP:
                if (blast.Hp <= 0)
                    return GetErrorDataWithID(ErrorType.IS_FAINTED);
                if (PvEBattleManager.Instance.PlayerBlast == blast)
                    return GetErrorDataWithID(ErrorType.ALREADY_IN_BATTLE);
                break;
        }

        return null;
    }


    public void ShowError(ErrorType errorType)
    {
        ErrorData errorData = GetErrorDataWithID(errorType);

        UIManager.Instance.ErrorView.AddError(errorData.Desc.GetLocalizedString());
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

