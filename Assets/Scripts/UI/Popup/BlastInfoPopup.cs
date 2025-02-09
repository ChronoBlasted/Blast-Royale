using Chrono.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlastInfoPopup : Popup
{
    [SerializeField] TMP_Text _blastNameTxt, _blastDescTxt, _blastLevel, _blastExp, _blastHp, _blastMana, _blastAttack, _blastDefense, _blastSpeed, _blastType;
    [SerializeField] Image _blastImg, _blastTypeColorImg, _borderImg;
    [SerializeField] GameObject _prestigeEvolveLayout, _moveLayout;
    [SerializeField] CustomButton _prestigeButton, _evolveButton;
    [SerializeField] List<MoveLayout> movesLayout;

    Blast _currentBlast;
    BlastData _currentBlastData;
    NakamaData _dataUtils;
    ColorManager _colorManager;

    public override void Init()
    {
        base.Init();

        _dataUtils = NakamaData.Instance;
        _colorManager = ColorManager.Instance;
    }

    public override void OpenPopup()
    {
        base.OpenPopup();

    }

    public override void ClosePopup()
    {
        base.ClosePopup();
    }

    public void UpdateData(Blast blast)
    {
        _currentBlast = blast;
        _currentBlastData = _dataUtils.GetBlastDataById(blast.data_id);

        _blastNameTxt.text = _dataUtils.GetBlastDataRef(blast.data_id).Name.GetLocalizedString();
        _blastDescTxt.text = _currentBlastData.desc;
        _blastLevel.text = "Lvl." + NakamaLogic.CalculateLevelFromExperience(_currentBlast.exp);
        _blastExp.text =
            _currentBlast.exp - NakamaLogic.CalculateExperienceFromLevel(NakamaLogic.CalculateLevelFromExperience(_currentBlast.exp))
            + " / " +
            (NakamaLogic.CalculateExperienceFromLevel(NakamaLogic.CalculateLevelFromExperience(_currentBlast.exp) + 1) - NakamaLogic.CalculateExperienceFromLevel(NakamaLogic.CalculateLevelFromExperience(_currentBlast.exp)));

        _blastHp.text = _currentBlast.MaxHp.ToString();
        _blastMana.text = _currentBlast.MaxMana.ToString();
        _blastAttack.text = _currentBlast.Attack.ToString();
        _blastDefense.text = _currentBlast.Defense.ToString();
        _blastSpeed.text = _currentBlast.Speed.ToString();
        _blastType.text = _currentBlastData.type.ToString();

        _blastImg.sprite = _dataUtils.GetBlastDataRef(blast.data_id).Sprite;
        _blastTypeColorImg.color = _colorManager.GetTypeColor(_currentBlastData.type);
        _borderImg.color = _colorManager.GetTypeColor(_currentBlastData.type);

        _moveLayout.SetActive(true);

        for (int i = 0; i < movesLayout.Count; i++)
        {
            if (i < _currentBlast.activeMoveset.Count)
            {
                movesLayout[i].gameObject.SetActive(true);
                movesLayout[i].Init(_dataUtils.GetMoveById(_currentBlast.activeMoveset[i]), null);
            }
            else movesLayout[i].gameObject.SetActive(false);
        }


        _prestigeEvolveLayout.SetActive(true);
    }

    public void UpdateData(BlastData blast)
    {
        _currentBlastData = blast;

        _blastNameTxt.text = _dataUtils.GetBlastDataRef(_currentBlastData.id).Name.GetLocalizedString();
        _blastDescTxt.text = _currentBlastData.desc;
        _blastLevel.text = "";
        _blastExp.text = "";

        _blastHp.text = _currentBlastData.hp.ToString();
        _blastMana.text = _currentBlastData.mana.ToString();
        _blastAttack.text = _currentBlastData.attack.ToString();
        _blastDefense.text = _currentBlastData.defense.ToString();
        _blastSpeed.text = _currentBlastData.speed.ToString();
        _blastType.text = _currentBlastData.type.ToString();


        _blastImg.sprite = _dataUtils.GetBlastDataRef(_currentBlast.data_id).Sprite;
        _blastTypeColorImg.color = _colorManager.GetTypeColor(_currentBlastData.type);
        _borderImg.color = _colorManager.GetTypeColor(_currentBlastData.type);

        _prestigeEvolveLayout.SetActive(false);
        _moveLayout.SetActive(false);
    }

    public void HandleOnEvolve()
    {
        NakamaManager.Instance.NakamaUserAccount.EvolveBlast(_currentBlast.uuid);
    }

    public void HandleOnPrestige()
    {

    }

    public void HandleSwapMove(int indexMove)
    {
        UIManager.Instance.MoveSelectorPopup.UpdateData(_currentBlast, _dataUtils.GetMoveById(_currentBlast.activeMoveset[indexMove]), indexMove);
        UIManager.Instance.MoveSelectorPopup.OpenPopup();
    }
}
