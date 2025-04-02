using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageLayout : MonoBehaviour
{
    [SerializeField] Image _bg;
    [SerializeField] TMP_Text _languageName;

    public TMP_Text LanguageName { get => _languageName; }

    public void UpdateState(bool isOn = false)
    {
        _bg.sprite = isOn ? ColorManager.Instance.ActiveSprite : ColorManager.Instance.InactiveSprite;
    }

    public void HandleOnChangeLanguage(int languageIndex)
    {
        UIManager.Instance.LanguagePopup.ChangeSelected(languageIndex);
    }
}
