using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguagePopup : Popup
{
    [SerializeField] TMP_Text _currentLanguageTxt;
    [SerializeField] List<LanguageLayout> _languageLayoutList;


    public override void Init()
    {
        gameObject.SetActive(true);

        StartCoroutine(UpdateSelectedLanguage());
    }

    public override void OpenPopup()
    {
        base.OpenPopup();
    }

    public override void ClosePopup()
    {
        base.ClosePopup();
    }

    public void ChangeSelected(int index)
    {
        var locale = LocalizationSettings.AvailableLocales.Locales[index];
        LocalizationSettings.SelectedLocale = locale;

        SaveHandler.SaveValue("currentLocale", locale.LocaleName);

        UIManager.Instance.ChangeView(UIManager.Instance.OpeningView);

        GameManager.Instance.ReloadScene();
    }

    IEnumerator UpdateSelectedLanguage()
    {
        var loadingOperation = LocalizationSettings.InitializationOperation;

        yield return loadingOperation;

        for (int i = 0; i < _languageLayoutList.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];

            if (LocalizationSettings.SelectedLocale == locale)
            {
                _currentLanguageTxt.text = _languageLayoutList[i].LanguageName.text;
                _languageLayoutList[i].UpdateState(true);
            }
            else _languageLayoutList[i].UpdateState(false);
        }

        base.Init();
    }

}
