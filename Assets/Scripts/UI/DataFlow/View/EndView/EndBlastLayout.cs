using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndBlastLayout : MonoBehaviour
{
    [SerializeField] Image _blastIco;
    [SerializeField] TMP_Text _blastName, _blastLevel;
    [SerializeField] SliderBar _hpSlider;

    Blast _currentBlast;

    public void Init(Blast blast)
    {
        _currentBlast = blast;

        BlastDataRef dataRef = NakamaData.Instance.GetBlastDataRef(blast.data_id);
        _blastName.text = dataRef.Name.GetLocalizedString();
        _blastLevel.text = "lvl." + blast.Level;
        _blastIco.sprite = NakamaData.Instance.GetSpriteWithBlast(_currentBlast);

        _hpSlider.Init(_currentBlast.Hp, _currentBlast.MaxHp);
    }
}
