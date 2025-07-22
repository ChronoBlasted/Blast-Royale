using Chrono.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBlastLayout : MonoBehaviour
{
    Blast _blast;

    [SerializeField] TMP_Text _nameTxt, _lvlTxt;
    [SerializeField] SliderBar _hpBar, _manaBar;
    [SerializeField] Image _borderImg, _blastImg;
    [SerializeField] LockLayout _lockLayout;
    [SerializeField] CustomButton _button;

    ErrorManager _errorManager;

    public CustomButton Button { get => _button; }

    private void Awake()
    {
        _errorManager = ErrorManager.Instance;
    }

    public void Init(Blast newBlast)
    {

        BlastData blastData = NakamaData.Instance.GetBlastDataById(newBlast.data_id);

        _blast = newBlast;

        _nameTxt.text = NakamaData.Instance.GetBlastDataRef(blastData.id).Name.GetLocalizedString();
        _lvlTxt.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(_blast.exp);

        _hpBar.Init(_blast.Hp, _blast.MaxHp);
        _manaBar.Init(_blast.Mana, _blast.MaxMana);

        _blastImg.sprite = NakamaData.Instance.GetBlastDataRef(blastData.id).Sprite;
        _borderImg.color = ResourceObjectHolder.Instance.GetTypeDataByType(blastData.type).Color;
    }

    public bool IsUnlocked(CHANGE_REASON changeReason)
    {
        switch (changeReason)
        {
            case CHANGE_REASON.HP:
                if (_blast.Hp != _blast.MaxHp)
                {
                    UnlockBlast();
                    return true;
                }
                break;

            case CHANGE_REASON.MANA:
                if (_blast.MaxMana != _blast.Mana)
                {
                    UnlockBlast();
                    return true;
                }
                break;

            case CHANGE_REASON.KO:
                if (_blast.Hp > 0)
                {
                    UnlockBlast();
                    return true;
                }
                break;

            case CHANGE_REASON.SWAP:
                if (NakamaManager.Instance.NakamaBattleManager.CurrentBattle.BattleManager.PlayerBlast != _blast && _blast.Hp > 0)
                {
                    UnlockBlast();
                    return true;
                }
                break;
        }

        var errorData = _errorManager.GetErrorDataForChangeReason(changeReason, _blast);
        if (errorData != null)
        {
            LockBlast(errorData);
        }

        return false;
    }


    void UnlockBlast()
    {
        _lockLayout.gameObject.SetActive(false);
    }

    void LockBlast(ErrorData error)
    {
        _lockLayout.LockTxt.text = error.Title.GetLocalizedString();
        _button.onClick.AddListener(() => _errorManager.ShowError(error.ErrorType));
        _lockLayout.gameObject.SetActive(true);
    }
}
