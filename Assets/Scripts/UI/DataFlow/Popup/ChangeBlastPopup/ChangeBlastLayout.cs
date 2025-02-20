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
                if (WildBattleManager.Instance.PlayerBlast != _blast && _blast.Hp > 0)
                {
                    UnlockBlast();
                    return true;
                }
                break;
        }

        LockBlast();
        return false;
    }


    void UnlockBlast()
    {
        _borderImg.DOFade(1f, 0);
    }

    void LockBlast()
    {
        _borderImg.DOFade(.5f, 0);
    }
}
