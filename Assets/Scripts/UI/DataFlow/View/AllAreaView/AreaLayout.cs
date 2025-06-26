using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class AreaLayout : MonoBehaviour
{
    [SerializeField] Image _areaImg, _selectedAreaImg, _selectedBtnBG;
    [SerializeField] TMP_Text _areaTitleTxt, _levelRangeTxt, _trophyRequiredTxt, _selectedBtnTxt;
    [SerializeField] AreaSingleBlastLayout _singleBlastPrefab;
    [SerializeField] Transform _singleBlastTransform;

    AreaData _data;
    public AreaData Data { get => _data; }

    public void Init(AreaData areaData)
    {
        _data = areaData;

        AreaDataRef dataRef = NakamaData.Instance.GetAreaDataRef(_data.id);

        _areaTitleTxt.text = dataRef.Name.GetLocalizedString();
        _areaImg.sprite = dataRef.Sprite;
        _levelRangeTxt.text = "Level : " + _data.blastLevels[0] + "-" + _data.blastLevels[1];
        //_trophyRequiredTxt.text = "+" + _data.trophyRequired;

        foreach (int blastId in _data.blastIds)
        {
            var currentSingleBlast = Instantiate(_singleBlastPrefab, _singleBlastTransform);
            currentSingleBlast.Init(NakamaData.Instance.GetBlastDataById(blastId));
        }
    }

    public void Select()
    {
        _selectedBtnBG.enabled = false;
        _selectedBtnTxt.text = "Selected";

        _selectedAreaImg.enabled = true;
    }

    public void Unselect()
    {
        _selectedBtnBG.enabled = true;
        _selectedBtnTxt.text = "Select";

        _selectedAreaImg.enabled = false;
    }

    public void HandleOnSelectClick()
    {
        UIManager.Instance.AllAreaView.HandleOnSelectArea(_data.id);

        ChangeAreaEvent myEvent = new ChangeAreaEvent
        {
            ChangeAreaAmount = 1,
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
    }
}
