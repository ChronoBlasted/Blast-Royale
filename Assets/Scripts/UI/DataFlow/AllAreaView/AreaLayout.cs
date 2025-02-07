using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaLayout : MonoBehaviour
{
    [SerializeField] Image _areaImg;
    [SerializeField] TMP_Text _areaTitleTxt, _levelRangeTxt, _trophyRequiredTxt;

    [SerializeField] AreaSingleBlastLayout _singleBlastPrefab;
    [SerializeField] Transform _singleBlastTransform;

    public void Init(AreaData areaData)
    {
        _areaTitleTxt.text = areaData.name;
        _levelRangeTxt.text = "Level : " + areaData.blastLevels[0] + "-" + areaData.blastLevels[1];
        _trophyRequiredTxt.text = "+" + areaData.trophyRequired;

        foreach (int blastId in areaData.blastIds)
        {
            var currentSingleBlast = Instantiate(_singleBlastPrefab, _singleBlastTransform);
            currentSingleBlast.Init(DataUtils.Instance.GetBlastDataById(blastId));
        }
    }

}
