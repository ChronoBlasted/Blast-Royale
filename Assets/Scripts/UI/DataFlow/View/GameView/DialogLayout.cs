using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _dialogTxt, meteoTxt;
    [SerializeField] CanvasGroup _cg;

    bool _skipAsync;
    public void UpdateText(string text)
    {
        _dialogTxt.text = text;
    }

    public void Show()
    {
        _cg.DOFade(1f, .2f);
    }

    public void Hide()
    {
        _cg.DOFade(0f, .2f).OnComplete(() => _dialogTxt.text = "");
    }


    public async Task UpdateTextAsync(string text)
    {
        _skipAsync = false;
        _dialogTxt.text = "";

        foreach (char c in text)
        {
            if (_skipAsync)
            {
                _dialogTxt.text = text;
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            _dialogTxt.text += c;
        }

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    public void SkipUpdateTextAsync()
    {
        _skipAsync = true;

        // A tester
    }

    public void SetMeteo(string newMeteo)
    {
        meteoTxt.text = newMeteo;
    }
}
