using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlastMiniSquadLayout : MonoBehaviour
{
    [SerializeField] int _blastIndex;
    [SerializeField] Image _bg, _bgGlow, _blastIco;
    [SerializeField] TMP_Text _blastName;
    [SerializeField] SliderBar _hpSlider, _manaSlider;
    [SerializeField] LockLayout _lockLayout;

    [SerializeField] Sprite _aliveBG, _deadBG, _activeBG;
    [SerializeField] Color _onGlow, _offGlow;

    Blast _currentBlast;

    public void Init(Blast blast)
    {
        _currentBlast = blast;

        BlastDataRef dataRef = NakamaData.Instance.GetBlastDataRef(blast.data_id);
        _blastName.text = dataRef.Name.GetLocalizedString();
        _blastIco.sprite = NakamaData.Instance.GetSpriteWithBlast(_currentBlast);

        UpdateUI();
    }

    public void UpdateUI()
    {
        UpdateSliders();
        UpdateBG();
    }

    void UpdateSliders()
    {
        _hpSlider.Init(_currentBlast.Hp, _currentBlast.MaxHp);
        _manaSlider.Init(_currentBlast.Mana, _currentBlast.MaxMana);
    }

    void UpdateBG()
    {
        _lockLayout.gameObject.SetActive(false);

        if (_currentBlast.Hp <= 0)
        {
            _bg.sprite = _deadBG;

            _lockLayout.LockTxt.text = ErrorManager.Instance.GetErrorDataWithID(ErrorType.IS_FAINTED).Desc.GetLocalizedString();
            _lockLayout.gameObject.SetActive(true);
        }
        else
        {
            _bg.sprite = _aliveBG;
        }

        if (_currentBlast == PvEBattleManager.Instance.PlayerBlast)
        {
            _bg.sprite = _activeBG;
            _bgGlow.color = _onGlow;
        }
        else
        {
            _bgGlow.color = _offGlow;
        }
    }

    public void HandleOnClick()
    {
        if (_currentBlast == PvEBattleManager.Instance.PlayerBlast) ErrorManager.Instance.ShowError(ErrorType.ALREADY_IN_BATTLE);
        else if (_currentBlast.Hp <= 0) ErrorManager.Instance.ShowError(ErrorType.IS_FAINTED);
        else PvEBattleManager.Instance.PlayerChangeBlast(_blastIndex);
    }
}
