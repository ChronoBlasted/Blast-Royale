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
        BlastData blastData = DataUtils.Instance.GetBlastDataById(newBlast.data_id);

        _blast = newBlast;

        _nameTxt.text = blastData.name;
        _lvlTxt.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(_blast.exp);

        _hpBar.Init(_blast.Hp, _blast.MaxHp);
        _manaBar.Init(_blast.Mana, _blast.MaxMana);

        _blastImg.sprite = DataUtils.Instance.GetBlastImgByID(blastData.id);
        _borderImg.color = ColorManager.Instance.GetTypeColor(blastData.type);
    }

    public bool IsLocked()
    {
        if (_blast.Hp == 0 || WildBattleManager.Instance.PlayerBlast == _blast)
        {
            LockBlast();
            return true;
        }
        else
        {
            UnlockBlast();
            return false;
        }
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
