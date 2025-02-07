using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageLayout : MonoBehaviour
{
    [SerializeField] Image _bg;
    [SerializeField] TMP_Text _languageName;

    [SerializeField] Sprite _onSprite, _offSprite;

    public TMP_Text LanguageName { get => _languageName; }

    public void UpdateState(bool isOn = false)
    {
        switch (isOn)
        {
            case true:
                _bg.sprite = _onSprite;
                break;
            case false:
                _bg.sprite = _offSprite;
                break;
        }
    }

}
