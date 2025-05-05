using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlastMiniSquadLayout : MonoBehaviour
{
    [SerializeField] int _blastIndex;
    [SerializeField] Image _bg, _blastIco;
    [SerializeField] TMP_Text _blastName;
    [SerializeField] SliderBar _hpSlider, _manaSlider;

    [SerializeField] Sprite _aliveBG, _deadBG, _activeBG;

    Blast _currentBlast;

    public void Init(Blast blast)
    {
        _currentBlast = blast;

        BlastDataRef dataRef = NakamaData.Instance.GetBlastDataRef(blast.data_id);
        _blastName.text = dataRef.Name.GetLocalizedString();
        _blastIco.sprite = dataRef.Sprite;

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
        if (_currentBlast.Hp <= 0)
        {
            _bg.sprite = _deadBG;
        }
        else
        {
            _bg.sprite = _aliveBG;
        }

        if (_currentBlast == WildBattleManager.Instance.PlayerBlast)
        {
            _bg.sprite = _activeBG;
        }
    }

    public void HandleOnClick()
    {
        WildBattleManager.Instance.PlayerChangeBlast(_blastIndex);
    }
}
