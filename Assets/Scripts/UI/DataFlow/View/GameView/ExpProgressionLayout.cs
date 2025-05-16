using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpProgressionLayout : MonoBehaviour
{
    [SerializeField] Image _ico;
    [SerializeField] TMP_Text _amount;

    RectTransform _rectTransform;
    Vector2 _onScreenPosition;
    Vector2 _offScreenPosition;

    Coroutine _hideCor;

    public TMP_Text Amount { get => _amount; }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _onScreenPosition = _rectTransform.anchoredPosition;
        _offScreenPosition = _onScreenPosition + Vector2.left * 500f;

        _rectTransform.anchoredPosition = _offScreenPosition;

        _amount.text = "+0";
    }

    public void Init(Sprite newIco, int newExpGain)
    {
        SetSprite(newIco);

        UIManager.Instance.DoSmoothTextInt(_amount, 0, newExpGain, "+");
    }

    public void SetSprite(Sprite newIco)
    {
        _ico.sprite = newIco;
    }

    public void Show()
    {
        DOTween.Kill(_rectTransform);

        _rectTransform.DOAnchorPos(_onScreenPosition, .5f).SetEase(Ease.OutBack);

        if (_hideCor != null)
        {
            StopCoroutine(_hideCor);
            _hideCor = null;
        }

        _hideCor = StartCoroutine(HideCor());
    }

    public void Hide()
    {
        DOTween.Kill(_rectTransform);

        _rectTransform.DOAnchorPos(_offScreenPosition, .5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _amount.text = "+0";
        });
    }

    IEnumerator HideCor()
    {
        yield return new WaitForSeconds(2f);
        Hide();
    }
}
