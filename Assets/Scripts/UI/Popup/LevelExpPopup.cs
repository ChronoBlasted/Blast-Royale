using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelExpPopup : Popup
{
    [SerializeField] TMP_Text _blastNameTxt, _blastLvlTxt;
    [SerializeField] SliderBar _expBar;
    [SerializeField] Image _blastImg;

    [SerializeField] Button _closeButton;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenPopup()
    {
        base.OpenPopup();
    }

    public override void ClosePopup()
    {
        base.ClosePopup();
    }

    public void UpdateData(Blast blast, Blast enemyBlast)
    {
        BlastData data = NakamaData.Instance.GetBlastDataById(blast.data_id);

        _blastNameTxt.text = NakamaData.Instance.GetBlastDataRef(data.id).Name.GetLocalizedString();
        _blastLvlTxt.text = "LVL." + blast.Level;

        _expBar.Init(blast.GetRatioExp(), blast.GetRatioExpNextLevel());

        var expGain = NakamaLogic.CalculateExpGain(NakamaData.Instance.GetBlastDataById(blast.data_id).expYield, blast.Level, enemyBlast.Level);

        _expBar.SetValueSmooth(blast.GetRatioExp() + expGain, .5f, 1f, DG.Tweening.Ease.InSine);

        _blastImg.sprite = NakamaData.Instance.GetBlastDataRef(data.id).Sprite;
    }

    public void UpdateClose(UnityAction unityAction)
    {
        _closeButton.onClick.RemoveAllListeners();

        _closeButton.onClick.AddListener(ClosePopup);
        _closeButton.onClick.AddListener(unityAction);
    }
}
