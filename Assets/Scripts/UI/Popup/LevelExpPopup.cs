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
    [SerializeField] Image _blastBorder, _blastImg;

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
        BlastData data = DataUtils.Instance.GetBlastDataById(blast.data_id);

        _blastNameTxt.text = data.name;
        _blastLvlTxt.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(blast.exp);

        _expBar.Init(blast.exp, NakamaLogic.CalculateExperienceFromLevel(NakamaLogic.CalculateLevelFromExperience(blast.exp) + 1));

        var expGain = NakamaLogic.CalculateExpGain(DataUtils.Instance.GetBlastDataById(blast.data_id).expYield, NakamaLogic.CalculateLevelFromExperience(blast.exp), NakamaLogic.CalculateLevelFromExperience(enemyBlast.exp));
        _expBar.SetValueSmooth(blast.exp + expGain, .5f, 1f, DG.Tweening.Ease.InSine);

        _blastImg.sprite = DataUtils.Instance.GetBlastImgByID(data.id);
        _blastBorder.color = DataUtils.Instance.GetTypeColor(data.type);
    }

    public void UpdateClose(UnityAction unityAction)
    {
        _closeButton.onClick.RemoveAllListeners();

        _closeButton.onClick.AddListener(ClosePopup);
        _closeButton.onClick.AddListener(unityAction);
    }
}
