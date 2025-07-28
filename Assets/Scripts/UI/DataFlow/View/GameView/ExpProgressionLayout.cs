using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpProgressionLayout : MonoBehaviour
{
    [SerializeField] CanvasGroup _cg;
    [SerializeField] Image _ico;
    [SerializeField] TMP_Text _amount, _level, _levelExpSlash;

    RectTransform _rectTransform;
    Vector2 _onScreenPosition;
    Vector2 _offScreenPosition;

    Coroutine _hideCor;

    int _lastRatioExp;
    int _lastNextRatioExp;
    int _lastLevel;

    public TMP_Text Amount { get => _amount; }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _onScreenPosition = _rectTransform.anchoredPosition;
        _offScreenPosition = _onScreenPosition + Vector2.left * 500f;

        _rectTransform.anchoredPosition = _offScreenPosition;

        _cg.alpha = 0f;

    }

    public void Init(Sprite newIco)
    {
        _ico.sprite = newIco;

        _amount.text = "+0";
    }

    public void UpdateData(int level, int ratioExp, int ratioNextLevel)
    {
        _lastRatioExp = ratioExp;
        _lastLevel = level;
        _lastNextRatioExp = ratioNextLevel;

        _level.text = "Level " + _lastLevel;
        UIManager.Instance.DoSmoothTextInt(_levelExpSlash, 0, _lastRatioExp, "", "/" + _lastNextRatioExp);
    }

    public int AddExp(int expGain)
    {
        int newTotal = _lastRatioExp + expGain;

        if (newTotal >= _lastNextRatioExp)
        {
            int surplus = newTotal - _lastNextRatioExp;

            UIManager.Instance.DoSmoothTextInt(_amount, 0, _lastNextRatioExp - _lastRatioExp, "+");
            UIManager.Instance.DoSmoothTextInt(_levelExpSlash, _lastRatioExp, _lastNextRatioExp, "", "/" + _lastNextRatioExp);

            return surplus;
        }

        UIManager.Instance.DoSmoothTextInt(_amount, 0, expGain, "+");
        UIManager.Instance.DoSmoothTextInt(_levelExpSlash, _lastRatioExp, _lastRatioExp + expGain, "", "/" + _lastNextRatioExp);

        _lastRatioExp = newTotal;
        return -1;
    }


    public void Show()
    {
        DOTween.Kill(_rectTransform);

        _cg.DOFade(1, .2f);
        _rectTransform.DOAnchorPos(_onScreenPosition, .5f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        DOTween.Kill(_rectTransform);

        _cg.DOFade(0f, .2f);

        _rectTransform.DOAnchorPos(_offScreenPosition, .5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            _amount.text = "+0";
        });
    }
}
