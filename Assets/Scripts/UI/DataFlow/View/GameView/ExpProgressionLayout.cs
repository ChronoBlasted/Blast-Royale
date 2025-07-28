using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpProgressionLayout : MonoBehaviour
{
    [SerializeField] CanvasGroup _cg;
    [SerializeField] Image _ico;
    [SerializeField] TMP_Text _amountTxt, _levelTxt, _levelExpSlashTxt;

    RectTransform _rectTransform;
    Vector2 _onScreenPosition;
    Vector2 _offScreenPosition;

    Coroutine _hideCor;

    int _exp;
    int _expForNextLevel;
    int _level;

    public TMP_Text Amount { get => _amountTxt; }
    public int Level { get => _level; }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _onScreenPosition = _rectTransform.anchoredPosition;
        _offScreenPosition = _onScreenPosition + Vector2.left * 500f;

        _rectTransform.anchoredPosition = _offScreenPosition;

        _cg.alpha = 0f;

    }

    public void Init(Sprite newIco, int level, int ratioExp)
    {
        _ico.sprite = newIco;

        _level = level;
        _exp = ratioExp;
        _expForNextLevel = NakamaLogic.GetRatioExpNextLevel(_level);

        UpdateLevel(level);

        _amountTxt.text = "+0";
    }

    void UpdateLevel(int level)
    {
        _level = level;
        _levelTxt.text = "Level " + (_level + 1);
    }

    public void UpdateData()
    {
        UIManager.Instance.DoSmoothTextInt(_levelExpSlashTxt, 0, _exp, "", "/" + NakamaLogic.GetRatioExpNextLevel(_level));
    }

    public int AddExp(int expGain, bool isInLoop = false)
    {
        if (isInLoop)
        {
            _level++;
            UpdateLevel(_level);
            _exp = 0;
            _expForNextLevel = NakamaLogic.GetRatioExpNextLevel(_level);
        }

        int nexExpGain = _exp + expGain;

        if (nexExpGain >= _expForNextLevel)
        {
            int surplus = nexExpGain - _expForNextLevel;

            UIManager.Instance.DoSmoothTextInt(_amountTxt, 0, _expForNextLevel - _exp, "+");
            UIManager.Instance.DoSmoothTextInt(_levelExpSlashTxt, _exp, _expForNextLevel, "", "/" + _expForNextLevel, 1, Ease.OutSine, () => _levelExpSlashTxt.transform.DOPunchScale(new Vector3(.2f, .2f, .2f), .5f, 1, 1));
            return surplus;
        }

        UIManager.Instance.DoSmoothTextInt(_amountTxt, 0, expGain, "+");
        UIManager.Instance.DoSmoothTextInt(_levelExpSlashTxt, _exp, _exp + expGain, "", "/" + _expForNextLevel);

        _exp = nexExpGain;
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
            _amountTxt.text = "+0";
        });
    }
}
