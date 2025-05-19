using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionSlotLayout : MonoBehaviour
{
    [SerializeField] Image _ico, _bg, _gradient;
    [SerializeField] TMP_Text _progressionTxt;
    [SerializeField] Color _normalColor, _activeColor;
    [SerializeField] Color _normalGradient, _activeGradient;

    [SerializeField] bool _isPermanent;

    [SerializeField] Sprite _battleIco, _chestIco, _bossIco;

    public void Init(int indexProgression)
    {
        _progressionTxt.text = indexProgression.ToString();

        if (indexProgression % 5 == 0 && indexProgression % 10 != 0)
        {
            _ico.sprite = _chestIco;
        }
        else if (indexProgression % 10 == 0)
        {
            _ico.sprite = _bossIco;
        }
        else
        {
            _ico.sprite = _battleIco;
        }

        if (_isPermanent == false)
        {
            SetInactive(true, 0f);
        }
    }

    public void InitSmooth(int indexProgression)
    {

        UIManager.Instance.DoSmoothTextInt(_progressionTxt, 0, indexProgression);

        if (indexProgression % 5 == 0 && indexProgression % 10 != 0)
        {
            _ico.sprite = _chestIco;
        }
        else if (indexProgression % 10 == 0)
        {
            _ico.sprite = _bossIco;
        }
        else
        {
            _ico.sprite = _battleIco;
        }

        if (_isPermanent == false)
        {
            SetInactive(true, 0f);
        }
    }

    public void SetActive()
    {
        _bg.DOColor(_activeColor, 0.2f);
        _gradient.DOColor(_activeGradient, 0.2f);

        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void SetInactive(bool isInstant = false, float delay = .7f)
    {
        _bg.DOColor(_normalColor, isInstant ? 0f : 0.2f).SetDelay(delay);
        _gradient.DOColor(_normalGradient, isInstant ? 0f : 0.2f).SetDelay(delay);

        transform.DOScale(Vector3.one * .8f, isInstant ? 0f : 0.2f).SetDelay(delay);
    }

    public void Punch()
    {
        transform.DOPunchScale(Vector3.one * .2f, 0.2f);
    }
}
